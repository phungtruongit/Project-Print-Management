using NLog;
using System;

namespace PrintManagement.ClientApp {
    public class LoggerManager : ILoggerManager {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        public void LogDebug(Exception ex, string message) => logger.Debug(ex, message);
        public void LogError(Exception ex, string message) => logger.Error(ex, message);
        public void LogInfo(Exception ex, string message) => logger.Info(ex, message);
        public void LogWarn(Exception ex, string message) => logger.Warn(ex, message);
        public void LogFatal(Exception ex, string message) => logger.Fatal(ex, message);
        public void LogTrace(Exception ex, string message) => logger.Trace(ex, message);
        public void LogDebug(string message) => logger.Debug(message);
        public void LogError(string message) => logger.Error(message);
        public void LogInfo(string message) => logger.Info(message);
        public void LogWarn(string message) => logger.Warn(message);
        public void LogFatal(string message) => logger.Fatal(message);
        public void LogTrace(string message) => logger.Trace(message);
    }
}
