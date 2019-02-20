using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProcessor
{
    public static class Worker
    {
        public static Worker()
        {
        }

        public static LogCollection ParseLog(string filePath)
        {
            LogCollection log = new LogCollection();

            using (TextReader text = new StreamReader(filePath))
            {
                while (text.Peek() > 0)
                {
                    string line = text.ReadLine();

                    var tsvIndex = line.IndexOf("\tTSV\t");

                    if (tsvIndex == -1) {
                        continue;
                    }
                }

                text.Close();
            }

            return log;
        }
    }