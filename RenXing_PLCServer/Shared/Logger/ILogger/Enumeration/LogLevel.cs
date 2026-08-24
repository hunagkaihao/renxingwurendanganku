using System.ComponentModel;

namespace Shared.Logger.ILogger.Enumeration
{
    public enum LogGrade
    {
        [Description("警告信息")]
        WARN = 1,
        [Description("调试信息")]
        DEBUG,
        [Description("一般信息")]
        INFO,
        [Description("严重错误")]
        FATAL,
        [Description("错误日志")]
        ERROR
    }
}
