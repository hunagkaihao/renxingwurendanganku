using Microsoft.Extensions.Configuration;

namespace Shared.Config
{
    public class PlcSetting
    {
        public int PlcId { get; set; }

        public string PlcName { get; set; } = string.Empty;

        public string DriverAssemblyName { get; set; } = string.Empty;

        public string DriverClassName { get; set; } = string.Empty;

        public string ConnectParameter { get; set; } = string.Empty;
    }

    public class PlcNodeSetting
    {
        public int NodeId { get; set; }

        public string NodeName { get; set; } = string.Empty;

        public string NodeAddr { get; set; } = string.Empty;

        public string NodeType { get; set; } = string.Empty;

        public string NodeAccess { get; set; } = string.Empty;      

        public int IsPublish { get; set; }

        public string PlcName { get; set; } = string.Empty;

        public string Remark { get; set; } = string.Empty;
    }

    public class BackGroundJob
    {
        public string BGJobName { get; set; } = string.Empty;
    }

    public class AgvJobConfig
    {
        public string Trigger { get; set; } = string.Empty;
        public string StartPoint { get; set; } = string.Empty;
        public string EndPoint { get; set; } = string.Empty;
    }



    public class ConfigData
    {
        public string LogDbConnString { get; set; } = "";
        public string LoggerClass { get; set; } = string.Empty;


        public string RedisConnString { get; set; } = "";
        public int RedisDBNumForPlcCache { get; set; } = 0;
        public int RedisDBNumForSimPlc { get; set; } = 0;
        public int CacheExpireTimeInMS { get; set; } = 0;
        public string PlcSvrDbConnString { get; set; } = "";
        public string PlcSettingSource { get; set; } = "";
        public List<PlcSetting> PlcSettings { get; set;} = new List<PlcSetting>();
        public List<PlcNodeSetting> PlcNodeSettings { get; set;} = new List<PlcNodeSetting>();

        public int DakSimInterval { get; set; } = 0;    
        public List<BackGroundJob> BackGroundJobs { get; set; } = new List<BackGroundJob>();
        public List<AgvJobConfig> AgvJobConfigs { get; set; } = new List<AgvJobConfig>();
    }

    public static class Settings
    {
        public static ConfigData ConfigData = new ConfigData();

        static Settings()
        {
            string settingFile = $@"{AppDomain.CurrentDomain.BaseDirectory}ConfigFiles/globalsettings.json";
            IConfiguration config = new ConfigurationBuilder().AddJsonFile(settingFile).Build();
            config.Bind(ConfigData);
        }
    }
}
