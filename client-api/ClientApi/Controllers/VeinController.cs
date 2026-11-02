using ClientApi.Model;
using ClientApi.NetApi;
using ClientApi.Vein;
using ClientApi.Vein.Dto;
using Microsoft.AspNetCore.Mvc;

namespace ClientApi.Controllers
{
    /// <summary>指静脉设备的识别、注册和删除接口。</summary>
    [ApiController]
    [Route("[controller]")]
    public class VeinController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<VeinController> _logger;

        public VeinController(ILogger<VeinController> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// 采集当前手指并匹配设备中的指静脉模板。
        /// </summary>
        /// <returns>匹配成功返回指静脉编号；等待手指失败返回 0，SDK 比对失败返回 -1。</returns>
        [HttpGet("VeinMatch")]
        public int VeinMatch()
        {
            int ret;
            int[] fingerId = new int[1];

            //等待手指放置
            if (!example_WaitFingerStatus((byte)0x03))
            {
                return 0;
                //return false;
            }

            //手指直接比对
            ret = BioVeinCaller.FV_DirectMatch(VeinHelper.g_devId, 0, 0, fingerId);
            if (ret != 0)
            {
                Console.WriteLine("比对失败: " + ret);

                return -1;
                //return false;

            }

            Console.WriteLine("比对成功，编号: " + fingerId[0]);
            return fingerId[0];
        }

        /// <summary>识别手指并返回设备就绪状态、匹配编号或 SDK 错误。</summary>
        /// <returns>明确的执行结果，不将设备故障当作未匹配。</returns>
        [HttpGet("VeinMatchResult")]
        public VeinOperationResultDto VeinMatchResult()
        {
            if (!VeinHelper.IsReady)
                return new(false, "指静脉设备未就绪，请检查启用设置、设备连接和启动日志。");
            if (!example_WaitFingerStatus(0x03, out var detectCode))
                return new(false, "等待放入手指失败。", sdkCode: detectCode);
            int[] fingerId = new int[1];
            int ret = BioVeinCaller.FV_DirectMatch(VeinHelper.g_devId, 0, 0, fingerId);
            if (ret != 0)
                return new(false, "指静脉 SDK 比对失败。", sdkCode: ret);
            if (fingerId[0] <= 0)
                return new(false, "设备未返回有效模板编号。");
            return new(true, $"匹配设备模板：{fingerId[0]}", fingerId[0]);
        }

        private byte[] g_featureRegister = new byte[512 * 6]; //注册特征缓冲区，特征长度512字节，这里演示采集同指3次
        private byte[] g_dynfeature = new byte[512 * 6];//所注册对象的学习特征缓冲区，学习特征可以提高比对体验
        private int g_dyncnt = 0;
        private int g_registerCnt = 0;
        /// <summary>
        /// 连续采集同一手指六次，并将模板写入设备。
        /// </summary>
        /// <param name="userId">业务用户编号；当前实现未使用此参数。</param>
        /// <param name="fingerId">写入设备的指静脉编号；默认 0 时采集后不写入。</param>
        /// <remarks>需要连接设备并按提示放置、移开同一根手指；接口不返回注册结果正文。</remarks>
        [HttpPost("VeinRegister")]
        public void VeinRegister(string userId, int fingerId=0)
            => VeinRegisterResult(userId, fingerId);

        /// <summary>连续采集同一手指六次，只有写入设备成功才返回成功结果。</summary>
        /// <param name="userId">业务用户编号，用于校验进度归属；不作为设备模板编号。</param>
        /// <param name="fingerId">必须为正数的设备模板编号。</param>
        /// <param name="operationId">可选的本次录入标识，用于查询真实采集进度；旧调用方可省略。</param>
        /// <returns>完成采集数、模板编号或失败阶段与原始 SDK 错误码。</returns>
        [HttpPost("VeinRegisterResult")]
        public VeinOperationResultDto VeinRegisterResult(string userId, int fingerId, Guid? operationId = null)
        {
            if (operationId == Guid.Empty)
                return new(false, "录入操作标识无效。");
            if (operationId.HasValue && !VeinRegistrationProgressStore.TryStart(operationId.Value, userId))
                return new(false, "该录入操作已经存在，请勿重复提交。");

            // 所有正常结束分支都发布最终设备结果，供仍在等待 HTTP 响应的页面读取。
            VeinOperationResultDto Finish(VeinOperationResultDto result)
            {
                VeinRegistrationProgressStore.Update(operationId, result.Success ? "completed" : "failed",
                    result.Msg, result.CompletedSamples, true, result.Success, result.SdkCode);
                return result;
            }
            if (fingerId <= 0)
                return Finish(new(false, "设备模板编号必须为正整数。"));
            if (!VeinHelper.IsReady)
                return Finish(new(false, "指静脉设备未就绪，请检查启用设置、设备连接和启动日志。"));
            int ret;
            int readCnt = 0; //读取模板次数的计数器
            byte[] featureData = new byte[512];

            g_registerCnt = 0;
            for (int i = 0; i < 6; i++)
            {
                VeinRegistrationProgressStore.Update(operationId, "place",
                    $"第 {i + 1}/6 次：请放入同一根手指，保持平稳。", readCnt);
                //等待手指放置
                if (!example_WaitFingerStatus((byte)0x03, out var detectCode))
                {
                    return Finish(new(false, "等待放入手指失败，请按页面提示及时放入手指。", completedSamples: readCnt, sdkCode: detectCode));
                }


                VeinRegistrationProgressStore.Update(operationId, "capturing",
                    $"正在采集第 {i + 1}/6 次特征，请保持手指平稳，不要移动。", readCnt);
                //采集指静脉模板
                ret = BioVeinCaller.FV_GrabFeature(VeinHelper.g_devId, featureData, (byte)0);
                if (ret != 0)
                {
                    Console.WriteLine("FV_GrabFeature ERR: " + ret);
                    return Finish(new(false, "采集指静脉特征失败，请保持手指平稳后重试。", completedSamples: readCnt, sdkCode: ret));
                    //return false;
                }

                //第一枚以后的特征要与它之前的特征进行简单验证，判断是否同一手指，
                if (i > 0)
                {
                    //与已经采集的所有特征进行确认，检查是否属于同一根手指
                    ret = BioVeinCaller.FV_IsSameFinger(featureData,//本次采集的指静脉特征值
                            g_featureRegister,//包含之前采集的指静脉特征值数据
                            (byte)readCnt, //第二个参数中包含的指静脉特征值的个数
                            (byte)0x03); //固定为3
                    if (ret != 0)
                    {
                        Console.WriteLine("请使用相同手指！");
                        Console.WriteLine("FV_IsSameFinger ERR: " + ret);
                        return Finish(new(false, "请使用同一根手指完成注册，放回时请保持相同位置和方向。", completedSamples: readCnt, sdkCode: ret));
                        //return false;
                    }
                }

                //累计数据
                Array.Copy(featureData, 0, g_featureRegister, i * 512, 512);
                readCnt++;

                Console.WriteLine("第" + readCnt + "枚静脉特征采集成功");
                VeinRegistrationProgressStore.Update(operationId, "remove",
                    $"第 {readCnt}/6 次采集成功，请完全移开手指，等待下一步提示。", readCnt);

                //等待手指放置
                if (!example_WaitFingerStatus((byte)0x00, out var removeCode))
                    return Finish(new(false, "等待移开手指失败，请在采集成功后完全移开手指。", completedSamples: readCnt, sdkCode: removeCode));
                //return false;
            }

            g_registerCnt = 6;
            VeinRegistrationProgressStore.Update(operationId, "writing", "六次采集已完成，正在写入设备模板，请稍候。", readCnt);
            //下载注册数据
            //int fingerId = VeinService.UploadUserVeinData(g_featureRegister, (int)userId);

            //return false;
            //byte fingerGroup = Convert.ToByte(userId);//不能用作用户ID，最大支持256
            byte fingerGroup = 1;
            byte[] extend = new byte[16];
            ret = BioVeinCaller.FV_DownloadFinger(VeinHelper.g_devId, fingerId, g_featureRegister, (byte)g_registerCnt, fingerGroup, extend);
            if (ret != 0)
            {
                Console.WriteLine("下载手指数据失败: " + ret);
                return Finish(new(false, "写入设备模板失败。", completedSamples: readCnt, sdkCode: ret));
                //return false;
            }
            Console.WriteLine("下载手指数据成功，分配编号: " + fingerId + "，特征个数：" + g_registerCnt);
            //Console.WriteLine("采集完成");
            //UpdateVeinData();

            return Finish(new(true, $"设备模板 {fingerId} 注册成功。", fingerId, readCnt));
        }

        /// <summary>查询指定录入操作的实际进度；仅读取内存，不调用或占用指静脉 SDK。</summary>
        /// <param name="operationId">注册请求提供的随机操作标识。</param>
        /// <returns>当前阶段及采集次数；未知或过期操作返回 404。</returns>
        [HttpGet("VeinRegisterProgress")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public ActionResult<VeinRegistrationProgressDto> VeinRegisterProgress(Guid operationId)
        {
            var progress = VeinRegistrationProgressStore.Get(operationId);
            // 业务上的“操作尚不存在”带固定业务码，供 WMS 与旧程序缺少整个路由的 404 区分。
            return progress == null
                ? NotFound(new { code = "VeinRegistrationNotFound", message = "本次录入进度尚未创建或已过期。" })
                : Ok(progress);
        }

        /// <summary>
        /// 单次采集指静脉，累计六次后完成注册。
        /// </summary>
        /// <param name="input">业务用户编号、已完成采集次数及上一次返回的特征数据。</param>
        /// <returns>本次采集或注册结果，包含成功标记、提示信息和累计特征数据。</returns>
        /// <remarks>首次 RegisterCount 为 0；后续递增至 5，并传回上次返回的 VeinFeature。</remarks>
        [HttpPost("VeinRegisterOnce")]
        public VeinRegisterOnceOutDto VeinRegisterOnce(VeinRegisterOnceDto input)
        {
            if (input.UserId == null)
            { 
                throw new ArgumentNullException(nameof(input.UserId));
            }
            byte[] veinFeature;
            if (input.VeinFeature == null|| input.VeinFeature == "")
            {
                veinFeature = new byte[512 * 6];
            }
            else
            {
                veinFeature = Convert.FromBase64String(input.VeinFeature);
            }
            int ret;
            //int readCnt = 0; //读取模板次数的计数器
            byte[] featureData = new byte[512];

                //等待手指放置
                if (!example_WaitFingerStatus((byte)0x03))
                {
                    //return;
                    //return false;
                }


                //采集指静脉模板
                ret = BioVeinCaller.FV_GrabFeature(VeinHelper.g_devId, featureData, (byte)0);
                if (ret != 0)
                {
                    Console.WriteLine("FV_GrabFeature ERR: " + ret);
                    return new VeinRegisterOnceOutDto($"错误代码{ret}",false,input.VeinFeature);
                    //return false;
                }

                //第一枚以后的特征要与它之前的特征进行简单验证，判断是否同一手指，
                if (input.RegisterCount > 0)
                {
                    //与已经采集的所有特征进行确认，检查是否属于同一根手指
                    ret = BioVeinCaller.FV_IsSameFinger(featureData,//本次采集的指静脉特征值
                            veinFeature,//包含之前采集的指静脉特征值数据
                            (byte)(input.RegisterCount), //第二个参数中包含的指静脉特征值的个数
                            (byte)0x03); //固定为3
                    if (ret != 0)
                    {
                        Console.WriteLine("请使用相同手指！");
                        Console.WriteLine("FV_IsSameFinger ERR: " + ret);
                    return new VeinRegisterOnceOutDto($"请使用相同手指{ret}", false, input.VeinFeature);
                    //return false;
                }
                }

                //累计数据
                Array.Copy(featureData, 0, veinFeature, input.RegisterCount * 512, 512);
                //readCnt++;

                Console.WriteLine("第" + (input.RegisterCount + 1) + "枚静脉特征采集成功");

            ////等待手指移开
            //if (!example_WaitFingerStatus((byte)0x00))
            //    //return;

            if (input.RegisterCount == 5)
            {
                g_registerCnt = 6;
                //下载注册数据
                int fingerId = VeinService.UploadUserVeinData(g_featureRegister, Convert.ToInt16(input.UserId));
                if (fingerId == 0)
                {
                    return new VeinRegisterOnceOutDto($"指静脉保存到数据库失败", false, input.VeinFeature);
                }

                //return false;
                //byte fingerGroup = Convert.ToByte(userId);//不能用作用户ID，最大支持256
                byte fingerGroup = 1;
                byte[] extend = new byte[16];
                ret = BioVeinCaller.FV_DownloadFinger(VeinHelper.g_devId, fingerId, veinFeature, (byte)g_registerCnt, fingerGroup, extend);
                if (ret != 0)
                {
                    Console.WriteLine("下载手指数据失败: " + ret);
                    return new VeinRegisterOnceOutDto($"下载手指数据失败{ret}", false, input.VeinFeature);
                    //return false;
                }
                Console.WriteLine("下载手指数据成功，分配编号: " + fingerId + "，特征个数：" + g_registerCnt);
                //Console.WriteLine("采集完成");
                //UpdateVeinData();
                return new VeinRegisterOnceOutDto($"指静脉注册成功", true, Convert.ToBase64String(veinFeature));
                //return true;
            }
            if (input.RegisterCount < 5)
            {
                return new VeinRegisterOnceOutDto($"第{input.RegisterCount + 1}枚静脉特征采集成功", true, Convert.ToBase64String(veinFeature));
            }
            else
            {
                return new VeinRegisterOnceOutDto($"超出注册次数{input.RegisterCount}", false, input.VeinFeature);
            }
        }

        /// <summary>
        /// 按指静脉编号删除设备模板和后台记录。
        /// </summary>
        /// <param name="input">待删除的非零指静脉编号。</param>
        /// <returns>设备删除成功返回 true；SDK 删除失败返回 false。</returns>
        /// <remarks>设备返回 -15 时仍会请求删除后台记录，但接口返回 false；编号为 0 时抛出异常。</remarks>
        [HttpPost("VeinDelete")]
        public bool VeinDelete(VeinDeleteDto input)
        {
            if (input.VeinId == 0)
            {
                throw new Exception("参数不能为0");
            }
            //删除指定的
            int ret;
            byte flg = 3;
            int fingerId = input.VeinId;
            //byte flg = 0;
            //int fingerId = 0;

            ret = BioVeinCaller.FV_DeleteFinger(VeinHelper.g_devId, flg, input.VeinId);
            Logger.Singleton.Debug(input.VeinId.ToString() + "指静脉删除返回值" + ret.ToString());
            if (ret != 0)
            {
                Console.WriteLine("删除手指失败: " + ret);
                if (ret == -15)
                {
                    //用户指静脉数据不存在时，删除数据库数据
                    VeinService.DeleteVeinDataById(input.VeinId);
                }
                return false;
            }
            VeinService.DeleteVeinDataById(input.VeinId);
            Console.WriteLine("删除手指成功，编号: " + input.VeinId);
            return true;
        }
        /// <summary>
        /// 等待放入手指
        /// </summary>
        /// <param name="bFingerStatus"></param>
        /// <returns></returns>
        private bool example_WaitFingerStatus(byte bFingerStatus)
            => example_WaitFingerStatus(bFingerStatus, out _);

        /// <summary>等待手指状态；SDK 故障保留错误码，正常等待超时的错误码为空。</summary>
        private bool example_WaitFingerStatus(byte bFingerStatus, out int? sdkCode)
        {
            sdkCode = null;
            int ret;
            int nTimes = 50; //检测的次数，必须大于0 //50次大概6秒左右
            int nInterval = 30;//每次检测的间隔，单位为毫秒
            byte[] fingerStatus = new byte[1];
            String msg = "";
            long time_begin = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (0 == bFingerStatus)
            {
                Console.Write("请移开手指！");
                //label2.Text = "请移开手指！";
            }
            else if (0x03 == bFingerStatus)
            {
                Logger.Singleton.Error("采集开始请放手指");
                Console.Write("请放手指！");
            }
            else
                return false;

            for (int nCnt = 0; nCnt < nTimes; nCnt++)
            {
                Console.Write(msg);
                ret = BioVeinCaller.FV_FingerDetect(VeinHelper.g_devId, fingerStatus);
                if (0 != ret)
                {
                    sdkCode = ret;
                    Console.WriteLine("FV_FingerDetect ERR: " + ret);
                    //label2.Text = "FV_GetSdkVersion ERR: " + ret;
                    return false;
                }
                if (bFingerStatus == fingerStatus[0])
                {
                    Console.WriteLine("");
                    return true;
                }
                Thread.Sleep(nInterval);
                if (0 == (nCnt % (1000 / nInterval)))
                {
                    msg += "*";
                }
            }
            long time_end = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Console.Write($"WaitFingerStatus cost { time_end - time_begin}");
            Logger.Singleton.Debug($"WaitFingerStatus cost { time_end - time_begin}");
            return false;
        }
    }
}
