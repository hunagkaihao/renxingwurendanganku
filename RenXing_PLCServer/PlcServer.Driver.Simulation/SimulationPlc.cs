using Newtonsoft.Json;
using PlcServer.Cache;
using PlcServer.Defines;
using PlcServer.Defines.Enum;
using PlcServer.Devices.IDeviceServices;
using PlcServer.Devices.Models;
using PlcServer.Driver.Base;
using Shared.Config;
using Shared.Logger.ILogger;
using StackExchange.Redis;

namespace PlcServer.Driver.Simulation
{
    public class SimulationPlc : PlcBase
    {
        private ConnectionMultiplexer mRedisClient;
        private int mRedisDBNum;
        private Dictionary<string, object?> mDicLastVal; //记录所有tag前一次读取到的值

        private readonly ILog _logger;
        private readonly IDeviceService _deviceService;

        public SimulationPlc(ICache cache, ILog logger, IDeviceService deviceService) : base(cache)
        {
            _logger = logger;
            _deviceService = deviceService;
            mRedisDBNum = Settings.ConfigData.RedisDBNumForSimPlc;
            mRedisClient = ConnectionMultiplexer.Connect(Settings.ConfigData.RedisConnString);
            mDicLastVal = new Dictionary<string, object?>();
        }

        public override void LoadTags()
        {
            mTags.Clear();

            if (string.IsNullOrEmpty(mPlcName))
            {
                string log = "为Plc加载相关节点标签时，发现Plc没有名称";
                _logger.Error(log, GetType().FullName);
            }

            List<PlcNode> lstNode = _deviceService.GetAllPlcNodesInPlc(PlcName);
            if (lstNode.Count <= 0)
            {
                string log = $"没有为PLC {PlcName} 配置变量";
                _logger.Error(log, GetType().FullName);
                return;
            }

            List<string> nodeNames = new List<string>();
            foreach (PlcNode node in lstNode)
            {
                //[0]
                string? strNodeName = node.NodeName;
                if (strNodeName == null)
                {
                    string log = $"加载Plc相关节点标签时，有节点名为空的错误配置";
                    _logger.Error(log, GetType().FullName);
                    continue;
                }
                if (nodeNames.Contains(strNodeName))
                {
                    string log = $"加载Plc相关节点标签时，节点名{strNodeName}重复";
                    _logger.Error(log, GetType().FullName);
                    continue;
                }
                nodeNames.Add(strNodeName);

                //[1]
                string? strNodeAddr = node.NodeAddr;
                if (strNodeAddr == null)
                {
                    string log = $"加载Plc相关节点标签时，发现节点名为{node.NodeName}的节点地址为空的错误配置";
                    _logger.Error(log, GetType().FullName);
                    continue;
                }

                //[2]
                EnumPlcTagType enumTagType = EnumPlcTagType.Bit;
                if (!Enum.TryParse<EnumPlcTagType>(node.NodeType, out enumTagType))
                {
                    string log = $"加载Plc相关节点标签时，节点名为{node.NodeName}的节点类型{node.NodeType}配置错误";
                    _logger.Error(log, GetType().FullName);
                    continue;
                }

                //[3]
                EnumTagAccess enumTagAccess = EnumTagAccess.Read;
                if (!Enum.TryParse<EnumTagAccess>(node.NodeAccess, out enumTagAccess))
                {
                    string log = $"加载Plc相关节点标签时，节点名为{node.NodeName}的节点访问类型{node.NodeAccess}配置错误";
                    _logger.Error(log, GetType().FullName);
                    continue;
                }

                //[4]
                bool isPublish = (node.IsPublish == 0 ? false : true);

                //[5]
                PlcTag tag = new PlcTag(strNodeName, strNodeAddr, enumTagType, enumTagAccess, isPublish);
                mTags.Add(tag.TagName, tag);
            }
        }

        public override void GroupTags()
        {
            mReadGroups.Clear();

            //[0]挑选访问类型为Read或ReadWrite的标签
            List<PlcTag> lstReadTag = new List<PlcTag>();
            foreach (var item in mTags)
            {
                PlcTag tag = item.Value;
                if (tag.TagAccess == EnumTagAccess.Read ||
                    tag.TagAccess == EnumTagAccess.ReadWrite)
                {
                    lstReadTag.Add(tag);
                }
            }
            if (lstReadTag.Count == 0)//没有找到
            {
                string log = $"为Plc节点标签分组时，发现名为{PlcName}的Plc没有定义访问类型为Read或ReadWrite的标签";
                _logger.Error(log, GetType().FullName);
                return;
            }

            //[1]根据“刷新时间”和“存储区域”对标签进行分组
            PlcTagGroup group_Input = new PlcTagGroup($"{PlcName}.Group_Input", (int)EnumMemoryType.Input);
            PlcTagGroup group_Output = new PlcTagGroup($"{PlcName}.Group_Output", (int)EnumMemoryType.Output);
            PlcTagGroup group_Memory = new PlcTagGroup($"{PlcName}.Group_Memory", (int)EnumMemoryType.Memory);
            List<PlcTagGroup> lstGroup_DataBlock = new List<PlcTagGroup>();
            List<int> lstDBNum_DataBlock = new List<int>();

            foreach (var tag in lstReadTag)
            {
                PlcAddress? address = null;
                try
                {
                    address = new PlcAddress(tag.TagAddr);
                }
                catch (Exception ex)
                {
                    string log = $"名为{tag.TagName}的标签，地址{tag.TagAddr}无效，Err：{ex.Message}";
                    _logger.Error(log, GetType().FullName);
                    continue;
                }
                if (address == null)
                {
                    continue;
                }
                if (address.MemoryType == EnumMemoryType.Input)
                    group_Input.AppendTag(tag, address.StartByte, address.ByteLength);
                else if (address.MemoryType == EnumMemoryType.Output)
                    group_Output.AppendTag(tag, address.StartByte, address.ByteLength);
                else if (address.MemoryType == EnumMemoryType.Memory)
                    group_Memory.AppendTag(tag, address.StartByte, address.ByteLength);
                else if (address.MemoryType == EnumMemoryType.DataBlock)
                {
                    int index = lstDBNum_DataBlock.IndexOf(address.DbNumber);
                    if (index == -1)
                    {
                        lstDBNum_DataBlock.Add(address.DbNumber);
                        PlcTagGroup group = new PlcTagGroup($"{PlcName}.Group_DB{address.DbNumber}", (int)EnumMemoryType.Memory, address.DbNumber);
                        group.AppendTag(tag, address.StartByte, address.ByteLength);
                        lstGroup_DataBlock.Add(group);
                    }
                    else
                    {
                        lstGroup_DataBlock[index].AppendTag(tag, address.StartByte, address.ByteLength);
                    }
                }

                mCache.AddTag(PlcName, tag.TagName);
                mDicLastVal.Add(tag.TagName, null);
            }

            if (group_Input.Tags.Count > 0)
                mReadGroups.Add(group_Input);
            if (group_Output.Tags.Count > 0)
                mReadGroups.Add(group_Output);
            if (group_Memory.Tags.Count > 0)
                mReadGroups.Add(group_Memory);
            foreach (var group in lstGroup_DataBlock)
            {
                if (group.Tags.Count > 0)
                    mReadGroups.Add(group);
            }

            foreach (var group in mReadGroups)
            {
                List<PlcTagValue> lstTagValue = new List<PlcTagValue>();
                foreach (var item in group.Tags)
                {
                    PlcTagValue value = new PlcTagValue(item.Value);
                    value.Quality = EnumQuality.Good;
                    value.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    switch (item.Value.TagType)
                    {
                        case EnumPlcTagType.I8:
                        case EnumPlcTagType.U8:
                        case EnumPlcTagType.I16:
                        case EnumPlcTagType.U16:
                        case EnumPlcTagType.I32:
                        case EnumPlcTagType.U32:
                            value.Value = "0";
                            break;
                        case EnumPlcTagType.F32:
                        case EnumPlcTagType.F64:
                            value.Value = "0.0";
                            break;
                        case EnumPlcTagType.U8Array:
                        case EnumPlcTagType.U16Array:
                            value.Value = "";
                            break;
                        case EnumPlcTagType.Bit:
                            value.Value = "False";
                            break;
                        default:
                            value.Value = "";
                            break;
                    }
                    lstTagValue.Add(value);
                }
                IDatabase db = mRedisClient.GetDatabase(mRedisDBNum);
                db.StringSet(group.GroupName, JsonConvert.SerializeObject(lstTagValue));
            }
        }

        public override void InitCache()
        {
            if (mReadGroups.Count == 0)
                return;

            foreach(var group in mReadGroups)
            {
                if (group.Tags.Count == 0)
                    continue;

                foreach(var keyValue in group.Tags)
                {
                    PlcTag tag = keyValue.Value;
                    PlcTagValue value = new PlcTagValue(tag);
                    value.Quality = EnumQuality.Good;
                    switch(tag.TagType)
                    {
                        case EnumPlcTagType.I8:
                        case EnumPlcTagType.U8:
                        case EnumPlcTagType.I16:
                        case EnumPlcTagType.U16:
                        case EnumPlcTagType.I32:
                        case EnumPlcTagType.U32:
                            value.Value = "0";
                            break;
                        case EnumPlcTagType.F32:
                        case EnumPlcTagType.F64:
                            value.Value = "0.0";
                            break;
                        case EnumPlcTagType.U8Array:
                        case EnumPlcTagType.U16Array:
                            value.Value = "";
                            break;
                        case EnumPlcTagType.Bit:
                            value.Value = "False";
                            break;
                        default:
                            value.Value = "";
                            break;
                    }
                    try
                    {
                        string strValue = JsonConvert.SerializeObject(value);
                        mCache.WriteTag(PlcName, tag.TagName, strValue);
                    }
                    catch(Exception ex)
                    {
                        _logger.Error($"初始化Plc内存时，Error:{ex.Message}", GetType().FullName);
                        continue;
                    }
                }
            }
        }

        public override async Task<bool> ConnectAsync()
        {
            await Task.Delay(1000).ConfigureAwait(false);
            base.mIsConnected = true;
            return true;
        }

        public override async Task DisConnectAsync()
        {
            await Task.Delay(200).ConfigureAwait(false);
            base.mIsConnected = false;
        }
                
        public override async Task ReadAllAsync()
        {
            foreach (var group in mReadGroups)
            {
                IDatabase db = mRedisClient.GetDatabase(mRedisDBNum);

                string? groupV = await db.StringGetAsync(group.GroupName).ConfigureAwait(false);
                if (groupV == null)
                {
                    continue;
                }
                List<PlcTagValue>? lstTagValue = JsonConvert.DeserializeObject<List<PlcTagValue>>(groupV);
                if (lstTagValue == null)
                {
                    continue;
                }
                List<Task> lstTask = new List<Task>();
                foreach (var value in lstTagValue)
                {
                    object? lastVal = mDicLastVal[value.Tag.TagName];
                    if (lastVal == null || lastVal.ToString() != value.Value)
                    {
                        mDicLastVal[value.Tag.TagName] = value.Value;
                        if(value.Tag.IsPublish)
                            lstTask.Add(mCache.WriteAndPublishTagAsync(PlcName, value.Tag.TagName, JsonConvert.SerializeObject(value)));
                        else    
                            lstTask.Add(mCache.WriteTagAsync(PlcName, value.Tag.TagName, JsonConvert.SerializeObject(value)));
                    }
                }
                await Task.WhenAll(lstTask).ConfigureAwait(false);
            }
            //Console.WriteLine($"{PlcName}读所有Tag点位，{DateTime.Now.ToLongTimeString()}，threadId:{Thread.CurrentThread.ManagedThreadId}");
        }

        public override async Task<PlcTagValue?> ReadTagAsync(string tagName)
        {
            if (!mTags.ContainsKey(tagName))
            {
                return null;
            }

            string? groupName = null;
            foreach (var group in mReadGroups)
            {
                if (group.Tags.ContainsKey(tagName))
                {
                    groupName = group.GroupName;
                    break;
                }
            }

            if(groupName == null)
            {
                return null;
            }

            IDatabase db = mRedisClient.GetDatabase(mRedisDBNum);
            string? groupV = await db.StringGetAsync(groupName);
            if(groupV == null)
            {
                return null;
            }

            List<PlcTagValue>? lstTagValue = JsonConvert.DeserializeObject<List<PlcTagValue>>(groupV);
            if(lstTagValue == null)
            {
                return null;
            }

            PlcTagValue? tagValue = null;
            foreach(var value in lstTagValue)
            {
                if(value.Tag.TagName == tagName)
                {
                    tagValue = value;
                    break;
                }
            }
            return tagValue;
        }

        public override async Task<bool> WriteTagAsync(string tagName, string tagValue)
        {
            if (!mTags.ContainsKey(tagName))
            {
                _logger.Error($"向变量{tagName}写值{tagValue}时，发现名为{PlcName}的Plc没有变量{tagName}", GetType().FullName);
                return false;
            }

            IDatabase db = mRedisClient.GetDatabase(mRedisDBNum);
            List<PlcTagValue>? lstTagValue = null;
            string? groupName = null;
            foreach (var item in mReadGroups)
            {
                if (item.Tags.ContainsKey(tagName))
                {
                    groupName = item.GroupName;
                    string? strGp = await db.StringGetAsync(item.GroupName);
                    if(strGp == null)
                        break;
                    lstTagValue = JsonConvert.DeserializeObject<List<PlcTagValue>>(strGp);
                    break;
                }
            }
            if (lstTagValue == null)
                return false;

            PlcTag tag = mTags[tagName];
            PlcTagValue value = new PlcTagValue(tag);
            value.Value = tagValue;
            value.Quality = EnumQuality.Good;
            value.TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                for (int i = 0; i < lstTagValue.Count; i++)
                {
                    if (lstTagValue[i].Tag.TagName == tagName)
                        lstTagValue[i] = value;
                }
                return await db.StringSetAsync(groupName, JsonConvert.SerializeObject(lstTagValue));
            }
            catch(Exception e)
            {
                _logger.Error($"向变量{tagName}写值{tagValue}时，发生错误：{e.Message}", GetType().FullName);
                return false;
            }
        }
    }
}