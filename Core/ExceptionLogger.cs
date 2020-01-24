using System;
using Serilog;

namespace Plus.Core
{
    public static class ExceptionLogger
    {
        public static void LogQueryError(string query, Exception exception)
        {
            Log.Error("Error in query:\r\n{Query}\r\n{Exception}\r\n\r\n", query, exception);
        }

        public static void LogException(Exception exception)
        {
            Log.Error("Exception:\r\n{Exception}\r\n\r\n", exception);
        }

        public static void LogCriticalException(Exception exception)
        {
            Log.Error("Critical Exception:\r\n{Exception}\r\n\r\n", exception);
        }

        public static void LogThreadException(Exception exception)
        {
            Log.Error("Thread Exception:\r\n{Exception}\r\n\r\n", exception);
        }

        public static void LogWiredException(Exception exception)
        {
            Log.Error("Wired Exception:\r\n{Exception}\r\n\r\n", exception);
        }
    }
}
