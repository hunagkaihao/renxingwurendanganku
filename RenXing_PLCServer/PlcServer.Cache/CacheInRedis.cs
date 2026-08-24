using Shared.Config;
using Shared.Logger.ILogger;
using StackExchange.Redis;

namespace PlcServer.Cache
{
    public class CacheInRedis : ICache
    {
        private ConnectionMultiplexer mRedisClient;
        private int mDatabaseNum = 0;
        private static readonly string RegisterChannelName = "RegisterChannel";

        private readonly ILog _logger;

        public CacheInRedis(ILog logger)
        {
            _logger = logger;

            try
            {
                mDatabaseNum = Settings.ConfigData.RedisDBNumForPlcCache;
                string? redisAddress = Settings.ConfigData.RedisConnString;
                mRedisClient = ConnectionMultiplexer.Connect(redisAddress);
            }
            catch(Exception ex)
            {
                _logger.Error($"Redis地址错误或Redis服务未开，{ex.Message}", GetType().FullName);
                throw;
            }
        }

        public bool InitCache()
        {
            throw new NotImplementedException();
            //try
            //{
            //    IDatabase db = mRedisClient.GetDatabase(mDatabaseNum);
            //    db.hash
            //}
            //catch (Exception e)
            //{
            //    string log = $"{e.Message}";
            //    PlcDbHelper.WriteLog(
            //        log,
            //        LogGrade.ERROR,
            //        "PlcServer.Cache.CacheInRedis.InitCache()");
            //    return false;
            //}
        }

        public bool AddTag(string plcName, string tagName, string initValue)
        {
            try
            {
                IDatabase db = mRedisClient.GetDatabase(mDatabaseNum);
                if (!db.KeyExists($"{plcName}.{tagName}"))
                    return db.StringSet(
                        $"{plcName}.{tagName}", 
                        initValue);
                else
                    return true;
            }
            catch(Exception e)
            {
                _logger.Error(e.Message, GetType().FullName);
                return false;
            }
        }

        public async Task<bool> AddTagAsync(string plcName, string tagName, string initValue)
        {
            try
            {
                IDatabase db = mRedisClient.GetDatabase(mDatabaseNum);
                bool bExist = await db.KeyExistsAsync($"{plcName}.{tagName}").ConfigureAwait(false);
                if (!bExist)
                    return await db.StringSetAsync(
                        $"{plcName}.{tagName}", 
                        initValue)
                        .ConfigureAwait(false);
                else
                    return true;
            }
            catch(Exception e)
            {
                _logger.Error(e.Message, GetType().FullName);
                return false;
            }
        }

        public bool SetTagLifeCycle(string plcName, string tagName, int lifeCycle = -1)
        {
            try
            {
                if (lifeCycle == -1)
                    return true;

                IDatabase db = mRedisClient.GetDatabase(mDatabaseNum);                
                return db.KeyExpire($"{plcName}.{tagName}", new TimeSpan(0,0,0,lifeCycle));
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, GetType().FullName);
                return false;
            }
        }

        public long RemoveTag(string plcName, string[] tagNames)
        {
            try
            {
                if(tagNames.Length <= 0)
                {
                    return 0;
                }
                IDatabase db = mRedisClient.GetDatabase(mDatabaseNum);
                RedisKey[] keys = new RedisKey[tagNames.Length];
                for(int i = 0; i < tagNames.Length; i++)
                {
                    keys[i] = new RedisKey($"{plcName}.{tagNames[i]}");
                }
                return db.KeyDelete(keys);
            }
            catch(Exception e)
            {
                _logger.Error(e.Message, GetType().FullName);
                return 0;
            }
        }

        public async Task<long> RemoveTagAsync(string plcName, string[] tagNames)
        {
            try
            {
                if (tagNames.Length <= 0)
                {
                    return 0;
                }
                IDatabase db = mRedisClient.GetDatabase(mDatabaseNum);
                RedisKey[] keys = new RedisKey[tagNames.Length];
                for (int i = 0; i < tagNames.Length; i++)
                {
                    keys[i] = new RedisKey($"{plcName}.{tagNames[i]}");
                }
                return await db.KeyDeleteAsync(keys).ConfigureAwait(false);
            }
            catch(Exception e)
            {
                _logger.Error(e.Message, GetType().FullName);
                return 0;
            }
        }

        public string? ReadTag(string plcName, string tagName)
        {
            try
            {
                IDatabase db = mRedisClient.GetDatabase(mDatabaseNum);
                return db.StringGet($"{plcName}.{tagName}");
            }
            catch(Exception e)
            {
                _logger.Error(e.Message, GetType().FullName);
                return "ERROR";
            }
        }

        public async Task<string?> ReadTagAsync(string plcName, string tagName)
        {
            try
            {
                IDatabase db = mRedisClient.GetDatabase(mDatabaseNum);
                return await db.StringGetAsync($"{plcName}.{tagName}").ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, GetType().FullName);
                return "ERROR";
            }
        }

        public bool WriteTag(string plcName, string tagName, string tagValue)
        {
            try
            {
                IDatabase db = mRedisClient.GetDatabase(mDatabaseNum);
                return db.StringSet($"{plcName}.{tagName}", tagValue);
            }
            catch(Exception e)
            {
                _logger.Error(e.Message, GetType().FullName);
                return false;
            }
        }

        public async Task<bool> WriteTagAsync(string plcName, string tagName, string tagValue)
        {
            try
            {
                IDatabase db = mRedisClient.GetDatabase(mDatabaseNum);
                return await db.StringSetAsync($"{plcName}.{tagName}", tagValue).ConfigureAwait(false);
            }
            catch(Exception e)
            {
                _logger.Error(e.Message, GetType().FullName);
                return false;
            }
        }

        public bool WriteAndPublishTag(string plcName, string tagName, string tagValue)
        {
            try
            {
                IDatabase db = mRedisClient.GetDatabase(mDatabaseNum);
                if (!db.StringSet($"{plcName}.{tagName}", tagValue))
                    return false;

                db.Publish(RedisChannel.Literal($"{plcName}.{tagName}"), tagValue);
                return true;
            }
            catch(Exception e)
            {
                _logger.Error(e.Message, GetType().FullName);
                return false;
            }
        }

        public async Task<bool> WriteAndPublishTagAsync(string plcName, string tagName, string tagValue)
        {
            try
            {
                IDatabase db = mRedisClient.GetDatabase(mDatabaseNum);
                bool res = await db.StringSetAsync($"{plcName}.{tagName}", tagValue).ConfigureAwait(false);
                if (!res)
                    return false;

                await db.PublishAsync(RedisChannel.Literal($"{plcName}.{tagName}"), tagValue).ConfigureAwait(false);
                return true;
            }
            catch(Exception e)
            {
                _logger.Error(e.Message, GetType().FullName);
                return false;
            }
        }

        public bool AddRegisterChannel()
        {
            try
            {
                IDatabase db = mRedisClient.GetDatabase(mDatabaseNum);
                if (!db.KeyExists(RegisterChannelName))
                    return db.StringSet(RegisterChannelName, "此为客户端注册通道");
                else
                    return true;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, GetType().FullName);
                return false;
            }
        }

        public void SubscribeRegisterChannel(Action<string, string> handle)
        {
            try
            {
                ISubscriber subscriber = mRedisClient.GetSubscriber();
                subscriber.Subscribe(RedisChannel.Literal(RegisterChannelName), (channel, value) => {
                    handle.Invoke(channel.ToString(), value.ToString());
                });
            }
            catch(Exception e)
            {
                _logger.Error(e.Message, GetType().FullName);
            }
        }

        public bool AddClientChannel(string clientChannel)
        {
            try
            {
                IDatabase db = mRedisClient.GetDatabase(mDatabaseNum);
                if (!db.KeyExists(clientChannel))
                    return db.StringSet(clientChannel, "此为客户端消息接收通道");
                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, GetType().FullName);
                return false;
            }
        }

        public void SubscribeClientChannel(string clientChannel, Action<string, string> handle)
        {
            try
            {
                ISubscriber subscriber = mRedisClient.GetSubscriber();
                subscriber.Unsubscribe(RedisChannel.Literal(clientChannel));
                subscriber.Subscribe(RedisChannel.Literal(clientChannel), (channel, value) => {
                    handle.Invoke(channel.ToString(), value.ToString());
                });
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, GetType().FullName);
            }
        }

        public bool RegisterClient(string clientChannel)
        {
            try
            {
                ISubscriber subscriber = mRedisClient.GetSubscriber();
                long recNum = subscriber.Publish(RedisChannel.Literal(RegisterChannelName), clientChannel);
                return recNum > 0;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, GetType().FullName);
                return false;
            }
        }

        /// <summary>
        /// 向客户端发送信息
        /// </summary>
        /// <param name="clientChannel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool SendClientMessage(string clientChannel, string message)
        {
            try
            {
                IDatabase db = mRedisClient.GetDatabase(mDatabaseNum);
                bool ret = db.StringSet(clientChannel, message);
                if (ret)
                    db.KeyExpire(clientChannel, new TimeSpan(0, 0, 0, 10));
                return ret;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, GetType().FullName);
                return false;
            }
        }

    }
}
