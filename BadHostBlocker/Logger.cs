using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace BadHostBlocker
{
    public static class Logger
    {
        private static object _padlock = new object();
        private static string _logfileroot;
        private static string _currentLogName;
        private static DateTime _currentLogNameSetAt = DateTime.MinValue;

        public const int MaxLogAge = 30;

        public static void Log(string message)
        {
            Log(string.Empty, message);
        }

        public static void Log(string prefix, string message)
        {
            try
            {
                Trace.WriteLine(message);
                var logFilePath = GetLogFilePath();

                lock (_padlock)
                {
                    using (TextWriter logWriter = new StreamWriter(logFilePath, true))
                    {
                        var lines = message.Replace("\r", "").Split(new[] { '\n' });
                        for (var xx = 0; xx < lines.Length; xx++)
                        {
                            logWriter.Write(DateTime.Now.ToString("HH:mm:ss.ffff"));
                            logWriter.Write("\t{0}\t", prefix);
                            logWriter.WriteLine(lines[xx]);
                        }
                        logWriter.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.Write(ex);
            }
        }

        private static string GetLogFilePath()
        {
            if (_logfileroot == null)
            {
                var assembly = Assembly.GetEntryAssembly();

                if (assembly != null)
                {
                    var assemblyName = assembly.GetName();
                    _logfileroot = assemblyName.Name;
                }
                else
                {
                    _logfileroot = "Logger";
                }
            }

            if (_currentLogName == null || _currentLogNameSetAt < DateTime.Today)
            {
                var name = _logfileroot + "_" + DateTime.Today.ToString("yyyyMMdd") + ".log";

                _currentLogName = Path.Combine(BadHostPlugin.RootPath, name);
                _currentLogNameSetAt = DateTime.Today;

                Utilities.ClearOldFiles(BadHostPlugin.RootPath, _logfileroot + "*.log", MaxLogAge);
            }
            
            return _currentLogName;
        }
    }
}
