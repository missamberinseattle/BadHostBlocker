using System;
using System.Collections.Generic;
using System.Text;
using Argosoft.Plugins;
using System.IO;
using System.Text.RegularExpressions;

namespace BadHostTestHarness
{
    public class TestMail
    {
        public TestMail(string ipAddress, string mailFrom, List<string> recipients, StringBuilder msgData)
        {
            IpAddress = ipAddress;
            MailFrom = mailFrom;
            Recipients = recipients;
            MessageData = msgData;
        }

        public string IpAddress;
        public string MailFrom;
        public List<string> Recipients;
        public StringBuilder MessageData;

        public PostProcessResult PostProcessResponse;
        public string Reply;

        public static List<TestMail> LoadTestMails(string sourceDir)
        {
            var files = Directory.GetFiles(sourceDir);
            var emails = new List<TestMail>();

            foreach (var file in files)
            {
                var msgData = new StringBuilder(File.ReadAllText(file));
                emails.Add(TestMail.Parse(msgData));
            }

            return emails;
        }

        private static TestMail Parse(StringBuilder msgData)
        {
            var lines = msgData.ToString().Replace("\n\t", " ").Replace("\r", "").Split(new[] { '\n' });
            var rawMail = msgData.ToString();
            var ipAddress = GetHeader(lines, "X-FromIP");
            var mailFrom = GetAddress(GetHeader(lines, "From"));
            var recipients = new List<string>();
            recipients.Add(GetAddress(GetHeader(lines, "To")));

            var mail = new TestMail(ipAddress, mailFrom, recipients, msgData);

            return mail;
        }

        private static string GetAddress(string address)
        {
            if (address == null)
            {
                return null;
            }
            const string emailPattern = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
            var match = Regex.Match(address, emailPattern);

            return match.Value;
        }

        private static string GetHeader(string[] lines, string name)
        {
            string result = null;

            foreach (var line in lines)
            {
                if (line.StartsWith(name + ":"))
                {
                    var colPos = line.IndexOf(":");

                    if (colPos > -1)
                    {
                        result = line.Substring(colPos + 1).Trim();
                        break;
                    }

                    if (line.Length == 0)
                    {
                        break;
                    }
                }
            }

            return result;
        }
    }
}
