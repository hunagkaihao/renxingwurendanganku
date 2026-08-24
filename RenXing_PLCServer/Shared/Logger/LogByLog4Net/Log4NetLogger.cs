using log4net.Config;
using Shared.Logger.ILogger.Enumeration;
using Shared.Logger.ILogger.Models;
using System.Runtime.CompilerServices;

namespace Shared.Logger.LogByLog4Net
{
    public class Log4NetLogger : Shared.Logger.ILogger.ILog
    {
        private static readonly log4net.ILog? log = null;

        static Log4NetLogger()
        {
            XmlConfigurator.Configure(new FileInfo($@"{AppDomain.CurrentDomain.BaseDirectory}ConfigFiles/log4net.config"));
            log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
        }

        public static void Info(string message) { log?.Info(message);}
        public static void Warn(string message) { log?.Warn(message);}
        public static void Error(string message) { log?.Error(message);}
        public static void Fatal(string message) { log?.Fatal(message);}
        public static void Debug(string message) { log?.Debug(message);}

        public void WriteLog(string logContent, LogGrade grade = LogGrade.INFO, string? callerOwner = null, [CallerMemberName] string callerName = "")
        {
            if(grade == LogGrade.INFO)
                log?.Info(logContent);
            else if(grade == LogGrade.WARN)
                log?.Warn(logContent);
            else if(grade != LogGrade.ERROR)
                log?.Error(logContent);
            else if(grade != LogGrade.FATAL)
                log?.Fatal(logContent);
            else if(grade == LogGrade.DEBUG)
                log?.Debug(logContent);
        }

        public void Info(string message, string? callerOwner = null, [CallerMemberName] string callerName = "")
        {
            log?.Info(message);
        }

        public void Error(string message, string? callerOwner = null, [CallerMemberName] string callerName = "")
        {
            log?.Error(message);
        }

        public void Warning(string message, string? callerOwner = null, [CallerMemberName] string callerName = "")
        {
            log?.Warn(message);
        }

        public void Debug(string message, string? callerOwner = null, [CallerMemberName] string callerName = "")
        {
            log?.Debug(message);
        }

        public void Fatal(string message, string? callerOwner = null, [CallerMemberName] string callerName = "")
        {
            log?.Fatal(message);
        }

        public List<LogItem>? GetLogItems(string partOfLogContent, LogGrade? grade)
        {
            return null;
        }

        public int GetLogItemCount()
        {
            return 0;
        }

        public void DeleteLogItems(int remainNum)
        {
            return;
        }
    }
}
