using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public static class Utils
    {
        public static bool ContainsCaseInsensitive(this string text, string value,
                StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            return text.IndexOf(value, stringComparison) >= 0;
        }

        public static bool ToBoolean(this string input)
        {
            //Define the false keywords
            String[] bFalse = { "false", "0", "off", "no" };

            //Return false for any of the false keywords or an empty/null value
            if (string.IsNullOrEmpty(input) || bFalse.Contains(input.ToLower()))
                return false;

            //Return true for anything not false
            return true;
        }

        public static Color ToColor(this string colorcode)
        {
            //string colorcode = "#FFFFFF00";
            //int argb = Int32.Parse(colorcode.Replace("#", ""), NumberStyles.HexNumber);
            if (Int32.TryParse(colorcode.Replace("#", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int result))
                return Color.FromArgb(result);
            else
                return Color.Transparent;
        }

        public static Color DarkerColor(Color color, float correctionfactor = 50f)
        {
            const float hundredpercent = 100f;
            correctionfactor = hundredpercent - correctionfactor;
            return Color.FromArgb((int)((color.R / hundredpercent) * correctionfactor),
                (int)((color.G / hundredpercent) * correctionfactor), (int)((color.B / hundredpercent) * correctionfactor));
        }

        public static Color LighterColor(Color color, float correctionfactor = 50f)
        {
            correctionfactor = correctionfactor / 100f;
            const float rgb255 = 255f;
            return Color.FromArgb((int)(color.R + ((rgb255 - color.R) * correctionfactor)), (int)(color.G + ((rgb255 - color.G) * correctionfactor)), (int)(color.B + ((rgb255 - color.B) * correctionfactor))
                );
        }
    }


}
