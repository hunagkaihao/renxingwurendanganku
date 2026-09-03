using Wcs.ConfigTool;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace Wcs.LogTool;

public static class SqliteLogHelper
{
    private static readonly object locker = new object();

    private static SqliteConnection connection = new SqliteConnection(Settings.Options.SqliteLogConnString);

    private static void OpenConnection()
    {
        if (connection.State != System.Data.ConnectionState.Closed)
            return;

        var connectionOptions = new SqliteConnectionStringBuilder(Settings.Options.SqliteLogConnString);
        if (!string.IsNullOrWhiteSpace(connectionOptions.DataSource)
            && !string.Equals(connectionOptions.DataSource, ":memory:", StringComparison.OrdinalIgnoreCase))
        {
            var directory = Path.GetDirectoryName(Path.GetFullPath(connectionOptions.DataSource));
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);
        }

        connection.Open();
        using var schemaCommand = connection.CreateCommand();
        schemaCommand.CommandText = @"
            CREATE TABLE IF NOT EXISTS logitems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Date TEXT NOT NULL,
                Grade TEXT NOT NULL,
                Message TEXT NOT NULL,
                Source TEXT NOT NULL
            );";
        schemaCommand.ExecuteNonQuery();
    }

    public static void WriteLog(SqliteLogItem logItem)
    {
        lock (locker)
        {
            try
            {
                OpenConnection();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"INSERT INTO logitems (Date, Grade, Message, Source) VALUES ($Date, $Grade, $Message, $Source)";
                    command.Parameters.AddWithValue("$Date", logItem.Date);
                    command.Parameters.AddWithValue("$Grade", logItem.Grade);
                    command.Parameters.AddWithValue("$Message", logItem.Message);
                    command.Parameters.AddWithValue("$Source", logItem.Source);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // 此处不能调用 SQLite 日志 provider，否则写入失败时会递归。
                Console.WriteLine("SQLite 日志写入失败：{0}", ex);
            }
        }            
    }

    public static int GetLogItemCount()
    {
        lock(locker)
        {
            try
            {
                OpenConnection();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT COUNT(*) FROM logitems";
                    var reader = command.ExecuteReader();
                    reader.Read();
                    int ret = reader.GetInt32(0);
                    reader.Close();
                    return ret;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("读取 SQLite 日志数量失败：{0}", ex);
                return 0;
            }
        }
    }

    public static List<SqliteLogItem> GetLogItems(string partOfLogContent, string grade, int logCntMax = 2000)
    {
        lock(locker)
        {
            try
            {
                OpenConnection();

                using (var command = connection.CreateCommand())
                {
                    if (grade == "%")
                    {
                        List<SqliteLogItem> result = new List<SqliteLogItem>();
                        if (partOfLogContent != "%")
                        {
                            command.CommandText = $"SELECT Id, Date, Grade, Message, Source FROM logitems WHERE Message LIKE $partOfLogContent ORDER BY Id DESC LIMIT $cnt";
                            command.Parameters.AddWithValue("$partOfLogContent", $"%{partOfLogContent}%");
                            command.Parameters.AddWithValue("$cnt", logCntMax);
                            var reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                SqliteLogItem item = new SqliteLogItem()
                                {
                                    Id = reader.GetInt32(0),
                                    Date = reader.GetString(1),
                                    Grade = reader.GetString(2),
                                    Message = reader.GetString(3),
                                    Source = reader.GetString(4)
                                };
                                result.Add(item);
                            }
                        }
                        else
                        {
                            command.CommandText = $"SELECT Id, Date, Grade, Message, Source FROM logitems ORDER BY Id DESC LIMIT $cnt";
                            command.Parameters.AddWithValue("$cnt", logCntMax);
                            var reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                SqliteLogItem item = new SqliteLogItem()
                                {
                                    Id = reader.GetInt32(0),
                                    Date = reader.GetString(1),
                                    Grade = reader.GetString(2),
                                    Message = reader.GetString(3),
                                    Source = reader.GetString(4)
                                };
                                result.Add(item);
                            }
                        }
                        return result;
                    }
                    else
                    {
                        List<SqliteLogItem> result = new List<SqliteLogItem>();
                        if (partOfLogContent != "%")
                        {
                            command.CommandText = $"SELECT Id, Date, Grade, Message, Source FROM logitems WHERE Grade = $grade AND Message LIKE $partOfLogContent ORDER BY Id DESC LIMIT $cnt";
                            command.Parameters.AddWithValue("$grade", grade);
                            command.Parameters.AddWithValue("$partOfLogContent", $"%{partOfLogContent}%");
                            command.Parameters.AddWithValue("$cnt", logCntMax);
                            var reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                SqliteLogItem item = new SqliteLogItem()
                                {
                                    Id = reader.GetInt32(0),
                                    Date = reader.GetString(1),
                                    Grade = reader.GetString(2),
                                    Message = reader.GetString(3),
                                    Source = reader.GetString(4)
                                };
                                result.Add(item);
                            }
                        }
                        else
                        {
                            command.CommandText = $"SELECT Id, Date, Grade, Message, Source FROM logitems WHERE Grade = $grade ORDER BY Id DESC LIMIT $cnt";
                            command.Parameters.AddWithValue("$grade", grade);
                            command.Parameters.AddWithValue("$cnt", logCntMax);
                            var reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                SqliteLogItem item = new SqliteLogItem()
                                {
                                    Id = reader.GetInt32(0),
                                    Date = reader.GetString(1),
                                    Grade = reader.GetString(2),
                                    Message = reader.GetString(3),
                                    Source = reader.GetString(4)
                                };
                                result.Add(item);
                            }
                        }
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("查询 SQLite 日志失败：{0}", ex);
                return new List<SqliteLogItem>();
            }
        }        
    }

    public static SqliteLogItem GetLastLogItem()
    {
        lock(locker)
        {
            try
            {
                OpenConnection();
                    
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT Id, Date, Grade, Message, Source FROM logitems ORDER BY Id DESC LIMIT 1";
                    var reader = command.ExecuteReader();
                    if(!reader.Read())
                        return null;
                        
                    SqliteLogItem item = new SqliteLogItem()
                    {
                        Id = reader.GetInt32(0),
                        Date = reader.GetString(1),
                        Grade = reader.GetString(2),
                        Message = reader.GetString(3),
                        Source = reader.GetString(4)
                    };
                    return item;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("读取最新 SQLite 日志失败：{0}", ex);
                return null;
            }
        } 
    }

    public static void DeleteLogItems(int remainNum)
    {
        lock(locker)
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
            try
            {
                SqliteLogItem lastItem = GetLastLogItem();
                if(lastItem == null)
                    return;

                OpenConnection();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"Delete From logitems Where Id <= $id";
                    command.Parameters.AddWithValue("$id", lastItem.Id - remainNum);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("清理 SQLite 历史日志失败：{0}", ex);
            }
        }
    }

}
