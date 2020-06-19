using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
http://www.blackbeltcoder.com/Articles/strings/a-sscanf-replacement-for-net

The scanf() format string provides a flexible way to describe the fields in the input string.
Although there are standards, different C compilers seemed to have slightly different rules about the meaning
of some parts of the format string. The following definition is for format strings used by the class
I'll present in this article.

Characters	Description
--------------------------
Whitespace	Any whitespace characters in the format string causes the position to advance to the next
non-whitespace character in the input string. Whitespace characters include spaces, tabs and new lines.
Non-Whitespace except percent (%)	Any character that is not a whitespace character or part of a format
specifier (which begins with a % character) advances past the same matching character in the input string.
Format specifier. A sequence that begins with a percent sign (%) to signify a format specifier, or field,
that will be parsed and stored in a variable. A format specifier has the following form.

%[*][width][modifiers]type

Items within square brackets ([]) are optional. The following table describes elements within the format specifier.

Element	Meaning
----------------
*	Indicates that this field is parsed normally but not stored in a variable.
width	Specifies the maximum number of characters to be read for this field.
modifiers	If supplied, modifies the size of the data type where the field is stored. If not supplied,
the default size is used. Supported modifiers are listed below.
hh: For integer fields, the result is stored in an 8-bit variable. Ignored for floating point fields.
h: For integer fields, the result is stored in a 16-bit variable. Ignored for floating point fields.
l For integer fields, the result is stored in a 64-bit variable. Floating point fields are stored in a double.
ll Same effect as the l modifier.
width	Specifies the maximum number of characters to include in this field.
type	Specifies the field type as described in the following table.

Type Meaning
-------------
c Reads a single character. If a width > 1 is specified, an array of characters is read.
d, i	Reads a decimal integer. Number may begin with 0 (octal), 0x (hexadecimal) or a + or - sign.
e, E, f, g,G Reads a floating point variable. Number may begin with a + or - sign, and may be written using exponential notation.
o Reads an unsigned octal integer
s Reads a string of characters up to the end of the input string, the next whitespace character, or until the number of characters specified for the width has been read.
u Reads an unsigned decimal integer. Number may begin with 0 (octal), 0x (hexadecimal) or a + sign.
x, X Reads an unsigned hexadecimal integer.
[ Reads a string of characters that are included within square brackets.
  For example, "[abc]" will read all characters that are either a, b, or c.
  Use "[^abc]" to read all character that are not a, b, or c.
  If the first character after "[" or after "[^" is "]", the closing square bracket is considered to be one
  of the characters rather than the end of the scanset.

My changes
-----------
JumpToText(input, format) added

 */




namespace Helpers
{
    /// <summary>
    /// Class that provides functionality of the standard C library sscanf()
    /// function.
    /// </summary>
    public class ScanFormatted
    {
        // Format type specifiers
        protected enum Types
        {
            Character,
            Decimal,
            Float,
            Hexadecimal,
            Octal,
            ScanSet,
            String,
            Unsigned
        }

        // Format modifiers
        protected enum Modifiers
        {
            None,
            ShortShort,
            Short,
            Long,
            LongLong
        }

        // Delegate to parse a type
        protected delegate bool ParseValue(TextParser input, FormatSpecifier spec);

        // Class to associate format type with type parser
        protected class TypeParser
        {
            public Types Type { get; set; }
            public ParseValue Parser { get; set; }
        }

        // Class to hold format specifier information
        protected class FormatSpecifier
        {
            public Types Type { get; set; }
            public Modifiers Modifier { get; set; }
            public int Width { get; set; }
            public bool NoResult { get; set; }
            public string ScanSet { get; set; }
            public bool ScanSetExclude { get; set; }
        }

        // Lookup table to find parser by parser type
        protected TypeParser[] _typeParsers;

        // Holds results after calling Parse()
        public List<object> Results;

        // Constructor
        public ScanFormatted()
        {
            // Populate parser type lookup table
            _typeParsers = new TypeParser[] {
            new TypeParser() { Type = Types.Character, Parser = ParseCharacter },
            new TypeParser() { Type = Types.Decimal, Parser = ParseDecimal },
            new TypeParser() { Type = Types.Float, Parser = ParseFloat },
            new TypeParser() { Type = Types.Hexadecimal, Parser = ParseHexadecimal },
            new TypeParser() { Type = Types.Octal, Parser = ParseOctal },
            new TypeParser() { Type = Types.ScanSet, Parser = ParseScanSet },
            new TypeParser() { Type = Types.String, Parser = ParseString },
            new TypeParser() { Type = Types.Unsigned, Parser = ParseDecimal }
        };
            // Allocate results collection
            Results = new List<object>();
        }

        /// <summary>
        /// Parses the input string according to the rules in the
        /// format string. Similar to the standard C library's
        /// sscanf() function. Parsed fields are placed in the
        /// class' Results member.
        /// </summary>
        /// <param name="input">String to parse</param>
        /// <param name="format">Specifies rules for parsing input</param>
        public int Parse(string input, string format)
        {

            input = JumpToText(input, format);         

            TextParser inp = new TextParser(input);
            TextParser fmt = new TextParser(format);
            List<object> results = new List<object>();
            FormatSpecifier spec = new FormatSpecifier();
            int count = 0;

            // Clear any previous results
            Results.Clear();

            // Process input string as indicated in format string
            while (!fmt.EndOfText && !inp.EndOfText)
            {
                if (ParseFormatSpecifier(fmt, spec))
                {
                    // Found a format specifier
                    TypeParser parser = _typeParsers.First(tp => tp.Type == spec.Type);
                    if (parser.Parser(inp, spec))
                        count++;
                    else
                        break;
                }
                else if (Char.IsWhiteSpace(fmt.Peek()))
                {
                    // Whitespace
                    inp.MovePastWhitespace();
                    fmt.MoveAhead();
                }
                else if (fmt.Peek() == inp.Peek())
                {
                    // Matching character
                    inp.MoveAhead();
                    fmt.MoveAhead();
                }
                else break;    // Break at mismatch
            }

            // Return number of fields successfully parsed
            return count;
        }

        /// <summary>
        /// Attempts to parse a field format specifier from the format string.
        /// </summary>
        protected bool ParseFormatSpecifier(TextParser format, FormatSpecifier spec)
        {
            // Return if not a field format specifier
            if (format.Peek() != '%')
                return false;
            format.MoveAhead();

            // Return if "%%" (treat as '%' literal)
            if (format.Peek() == '%')
                return false;

            // Test for asterisk, which indicates result is not stored
            if (format.Peek() == '*')
            {
                spec.NoResult = true;
                format.MoveAhead();
            }
            else spec.NoResult = false;

            // Parse width
            int start = format.Position;
            while (Char.IsDigit(format.Peek()))
                format.MoveAhead();
            if (format.Position > start)
                spec.Width = int.Parse(format.Extract(start, format.Position));
            else
                spec.Width = 0;

            // Parse modifier
            if (format.Peek() == 'h')
            {
                format.MoveAhead();
                if (format.Peek() == 'h')
                {
                    format.MoveAhead();
                    spec.Modifier = Modifiers.ShortShort;
                }
                else spec.Modifier = Modifiers.Short;
            }
            else if (Char.ToLower(format.Peek()) == 'l')
            {
                format.MoveAhead();
                if (format.Peek() == 'l')
                {
                    format.MoveAhead();
                    spec.Modifier = Modifiers.LongLong;
                }
                else spec.Modifier = Modifiers.Long;
            }
            else spec.Modifier = Modifiers.None;

            // Parse type
            switch (format.Peek())
            {
                case 'c':
                    spec.Type = Types.Character;
                    break;
                case 'd':
                case 'i':
                    spec.Type = Types.Decimal;
                    break;
                case 'a':
                case 'A':
                case 'e':
                case 'E':
                case 'f':
                case 'F':
                case 'g':
                case 'G':
                    spec.Type = Types.Float;
                    break;
                case 'o':
                    spec.Type = Types.Octal;
                    break;
                case 's':
                    spec.Type = Types.String;
                    break;
                case 'u':
                    spec.Type = Types.Unsigned;
                    break;
                case 'x':
                case 'X':
                    spec.Type = Types.Hexadecimal;
                    break;
                case '[':
                    spec.Type = Types.ScanSet;
                    format.MoveAhead();
                    // Parse scan set characters
                    if (format.Peek() == '^')
                    {
                        spec.ScanSetExclude = true;
                        format.MoveAhead();
                    }
                    else spec.ScanSetExclude = false;
                    start = format.Position;
                    // Treat immediate ']' as literal
                    if (format.Peek() == ']')
                        format.MoveAhead();
                    format.MoveTo(']');
                    if (format.EndOfText)
                        throw new Exception("Type specifier expected character : ']'");
                    spec.ScanSet = format.Extract(start, format.Position);
                    break;
                default:
                    string msg = String.Format("Unknown format type specified : '{0}'", format.Peek());
                    throw new Exception(msg);
            }
            format.MoveAhead();
            return true;
        }

        /// <summary>
        /// Parse a character field
        /// </summary>
        private bool ParseCharacter(TextParser input, FormatSpecifier spec)
        {
            // Parse character(s)
            int start = input.Position;
            int count = (spec.Width > 1) ? spec.Width : 1;
            while (!input.EndOfText && count-- > 0)
                input.MoveAhead();

            // Extract token
            if (count <= 0 && input.Position > start)
            {
                if (!spec.NoResult)
                {
                    string token = input.Extract(start, input.Position);
                    if (token.Length > 1)
                        Results.Add(token.ToCharArray());
                    else
                        Results.Add(token[0]);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parse integer field
        /// </summary>
        private bool ParseDecimal(TextParser input, FormatSpecifier spec)
        {
            int radix = 10;

            // Skip any whitespace
            input.MovePastWhitespace();

            // Parse leading sign
            int start = input.Position;
            if (input.Peek() == '+' || input.Peek() == '-')
            {
                input.MoveAhead();
            }
            else if (input.Peek() == '0')
            {
                if (Char.ToLower(input.Peek(1)) == 'x')
                {
                    radix = 16;
                    input.MoveAhead(2);
                }
                else
                {
                    radix = 8;
                    input.MoveAhead();
                }
            }

            // Parse digits
            while (IsValidDigit(input.Peek(), radix))
                input.MoveAhead();

            // Don't exceed field width
            if (spec.Width > 0)
            {
                int count = input.Position - start;
                if (spec.Width < count)
                    input.MoveAhead(spec.Width - count);
            }

            // Extract token
            if (input.Position > start)
            {
                if (!spec.NoResult)
                {
                    if (spec.Type == Types.Decimal)
                        AddSigned(input.Extract(start, input.Position), spec.Modifier, radix);
                    else
                        AddUnsigned(input.Extract(start, input.Position), spec.Modifier, radix);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parse a floating-point field
        /// </summary>
        private bool ParseFloat(TextParser input, FormatSpecifier spec)
        {
            // Skip any whitespace
            input.MovePastWhitespace();

            // Parse leading sign
            int start = input.Position;
            if (input.Peek() == '+' || input.Peek() == '-')
                input.MoveAhead();

            // Parse digits
            bool hasPoint = false;
            while (Char.IsDigit(input.Peek()) || input.Peek() == '.')
            {
                if (input.Peek() == '.')
                {
                    if (hasPoint)
                        break;
                    hasPoint = true;
                }
                input.MoveAhead();
            }

            // Parse exponential notation
            if (Char.ToLower(input.Peek()) == 'e')
            {
                input.MoveAhead();
                if (input.Peek() == '+' || input.Peek() == '-')
                    input.MoveAhead();
                while (Char.IsDigit(input.Peek()))
                    input.MoveAhead();
            }

            // Don't exceed field width
            if (spec.Width > 0)
            {
                int count = input.Position - start;
                if (spec.Width < count)
                    input.MoveAhead(spec.Width - count);
            }

            // Because we parse the exponential notation before we apply
            // any field-width constraint, it becomes awkward to verify
            // we have a valid floating point token. To prevent an
            // exception, we use TryParse() here instead of Parse().
            double result;

            // Extract token
            if (input.Position > start &&
                double.TryParse(input.Extract(start, input.Position), out result))
            {
                if (!spec.NoResult)
                {
                    if (spec.Modifier == Modifiers.Long ||
                        spec.Modifier == Modifiers.LongLong)
                        Results.Add(result);
                    else
                        Results.Add((float)result);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parse hexadecimal field
        /// </summary>
        protected bool ParseHexadecimal(TextParser input, FormatSpecifier spec)
        {
            // Skip any whitespace
            input.MovePastWhitespace();

            // Parse 0x prefix
            int start = input.Position;
            if (input.Peek() == '0' && input.Peek(1) == 'x')
                input.MoveAhead(2);

            // Parse digits
            while (IsValidDigit(input.Peek(), 16))
                input.MoveAhead();

            // Don't exceed field width
            if (spec.Width > 0)
            {
                int count = input.Position - start;
                if (spec.Width < count)
                    input.MoveAhead(spec.Width - count);
            }

            // Extract token
            if (input.Position > start)
            {
                if (!spec.NoResult)
                    AddUnsigned(input.Extract(start, input.Position), spec.Modifier, 16);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parse an octal field
        /// </summary>
        private bool ParseOctal(TextParser input, FormatSpecifier spec)
        {
            // Skip any whitespace
            input.MovePastWhitespace();

            // Parse digits
            int start = input.Position;
            while (IsValidDigit(input.Peek(), 8))
                input.MoveAhead();

            // Don't exceed field width
            if (spec.Width > 0)
            {
                int count = input.Position - start;
                if (spec.Width < count)
                    input.MoveAhead(spec.Width - count);
            }

            // Extract token
            if (input.Position > start)
            {
                if (!spec.NoResult)
                    AddUnsigned(input.Extract(start, input.Position), spec.Modifier, 8);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parse a scan-set field
        /// </summary>
        protected bool ParseScanSet(TextParser input, FormatSpecifier spec)
        {
            // Parse characters
            int start = input.Position;
            if (!spec.ScanSetExclude)
            {
                while (spec.ScanSet.Contains(input.Peek()))
                    input.MoveAhead();
            }
            else
            {
                while (!input.EndOfText && !spec.ScanSet.Contains(input.Peek()))
                    input.MoveAhead();
            }

            // Don't exceed field width
            if (spec.Width > 0)
            {
                int count = input.Position - start;
                if (spec.Width < count)
                    input.MoveAhead(spec.Width - count);
            }

            // Extract token
            if (input.Position > start)
            {
                if (!spec.NoResult)
                    Results.Add(input.Extract(start, input.Position));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parse a string field
        /// </summary>
        private bool ParseString(TextParser input, FormatSpecifier spec)
        {
            // Skip any whitespace
            input.MovePastWhitespace();

            // Parse string characters
            int start = input.Position;
            while (!input.EndOfText && !Char.IsWhiteSpace(input.Peek()))
                input.MoveAhead();

            // Don't exceed field width
            if (spec.Width > 0)
            {
                int count = input.Position - start;
                if (spec.Width < count)
                    input.MoveAhead(spec.Width - count);
            }

            // Extract token
            if (input.Position > start)
            {
                if (!spec.NoResult)
                    Results.Add(input.Extract(start, input.Position));
                return true;
            }
            return false;
        }

        // Determines if the given digit is valid for the given radix
        private bool IsValidDigit(char c, int radix)
        {
            int i = "0123456789abcdef".IndexOf(Char.ToLower(c));
            if (i >= 0 && i < radix)
                return true;
            return false;
        }

        // Parse signed token and add to results
        private void AddSigned(string token, Modifiers mod, int radix)
        {
            object obj;
            if (mod == Modifiers.ShortShort)
                obj = Convert.ToSByte(token, radix);
            else if (mod == Modifiers.Short)
                obj = Convert.ToInt16(token, radix);
            else if (mod == Modifiers.Long ||
                mod == Modifiers.LongLong)
                obj = Convert.ToInt64(token, radix);
            else
                obj = Convert.ToInt32(token, radix);
            Results.Add(obj);
        }

        // Parse unsigned token and add to results
        private void AddUnsigned(string token, Modifiers mod, int radix)
        {
            object obj;
            if (mod == Modifiers.ShortShort)
                obj = Convert.ToByte(token, radix);
            else if (mod == Modifiers.Short)
                obj = Convert.ToUInt16(token, radix);
            else if (mod == Modifiers.Long ||
                mod == Modifiers.LongLong)
                obj = Convert.ToUInt64(token, radix);
            else
                obj = Convert.ToUInt32(token, radix);
            Results.Add(obj);
        }


        private string JumpToText(string input, string format, string textToJump = null)
        {
  
            int firstPercentPosition = format.IndexOf("%");
            if (firstPercentPosition > 0)
            {
                var prefix = format.Substring(0, firstPercentPosition);
                int matchPos = input.IndexOf(prefix);
                if (matchPos >= 0)
                    return input.Substring(matchPos).TrimStart();
                else
                    return input;
            }
            return input;
        }  
    }
}
