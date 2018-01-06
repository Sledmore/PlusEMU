using log4net;
using System;

namespace Plus.Core
{
    public static class ExceptionLogger
    {
        private static readonly ILog SqlLogger = LogManager.GetLogger("MySQL");
        private static readonly ILog ThreadLogger = LogManager.GetLogger("Thread");
        private static readonly ILog DefaultLogger = LogManager.GetLogger("Exception");
        private static readonly ILog CriticalExceptionLogger = LogManager.GetLogger("Critical");
        private static readonly ILog WiredLogger = LogManager.GetLogger("Wired");

        public static void LogQueryError(string query, Exception exception)
        {
            SqlLogger.Error("Error in query:\r\n" + query + "\r\n" + exception + "\r\n\r\n");
        }

        public static void LogException(Exception exception)
        {
            DefaultLogger.ErrorFormat("Exception:\r\n" + exception + "\r\n\r\n");
        }

        public static void LogCriticalException(Exception exception)
        {
            CriticalExceptionLogger.Error("Critical Exception:\r\n" + exception + "\r\n\r\n");
        }

        public static void LogThreadException(Exception exception)
        {
            ThreadLogger.Error("Thread Exception:\r\n" + exception + "\r\n\r\n");
        }

        public static void LogWiredException(Exception exception)
        {
            WiredLogger.Error("Wired Exception:\r\n" + exception + "\r\n\r\n");
        }
    }
}
