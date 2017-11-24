using log4net;
using System;

namespace Plus.Core
{
    public static class ExceptionLogger
    {
        private static readonly ILog _sqlLogger = LogManager.GetLogger("MySQL");
        private static readonly ILog _threadLogger = LogManager.GetLogger("Thread");
        private static readonly ILog _exceptionLogger = LogManager.GetLogger("Exception");
        private static readonly ILog _criticalExceptionLogger = LogManager.GetLogger("Critical");
        private static readonly ILog _wiredLogger = LogManager.GetLogger("Wired");

        public static void LogQueryError(string query, Exception exception)
        {
            _sqlLogger.Error("Error in query:\r\n" + query + "\r\n" + exception + "\r\n\r\n");
        }

        public static void LogException(Exception exception)
        {
            _exceptionLogger.ErrorFormat("Exception:\r\n" + exception + "\r\n\r\n");
        }

        public static void LogCriticalException(Exception exception)
        {
            _criticalExceptionLogger.Error("Critical Exception:\r\n" + exception + "\r\n\r\n");
        }

        public static void LogThreadException(Exception exception)
        {
            _threadLogger.Error("Thread Exception:\r\n" + exception + "\r\n\r\n");
        }

        public static void LogWiredException(Exception exception)
        {
            _wiredLogger.Error("Wired Exception:\r\n" + exception + "\r\n\r\n");
        }
    }
}
