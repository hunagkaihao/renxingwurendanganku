using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Services;
using Ecs.ConfigTool;
using Ecs.LogTool;
using Ecs.RedisTool;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ecs.Mjj;

public class MjjOpResult
{
    public bool success { get; set; }
    public string errMsg { get; set; }
}

public class MjjManager : IDomainService
{
    private readonly IRedisClient _redisClient;
    private readonly ILogger<MjjManager> _logger;
    private readonly IOptions<ConfigOptions> _options;

    public MjjManager(
        IRedisClient redisClient, 
        ILogger<MjjManager> logger,
        IOptions<ConfigOptions> options)
    {
        _options = options;
        _logger = logger;
        _redisClient = redisClient;
        _redisClient.Build(_options.Value.RedisConnStr, _options.Value.DefaultRedisNo);
    }

    public async Task<MjjStatus> GetMjjStatusAync()
    {
        try
        {
            KeyValuePair<string, string>[] pairs = await _redisClient.GetAllHashFieldValuePairsAsync(EcsConsts.MjjStatusChannel).ConfigureAwait(false);
            if(pairs == null || pairs.Length == 0)
                return new MjjStatus();
            
            Dictionary<string, string> dicResult = new Dictionary<string, string>();
            foreach(var pair in pairs)
            {
                if(pair.Key != null)
                    dicResult[pair.Key] = pair.Value;
            }

            return new MjjStatus
            {
                Co2 = dicResult.ContainsKey("CO2") ? dicResult["CO2"] : "none",
                ColNo = dicResult.ContainsKey("COLNO") ? dicResult["COLNO"] : "none",
                ColumnDWZT_changed = dicResult.ContainsKey("COLUMNDWZT_CHANGED") ? dicResult["COLUMNDWZT_CHANGED"] : "none",
                ColumnStatus = dicResult.ContainsKey("ColumnStatus") ? dicResult["ColumnStatus"] : "none",
                Data = dicResult.ContainsKey("DATA") ? dicResult["DATA"] : "none",
                Hum = dicResult.ContainsKey("Hum") ? dicResult["Hum"] : "none",
                IsBJ = dicResult.ContainsKey("IsBJ") ? dicResult["IsBJ"] : "none",
                IsLock = dicResult.ContainsKey("IsLock") ? dicResult["IsLock"] : "none",
                IsPower = dicResult.ContainsKey("IsPower") ? dicResult["IsPower"] : "none",
                IsVent = dicResult.ContainsKey("IsVent") ? dicResult["IsVent"] : "none",
                IsZDKJ = dicResult.ContainsKey("IsZDKJ") ? dicResult["IsZDKJ"] : "none",
                MjjZTLX = dicResult.ContainsKey("MJJZTLX") ? dicResult["MJJZTLX"] : "none",
                MjjZTLXName = dicResult.ContainsKey("MJJZTLXName") ? dicResult["MJJZTLXName"] : "none",
                Pm10 = dicResult.ContainsKey("PM10") ? dicResult["PM10"] : "none",
                Pm2_5 = dicResult.ContainsKey("PM2_5") ? dicResult["PM2_5"] : "none",
                QuNo = dicResult.ContainsKey("QUNO") ? dicResult["QUNO"] : "none",
                Temp = dicResult.ContainsKey("Temp") ? dicResult["Temp"] : "none",
                Tvoc = dicResult.ContainsKey("TVOC") ? dicResult["TVOC"] : "none"
            };
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new MjjStatus();
        }
    }

    public async Task<MjjOpResult> MoveLeftAsync(byte colNo)
    {
        try
        {
            MjjMessage msg = new MjjMessage()
            {
                Cmd = "MoveLeft",
                Para = colNo.ToString(),
                ResponseChannel = Guid.NewGuid().ToString()
            };
            string strMsg = JsonConvert.SerializeObject(msg);
            bool ret = await _redisClient.PublishAsync(EcsConsts.MjjCmdChannel, strMsg).ConfigureAwait(false);
            if(!ret)
                throw new Exception("Mjj服务没有收到MoveLeft指令");

            long firstTime = DateTime.Now.Ticks;
            while(true)
            {
                await Task.Delay(200).ConfigureAwait(false);

                long thisTime = DateTime.Now.Ticks;
                TimeSpan span = new TimeSpan(thisTime - firstTime);
                if(span.TotalMilliseconds > _options.Value.MjjOperateTimeout)
                    throw new Exception("MoveLeft密集架指令反馈超时");

                string result = await _redisClient.GetStringValueAsync(msg.ResponseChannel).ConfigureAwait(false);
                if(result == null) //未收到反馈
                    continue;

                if(!bool.TryParse(result, out bool bRslt))
                    throw new Exception($"收到Mjj服务的MoveLeft指令反馈，但反馈的值为{result}，无法转换为bool");

                _redisClient.RemoveKey(msg.ResponseChannel);
                return new MjjOpResult(){ success = bRslt, errMsg = bRslt ? null : "MoveLeft失败" };
            }
            
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new MjjOpResult() { success = false, errMsg = ex.Message };
        }
    }

    public async Task<MjjOpResult> MoveRightAsync(byte colNo)
    {
        try
        {
            MjjMessage msg = new MjjMessage()
            {
                Cmd = "MoveRight",
                Para = colNo.ToString(),
                ResponseChannel = Guid.NewGuid().ToString()
            };
            string strMsg = JsonConvert.SerializeObject(msg);
            bool ret = await _redisClient.PublishAsync(EcsConsts.MjjCmdChannel, strMsg).ConfigureAwait(false);
            if(!ret)
                throw new Exception("Mjj服务没有收到MoveRight指令");

            long firstTime = DateTime.Now.Ticks;
            while(true)
            {
                await Task.Delay(200).ConfigureAwait(false);

                long thisTime = DateTime.Now.Ticks;
                TimeSpan span = new TimeSpan(thisTime - firstTime);
                if(span.TotalMilliseconds > _options.Value.MjjOperateTimeout)
                    throw new Exception("MoveRight密集架指令反馈超时");

                string result = await _redisClient.GetStringValueAsync(msg.ResponseChannel).ConfigureAwait(false);
                if(result == null) //未收到反馈
                    continue;

                if(!bool.TryParse(result, out bool bRslt))
                    throw new Exception($"收到Mjj服务的MoveRight指令反馈，但反馈的值为{result}，无法转换为bool");

                _redisClient.RemoveKey(msg.ResponseChannel);
                return new MjjOpResult(){ success = bRslt, errMsg = bRslt ? null : "MoveRight失败" };
            }
            
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new MjjOpResult() { success = false, errMsg = ex.Message };
        }
    }

    public async Task<MjjOpResult> OpenMjjAsync(byte colNo, byte zyNo, byte inoutState)
    {
        try
        {
            MjjMessage msg = new MjjMessage()
            {
                Cmd = "Open",
                Para = $"{colNo}@#${zyNo}@#${inoutState}",
                ResponseChannel = Guid.NewGuid().ToString()
            };
            string strMsg = JsonConvert.SerializeObject(msg);
            bool ret = await _redisClient.PublishAsync(EcsConsts.MjjCmdChannel, strMsg).ConfigureAwait(false);
            if(!ret)
                throw new Exception("Mjj服务没有收到Open指令");

            long firstTime = DateTime.Now.Ticks;
            while(true)
            {
                await Task.Delay(200).ConfigureAwait(false);

                long thisTime = DateTime.Now.Ticks;
                TimeSpan span = new TimeSpan(thisTime - firstTime);
                if(span.TotalMilliseconds > _options.Value.MjjOperateTimeout)
                    throw new Exception("Open密集架指令反馈超时");

                string result = await _redisClient.GetStringValueAsync(msg.ResponseChannel).ConfigureAwait(false);
                if(result == null) //未收到反馈
                    continue;

                if(!bool.TryParse(result, out bool bRslt))
                    throw new Exception($"收到Mjj服务的Open指令反馈，但反馈的值为{result}，无法转换为bool");

                _redisClient.RemoveKey(msg.ResponseChannel);
                return new MjjOpResult(){ success = bRslt, errMsg = bRslt ? null : "Open失败" };
            }
            
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new MjjOpResult() { success = false, errMsg = ex.Message };
        }
    }

    public async Task<MjjOpResult> ResetMjjAsync()
    {
        try
        {
            MjjMessage msg = new MjjMessage()
            {
                Cmd = "Reset",
                Para = "",
                ResponseChannel = Guid.NewGuid().ToString()
            };
            string strMsg = JsonConvert.SerializeObject(msg);
            bool ret = await _redisClient.PublishAsync(EcsConsts.MjjCmdChannel, strMsg).ConfigureAwait(false);
            if(!ret)
                throw new Exception("Mjj服务没有收到Reset指令");

            long firstTime = DateTime.Now.Ticks;
            while(true)
            {
                await Task.Delay(200).ConfigureAwait(false);

                long thisTime = DateTime.Now.Ticks;
                TimeSpan span = new TimeSpan(thisTime - firstTime);
                if(span.TotalMilliseconds > _options.Value.MjjOperateTimeout)
                    throw new Exception("Reset密集架指令反馈超时");

                string result = await _redisClient.GetStringValueAsync(msg.ResponseChannel).ConfigureAwait(false);
                if(result == null) //未收到反馈
                    continue;

                if(!bool.TryParse(result, out bool bRslt))
                    throw new Exception($"收到Mjj服务的Reset指令反馈，但反馈的值为{result}，无法转换为bool");

                _redisClient.RemoveKey(msg.ResponseChannel);
                return new MjjOpResult(){ success = bRslt, errMsg = bRslt ? null : "Reset失败" };
            }
            
        }
        catch(Exception ex)
        {
            _logger.Error(ex.Message);
            return new MjjOpResult() { success = false, errMsg = ex.Message };
        }
    }

    /// <summary>
    /// 判断密集架是否在目标位置
    /// </summary>
    /// <param name="colNo"></param>
    /// <param name="zyNo"></param>
    /// <param name="columnStatus"></param>
    /// <returns>MjjOpResult.errMsg为null时，MjjOpResult.success为true表示在目标位，为false表示不在目标位，MjjOpResult.errMsg不为null时，表示判断发送错误，此时MjjOpResult.success不能表示是否在目标位</returns>
    public MjjOpResult IsMjjAtTargetPosition(byte colNo, byte zyNo, string columnStatus)
    {
        int colCnt = _options.Value.MjjColCnt;
        int colCntLeftOfFixCol = _options.Value.MjjColCntLeftOfFixCol;
        int colCntRightOfFixCol = _options.Value.MjjColCntRightOfFixCol;
        string fixedColPos = _options.Value.MjjFixColPos.ToLower();
        bool fixColAvailable = _options.Value.MjjFixColAvailable;
        
        if(zyNo < 1 || zyNo > 2) //非法左右值
            return new MjjOpResult() { success = false, errMsg = $"非法的左右值：{zyNo}，应为1：左，2：右"};

        if(fixedColPos != "left" && fixedColPos != "right" && fixedColPos != "middle")
            return new MjjOpResult() { success = false, errMsg = $"密集架固定列配置有误，应为left、right、middle之一，但实际为{fixedColPos}" };

        if(colCnt != colCntLeftOfFixCol + colCntRightOfFixCol + 1)
            return new MjjOpResult() { success = false, errMsg = $"密集架列数量配置有误，应确保如下等式成立，列数 = 固定列左侧列数 + 固定列右侧列数 + 1" };

        if(colNo < 1 || colNo > colCnt)
            return new MjjOpResult() { success = false, errMsg = $"非法的列值：{colNo}，应在1~{colCnt}之间"};

        if(columnStatus.Length != colCnt)
            return new MjjOpResult() { success = false, errMsg = $"列状态信息{columnStatus}包含的字符数错误，应为{colCnt}，实际为{columnStatus.Length}" };

        if(fixedColPos == "left") //左固定
        {              
            if(colCntLeftOfFixCol != 0)
                return new MjjOpResult() { success = false, errMsg = $"密集架列数量配置有误，左固定密集架的固定列左侧列数应为0，当前为{colCntLeftOfFixCol}" };
        
            if(colCnt == 1) //只有1列，是不允许的
                return new MjjOpResult() { success = false, errMsg = $"密集架列数量配置有误，左固定密集架的列数须大于1" };

            if(zyNo == 1) //左侧
            {
                if(colNo == 1) 
                {
                    if(!fixColAvailable) //左固定列左侧不能取放
                        return new MjjOpResult() { success = false, errMsg = $"左固定列左侧无法到达"};
                    else //左固定列左侧可以取放，密集架不需要移动
                        return new MjjOpResult() { success = true, errMsg = null };
                }                        

                string last = columnStatus.Substring(colNo - 2, 1);
                string me = columnStatus.Substring(colNo - 1, 1);
                if(last == "1" && me == "2")
                    return new MjjOpResult() { success = true, errMsg = null };
                else
                    return new MjjOpResult() { success = false, errMsg = null };
            }
            else //右侧
            {
                if(colNo == colCnt) //最右侧活动列，密集架应处于闭合状态
                {
                    string me = columnStatus.Substring(colNo - 1, 1);
                    if(me == "1")
                        return new MjjOpResult() { success = true, errMsg = null };
                    else
                        return new MjjOpResult() { success = false, errMsg = null };
                }
                else
                {
                    string me = columnStatus.Substring(colNo - 1, 1);
                    string next = columnStatus.Substring(colNo, 1);
                    if(me == "1" && next == "2")
                        return new MjjOpResult() { success = true, errMsg = null };
                    else
                        return new MjjOpResult() { success = false, errMsg = null };
                }
            }
        }
        else if(fixedColPos == "right") //右固定
        {
            if(colCntRightOfFixCol != 0)
                return new MjjOpResult() { success = false, errMsg = $"密集架列数量配置有误，右固定密集架的固定列右侧列数应为0，当前为{colCntRightOfFixCol}" };
        
            if(colCnt == 1) //只有1列，是不允许的
                return new MjjOpResult() { success = false, errMsg = $"密集架列数量配置有误，右固定密集架的列数须大于1" };

            if(zyNo == 1) //左侧
            {
                if(colNo == 1) //最左侧活动列左侧，密集架处于闭合状态
                {                       
                    string me = columnStatus.Substring(0, 1);
                    if(me == "2")
                        return new MjjOpResult() { success = true, errMsg = null };
                    else
                        return new MjjOpResult() { success = false, errMsg = null };
                }
                else
                {
                    string me = columnStatus.Substring(colNo - 1, 1);
                    string last = columnStatus.Substring(colNo - 2, 1);
                    if(last == "1" && me == "2")
                        return new MjjOpResult() { success = true, errMsg = null };
                    else
                        // return new MjjOpResult() { success = false, errMsg = $"尚未在目标位置，当前为{columnStatus}" };
                        return new MjjOpResult() { success = false, errMsg = null };
                }
            }
            else //右侧
            {
                if(colNo == colCnt) 
                {
                    if(!fixColAvailable) //右固定列右侧，无法到达
                        return new MjjOpResult() { success = false, errMsg = $"右固定列右侧无法到达"};
                    else //右固定列右侧，可以到达，密集架无需移动
                        return new MjjOpResult() { success = true, errMsg = null };
                }
                else
                {
                    string me = columnStatus.Substring(colNo - 1, 1);
                    string next = columnStatus.Substring(colNo, 1);
                    if(me == "1" && next == "2")
                        return new MjjOpResult() { success = true, errMsg = null };
                    else
                        return new MjjOpResult() { success = false, errMsg = null };
                }
            }
        }
        else //中间固定
        {
            if(colCntLeftOfFixCol == 0 || colCntRightOfFixCol == 0)
                return new MjjOpResult() { success = false, errMsg = $"密集架列数量配置有误，中间固定密集架的固定列左侧列数和固定列右侧列数不能为0" };

            if(zyNo == 1) //左侧
            {
                if(colNo == 1) //最左列左侧
                {
                    string me = columnStatus.Substring(0, 1);
                    if(me == "2")
                        return new MjjOpResult() { success = true, errMsg = null };
                    else
                        return new MjjOpResult() { success = false, errMsg = null };
                }
                else if(colNo == colCntLeftOfFixCol + 1) //固定列
                {
                    string last = columnStatus.Substring(colNo - 2, 1);
                    if(last == "1")
                        return new MjjOpResult() { success = true, errMsg = null };
                    else
                        return new MjjOpResult() { success = false, errMsg = null };
                }
                else if(colNo == colCntLeftOfFixCol + 2) //固定列右侧一列
                {
                    string me = columnStatus.Substring(colNo - 1, 1);
                    if(me == "2")
                        return new MjjOpResult() { success = true, errMsg = null };
                    else
                        return new MjjOpResult() { success = false, errMsg = null };
                }
                else
                {
                    string me = columnStatus.Substring(colNo - 1, 1);
                    string last = columnStatus.Substring(colNo - 2, 1);
                    if(last == "1" && me == "2")
                        return new MjjOpResult() { success = true, errMsg = null };
                    else
                        return new MjjOpResult() { success = false, errMsg = null };
                }
            }
            else //右侧
            {
                if(colNo == colCnt) //最右列右侧
                {
                    string me = columnStatus.Substring(colCnt - 1, 1);
                    if(me == "1")
                        return new MjjOpResult() { success = true, errMsg = null };
                    else
                        return new MjjOpResult() { success = false, errMsg = null };
                }
                else if(colNo == colCntLeftOfFixCol + 1) //固定列
                {
                    string next = columnStatus.Substring(colNo, 1);
                    if(next == "2")
                        return new MjjOpResult() { success = true, errMsg = null };
                    else
                        return new MjjOpResult() { success = false, errMsg = null };
                }
                else if(colNo == colCntLeftOfFixCol) //固定列左侧一列
                {
                    string me = columnStatus.Substring(colNo - 1, 1);
                    if(me == "1")
                        return new MjjOpResult() { success = true, errMsg = null };
                    else
                        return new MjjOpResult() { success = false, errMsg = null };
                }
                else
                {
                    string me = columnStatus.Substring(colNo - 1, 1);
                    string next = columnStatus.Substring(colNo, 1);
                    if(me == "1" && next == "2")
                        return new MjjOpResult() { success = true, errMsg = null };
                    else
                        return new MjjOpResult() { success = false, errMsg = null };
                }
            }
        }
        
    }

    /// <summary>
    /// 判断密集架是否在闭合位置
    /// </summary>
    /// <param name="columnStatus"></param>
    /// <returns>MjjOpResult.errMsg为null时，MjjOpResult.success为true表示在闭合位，为false表示不在闭合位，MjjOpResult.errMsg不为null时，表示判断发送错误，此时MjjOpResult.success不能表示是否在闭合位</returns>
    public MjjOpResult IsMjjAtClosedPosition(string columnStatus)
    {
        int colCnt = _options.Value.MjjColCnt;
        string fixedColPos = _options.Value.MjjFixColPos.ToLower();

        byte targetColNo;
        byte targetZYNo;

        if (fixedColPos == "left")
        {
            targetColNo = (byte)colCnt;
            targetZYNo = 2;
        }
        else if(fixedColPos == "right")
        {
            targetColNo = 1;
            targetZYNo = 1;
        }
        else if(fixedColPos == "middle")
        {
            targetColNo = 1;
            targetZYNo = 1;
            MjjOpResult result = IsMjjAtTargetPosition(targetColNo, targetZYNo, columnStatus);
            if(!result.success || result.errMsg != null)
                return result;

            targetColNo = (byte)colCnt;
            targetZYNo = 2;
            return IsMjjAtTargetPosition(targetColNo, targetZYNo, columnStatus);
        }
        else
            return new MjjOpResult() { success = false, errMsg = $"密集架的配置错误，固定列位置无法识别，应为left、right、middle，实际为{fixedColPos}" };
        
        return IsMjjAtTargetPosition(targetColNo, targetZYNo, columnStatus);
    }

    public bool IsMjjPosCanReach(byte colNo, byte zyNo, out string cannotReachReason)
    {
        cannotReachReason = string.Empty;

        int mjjColCnt = _options.Value.MjjColCnt; //密集架列数
        string fixCol = _options.Value.MjjFixColPos.ToLower(); //固定列位置
        bool fixColAvailable = _options.Value.MjjFixColAvailable; //固定列是否左右均可用
        if(colNo < 1 || colNo > mjjColCnt)
        {
            cannotReachReason = $"目标列为{colNo}，不在有效范围1~{mjjColCnt}内";
            return false;
        }

        if(zyNo != 1 && zyNo != 2)
        {
            cannotReachReason = $"左右值为{zyNo}，不在有效范围1~2内";
            return false;
        }

        if(fixCol == "left" && colNo == 1 && zyNo == 1 && !fixColAvailable)
        {
            cannotReachReason = $"密集架为左固定，且第1列左侧不能使用";
            return false;
        }

        if(fixCol == "right" && colNo == mjjColCnt && zyNo == 2 && !fixColAvailable)
        {
            cannotReachReason = $"密集架为右固定，且最右列（第{mjjColCnt}列）右侧不能使用";
            return false;
        }

        return true;
    }

    public int GetMjjColFromWmsCellRow(int cellRow)
    {
        string wmsFirstRowPos = _options.Value.WmsFirstRowPos.ToLower();
        int wmsFirstRowNo = _options.Value.WmsFirstRowNo;
        int wmsRowCnt = _options.Value.WmsRowCnt;
        int colCnt = _options.Value.MjjColCnt;
        
        if(wmsFirstRowPos != "left" && wmsFirstRowPos != "right")
            return -1;

        if(wmsFirstRowNo != 0 && wmsFirstRowNo != 1)
            return -1;

        if(cellRow > wmsRowCnt || cellRow < 1)
            return -1;

        if(wmsFirstRowPos == "left" && wmsFirstRowNo == 0)
        {
            int temp = 0;
            if(cellRow % 2 == 0) //偶数
                temp = cellRow;
            else
                temp = cellRow - 1;
            return temp / 2 + 1;
        }

        if(wmsFirstRowPos == "left" && wmsFirstRowNo == 1)
        {
            int temp = 0;
            if(cellRow % 2 == 0) //偶数
                temp = cellRow;
            else
                temp = cellRow + 1;
            return temp / 2;
        }

        if(wmsFirstRowPos == "right" && wmsFirstRowNo == 0)
        {
            int temp = 0;
            if(cellRow % 2 == 0) //偶数
                temp = cellRow;
            else
                temp = cellRow - 1;
            return colCnt - temp / 2;
        }

        if(wmsFirstRowPos == "right" && wmsFirstRowNo == 1)
        {
            int temp = 0;
            if(cellRow % 2 == 0) //偶数
                temp = cellRow;
            else
                temp = cellRow + 1;
            return colCnt - temp / 2 + 1;
        }

        return -1;
    }

    public int GetMjjZYNoFromCellRow(int cellRow)
    {
        string wmsFirstRowPos = _options.Value.WmsFirstRowPos.ToLower();
        int wmsFirstRowNo = _options.Value.WmsFirstRowNo;
        int wmsRowCnt = _options.Value.WmsRowCnt;
        int colCnt = _options.Value.MjjColCnt;
        
        if(wmsFirstRowPos != "left" && wmsFirstRowPos != "right")
            return -1;

        if(wmsFirstRowNo != 0 && wmsFirstRowNo != 1)
            return -1;

        if(cellRow > wmsRowCnt || cellRow < 1)
            return -1;

        if(wmsFirstRowPos == "left" && wmsFirstRowNo == 0)
        {
            if(cellRow % 2 == 0) //偶数
                return 1;
            else
                return 2;
        }

        if(wmsFirstRowPos == "left" && wmsFirstRowNo == 1)
        {
            if(cellRow % 2 == 0) //偶数
                return 2;
            else
                return 1;
        }

        if(wmsFirstRowPos == "right" && wmsFirstRowNo == 0)
        {
            if(cellRow % 2 == 0) //偶数
                return 2;
            else
                return 1;
        }

        if(wmsFirstRowPos == "right" && wmsFirstRowNo == 1)
        {
            if(cellRow % 2 == 0) //偶数
                return 1;
            else
                return 2;
        }

        return -1;
    }
}