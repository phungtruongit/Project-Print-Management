using System;

namespace PrintManagement.ClientApp {
    public interface ILoggerManager {
        void LogInfo(Exception ex, string message);
        void LogWarn(Exception ex, string message);
        void LogDebug(Exception ex, string message);
        void LogError(Exception ex, string message);
        void LogFatal(Exception ex, string message);
        void LogTrace(Exception ex, string message);

        void LogInfo(string message);
        void LogWarn(string message);
        void LogDebug(string message);
        void LogError(string message);
        void LogFatal(string message);
        void LogTrace(string message);
    }
}
