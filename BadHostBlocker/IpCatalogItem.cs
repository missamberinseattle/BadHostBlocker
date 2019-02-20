using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;

namespace BadHostBlocker
{
    public class IpCatalogItem : IComparable
    {
        private readonly IPAddress _defaultIp = new IPAddress(new byte[] { 0, 0, 0, 0 });
        private readonly DateTime _defaultDate = new DateTime(1900, 1, 1);

        private static List<IpCatalogItem> _cachedCatalog = null;

        public IpCatalogItem()
        {
            ParseHosts(string.Empty);
        }

        public IpCatalogItem(XmlNode ip)
        {
            // <ip address="198.23.211.254" messageCount="1" firstSeen="9/9/2014 2:50:43 PM" lastSeen="9/9/2014 2:50:43 PM"/>
            var address = DataTools.GetAttributeValue(ip, "address", _defaultIp);
            var messageCount = DataTools.GetAttributeValue(ip, "messageCount", -1);
            var firstSeen = DataTools.GetAttributeValue(ip, "firstSeen", _defaultDate);
            var lastSeen = DataTools.GetAttributeValue(ip, "lastSeen", _defaultDate);
            var modified = DataTools.GetAttributeValue(ip, "modified", _defaultDate);
            var state = DataTools.GetAttributeValue(ip, "state", ListState.None);
            var host = DataTools.GetAttributeValue(ip, "host", string.Empty);

            Init(address, messageCount, firstSeen, lastSeen, host, state, modified);
            
            ParseHosts(host);
        }

        public IpCatalogItem(IPAddress ip, int messageCount, DateTime firstSeen, DateTime lastSeen, string hostname, ListState state, DateTime modified)
        {
            Init(ip, messageCount, firstSeen, lastSeen, hostname, state, modified);
        }

        private void Init(IPAddress ip, int messageCount, DateTime firstSeen, DateTime lastSeen, string hostName, ListState state, DateTime modified)
        {
            IP = ip;
            MessageCount = messageCount;
            FirstSeen = firstSeen;
            LastSeen = lastSeen;
            State = state;
            Modified = modified;
            ParseHosts(hostName);
        }

        public bool IsValid
        {
            get
            {
                var isValid = true;
                var test = IP.ToString();

                if (test == "0.0.0.0" || test == "127.0.0.1" || test == "::1")
                {
                    isValid = false;
                }

                return isValid;
            }
        }

        public IPAddress IP { get; set; }

        public int MessageCount { get; set; }

        public DateTime FirstSeen { get; set; }

        public DateTime LastSeen { get; set; }

        public string HostName { get; set; }

        public string[] HostAliases { get; private set; }

        public ListState State { get; set; }

        public DateTime Modified { get; set; }

        private static List<IpCatalogItem> CachedCatalog
        {
            get
            {
                return _cachedCatalog;
            }
            set
            {
                _cachedCatalog = value;
            }
        }

        public void ParseHosts(string hosts)
        {
            var aliases = new List<string>();

            if (string.IsNullOrEmpty(hosts))
            {
                HostName = string.Empty;
                HostAliases = aliases.ToArray();
                return;
            }

            var names = hosts.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            HostName = names[0];

            for (var xx = 1; xx < names.Length; xx++)
            {
                aliases.Add(names[xx]);
            }

            HostAliases = aliases.ToArray();
        }

        private string GetHostList()
        {
            var list = new StringBuilder();

            list.Append(HostName);
            if (HostAliases != null && HostAliases.Length > 0)
            {
                foreach (var alias in HostAliases)
                {
                    list.Append(";");
                    list.Append(alias);
                }
            }

            return list.ToString();
        }

        public string ToXml()
        {
            return string.Format("<ip address=\"{0}\" messageCount=\"{1}\" firstSeen=\"{2}\" lastSeen=\"{3}\" host=\"{4}\" state=\"{5}\" modified=\"{6}\" />",
                    IP, MessageCount, FirstSeen, LastSeen, GetHostList(), State, Modified);
        }

        public void ResolveHostName()
        {
            HostName = null;
            HostAliases = null;

            if (IP == null) return;

            try
            {
                IPHostEntry host;

                host = Dns.GetHostEntry(IP);

                HostName = host.HostName;
                HostAliases = host.Aliases;
            }
            catch (Exception ex)
            {
                Utilities.Echo("Could not result " + IP.ToString());
                Utilities.Echo("Exception: " + ex.Message);

                HostName = IP.ToString();
            }

            Modified = DateTime.Now;
        }

        public override string ToString()
        {
            var output = new StringBuilder();

            output.AppendFormat(" {{ {0}/{1}; ", IP, HostName);

            if (HostAliases == null || HostAliases.Length == 0)
            {
                output.Append(" -");
            }
            else
            {
                for (var xx = 0; xx < HostAliases.Length; xx++)
                {
                    if (xx > 0)
                    {
                        output.Append("/");
                    }
                    output.Append(HostAliases[xx]);
                }
            }

            output.AppendFormat("; {0}; {1}; {2}; {3}", State, MessageCount, FirstSeen, LastSeen);


            output.Append(" }");
            return output.ToString();
        }

        public static List<IpCatalogItem> LoadCatalog(string path)
        {
            var rawCatalog = new XmlDocument();

            Logger.Log("Loading catalog from " + path);

            try
            {
                rawCatalog.Load(path);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not load catalog " + path, ex);
            }

            var ipElements = rawCatalog.GetElementsByTagName("ip");
            var ipList = new List<IpCatalogItem>();

            foreach (XmlNode ip in ipElements)
            {
                var item = new IpCatalogItem(ip);
                ipList.Add(item);
                // Utilities.Echo("Loaded: " + item.ToString());
            }

            ipList.Sort();
            Utilities.Echo("Loaded " + ipList.Count + " IP addresses");
            CachedCatalog = ipList;

            return ipList;
        }

        public static void Save(List<IpCatalogItem> catalog, string path)
        {
            var rawXml = new StringBuilder();

            rawXml.AppendLine("<catalog>");

            foreach (var item in catalog)
            {
                if (item.IsValid)
                {
                    rawXml.AppendLine("\t" + item.ToXml());
                }
            }

            rawXml.AppendLine("</catalog>");

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rawXml.ToString());

            xmlDoc.Save(path);
        }

        public static IpCatalogItem Find(string ipAddress, List<IpCatalogItem> catalog)
        {
            return Find(ipAddress, catalog, 0, catalog.Count - 1);
        }

        public static IpCatalogItem Find(string ipAddress, List<IpCatalogItem> catalog, int lowerBound, int upperBound)
        {
            if (upperBound < lowerBound)
            {
                return null;
            }

            if (lowerBound == upperBound && catalog[lowerBound].IP.ToString() == ipAddress)
            {
                return catalog[lowerBound];
            }

            if (lowerBound == upperBound)
            {
                return null;
            }

            // Get the middle value of the range
            // (0, 10) = (10 - 0) / 2 + 0 = 10 / 2 + 0 = 5 + 0 = 5
            // (5, 10) = (10 - 5) / 2 + 0 = 5 / 2 + 5 = 2 + 5 = 7
            // (0, 88) = (88 - 0) / 2 + 0 = 88 / 2 + 0 = 44 + 0 = 44
            // (44, 88) = (88 - 44) / 2 + 44 = 44 / 2 + 44 = 22 + 44 = 66
            // (44, 66) = (66 - 44) / 2 + 44 = 22 / 2 + 44 = 11 + 44 = 55

            var middleIndex = (upperBound - lowerBound) / 2 + lowerBound;
            var middleValue = catalog[middleIndex].IP.ToString();

            if (ipAddress == middleValue)
            {
                return catalog[middleIndex];
            }

            if (ipAddress.CompareTo(middleValue) < 0)
            {
                return Find(ipAddress, catalog, lowerBound, middleIndex - 1);
            }

            if (ipAddress.CompareTo(middleValue) > 0)
            {
                return Find(ipAddress, catalog, middleIndex + 1, upperBound);
            }

            return null;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            if (obj is IpCatalogItem)
            {
                return CompareTo((IpCatalogItem)obj);
            }

            return 1;
        }

        public int CompareTo(IpCatalogItem ipItem)
        {
            if (ipItem == null) return 1;
            if (ipItem.IP == null) return 1;

            var thisIp = IP.ToString();
            var thatIp = ipItem.IP.ToString();

            var result = thisIp.CompareTo(thatIp);

            return result;
        }
    }
}
