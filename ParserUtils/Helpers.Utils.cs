using System;
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
    }


    public static class WindowHelper
    {
        public static Process ViewFileInNotepad(string fileToOpen, bool isModal = false)
        {
            if (!File.Exists(fileToOpen)) return null;

            var process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = "notepad.exe",
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


}
