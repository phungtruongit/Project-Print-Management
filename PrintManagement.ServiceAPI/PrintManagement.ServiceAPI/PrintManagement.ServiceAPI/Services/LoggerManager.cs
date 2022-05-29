using Microsoft.Extensions.Logging;
using System;

namespace PrintManagement.ServiceAPI.Services {
    public class LoggerManager : ILogger {
        private static NLog.ILogger logger = NLog.LogManager.GetCurrentClassLogger();
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

        public IDisposable BeginScope<TState>(TState state) => throw new NotSupportedException();

        public bool IsEnabled(LogLevel logLevel) => logger.IsEnabled(translateNLogLevels(logLevel));

        private static NLog.LogLevel translateNLogLevels(LogLevel logLevel)
            => logLevel switch {
                LogLevel.Debug => NLog.LogLevel.Debug,
                LogLevel.Error => NLog.LogLevel.Error,
                LogLevel.Critical => NLog.LogLevel.Fatal,
                LogLevel.Information => NLog.LogLevel.Info,
                LogLevel.Trace => NLog.LogLevel.Trace,
                LogLevel.Warning => NLog.LogLevel.Warn,
                LogLevel.None => NLog.LogLevel.Off,
                _ => throw new NotImplementedException()
            };

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            => logger.Log<TState>(translateNLogLevels(logLevel), state);
    }
}