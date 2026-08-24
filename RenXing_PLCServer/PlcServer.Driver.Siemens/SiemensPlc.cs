using Newtonsoft.Json;
using PlcServer.Cache;
using PlcServer.Defines;
using PlcServer.Defines.Enum;
using PlcServer.Devices.IDeviceServices;
using PlcServer.Devices.Models;
using PlcServer.Driver.Base;
using S7.Net;
using S7.Net.Types;
using Shared.Logger.ILogger;
using System.Text;
using System.Text.RegularExpressions;

namespace PlcServer.Driver.Siemens
{
    public class SiemensPlc : PlcBase
    {
        private struct TagPara
        {
            public int StartByte;
            public int ByteCount;
        }

        private Plc? mPlc;
        private Dictionary<string, TagPara> mDicTagPara; //记录所有tag的起始字节和字节数量
        private Dictionary<string, object?> mDicLastVal; //记录所有tag前一次读取到的值

        private readonly ILog _logger;
        private readonly IDeviceService _deviceService;

        public SiemensPlc(ICache cache, ILog logger, IDeviceService deviceService)
            :base(cache)
        {
            _logger = logger;
            _deviceService = deviceService;
            mPlc = null;
            mDicTagPara = new Dictionary<string, TagPara>();
            mDicLastVal = new Dictionary<string, object?>();            
        }

        public override async Task<bool> ConnectAsync()
        {
            while (mPlc == null)
            {
                string[] parameters = ConnParas.Split(";", StringSplitOptions.RemoveEmptyEntries);
                if (parameters.Length != 4)
                {
                    _logger.Error($"西门子Plc {PlcName} 连接参数数量不正确", GetType().FullName);
                    break;
                }

                CpuType cpuType = CpuType.S71200;
                if (!Enum.TryParse<CpuType>(parameters[0], out cpuType))
                {
                    _logger.Error($"西门子Plc {PlcName} 对应的CpuType配置错误", GetType().FullName);
                    break;
                }

                string ip = parameters[1];
                Regex regex = new Regex(@"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$");
                if (!regex.IsMatch(ip))
                {
                    _logger.Error($"西门子Plc {{PlcName}} 对应的IP配置错误", GetType().FullName);
                    break;
                }

                short rack = 0;
                if (!short.TryParse(parameters[2], out rack))
                {
                    _logger.Error($"西门子Plc {{PlcName}} 对应的Rack配置错误", GetType().FullName);
                    break;
                }

                short slot = 0;
                if (!short.TryParse(parameters[3], out slot))
                {
                    _logger.Error($"西门子Plc {{PlcName}} 对应的Slot配置错误", GetType().FullName);
                    break;
                }
                mPlc = new Plc(cpuType, ip, rack, slot);
                break;
            }
            
            if(mPlc == null)
            {
                mIsConnected = false;
                return false;
            }

            try
            {
                await mPlc.OpenAsync();
                mIsConnected = true;
                return true;
            }
            catch(Exception ex)
            {
                _logger.Error($"连接IP为{mPlc.IP}的Plc失败：{ex.Message}", GetType().FullName);
                mIsConnected = false;
                return false;
            }
        }

        public override async Task DisConnectAsync()
        {
            if (mPlc == null)
                return;

            if (!mIsConnected)
                return;

            await Task.Delay(1);
            mPlc.Close();
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
                if(nodeNames.Contains(strNodeName))
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
                if (enumTagType == EnumPlcTagType.I8 ||
                    enumTagType == EnumPlcTagType.U16Array)
                {
                    string log = $"加载Plc相关节点标签时，节点名为{node.NodeName}的节点类型{node.NodeType}无法识别";
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
                //mCache.AddTag(PlcName, tag.TagName);
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
            PlcTagGroup group_Input = new PlcTagGroup($"{PlcName}.Group_Input", (int)DataType.Input);
            PlcTagGroup group_Output = new PlcTagGroup($"{PlcName}.Group_Output", (int)DataType.Output);
            PlcTagGroup group_Memory = new PlcTagGroup($"{PlcName}.Group_Memory", (int)DataType.Memory);
            List<PlcTagGroup> lstGroup_DataBlock = new List<PlcTagGroup>();
            List<int> lstDBNum_DataBlock = new List<int>();

            foreach (var tag in lstReadTag)
            {
                PLCAddress? address = null;
                try
                {
                    address = new PLCAddress(tag.TagAddr);
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
                if (address.DataType == DataType.Input)
                    group_Input.AppendTag(tag, address.StartByte, address.ByteLength);
                else if (address.DataType == DataType.Output)
                    group_Output.AppendTag(tag, address.StartByte, address.ByteLength);
                else if (address.DataType == DataType.Memory)
                    group_Memory.AppendTag(tag, address.StartByte, address.ByteLength);
                else if (address.DataType == DataType.DataBlock)
                {
                    int index = lstDBNum_DataBlock.IndexOf(address.DbNumber);
                    if (index == -1)
                    {
                        lstDBNum_DataBlock.Add(address.DbNumber);
                        PlcTagGroup group = new PlcTagGroup($"{PlcName}.Group_DB{address.DbNumber}", (int)DataType.DataBlock, address.DbNumber);
                        group.AppendTag(tag, address.StartByte, address.ByteLength);
                        lstGroup_DataBlock.Add(group);
                    }
                    else
                    {
                        lstGroup_DataBlock[index].AppendTag(tag, address.StartByte, address.ByteLength);
                    }
                }

                mDicTagPara.Add(tag.TagName, new TagPara()
                {
                    StartByte = address.StartByte,
                    ByteCount = address.ByteLength
                });
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
        }

        public override void InitCache()
        {
            if (mReadGroups.Count == 0)
                return;

            foreach (var group in mReadGroups)
            {
                if (group.Tags.Count == 0)
                    continue;

                foreach (var keyValue in group.Tags)
                {
                    PlcTag tag = keyValue.Value;
                    PlcTagValue value = new PlcTagValue(tag);
                    switch (tag.TagType)
                    {
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
                            value.Value = "";
                            break;
                        case EnumPlcTagType.Bit:
                            value.Value = "False";
                            break;
                        default:
                            {
                                _logger.Error($"{tag.TagName}的类型{tag.TagType}无法解析", GetType().FullName);
                                continue;
                            }
                    }
                    try
                    {
                        string strValue = JsonConvert.SerializeObject(value);
                        mCache.WriteTag(PlcName, tag.TagName, strValue);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"初始化Plc内存时，Error:{ex.Message}", GetType().FullName);
                        continue;
                    }
                }
            }
        }

        public override async Task ReadAllAsync()
        {
            if(mReadGroups.Count == 0 || mPlc == null || !mIsConnected)
                return;

            bool bConnBreak = true;
            foreach(var group in mReadGroups)
            {
                //[0] 以Group为单位，从PLC读取数据
                int byteNumOfGroup = group.MaxAddr - group.MinAddr + 1;
                if (byteNumOfGroup <= 0)
                {
                    continue;
                }

                byte[] data = new byte[byteNumOfGroup];
                try
                {
                    data = await mPlc.ReadBytesAsync(
                    (DataType)group.MemoryArea,
                    group.MemAreaNo,
                    group.MinAddr,
                    byteNumOfGroup
                    );
                }
                catch
                {
                    continue; //读取失败
                }

                bConnBreak = false; //只要其中一个Group读取正常，说明PLC连接没有断开

                //[1] 对每个tag进行解析
                foreach (var tag in group.Tags)
                {
                    object? lastVal = mDicLastVal[tag.Key];
                    if (!mDicTagPara.ContainsKey(tag.Key)) //此tag参数有误
                    {
                        PlcTagValue value = new PlcTagValue(tag.Value); //使用默认值作为当前值
                        if (lastVal == null || lastVal.ToString() != value.Value)
                        {
                            if(tag.Value.IsPublish)
                                mCache.WriteAndPublishTag(PlcName, tag.Key, JsonConvert.SerializeObject(value));
                            else    
                                mCache.WriteTag(PlcName, tag.Key, JsonConvert.SerializeObject(value));
                            mDicLastVal[tag.Key] = value.Value;
                            _logger.Error($"\"{PlcName}\".\"{tag.Key}\"的地址\"{tag.Value.TagAddr}\"不正确，无法读取", GetType().FullName);
                        }
                        continue;
                    }

                    int byteCount = mDicTagPara[tag.Key].ByteCount;
                    int startByte = mDicTagPara[tag.Key].StartByte;
                    if (byteCount <= 0 ||                       //Tag不包含字节
                        startByte < 0 ||                        //Tag起始字节索引不能小于0
                        startByte + byteCount > byteNumOfGroup + group.MinAddr) //Tag超出所在组范围
                    {
                        PlcTagValue value = new PlcTagValue(tag.Value); //使用默认值作为当前值
                        if (lastVal == null || lastVal.ToString() != value.Value)
                        {
                            if (tag.Value.IsPublish)
                                mCache.WriteAndPublishTag(PlcName, tag.Key, JsonConvert.SerializeObject(value));
                            else
                                mCache.WriteTag(PlcName, tag.Key, JsonConvert.SerializeObject(value));
                            mDicLastVal[tag.Key] = value.Value;
                            _logger.Error($"\"{PlcName}\".\"{tag.Key}\"经过分析得到的字节数超出范围，无法读取", GetType().FullName);
                        }
                        continue;
                    }

                    byte[] tagValInBytes = new byte[byteCount];
                    for(int i = 0; i < byteCount; i++)
                    {
                        tagValInBytes[i] = data[startByte - group.MinAddr + i];
                    }
                    object? objVal = null;
                    switch (tag.Value.TagType)
                    {
                        case EnumPlcTagType.Bit:
                            {
                                byte bitAdr = 0;
                                try
                                {
                                    PLCAddress.Parse(
                                    tag.Value.TagAddr,
                                    out DataType dType,
                                    out int dbNum,
                                    out VarType vType,
                                    out int addr,
                                    out int bitNum);
                                    bitAdr = (byte)bitNum;
                                }
                                catch
                                {
                                    PlcTagValue value = new PlcTagValue(tag.Value); //使用默认值作为当前值
                                    if (lastVal == null || lastVal.ToString() != value.Value)
                                    {
                                        if (tag.Value.IsPublish)
                                            mCache.WriteAndPublishTag(PlcName, tag.Key, JsonConvert.SerializeObject(value));
                                        else
                                            mCache.WriteTag(PlcName, tag.Key, JsonConvert.SerializeObject(value));
                                        mDicLastVal[tag.Key] = value.Value;
                                        _logger.Error($"{PlcName}.{tag.Value.TagName}的地址{tag.Value.TagAddr}无法解析", GetType().FullName);
                                    }
                                    continue;
                                }
                                objVal = Bit.FromByte(tagValInBytes[0], bitAdr);
                            }
                            break;
                        case EnumPlcTagType.U8:
                            objVal = tagValInBytes[0];
                            break;
                        case EnumPlcTagType.U16:
                            objVal = Word.FromByteArray(tagValInBytes);
                            break;
                        case EnumPlcTagType.I16:
                            objVal = Int.FromByteArray(tagValInBytes);
                            break;
                        case EnumPlcTagType.U32:
                            objVal = DWord.FromByteArray(tagValInBytes);
                            break;
                        case EnumPlcTagType.I32:
                            objVal = DInt.FromByteArray(tagValInBytes);
                            break;
                        case EnumPlcTagType.F32:
                            objVal = Real.FromByteArray(tagValInBytes);
                            break;
                        case EnumPlcTagType.F64:
                            objVal = LReal.FromByteArray(tagValInBytes);
                            break;
                        case EnumPlcTagType.U8Array:
                            objVal = Encoding.GetEncoding(28591).GetString(tagValInBytes);
                            break;
                        default:
                            {
                                PlcTagValue value = new PlcTagValue(tag.Value); //使用默认值作为当前值
                                if (lastVal == null || lastVal.ToString() != value.Value)
                                {
                                    if (tag.Value.IsPublish)
                                        mCache.WriteAndPublishTag(PlcName, tag.Key, JsonConvert.SerializeObject(value));
                                    else
                                        mCache.WriteTag(PlcName, tag.Key, JsonConvert.SerializeObject(value));
                                    mDicLastVal[tag.Key] = value.Value;
                                    _logger.Error($"{tag.Value.TagName}的类型{tag.Value.TagType}无法解析", GetType().FullName);
                                }
                            }
                            continue;
                    }
                    
                    if(objVal == null || objVal.ToString() == null)
                    {
                        PlcTagValue value = new PlcTagValue(tag.Value); //使用默认值作为当前值
                        if (lastVal == null || lastVal.ToString() != value.Value)
                        {
                            if (tag.Value.IsPublish)
                                mCache.WriteAndPublishTag(PlcName, tag.Key, JsonConvert.SerializeObject(value));
                            else
                                mCache.WriteTag(PlcName, tag.Key, JsonConvert.SerializeObject(value));
                            mDicLastVal[tag.Key] = "";
                            _logger.Error($"\"{PlcName}\".\"{tag.Key}\"读取到的值为null", GetType().FullName);
                        }
                        continue;
                    }

                    PlcTagValue tagValue = new PlcTagValue(tag.Value);
                    tagValue.Quality = EnumQuality.Good;
                    tagValue.TimeStamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    tagValue.Value = objVal.ToString()!;
                    if (lastVal == null || lastVal.ToString() != tagValue.Value)
                    {
                        //<test
                        //if(tag.Value.TagName == "BranchInHasPallet")
                        //{
                        //    PlcDbHelper.WriteLog(
                        //        $"\"{tag.Value.TagName}\"：{tagValue.Value}",
                        //        LogGrade.INFO,
                        //        "PlcServer.Driver.Siemens.SiemensPlc.ReadAllAsync()");
                        //}
                        //test>

                        if (tag.Value.IsPublish)
                            mCache.WriteAndPublishTag(PlcName, tag.Key, JsonConvert.SerializeObject(tagValue));
                        else
                            mCache.WriteTag(PlcName, tag.Key, JsonConvert.SerializeObject(tagValue));
                        mDicLastVal[tag.Key] = tagValue.Value;
                    }
                }
            }

            mIsConnected = !bConnBreak;
        }

        public override async Task<PlcTagValue?> ReadTagAsync(string tagName)
        {
            if (mPlc == null || !mIsConnected)
            {
                _logger.Error($"{PlcName}未连接，无法读取变量{tagName}", GetType().FullName);
                return null;
            }

            if (!mTags.ContainsKey(tagName))
            {
                _logger.Error($"{PlcName}不包含变量{tagName}，无法读取", GetType().FullName);
                return null;
            }

            PlcTag tag = mTags[tagName];

            DataType dataType;
            VarType varType;
            int db, startByteAdr, bitAdr = 0, byteCount;
            try
            {
                PLCAddress.Parse(
                tag.TagAddr,
                out dataType,
                out db,
                out varType,
                out startByteAdr,
                out bitAdr,
                out byteCount);
            }
            catch (Exception ex)
            {
                _logger.Error($"{PlcName}.{tagName}的地址{tag.TagAddr}分析失败，{ex.Message}", GetType().FullName);
                return null;
            }
            try
            {
                object? objVal = null;
                if (tag.TagType == EnumPlcTagType.I8 ||
                    tag.TagType == EnumPlcTagType.U16Array)
                {
                    _logger.Error($"{PlcName}.{tagName}的类型{tag.TagType.ToString()}不能识别", GetType().FullName);
                    return null;
                }
                else if (tag.TagType == EnumPlcTagType.U8Array)
                {
                    objVal = await mPlc.ReadAsync(dataType, db, startByteAdr, varType, byteCount);
                }
                else
                {
                    objVal = await mPlc.ReadAsync(dataType, db, startByteAdr, varType, 1, (byte)bitAdr);
                }

                if (objVal == null || objVal.ToString() == null)
                {
                    _logger.Error($"{PlcName}.{tagName}的值读取失败", GetType().FullName);
                    return null;
                }

                PlcTagValue tagValue = new PlcTagValue(tag);
                if (tag.TagType == EnumPlcTagType.U8Array)
                {
                    tagValue.Value = Encoding.GetEncoding(28591).GetString((byte[])objVal);
                }
                else
                {
                    tagValue.Value = objVal.ToString()!;
                }
                tagValue.Quality = EnumQuality.Good;
                tagValue.TimeStamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                mCache.WriteTag(PlcName, tagName, JsonConvert.SerializeObject(tagValue));
                return tagValue;
            }
            catch(Exception ex)
            {
                _logger.Error($"{PlcName}.{tagName}读取失败，{ex.Message}", GetType().FullName);
                return null;
            }
        }

        public override async Task<bool> WriteTagAsync(string tagName, string tagValue)
        {
            if (mPlc == null || !mIsConnected)
            {
                _logger.Error($"{PlcName}未连接，无法写变量{tagName}", GetType().FullName);
                return false;
            }

            if (!mTags.ContainsKey(tagName))
            {
                _logger.Error($"{PlcName}不包含变量{tagName}，无法写入", GetType().FullName);
                return false;
            }

            //[0] tagName地址解析
            object? value = null;
            int bitAdr = -1;
            int startByte = -1;
            DataType dataType;
            int dbNumber = -1;
            int byteCount = -1;
            PlcTag tag = mTags[tagName];
            try
            {
                PLCAddress.Parse(tag.TagAddr,
                    out dataType,
                    out dbNumber,
                    out VarType vt,
                    out startByte,
                    out bitAdr,
                    out byteCount);
            }
            catch(Exception ex)
            {
                _logger.Error($"{PlcName}.{tagName}写入失败，地址{tag.TagAddr}无法解析，{ex.Message}", GetType().FullName);
                return false;
            }

            //[1] tagValue转byte数组
            switch (tag.TagType)
            {
                case EnumPlcTagType.Bit:
                    if (!bool.TryParse(tagValue, out bool bitValue))
                    {
                        _logger.Error($"{PlcName}.{tagName}写入失败，值{tagValue}与类型{tag.TagType.ToString()}不符", GetType().FullName);
                        return false;
                    }
                    value = bitValue;
                    if(bitAdr < 0 || bitAdr > 7)
                    {
                        _logger.Error($"{PlcName}.{tagName}写入失败，bit地址{bitAdr}不在范围0-7之内", GetType().FullName);
                        return false;
                    }
                    break;
                case EnumPlcTagType.U8:
                    if (!byte.TryParse(tagValue, out byte u8Value))
                    {
                        _logger.Error($"{PlcName}.{tagName}写入失败，值{tagValue}与类型{tag.TagType.ToString()}不符", GetType().FullName);
                        return false;
                    }
                    value = new byte[] { u8Value };
                    bitAdr = -1;
                    break;
                case EnumPlcTagType.U16:
                    if (!UInt16.TryParse(tagValue, out UInt16 u16Value))
                    {
                        _logger.Error($"{PlcName}.{tagName}写入失败，值{tagValue}与类型{tag.TagType.ToString()}不符", GetType().FullName);
                        return false;
                    }
                    value = Word.ToByteArray(u16Value);
                    bitAdr = -1;
                    break;
                case EnumPlcTagType.I16:
                    if (!Int16.TryParse(tagValue, out Int16 i16Value))
                    {
                        _logger.Error($"{PlcName}.{tagName}写入失败，值{tagValue}与类型{tag.TagType.ToString()}不符", GetType().FullName);
                        return false;
                    }
                    value = Int.ToByteArray(i16Value);
                    bitAdr = -1;
                    break;
                case EnumPlcTagType.U32:
                    if (!UInt32.TryParse(tagValue, out UInt32 u32Value))
                    {
                        _logger.Error($"{PlcName}.{tagName}写入失败，值{tagValue}与类型{tag.TagType.ToString()}不符", GetType().FullName);
                        return false;
                    }
                    value = DWord.ToByteArray(u32Value);
                    bitAdr = -1;
                    break;
                case EnumPlcTagType.I32:
                    if (!Int32.TryParse(tagValue, out Int32 i32Value))
                    {
                        _logger.Error($"{PlcName}.{tagName}写入失败，值{tagValue}与类型{tag.TagType.ToString()}不符", GetType().FullName);
                        return false;
                    }
                    value = DInt.ToByteArray(i32Value);
                    bitAdr = -1;
                    break;
                case EnumPlcTagType.F32:
                    if (!float.TryParse(tagValue, out float f32Value))
                    {
                        _logger.Error($"{PlcName}.{tagName}写入失败，值{tagValue}与类型{tag.TagType.ToString()}不符", GetType().FullName);
                        return false;
                    }
                    value = Real.ToByteArray(f32Value);
                    bitAdr = -1;
                    break;
                case EnumPlcTagType.F64:
                    if (!double.TryParse(tagValue, out double f64Value))
                    {
                        _logger.Error($"{PlcName}.{tagName}写入失败，值{tagValue}与类型{tag.TagType.ToString()}不符", GetType().FullName);
                        return false;
                    }
                    value = LReal.ToByteArray(f64Value);
                    bitAdr = -1;
                    break;
                case EnumPlcTagType.U8Array:
                    try
                    {
                        value = Encoding.GetEncoding(28591).GetBytes(tagValue);
                        bitAdr = -1;
                    }
                    catch(Exception ex)
                    {
                        _logger.Error($"{PlcName}.{tagName}写入失败，值{tagValue}与类型{tag.TagType.ToString()}不符，{ex.Message}", GetType().FullName);
                        return false;
                    }
                    break;
                default:
                    _logger.Error($"{PlcName}.{tagName}写入失败，类型{tag.TagType.ToString()}不能识别", GetType().FullName);
                    return false;
            }

            try
            {
                await mPlc.WriteAsync(dataType, dbNumber, startByte, value, bitAdr);
                return true;
            }
            catch(Exception ex)
            {
                _logger.Error($"{PlcName}.{tagName}写入失败，{ex.Message}", GetType().FullName);
                return false;
            }
        }
    }
}
