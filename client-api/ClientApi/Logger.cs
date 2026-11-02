using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace ClientApi
{
    public sealed class Logger
    {
        private static Logger logger;
        private static ILog log;

        private Logger()
        {
        }

        public static Logger Singleton
        {
            get
            {
                if (logger == null)
                {
                    try
                    {
                        ILoggerRepository repository = LogManager.GetRepository(Assembly.GetEntryAssembly());
                        XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
                        logger = new Logger();
                        log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return logger;
            }
        }

        public void Debug(string message)
        {

            log.Debug(message);

        }

        public void Info(string message)
        {
            log.Info(message);
        }

        public void Error(string message)
        {
            log.Error(message);
        }

        public void Fatal(string message)
        {
            log.Fatal(message);
        }
    }
}
