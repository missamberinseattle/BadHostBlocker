using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;

namespace BadHostBlocker
{
    public static class DataTools
    {
        public static ListState GetAttributeValue(XmlNode node, string name, ListState defaultValue)
        {
            if (node == null) return defaultValue;

            var attribute = node.Attributes.GetNamedItem(name);

            if (attribute == null) return defaultValue;

            var rawValue = attribute.Value;
            var result = defaultValue;

            try 
            {
                result = (ListState) Enum.Parse(typeof(ListState), rawValue);
            }
            catch 
            {
                // Do nothing
            }

            return result;
        }

        public static IPAddress GetAttributeValue(XmlNode node, string name, IPAddress defaultValue)
        {
            if (node == null) return defaultValue;

            var attribute = node.Attributes.GetNamedItem(name);

            if (attribute == null) return defaultValue;

            var rawValue = attribute.Value;
            IPAddress address;

            if (!IPAddress.TryParse(rawValue, out address))
            {
                return defaultValue;
            }

            return address;
        }

        public static T GetAttributeValue<T>(XmlNode node, string name, T defaultValue)
        {
            if (node == null) return defaultValue;

            var attribute = node.Attributes.GetNamedItem(name);

            if (attribute == null) return defaultValue;

            var rawValue = attribute.Value;
            var result = defaultValue;

            try
            {
                result = (T)Convert.ChangeType(rawValue, typeof(T));
            }
            catch
            {
                // Do nothing
            }

            return result;
        }
    }
}
