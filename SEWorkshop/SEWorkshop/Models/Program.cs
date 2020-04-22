using NLog;
using SEWorkshop.Exceptions;
using System;

namespace SEWorkshop.Models
{
    class Program
    {
        private const string LOG_FILE_NM = "log.txt";

        private static void ConfigLog()
        {
            var config = new NLog.Config.LoggingConfiguration();
            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = LOG_FILE_NM };
            // Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            // Apply config           
            NLog.LogManager.Configuration = config;
        }

        static void Main(string[] args)
        {
            ConfigLog();
        }
    }
}