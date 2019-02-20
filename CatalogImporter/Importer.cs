using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Net;
using BadHostBlocker;
using System.Threading;

namespace CatalogImporter
{
    public class Importer
    {
        private string _catalogPath;

        public bool StopProcessing { get; set; }
        public double MaxHostNameAge
        {
            get
            {
                return 30;
            }
        }

        public string CatalogPath
        {
            get
            {
                if (_catalogPath == null)
                {
                    _catalogPath = ConfigurationManager.AppSettings["IpCatalog"];
                }

                return _catalogPath;
            }
        }

        public void Run()
        {
            // Do initial run
            ProcessCatalog();
            // Set up FileWatcher
            // Sleep until done
        }

        private void ProcessCatalog()
        {
            var catalog = LoadCatalog();
            var toProcess = 1;
            var batchId = 0;

            while (toProcess > 0)
            {
                batchId++;
                toProcess = ResolveHostNames(catalog, batchId);
                SaveCatalog(catalog);
                Echo("Sleeping for 60 seconds...");
                Thread.Sleep(60000);
            }

        }

        private void SaveCatalog(List<IpCatalogItem> catalog)
        {
            IpCatalogItem.Save(catalog, CatalogPath);
        }

        private int ResolveHostNames(List<IpCatalogItem> catalog, int batchId = 0)
        {
            var processCount = 0;
            var batchSize = 100;
            var toProcess = 0;
            var echoStop = batchSize / 5;

            for (var xx = 0; xx < catalog.Count; xx++)
            {
                string message;
                var ip = catalog[xx];
                var entryAge = DateTime.Now.Subtract(ip.Modified);
                toProcess = catalog.Count - xx;
                if (ip.HostName == null || entryAge.TotalDays > MaxHostNameAge)
                {
                    message = string.Format("BatchItem {2}-{3}: Resolving {0} of {1}", xx + 1, catalog.Count, batchId, processCount);
                    Echo(message);

                    ip.ResolveHostName();

                    if (ip.HostName != null && ip.HostName != ip.IP.ToString())
                    {
                        message = "   " + ip.IP + " -> " + ip.HostName;
                        if (ip.HostAliases != null && ip.HostAliases.Length > 0)
                        {
                            message += " (";
                            for (var aa = 0; aa < ip.HostAliases.Length; aa++)
                            {
                                if (aa > 0) message += "; ";
                                message += ip.HostAliases[aa];
                            }
                            message += ")";
                        }
                        processCount++;
                    }
                    else if (ip.HostName == null)
                    {
                        message = "   skipped " + ip.IP;
                    }
                    else
                    {
                        message = "   no host records for " + ip.IP;
                    }
                }
                else
                {
                    message = string.Format("Skipping {0} ({1} of {2})", ip, xx + 1, catalog.Count);
                }

                if (xx % echoStop == echoStop - 1)
                {
                    Echo(message);
                }

                if (processCount >= batchSize )
                {
                    Echo("Processed batch " + batchId + ", " + batchSize + " records");
                    break;
                }
            }

            return toProcess - 1;
        }

        private List<IpCatalogItem> LoadCatalog()
        {
            var catalog = IpCatalogItem.LoadCatalog(CatalogPath);
            catalog.Sort();
            return catalog;
        }

        public static void Echo(string text)
        {
            Logger.Log(text);
            Console.WriteLine(text);
        }
    }
}
