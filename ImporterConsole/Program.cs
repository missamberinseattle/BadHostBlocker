using System;
using System.Collections.Generic;
using System.Text;
using CatalogImporter;
using System.IO;
using System.Threading;

namespace ImporterConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var importer = new Importer();
            var catalogPath = importer.CatalogPath;
            var lastUpdate = DateTime.MinValue;
            var doLoop = true;

            while (doLoop)
            {
                var fileInfo = new FileInfo(catalogPath);

                if (fileInfo.LastWriteTime.CompareTo(lastUpdate) > 0) 
                {
                    Importer.Echo("Detected catalog change, sleeping for 10 seconds");
                    SleepFor(10);
                    importer.Run();

                    lastUpdate = DateTime.Now;
                }
                else
                {
                    Console.WriteLine("No updates"); 
                }
                SleepFor(300);
            }
        }

        private static void SleepFor(int seconds)
        {
            const string format = "Sleeping for {0}...";

            for (var xx = seconds; xx > 0; xx--)
            {
                var message = string.Format(format, xx);
                Console.Write(message);
                Thread.Sleep(1000);
                foreach(var character in message)
                {
                    Console.Write((char)8);
                    Console.Write(" ");
                    Console.Write((char)8);
                }
            }
        }
    }
}
