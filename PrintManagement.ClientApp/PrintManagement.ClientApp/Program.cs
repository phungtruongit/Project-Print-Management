using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrintManagement.ClientApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ConfigureNLog();
            Application.Run(new FormLogin(new LoggerManager()));
        }

        private static void ConfigureNLog()
        {
            var config = new LoggingConfiguration();
            //File
            var fileTarget = new FileTarget
            {
                Name = "file",
                Layout = @"${longdate} - ${level:uppercase=true}:${newline}${message:truncate=200}...TRUNCATED${onexception:${newline}EXCEPTION\: ${exception:format=ToString:truncate=200}...TRUNCATED}${newline}",
                FileName = $@"{AppDomain.CurrentDomain.BaseDirectory}\Debug.log",
                KeepFileOpen = false,
                ArchiveFileName = "Debug_${shortdate}.{##}.log",
                ArchiveNumbering = ArchiveNumberingMode.Sequence,
                ArchiveEvery = FileArchivePeriod.Day,
                MaxArchiveFiles = 30,
            };
            ////Event
            //var eventLogTarget = new EventLogTarget {
            //    Name = "eventlog",
            //    Source = "${appName}",
            //    Layout = "${message}${newline}${exception:format=ToString}",
            //};

            config.AddTarget("logfile", fileTarget);
            //config.AddTarget("eventlog", eventLogTarget);
            var rule = new LoggingRule("*", LogLevel.Trace, fileTarget);
            config.LoggingRules.Add(rule);
            LogManager.Configuration = config;
        }
    }
}
