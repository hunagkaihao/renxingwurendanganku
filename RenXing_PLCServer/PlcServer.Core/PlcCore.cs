using PlcServer.Cache;
using PlcServer.Devices.IDeviceServices;
using PlcServer.Devices.Models;
using PlcServer.Driver.Base;
using Shared.Logger.ILogger;
using System.Reflection;

namespace PlcServer.Core
{
    public class PlcCore
    {
        private List<PlcBase> mPlcs;
        private CancellationTokenSource mCancelSource;
        private CancellationToken mCancelToken;
        //Dictionary<plcName, Queue<KeyValuePair<tempClientChannel.tagName, tagValue>>> mWriteQueue;
        private Dictionary<string, Queue<KeyValuePair<string, string>>> mWriteQueues;
        private readonly ICache mPlcClientListener;
        private readonly IDeviceService mDeviceService;
        private readonly ILog mLogger;
        private readonly object mLock;
        private readonly IServiceProvider mServiceProvider;

        public PlcCore(ICache plcClientListener, IDeviceService deviceService, ILog logger, IServiceProvider serviceProvider)
        {
            mPlcs = new List<PlcBase>();
            mCancelSource = new CancellationTokenSource();
            mCancelToken = mCancelSource.Token;
            mWriteQueues = new Dictionary<string, Queue<KeyValuePair<string, string>>>();
            mLock = new object();
            mLogger = logger;

            //监听Plc服务的客户端的连接请求
            mPlcClientListener = plcClientListener;
            mPlcClientListener.AddRegisterChannel();
            mPlcClientListener.SubscribeRegisterChannel(HandlePlcClientRegisterRequest);

            //创建Plc实例
            mServiceProvider = serviceProvider;
            mDeviceService = deviceService;
            List<PlcDevice> lstPlcDevice = mDeviceService.GetAllPlcDevices();
            if (lstPlcDevice.Count < 1)
            {
                mLogger.Error("没有关于Plc设备的配置", GetType().FullName);
                return;
            }

            for (int i = 0; i < lstPlcDevice.Count; i++)
            {
                PlcDevice plcDevice = lstPlcDevice[i];
                //[1]
                string? assemblyName = plcDevice.DriverAssemblyName;
                string? className = plcDevice.DriverClassName;
                if (string.IsNullOrEmpty(assemblyName) || string.IsNullOrEmpty(className))
                {
                    string log = $"Plc实例创建失败，程序集或者类名配置错误，程序集：{assemblyName}，类名：{className}";
                    mLogger.Error(log, GetType().FullName);
                    continue;
                }

                //[2]
                Assembly ass = Assembly.Load(assemblyName);
                List<Type> types = ass.GetTypes().Where(t => t.IsClass && t.Name == className).ToList();
                if (types.Count == 0)
                {
                    string log = $"Plc实例创建失败，无法获取程序集{assemblyName}中类型{className}的信息";
                    mLogger.Error(log, GetType().FullName);
                    continue;
                }

                //[3]
                PlcBase? plc = (PlcBase?)mServiceProvider.GetService(types[0]);
                if (plc == null)
                {
                    string log = $"从程序集{assemblyName}创建类型为{className}的Plc实例失败";
                    mLogger.Error(log, GetType().FullName);
                    continue;
                }

                //[4]
                plc.PlcName = plcDevice.PlcName ?? $"Plc{i + 1}";
                plc.ConnParas = plcDevice.ConnectParameter ?? "";
                mPlcs.Add(plc);

                Queue<KeyValuePair<string, string>> queue = new Queue<KeyValuePair<string, string>>();
                mWriteQueues.Add(plc.PlcName, queue);
            }
        }

        private void Enqueue(string plcName, string tagName, string tagValue, string tempClientChannel)
        {
            lock(mLock)
            {
                if(!mWriteQueues.ContainsKey(plcName))
                {
                    mPlcClientListener.SendClientMessage(tempClientChannel, false.ToString());
                    mLogger.Error($"加入队列，发现队列对应的名为{plcName}的Plc不存在", GetType().FullName);
                    return;
                }
                mWriteQueues[plcName].Enqueue(new KeyValuePair<string, string>($"{tempClientChannel}@#${tagName}", tagValue));
            }
        }

        private KeyValuePair<string, string>? Dequeue(string plcName)
        {
            lock(mLock)
            {
                if (!mWriteQueues.ContainsKey(plcName))
                {
                    mLogger.Error($"出队列，发现队列对应的名为{plcName}的Plc不存在", GetType().FullName);
                    return null;
                }
                if (mWriteQueues[plcName].Count < 1)
                    return null;

                return mWriteQueues[plcName].Dequeue();
            }            
        }

        private List<KeyValuePair<string, string>>? DequeueAll(string plcName)
        {
            lock (mLock)
            {
                if (!mWriteQueues.ContainsKey(plcName))
                {
                    mLogger.Error($"出队列，发现队列对应的名为{plcName}的Plc不存在", GetType().FullName);
                    return null;
                }

                Queue<KeyValuePair<string, string>>? queue = mWriteQueues[plcName];
                if (queue == null)
                    return null;

                if (queue.Count < 1)
                    return null;

                List<KeyValuePair<string, string>> lstPair = new List<KeyValuePair<string, string>>();
                while(queue.Count > 0)
                {
                    lstPair.Add(queue.Dequeue());
                }
                return lstPair;
            }
        }

        private bool IsQueueExist(string plcName)
        {
            lock(mLock)
            {
                return mWriteQueues.ContainsKey(plcName);
            }
        }

        /// <summary>
        /// 处理客户端的注册请求
        /// </summary>
        /// <param name="listenChannel"></param>
        /// <param name="clientName"></param>
        private void HandlePlcClientRegisterRequest(string listenChannel, 
                                                    string clientName)
        {
            if (string.IsNullOrEmpty(clientName))
            {
                mLogger.Error($"客户端名称为空，为客户端添加Redis消息通道失败", GetType().FullName);
                return;
            }

            if(!mPlcClientListener.AddClientChannel(clientName))
            {
                mLogger.Error($"为客户端{clientName}添加Redis消息通道失败", GetType().FullName);
                return;
            }

            mPlcClientListener.SubscribeClientChannel(clientName, HandleClientMessage);
        }

        private void HandleClientMessage(string msgFromClientChannel, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                mLogger.Error($"通道{msgFromClientChannel}收到空的\"写\"PLC消息", GetType().FullName);
                return;
            }
            string[] parts = message.Split("@#$");
            if (parts.Length != 4)
            {
                mLogger.Error($"通道{msgFromClientChannel}收到\"写\"PLC消息:{message}，格式不正确，正确格式为：Plc名@#$变量名@#$变量值@#$临时频道", GetType().FullName);
                return;
            }
            KeyValuePair<string, string> keyValue = new KeyValuePair<string, string>(parts[1], parts[2]);
            PlcWriteTag(parts[0], keyValue, parts[3]);
        }

        private async Task PlcReadAsync(PlcBase plc)
        {
            if (!plc.IsConnected)
            {
                await plc.ConnectAsync().ConfigureAwait(false);
                DequeueAll(plc.PlcName);//没有连接前的写命令视为无效
            }
            if (plc.IsConnected)
            {
                //防止读写冲突，将读和写顺序执行
                //[0]读
                await plc.ReadAllAsync().ConfigureAwait(false);
                //[1]写
                List<KeyValuePair<string, string>>? lstPair = DequeueAll(plc.PlcName);
                if (lstPair == null)
                    return;

                foreach(var pair in lstPair)
                {
                    string[] parts = pair.Key.Split("@#$");
                    if (parts.Length != 2)
                        continue;
                    bool ret = await plc.WriteTagAsync(parts[1], pair.Value).ConfigureAwait(false);
                    mPlcClientListener.SendClientMessage(parts[0], ret.ToString());
                }
            }
        }

        private void PlcWriteTag(string plcName, KeyValuePair<string, string> tagNmValuePair, string tempClientChannel)
        {
            if(!IsQueueExist(plcName))
            {
                mPlcClientListener.SendClientMessage(tempClientChannel, false.ToString());
                mLogger.Error($"向名为{plcName}的Plc的变量{tagNmValuePair.Key}写值时发生错误，名为{plcName}的Plc不存在", GetType().FullName);
                return;
            }
            Enqueue(plcName, tagNmValuePair.Key, tagNmValuePair.Value, tempClientChannel);
        }

        public void StopWork()
        {
            mCancelSource.Cancel();
        }

        public async Task WorkAsync()
        {
            //[0]加载及分组节点标签
            foreach (var plc in mPlcs)
            {
                plc.LoadTags();
                plc.GroupTags();
                plc.InitCache();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("info");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($": {plc.PlcName}完成变量加载，共{plc.GroupQuantity()}个组，包含{plc.TagQuantity()}个变量");
            }

            //[1]各PLC通讯任务缓存
            Dictionary<string, Task?> dicPlcCommTask = new Dictionary<string, Task?>();
            foreach (var plc in mPlcs)
            {
                dicPlcCommTask.Add(plc.PlcName, null);
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("info");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($": Plc服务器开始运行");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            //[2]循环并发执行各PLC通讯，每个Plc两次读取间隔为：读的消耗时间 + （10~20ms）
            await Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        //[2-0]遍历寻找空闲的Plc
                        List<PlcBase> idlePlc = new List<PlcBase>();
                        foreach (var plc in mPlcs)
                        {
                            Task? task = dicPlcCommTask[plc.PlcName];
                            if (task == null ||
                                task.Status == TaskStatus.Canceled ||
                                task.Status == TaskStatus.Faulted ||
                                task.Status == TaskStatus.RanToCompletion)
                            {
                                idlePlc.Add(plc);
                            }
                        }

                        //[2-1] 释放10ms的线程控制权
                        await Task.Delay(10).ConfigureAwait(false);
                        //Thread.Sleep(10);

                        //[2-2] 各个Plc同时并发读取
                        foreach (var plc in idlePlc)
                        {
                            dicPlcCommTask[plc.PlcName] = PlcReadAsync(plc);
                        }

                        //[2-3] 退出判断
                        if (mCancelToken.IsCancellationRequested)
                        {
                            await Task.WhenAll(dicPlcCommTask.Values!); //等待通讯工作结束

                            //断开各Plc连接
                            List<Task> lstTask = new List<Task>();
                            foreach (var plc in mPlcs)
                            {
                                lstTask.Add(Task.Run(() => plc.DisConnectAsync()));
                            }
                            await Task.WhenAll(lstTask);

                            break;
                        }
                    }
                    catch(Exception ex)
                    {
                        mLogger.Error(ex.Message, GetType().FullName);
                    }                    
                }
            });
        }
    }
}
