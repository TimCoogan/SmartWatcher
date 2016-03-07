using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;

namespace SmartWatcher.Services
{
    class ServiceLogger
    {
        private static readonly ILog Logger = LogManager.GetLogger("SmartWatcherLogger");

        public static void LogDebug(string value)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug(value);
            }
        }

        public static void LogInfo(string value)
        {
            if (Logger.IsInfoEnabled)
            {
                Logger.Info(value);
            }
        }

        public static void LogWarn(string value)
        {
            if (Logger.IsWarnEnabled)
            {
                Logger.Warn(value);
            }
        }

        public static void LogError(string value)
        {
            if (Logger.IsErrorEnabled)
            {
                Logger.Error(value);
            }
        }

        public static void LogFatal(string value)
        {
            if (Logger.IsFatalEnabled)
            {
                Logger.Fatal(value);
            }
        }

    }
}
