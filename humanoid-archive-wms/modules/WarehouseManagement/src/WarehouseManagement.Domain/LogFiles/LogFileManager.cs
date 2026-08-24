using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace WarehouseManagement.LogFiles
{
    public class LogFileManager: PlanDomainService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IHttpContextAccessor _contextAccessor;

        public LogFileManager( ICurrentUser currentUser, IHttpContextAccessor contextAccessor)
        {
            _currentUser=currentUser;
            _contextAccessor=contextAccessor;
        }

        public async Task<List<LogFile>> GetListAsync()
        {
            string ip = _contextAccessor.HttpContext.Connection.LocalIpAddress.MapToIPv4().ToString();
            string port = _contextAccessor.HttpContext.Connection.LocalPort.ToString();
            List<LogFile> logFiles = new List<LogFile>();
            try
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory+ "wwwroot/logs";
                List<FileInfo> fileInfos = new DirectoryInfo(path).GetFiles("").ToList();
                foreach (FileInfo fileInfo in fileInfos)
                {
                    LogFile logFile = new LogFile(fileInfo.Name,fileInfo.CreationTime.ToString(),fileInfo.LastWriteTime.ToString(),$"http://{ip}:{port}/logs/{fileInfo.Name}");
                    logFiles.Add(logFile);
                    //后续可以考虑对文件进行压缩
                    //优化单独的权限
                }
            }
            catch (Exception e)
            {

                throw;
            }
            return logFiles;
        }
        
    }
}
