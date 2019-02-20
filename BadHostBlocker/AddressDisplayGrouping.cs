using System;
using System.Collections.Generic;
using System.Text;

namespace BadHostBlocker
{
    public class AddressDisplayGrouping
    {
        public AddressDisplayGrouping(string address)
        {
            Address = address;
            DisplayNames = new List<string>();
        }

        public string Address { get; private set; }
        public List<string> DisplayNames { get; private set; }

        public override string ToString()
        {
            var output = new StringBuilder();
            output.Append(Address);
            output.Append(" { ");

            for (var xx = 0; xx < DisplayNames.Count; xx++)
            {
                if (xx > 0)
                {
                    output.Append("; ");
                }

                output.Append(DisplayNames[xx]);
            }

            output.Append(" }");

            return output.ToString();
        }

        public static bool TryParse(string text, out AddressDisplayGrouping adGroup, out string failReason)
        {
            failReason = null;
            adGroup = null;

            if (string.IsNullOrEmpty(text))
            {
                failReason = "Source text is null or empty";
                return false;
            }

            var equalPos = text.IndexOf("=");
            if (equalPos == 0)
            {
                failReason = "Bad format. Text should be address=display1;display2";
                return false;
            }

            var address = text.Substring(0, equalPos);
            adGroup = new AddressDisplayGrouping(address);

            var displayNames = text.Substring(equalPos + 1).Split(new[] { ';' },
                StringSplitOptions.RemoveEmptyEntries);

            adGroup.DisplayNames.AddRange(displayNames);

            return true;
        }
    }
}
