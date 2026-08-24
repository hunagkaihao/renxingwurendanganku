using Shared.Logger.ILogger.Enumeration;
using Shared.Logger.ILogger.Models;
using System.Runtime.CompilerServices;

namespace Shared.Logger.ILogger
{
    public interface ILog
    {
        public void WriteLog(string logContent, LogGrade grade = LogGrade.INFO, string? callerOwner = null, [CallerMemberName]string callerName = "");

        public void Info(string message, string? callerOwner = null, [CallerMemberName] string callerName = "");

        public void Error(string message, string? callerOwner = null, [CallerMemberName] string callerName = "");

        public void Warning(string message, string? callerOwner = null, [CallerMemberName] string callerName = "");

        public void Debug(string message, string? callerOwner = null, [CallerMemberName] string callerName = "");

        public void Fatal(string message, string? callerOwner = null, [CallerMemberName] string callerName = "");

        public List<LogItem>? GetLogItems(string partOfLogContent, LogGrade? grade);

        public int GetLogItemCount();

        public void DeleteLogItems(int remainNum);
    }
}
