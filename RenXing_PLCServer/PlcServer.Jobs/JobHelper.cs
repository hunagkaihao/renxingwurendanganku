using Newtonsoft.Json;
using PlcServer.Defines;
using PlcServer.Defines.Enum;
using Shared.Config;
using Shared.Logger.ILogger;
using Shared.Redis.IRedisCli;

namespace PlcServer.Jobs
{
    public class JobHelper
    {
        private static readonly string PlcServerRegChannelName = "RegisterChannel";
        private static readonly string TagTempChannelName = "JobsTriggerTemp";
        private const string mPlcRedisClientName = "JobsClient";
        private bool mPlcServerRegistered;
        private IRedisClient mPlcRedisClient;

        private ILog mLogger;

        public JobHelper(ILog logger, IRedisClient plcRedisClient)
        {
            mLogger = logger;

            mPlcServerRegistered = false;
            mPlcRedisClient = plcRedisClient;
            mPlcRedisClient.Build(Settings.ConfigData.RedisConnString, Settings.ConfigData.RedisDBNumForPlcCache);

            try
            {
                RegisterPlcServerClient(); //mPlcRedisClient注册到Plc服务器
                string?[] fields = mPlcRedisClient.GetHashFields(TagTempChannelName);
                mPlcRedisClient.RemoveHashFields(TagTempChannelName, fields);
            }
            catch (Exception e)
            {
                mLogger.Error(e.Message, GetType().FullName);
            }
        }

        public bool? IsPlcTagExist(string plcName, string tagName)
        {
            try
            {
                return mPlcRedisClient.IsKeyExist($"{plcName}.{tagName}");
            }
            catch (Exception e)
            {
                mLogger.Error(e.Message, GetType().FullName);
                return null;
            }
        }

        public PlcTagValue? ReadPlcTag(string plcName, string tagName)
        {
            try
            {
                string? val = mPlcRedisClient.GetStringValue($"{plcName}.{tagName}");
                if (val == null)
                {
                    mLogger.Error($"PLC变量 {plcName}.{tagName} 不存在，无法读取", GetType().FullName);
                    return null;
                }

                return JsonConvert.DeserializeObject<PlcTagValue>(val);
            }
            catch (Exception e)
            {
                mLogger.Error(e.Message, GetType().FullName);
                return null;
            }
        }

        public async Task<PlcTagValue?> ReadPlcTagAsync(string plcName, string tagName)
        {
            try
            {
                string? val = await mPlcRedisClient.GetStringValueAsync($"{plcName}.{tagName}");
                if (val == null)
                {
                    mLogger.Error($"PLC变量 {plcName}.{tagName} 不存在，无法读取", GetType().FullName);
                    return null;
                }

                return JsonConvert.DeserializeObject<PlcTagValue>(val);
            }
            catch (Exception e)
            {
                mLogger.Error($"{e.Message}", GetType().FullName);
                return null;
            }
        }

        public bool WritePlcTag(string plcName, string tagName, string tagValue)
        {
            string tempChannel = Guid.NewGuid().ToString();//接收写PLC操作结果的临时频道
            try
            {
                if (!mPlcServerRegistered)
                {
                    RegisterPlcServerClient();
                    Thread.Sleep(500);
                }
                if (!mPlcServerRegistered)
                {
                    mLogger.Error($"该客户端在Plc服务器上注册失败,无法将值{tagValue}写入变量{plcName}.{tagName}", GetType().FullName);
                    return false;
                }

                bool ret = mPlcRedisClient.Publish(mPlcRedisClientName, $"{plcName}@#${tagName}@#${tagValue}@#${tempChannel}");

                if (!ret) //Plc服务器没有收到发布
                {
                    RegisterPlcServerClient(); //可能Plc服务被关闭了，尝试重新注册
                    if (!mPlcServerRegistered) //没有注册成功
                    {
                        string msg = $"Plc服务可能被关闭，客户端{mPlcRedisClientName}在Plc服务器上注册失败,无法写Tag：{plcName}.{tagName}";
                        mLogger.Error(msg, GetType().FullName);
                        return false;
                    }
                    Thread.Sleep(500);
                    //成功注册，重新尝试写变量
                    ret = mPlcRedisClient.Publish(mPlcRedisClientName, $"{plcName}@#${tagName}@#${tagValue}@#${tempChannel}");
                }
                if (!ret)
                {
                    string msg = $"Plc服务可能被关闭，客户端{mPlcRedisClientName}发布的写变量{plcName}.{tagName}命令没有被PLC服务器接收到";
                    mLogger.Error(msg, GetType().FullName);
                    return false;
                }

                ret = false;
                long point1 = DateTime.Now.Ticks;
                while (true)
                {
                    Thread.Sleep(10);
                    long point2 = DateTime.Now.Ticks;
                    TimeSpan ts = new TimeSpan(point2 - point1);
                    if (ts.TotalMilliseconds > 5000) //超过5s，视为写操作失败
                    {
                        string msg = $"客户端{mPlcRedisClientName}发布的写变量{plcName}.{tagName}命令，PLC服务器已接收到，但客户端没有接收到服务器的写Tag结果，已超时";
                        mLogger.Error(msg, GetType().FullName);
                        break;
                    }

                    string? val = mPlcRedisClient.GetStringValue(tempChannel);
                    if (val == null) //还未收到写操作的反馈
                        continue;

                    if (!bool.TryParse(val, out ret)) //收到反馈
                    {
                        string msg = $"客户端{mPlcRedisClientName}发布将值{tagValue}写变量{plcName}.{tagName}的命令，PLC服务器反馈写Tag失败";
                        mLogger.Error(msg, GetType().FullName);
                        ret = false;
                    }
                    mPlcRedisClient.RemoveKey(tempChannel);
                    break;
                }
                return ret;
            }
            catch (Exception e)
            {
                mLogger.Error(e.Message, GetType().FullName);
                return false;
            }
        }

        public async Task<bool> WritePlcTagAsync(string plcName, string tagName, string tagValue)
        {
            string tempChannel = Guid.NewGuid().ToString();//接收写PLC操作结果的临时频道
            try
            {
                if (!mPlcServerRegistered)
                {
                    RegisterPlcServerClient();
                    Thread.Sleep(500);
                }
                if (!mPlcServerRegistered)
                {
                    mLogger.Error($"该客户端在Plc服务器上注册失败,无法将值{tagValue}写入变量{plcName}.{tagName}", GetType().FullName);
                    return false;
                }

                bool ret = await mPlcRedisClient.PublishAsync(mPlcRedisClientName, $"{plcName}@#${tagName}@#${tagValue}@#${tempChannel}");

                if (!ret) //Plc服务器没有收到发布
                {
                    RegisterPlcServerClient(); //可能Plc服务被关闭了，尝试重新注册
                    if (!mPlcServerRegistered) //没有注册成功
                    {
                        string msg = $"Plc服务可能被关闭，客户端{mPlcRedisClientName}在Plc服务器上注册失败,无法写Tag：{plcName}.{tagName}";
                        mLogger.Error(msg, GetType().FullName);
                        return false;
                    }
                    Thread.Sleep(500);
                    //成功注册，重新尝试写变量
                    ret = await mPlcRedisClient.PublishAsync(mPlcRedisClientName, $"{plcName}@#${tagName}@#${tagValue}@#${tempChannel}");
                }
                if (!ret)
                {
                    string msg = $"Plc服务可能被关闭，客户端{mPlcRedisClientName}发布的写变量{plcName}.{tagName}命令没有被PLC服务器接收到";
                    mLogger.Error(msg, GetType().FullName);
                    return false;
                }

                ret = false;
                long point1 = DateTime.Now.Ticks;
                while (true)
                {
                    await Task.Delay(10);
                    long point2 = DateTime.Now.Ticks;
                    TimeSpan ts = new TimeSpan(point2 - point1);
                    if (ts.TotalMilliseconds > 5000) //超过5s，视为写操作失败
                    {
                        string msg = $"客户端{mPlcRedisClientName}发布的写变量{plcName}.{tagName}命令，PLC服务器已接收到，但客户端没有接收到服务器的写Tag结果，已超时";
                        mLogger.Error(msg, GetType().FullName);
                        break;
                    }

                    string? val = await mPlcRedisClient.GetStringValueAsync(tempChannel);
                    if (val == null) //还未收到写操作的反馈
                        continue;

                    if (!bool.TryParse(val, out ret)) //收到反馈
                    {
                        string msg = $"客户端{mPlcRedisClientName}发布将值{tagValue}写变量{plcName}.{tagName}的命令，PLC服务器反馈写Tag失败";
                        mLogger.Error(msg, GetType().FullName);
                        ret = false;
                    }
                    await mPlcRedisClient.RemoveKeyAsync(tempChannel);
                    break;
                }
                return ret;
            }
            catch (Exception e)
            {
                mLogger.Error(e.Message, GetType().FullName);
                return false;
            }
        }

        public bool IsPlcTagValueChange(string plcName, string tagName)
        {
            PlcTagValue? value = ReadPlcTag(plcName, tagName);
            if (value == null || value.Quality == EnumQuality.Bad) //PLC变量不存在，或没有读到PLC值
                return false;

            string? oldValue = GetPlcTagTemp(plcName, tagName);
            if (oldValue == null) //首次比较，还没有缓存值，将当前值写入缓存值，返回未变化
            {
                SetPlcTagTemp(plcName, tagName, value.Value);
                return false;
            }

            if (oldValue == "ERROR") //读取时，发生错误
            {
                return false;
            }

            if (value.Value == "0") //0表示复位值，返回未变化
            {
                SetPlcTagTemp(plcName, tagName, value.Value);
                return false;
            }

            if (oldValue != value.Value)
            {
                SetPlcTagTemp(plcName, tagName, value.Value);
                return true;
            }

            return false;
        }

        private bool RegisterPlcServerClient()
        {
            try
            {
                mPlcServerRegistered = mPlcRedisClient.Publish(PlcServerRegChannelName, mPlcRedisClientName);
                return mPlcServerRegistered;
            }
            catch (Exception ex)
            {
                mLogger.Error($"{ex.Message}", GetType().FullName);
                mPlcServerRegistered = false;
                return false;
            }
        }

        private string? GetPlcTagTemp(string plcName, string tagName)
        {
            try
            {
                return mPlcRedisClient.GetHashValue(TagTempChannelName, $"{plcName}.{tagName}");
            }
            catch (Exception e)
            {
                mLogger.Error($"{e.Message}", GetType().FullName);
                return "ERROR";
            }
        }

        private void SetPlcTagTemp(string plcName, string tagName, string value)
        {
            try
            {
                mPlcRedisClient.SetHashValue(TagTempChannelName, $"{plcName}.{tagName}", value);
            }
            catch (Exception e)
            {
                mLogger.Error($"{e.Message}", GetType().FullName);
            }
        }
    }
}