﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        
        public static object GetConfigValue<T>(string configKey)
        {
            var reader = new AppSettingsReader();
            var appSettings = ConfigurationManager.AppSettings;
            for (int i = 0; i < appSettings.Count; i++)
            {
                var key = appSettings.GetKey(i);
                if (string.Equals(key, configKey, StringComparison.InvariantCultureIgnoreCase))
                {
                    var value = reader.GetValue(key, typeof(T));
                    object result;
                    try
                    {
                        result = (T)value;
                    }
                    catch
                    {
                        result = default(T);
                    }
                    return result;
                }
            }
            return null;
        }
        
        public static void UpdateConfig(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            
            if (config.AppSettings.Settings[key] != null)
                config.AppSettings.Settings[key].Value = value;
            else
                config.AppSettings.Settings.Add(key, value);

            config.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("appSettings");
        }
    }


    public static class WindowHelper
    {
        public static Process ViewFileInExternalEditor(string externalEditorExecutablePath, string fileToOpen, bool isModal = false)
        {
            if (!File.Exists(fileToOpen)) return null;

            var process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = externalEditorExecutablePath,
                Arguments = fileToOpen
            };

            process.Start();
            if (isModal)
                process.WaitForExit();

            return process;

        }

        public static void BringProcessToFront(Process process)
        {
            IntPtr handle = process.MainWindowHandle;
            if (IsIconic(handle))
            {
                ShowWindow(handle, SW_RESTORE);
            }

            SetForegroundWindow(handle);
        }

        const int SW_RESTORE = 9;

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);
    }

    public static class Prompt
    {
        public static string ShowDialog(string text, string headerText, string caption)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterParent
            };
            //Label textLabel = new Label() { Left = 50, Top = 20, Text = headerText };
            TextBox textBox = new TextBox() { Left = 50, Top = 20, Width = 400 };
            textBox.Text = text;
            textBox.Multiline = true;
            textBox.WordWrap = true;
            textBox.ReadOnly = true;
            Button confirmation = new Button() { Text = "OK", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            //prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }


    /// <summary>
    /// Provides extension methods for strings.
    /// </summary>
    public static class StringExt
    {
        /// <summary>
        /// Forces the string to word wrap so that each line doesn't exceed the maxLineLength.
        /// </summary>
        /// <param name="str">The string to wrap.</param>
        /// <param name="maxLength">The maximum number of characters per line.</param>
        /// <returns></returns>
        public static string Wrap(this string str, int maxLength)
        {
            return Wrap(str, maxLength, "");
        }

        /// <summary>
        /// Forces the string to word wrap so that each line doesn't exceed the maxLineLength.
        /// </summary>
        /// <param name="str">The string to wrap.</param>
        /// <param name="maxLength">The maximum number of characters per line.</param>
        /// <param name="prefix">Adds this string to the beginning of each line.</param>
        /// <returns></returns>
        public static string Wrap(this string str, int maxLength, string prefix)
        {
            if (string.IsNullOrEmpty(str)) return "";
            if (maxLength <= 0) return prefix + str;

            var lines = new List<string>();

            // breaking the string into lines makes it easier to process.
            foreach (string line in str.Split("\n".ToCharArray()))
            {
                var remainingLine = line.Trim();
                do
                {
                    var newLine = GetLine(remainingLine, maxLength - prefix.Length);
                    lines.Add(newLine);
                    remainingLine = remainingLine.Substring(newLine.Length).Trim();
                    // Keep iterating as int as we've got words remaining 
                    // in the line.
                } while (remainingLine.Length > 0);
            }

            return string.Join(Environment.NewLine + prefix, lines.ToArray());
        }
        private static string GetLine(string str, int maxLength)
        {
            // The string is less than the max length so just return it.
            if (str.Length <= maxLength) return str;

            // Search backwords in the string for a whitespace char
            // starting with the char one after the maximum length
            // (if the next char is a whitespace, the last word fits).
            for (int i = maxLength; i >= 0; i--)
            {
                if (char.IsWhiteSpace(str[i]))
                    return str.Substring(0, i).TrimEnd();
            }

            // No whitespace chars, just break the word at the maxlength.
            return str.Substring(0, maxLength);
        }
    }


}
