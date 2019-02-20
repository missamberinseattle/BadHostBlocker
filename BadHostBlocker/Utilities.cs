using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.IO;

namespace BadHostBlocker
{
    public static class Utilities
    {
        internal static void Echo(string message)
        {
            Console.WriteLine(message);
        }

        public static bool IsBase64String(string s)
        {
            s = s.Replace("\r\n", "").Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        public static string Base64Decode(string value)
        {
            if (!IsBase64String(value)) return value;

            var bytes = Convert.FromBase64String(value);
            string decodedString = Encoding.UTF8.GetString(bytes);

            return decodedString;
        }

        public static MailAddress ParseMailAddress(string text)
        {
            try
            {
                text = text.Replace("\"", ""); // strip out the quotes
                return new MailAddress(text);
            }
            catch (Exception ex)
            {
                Logger.Log("Could not resolve \"" + text + "\" to an e-mail address. " + ex.Message);
                return null;
            }
        }

        public static void ClearOldFiles(string directory, string pattern, int maxLogAge)
        {
            var files = Directory.GetFiles(directory, pattern);
            var oldest = DateTime.Now.AddDays(maxLogAge * -1);

            for(var xx = 0; xx < files.Length; xx++)
            {
                try
                {
                    var fileInfo = new FileInfo(files[xx]);

                    if (fileInfo.LastWriteTime < oldest)
                    {
                        File.Delete(files[xx]);
                    }
                }
                catch (IOException ex)
                {
                    Logger.Log("ClearOldFiles", "Could not delete old file; " + files[xx]);
                    Logger.Log("ClearOldFiles", ex.Message);
                }
            }
        }
    }
}
