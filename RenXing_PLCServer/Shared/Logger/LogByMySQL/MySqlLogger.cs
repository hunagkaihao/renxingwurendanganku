using Microsoft.EntityFrameworkCore;
using Shared.Logger.ILogger.Enumeration;
using Shared.Logger.ILogger.Models;
using System.Runtime.CompilerServices;

namespace Shared.Logger.LogByMySQL
{
    public class MySqlLogger : Shared.Logger.ILogger.ILog
    {
        private void Record(string logContent, LogGrade grade, string? callerOwner, string callerName)
        {
            using (var context = new MySqlLogContext())
            {
                try
                {
                    DbSet<LogItem>? logItemSet = context.logitems;
                    if (logItemSet == null)
                        return;

                    LogItem item = new()
                    {
                        Message = logContent,
                        Grade = grade.ToString(),
                        Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff"),
                        Source = $"{callerOwner}.{callerName}(...)"
                    };

                    logItemSet.Add(item);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")}\tERROR\tPcs.Logger.LogService.Instance.log()\t{ex.Message}");
                }
            }
        }

        public void WriteLog(string message, LogGrade grade = LogGrade.INFO, string? callerOwner = null, [CallerMemberName] string callerName = "")
        {
            Record(message, grade, callerOwner, callerName);
        }

        public void Info(string message, string? callerOwner = null, [CallerMemberName] string callerName = "")
        {
            Record(message, LogGrade.INFO, callerOwner, callerName);
        }

        public void Error(string message, string? callerOwner = null, [CallerMemberName] string callerName = "")
        {
            Record(message, LogGrade.ERROR, callerOwner, callerName);
        }

        public void Warning(string message, string? callerOwner = null, [CallerMemberName] string callerName = "")
        {
            Record(message, LogGrade.WARN, callerOwner, callerName);
        }

        public void Debug(string message, string? callerOwner = null, [CallerMemberName] string callerName = "")
        {
            Record(message, LogGrade.DEBUG, callerOwner, callerName);
        }

        public void Fatal(string message, string? callerOwner = null, [CallerMemberName] string callerName = "")
        {
            Record(message, LogGrade.FATAL, callerOwner, callerName);
        }

        public List<LogItem>? GetLogItems(string partOfLogContent, LogGrade? grade)
        {
            using (var context = new MySqlLogContext())
            {
                try
                {
                    DbSet<LogItem>? logItemSet = context.logitems;
                    if (logItemSet == null)
                        return new List<LogItem>();

                    if (grade == null)
                    {
                        return logItemSet
                            .AsNoTracking()
                            .Where(i => i.Message.Contains(partOfLogContent)) //message不可能为null
                            .OrderBy(i => i.Id)
                            .ToList();
                    }
                    else
                    {
                        return logItemSet
                            .AsNoTracking()
                            .Where(i => i.Message.Contains(partOfLogContent) && i.Grade == grade.ToString())
                            .OrderBy(i => i.Id)
                            .ToList();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")}\tERROR\tPcs.Logger.LogService.Instance.GetLogItems()\t{ex.Message}");
                    return null;
                }
            }
        }

        public int GetLogItemCount()
        {
            using (var context = new MySqlLogContext())
            {
                try
                {
                    return context.logitems?.Count() ?? 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")}\tERROR\tPcs.Logger.LogService.Instance.GetLogItemCount()\t{ex.Message}");
                    return 0;
                }
            }
        }

        public void DeleteLogItems(int remainNum)
        {
            if (remainNum < 0)
            {
                return;
            }
            int totalNum = GetLogItemCount();
            if (totalNum <= remainNum)
            {
                return;
            }
            using (var context = new MySqlLogContext())
            {
                try
                {
                    int maxIdx = context.logitems!
                        .OrderBy(i => i.Id)
                        .Last().Id;
                    context.Database.ExecuteSqlRaw($"Delete From LogItems Where Id <= {maxIdx - remainNum}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")}\tERROR\tPcs.Logger.LogService.Instance.DeleteLogItems()\t{ex.Message}");
                }
            }
        }
    }
}
