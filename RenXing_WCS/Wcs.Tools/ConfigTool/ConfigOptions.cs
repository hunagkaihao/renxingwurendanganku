using System.Collections.Generic;

namespace Wcs.ConfigTool
{
    public class PlcHeartBeatSet
    {
        public string PlcName { get; set; } = string.Empty;
        public string HeartTagName { get; set; } = string.Empty; //需要为整型数据
        public int CycleTime { get; set; }
    }

    public class MjjAvoidPos
    {
        public string LmTarget { get; set; } = string.Empty;
        public byte MjjAvoidCol { get; set; } = 0;
        public byte MjjAvoidZY { get; set; } = 0;
    }
    public class User
    {
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class TestItem
    {
        public int RowNo { get; set; } = 0;
        public int StartColNo { get; set; } = 0;
        public int EndColNo { get; set; } = 0;
        public int StartLayerNo { get; set; } = 0;
        public int EndLayerNo { get; set; } = 0;
        public string Specs { get; set; } = string.Empty;
    }

    public class ConfigOptions
    {
        public string BaseUrl { get; set; } = string.Empty;

        public string HubCliMethod_UpdateMjjStatus { get; set; } = string.Empty;
        public string HubCliMethod_UpdatePlcTags { get; set; } = string.Empty;
        public string HubCliMethod_UpdateUndoneOrders { get; set; } = string.Empty;
        public string HubCliMethod_UpdateWcsStatus { get; set; } = string.Empty;

        public string SqliteLogConnString { get; set; } = "";
        public int LogClearInterval { get; set; } = 0;
        public int LogMaxVolume { get; set; } = 0;
        public string RedisConnStr { get; set; } = "";
        public int DefaultRedisNo { get; set; } = 0;
        public int PlcRedisNo { get; set; } = 0;

        public bool RemovePlcTagTempValueOnStart { get; set; }
        public List<PlcHeartBeatSet> HeartBeatsFromPlc { get; set; } = new List<PlcHeartBeatSet>();
        public List<PlcHeartBeatSet> HeartBeatsToPlc { get; set; } = new List<PlcHeartBeatSet>();
        public List<string> PlcTagMonitors { get; set; } = new List<string>();

        public string DiaptchStrategy { get; set; } = string.Empty;
        public int DispatchTaskMaxHandlingNum { get; set; } = 0;
        public string DispatchServerName { get; set; } = string.Empty;
        public string DispatchConditionChannelName { get; set; } = string.Empty;
        public int DispatchOrderRecordHoldTime { get; set; } = 0;
        public int DispatchOrderCntMngInterval { get; set; } = 0;
        public bool OpenDoorAfterWmsAllowed { get; set; } = false;

        public string WmsFirstRowPos { get; set; } = string.Empty;
        public int WmsFirstRowNo { get; set; } = 0;
        public bool WmsPlcRowConsistence { get; set; } = false;
        public bool WmsPlcColConsistence { get; set; } = false;
        public int WmsRowCnt { get; set; } = 0;

        public int MjjColCnt { get; set; } = 0;
        public string MjjFixColPos { get; set; } = string.Empty;
        public bool MjjFixColAvailable { get; set; } = false;
        public int MjjColCntLeftOfFixCol { get; set; } = 0;
        public int MjjColCntRightOfFixCol { get; set; } = 0;
        public int MjjOperateTimeout { get; set; } = 0;

        public bool MjjNeedAvoidLm { get; set; } = false;
        public List<MjjAvoidPos> MjjAvoidLmPos { get; set; } = new List<MjjAvoidPos>();

        public List<TestItem> Test { get; set; } = new List<TestItem>();

        public User User { get; set; }
        public string WMSUrl { get; set; }=string.Empty;

        public int ChkTime { get; set; } = 50;
    }
}
