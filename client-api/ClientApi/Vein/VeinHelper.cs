namespace ClientApi.Vein
{
    public class VeinHelper
    {
        private Thread threadFinger = null;
        private Thread threadveinInt = null;
        private Thread threadveinTest = null;

        //public Form1 _mClient { get; set; }
        //public VeinHelper(Form1 mClient)
        public VeinHelper()
        {
            Console.WriteLine("bb");
            // mClient.label1.Text = "hello";
            //_mClient = mClient;
            #region 指静脉初始化
            threadveinInt = new Thread(new ThreadStart(VeinInt));
            threadveinInt.IsBackground = true;
            threadveinInt.Start();

            if (!threadveinInt.IsAlive)
            {
                Thread.Sleep(100);
            }
            #endregion
        }

        /// <summary>
        /// 指静脉注册
        /// </summary>
        /// <param name="userId"></param>
        public void register(int userId)
        {
            Logger.Singleton.Error("开始注册指静脉");
            try
            {
                ShowTips("新增指静脉开始 ");
                ShowTips5("每根手指需采集6次 ");
                ////可以调用语音
                //_mClient.synth?.SpeakAsyncCancelAll();
                //_mClient.synth?.SpeakAsync("新增指静脉开始");
                startVeinSearchhread2(userId);
            }
            catch (Exception e)
            {

                Logger.Singleton.Error("注册指静脉异常：" + e.Message);
            }
        }

        public void speak(string msg)
        {
            //_mClient.synth?.SpeakAsyncCancelAll();
            //_mClient.synth?.SpeakAsync(msg);
        }

        /// <summary>
        /// 根据静脉数据ID删除
        /// </summary>
        /// <param name="veinId"></param>
        public void delete(int veinId)
        {
            example_DeleteFinger(veinId);
        }

        public void veintest()
        {
            ShowTips("开始测试");
            threadveinTest = new Thread(new ThreadStart(example_DirectMatch));
            threadveinTest.IsBackground = true;
            threadveinTest.Start();

            if (!threadveinTest.IsAlive)
            {
                Thread.Sleep(100);
            }

            //UpdateVeinData();
        }
        //public void usersync()
        //{
        //    //存在隐患  密码变化？？ 安全性
        //    FaceService.LoginGetToken("admin", "123qwe");
        //    #region 人脸数据同步
        //    List<UserFace> userFaces = FaceService.GetAllSUserFace();
        //    string[] aiuserlist = FaceService.AIGetUserList();
        //    foreach (var item in userFaces)
        //    {
        //        if (aiuserlist == null)
        //        {
        //            //string faceimage = ImageUtil.WebImageToBase64("http://192.168.1.188:21021/UploadFile/User/face/5/7f43134e-9f95-4eb0-ae0d-863ab61ea7b7.jpg");
        //            string faceimage = ImageUtil.WebImageToBase64(item.FaceUrl);
        //            FaceService.AIFaceAdd1(faceimage, item.UserId.ToString());
        //            break;
        //        }
        //        if (aiuserlist.Contains(item.UserId.ToString()))
        //        {
        //            //MessageBox.Show("aa");
        //        }
        //        else
        //        {
        //            //同步新增用户
        //            //string faceimage = ImageUtil.WebImageToBase64("http://192.168.1.188:21021/UploadFile/User/face/5/7f43134e-9f95-4eb0-ae0d-863ab61ea7b7.jpg");
        //            string faceimage = ImageUtil.WebImageToBase64(item.FaceUrl);
        //            FaceService.AIFaceAdd1(faceimage, item.UserId.ToString());
        //            Logger.Singleton.Info(item.UserId + "-" + "已同步人脸数据");
        //        }

        //    }
        //    if (aiuserlist != null)
        //    {
        //        for (int i = 0; i < aiuserlist.Length; i++)
        //        {
        //            if (userFaces.Find(x => x.UserId == Convert.ToInt32(aiuserlist[i])) == null)
        //            {
        //                //删除数据库中不存在的用户
        //                FaceService.AIFaceDelete(aiuserlist[i]);
        //                Logger.Singleton.Info(aiuserlist[i] + "-" + "已删除人脸数据");
        //            }

        //        }
        //    }
        //    #endregion
        //    #region 指静脉数据同步
        //    List<UserVein> userVeins = FaceService.GetAllSUserVein();
        //    foreach (var item in userVeins)
        //    {
        //        VeinJsObject.DownloadVein(item);
        //        Logger.Singleton.Info(item.UserId + "-" + item.VeinId + "已同步指静脉");
        //    }
        //    //VeinJsObject.CloseDevice();
        //    #endregion
        //}


        /// <summary>
        /// 验证指静脉数据
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string veinverify(int userId)
        {
            #region 常规的使用方式
            ////Console.WriteLine("主线程执行业务处理.");
            ////创建任务
            //Task<string> task = new Task<string>(() =>
            //{
            //    return example_DirectMatch(userId);

            //});
            ////启动任务,并安排到当前任务队列线程中执行任务(System.Threading.Tasks.TaskScheduler)
            //task.Start();
            //return await task;
            //////Console.WriteLine("主线程执行其他处理");
            ////task.Wait(500);
            #endregion
            //this.speak("指静脉验证开始");
            return example_DirectMatch(userId);

        }
        private void startVeinSearchhread2(int userId)
        {
            threadFinger = new Thread(new ParameterizedThreadStart(example_Register));
            threadFinger.IsBackground = true;
            threadFinger.Start(userId);

            if (!threadFinger.IsAlive)
            {
                Thread.Sleep(100);
            }
        }


        #region 指静脉

        public static byte[] g_devId = new byte[64]; //使用的设备
        private byte[] g_featureRegister = new byte[512 * 6]; //注册特征缓冲区，特征长度512字节，这里演示采集同指3次
        private byte[] g_dynfeature = new byte[512 * 6];//所注册对象的学习特征缓冲区，学习特征可以提高比对体验
        private int g_dyncnt = 0;
        private int g_registerCnt = 0;
        private bool g_iscom = true; //是使用串口类型（支持串口或者USB）
        private static ClientApi.Configuration.VeinConnectionSettings? _connectionSettings;

        /// <summary>仅设备装载、枚举与打开全部完成后为 true；启用开关不等于就绪。</summary>
        public static bool IsReady { get; private set; }

        /// <summary>读取启动时的连接配置，未配置时阻止调用设备 SDK。</summary>
        private static ClientApi.Configuration.VeinConnectionSettings ConnectionSettings => _connectionSettings
            ?? throw new InvalidOperationException("指静脉连接配置尚未初始化。");

        /// <summary>保存已验证的设备连接配置；修改配置后需重启设备服务。</summary>
        public static void Configure(ClientApi.Configuration.VeinConnectionSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings);
            settings.Validate();
            _connectionSettings = settings;
        }
        public static void VeinInt()
        {
            IsReady = false;
            bool ret = true;
            //增加是否串口判断
            if (ConnectionSettings.UseSerialPort)
            {
                ret = Vein_LoadingDevice();//
                if (!ret) return;
            }
            ret = Vein_EnumDevice();

            if (ret)
            {
                ret = Vein_OpenDevice();
            }
            IsReady = ret;
        }

        private static bool Vein_LoadingDevice()
        {
            int ret;
            var settings = ConnectionSettings;
            String ckstr = settings.ComName;//串口号
            byte[] devId = new byte[64];
            byte[] ttyName = new byte[32];
            int baudRate = settings.ComRate;//串口波特率
            int libIndex = settings.ComProtocol ?? throw new InvalidOperationException("未配置指静脉串口协议号。");

            byte[] byteArray = System.Text.Encoding.Default.GetBytes(ckstr);

            Array.Copy(byteArray, ttyName, byteArray.Length);

            //装载串口设备
            ret = BioVeinCaller.FV_LoadingDevice(devId, ttyName, baudRate, libIndex);
            if (ret != 0)
            {
                Console.WriteLine("FV_LoadingDevice ERR: " + ret);
                return false;
            }

            return true;
        }

        //public static void DownloadVein(UserVein userVein)
        //{
        //    //bool ret = true;
        //    //ret = example_EnumDevice();

        //    //if (ret)
        //    //{
        //    //    ret = S_OpenDevice();
        //    //}
        //    //byte fingerGroup = Convert.ToByte(userVein.UserId);//不能用作用户ID，最大支持256
        //    byte fingerGroup = 1;
        //    byte[] extend = new byte[16];
        //    int ret1;
        //    ret1 = BioVeinCaller.FV_DownloadFinger(Form1.g_devId, userVein.VeinId, userVein.FeatureData, (byte)userVein.FeatureCnt, fingerGroup, extend);
        //    if (ret1 != 0)
        //    {
        //        Console.WriteLine("下载手指数据失败: " + ret1);
        //        return;
        //        //return false;
        //    }
        //    Console.WriteLine("下载手指数据成功，分配编号: " + userVein.VeinId + "，特征个数：" + userVein.FeatureCnt);

        //}
        /// <summary>
        /// 获取所有的指静脉设备
        /// </summary>
        /// <returns></returns>
        /// 
        private static bool Vein_EnumDevice()
        {
            byte[] devIds = new byte[10 * 64];
            int ret;

            //每64字节放置一个设备名称，最大查找深度为10个设备
            ret = BioVeinCaller.FV_EnumDevice(devIds);
            if (ret != 0)
            {
                Console.WriteLine("FV_EnumDevice ERR: " + ret);
                return false;
            }

            //例程使用第一个设备演示
            Array.Copy(devIds, g_devId, 64);

            Console.WriteLine("使用第一个设备测试");

            return true;
        }

        /// <summary>
        /// 打开指静脉数据
        /// </summary>
        /// <returns></returns>
        private static bool Vein_OpenDevice()
        {
            int ret;
            String msg;
            byte[] serialNum = new byte[32];
            // FV_GetDevParam 写入 512 字节；短缓冲区会导致原生 SDK 越界破坏托管内存。
            byte[] devParam = new byte[512];

            ret = BioVeinCaller.FV_InitDevice(g_devId);
            if (ret != 0)
            {
                Console.WriteLine("FV_InitDevice ERR: " + ret);
                return false;
            }
            ret = BioVeinCaller.FV_OpenDevice(g_devId, 0);
            if (ret != 0)
            {
                Console.WriteLine("FV_OpenDevice ERR: " + ret);
                return false;
            }

            ret = BioVeinCaller.FV_GetDevSerialNum(g_devId, serialNum);
            if (ret != 0)
            {
                Console.WriteLine("FV_GetDevSerialNum ERR: " + ret);
                return false;
            }

            ret = BioVeinCaller.FV_GetDevParam(g_devId, devParam);
            if (ret != 0)
            {
                Console.WriteLine("FV_GetDevParam ERR: " + ret);
                return false;
            }

            msg = "打开设备成功，序列号：";
            msg += System.Text.Encoding.Default.GetString(serialNum);
            Console.WriteLine(msg);
            //ShowTips(msg);
            return true;
        }

        private static bool S_OpenDevice()
        {
            int ret;
            String msg;
            byte[] serialNum = new byte[32];
            // FV_GetDevParam 写入 512 字节，需与 SDK 输出缓冲区契约一致。
            byte[] devParam = new byte[512];

            ret = BioVeinCaller.FV_InitDevice(g_devId);
            ret = BioVeinCaller.FV_OpenDevice(g_devId, 0);
            if (ret != 0)
            {
                Console.WriteLine("FV_OpenDevice ERR: " + ret);
                return false;
            }

            ret = BioVeinCaller.FV_GetDevSerialNum(g_devId, serialNum);
            if (ret != 0)
            {
                Console.WriteLine("FV_GetDevSerialNum ERR: " + ret);
                return false;
            }

            ret = BioVeinCaller.FV_GetDevParam(g_devId, devParam);
            if (ret != 0)
            {
                Console.WriteLine("FV_GetDevParam ERR: " + ret);
                return false;
            }

            msg = "打开设备成功，序列号：";
            msg += System.Text.Encoding.Default.GetString(serialNum);
            Console.WriteLine(msg);
            return true;
        }
        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <returns></returns>
        public static bool CloseDevice()
        {
            int ret;
            ret = BioVeinCaller.FV_CloseDevice(g_devId);
            return true;
        }

        public void ShowTips(string tips)
        {
            //this._mClient.BeginInvoke(new EventHandler(delegate
            //{
            //    _mClient.veinInfo1.Text = tips;
            //    //_mClient.label1.Text = tips;
            //    //label2.Text = tips;
            //}));
        }
        public void ShowTips5(string tips)
        {
            //this._mClient.BeginInvoke(new EventHandler(delegate
            //{
            //    _mClient.veinInfo2.Text = tips;
            //}));
        }
        /// <summary>
        /// 等待放入手指
        /// </summary>
        /// <param name="bFingerStatus"></param>
        /// <returns></returns>
        private bool example_WaitFingerStatus(byte bFingerStatus)
        {
            int ret;
            int nTimes = 1000; //检测的次数，必须大于0
            int nInterval = 30;//每次检测的间隔，单位为毫秒
            byte[] fingerStatus = new byte[1];
            String msg = "";

            if (0 == bFingerStatus)
            {
                Console.Write("请移开手指！");
                ShowTips("请移开手指！");
                //label2.Text = "请移开手指！";
            }
            else if (0x03 == bFingerStatus)
            {
                Logger.Singleton.Error("采集开始请放手指");
                Console.Write("请放手指！");
                ShowTips("采集开始请放手指！");
            }
            else
                return false;

            for (int nCnt = 0; nCnt < nTimes; nCnt++)
            {
                Console.Write(msg);
                ret = BioVeinCaller.FV_FingerDetect(g_devId, fingerStatus);
                if (0 != ret)
                {
                    Console.WriteLine("FV_GetSdkVersion ERR: " + ret);
                    ShowTips("FV_GetSdkVersion ERR: " + ret);
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
            return false;
        }
        /// <summary>
        /// 注册设备的本地用户
        /// </summary>
        /// <returns></returns>
        private void example_Register(Object userId)
        {

            int ret;
            int readCnt = 0; //读取模板次数的计数器
            byte[] featureData = new byte[512];

            g_registerCnt = 0;
            for (int i = 0; i < 6; i++)
            {
                //等待手指放置
                if (!example_WaitFingerStatus((byte)0x03))
                {
                    //return;
                    //return false;
                }


                //采集指静脉模板
                ret = BioVeinCaller.FV_GrabFeature(g_devId, featureData, (byte)0);
                if (ret != 0)
                {
                    Console.WriteLine("FV_GrabFeature ERR: " + ret);
                    ShowTips("FV_GrabFeature ERR: " + ret);
                    // label2.Text = "FV_GrabFeature ERR: " + ret;
                    return;
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
                        ShowTips5("请使用相同手指！" + "FV_IsSameFinger ERR: " + ret);
                        return;
                        //return false;
                    }
                }

                //累计数据
                Array.Copy(featureData, 0, g_featureRegister, i * 512, 512);
                readCnt++;

                Console.WriteLine("第" + readCnt + "枚静脉特征采集成功");
                ShowTips5("第" + readCnt + "/6枚静脉特征采集成功");

                //等待手指放置
                if (!example_WaitFingerStatus((byte)0x00))
                    return;
                //return false;
            }

            g_registerCnt = 6;
            //下载注册数据
            int fingerId = 0;
            //int fingerId = VeinService.UploadUserVeinData(g_featureRegister, (int)userId);
            if (fingerId == 0)
            {
                ShowTips("上传服务器失败");
                return;
            }
            //return false;
            //byte fingerGroup = Convert.ToByte(userId);//不能用作用户ID，最大支持256
            byte fingerGroup = 1;
            byte[] extend = new byte[16];
            ret = BioVeinCaller.FV_DownloadFinger(g_devId, fingerId, g_featureRegister, (byte)g_registerCnt, fingerGroup, extend);
            if (ret != 0)
            {
                ShowTips("下载手指数据失败:" + ret);
                //Console.WriteLine("下载手指数据失败: " + ret);
                return;
                //return false;
            }
            //Console.WriteLine("下载手指数据成功，分配编号: " + fingerId + "，特征个数：" + g_registerCnt);
            //Console.WriteLine("采集完成");
            ShowTips("采集完成");
            //UpdateVeinData();

            //return true;
        }
        /// <summary>
        /// 匹配用户
        /// </summary>
        /// <returns></returns>
        private void example_DirectMatch()
        {
            int ret;
            int[] fingerId = new int[1];

            //等待手指放置
            if (!example_WaitFingerStatus((byte)0x03))
            {
                return;
                //return false;
            }

            //手指直接比对
            ret = BioVeinCaller.FV_DirectMatch(g_devId, 0, 0, fingerId);
            if (ret != 0)
            {
                //Console.WriteLine("比对失败: " + ret);
                ShowTips("比对失败: " + ret);

                return;
                //return false;

            }

            //Console.WriteLine("比对成功，编号: " + fingerId[0]);
            ShowTips("比对成功，编号: " + fingerId[0]);

            //label2.Text = "比对成功，编号: " + fingerId[0];
            //this.BeginInvoke(new EventHandler(delegate
            //{
            //    for (int i = 0; i < this.listViewVein.Items.Count; i++)
            //    {
            //        if (fingerId[0] == Convert.ToInt32(this.listViewVein.Items[i].SubItems[1].Text))
            //        {
            //            Console.WriteLine("比对一致，用户ID: " + userId);
            //            //label2.Text = "比对一致，用户ID: " + userId;
            //            ShowTips("比对一致，用户ID: " + userId);
            //            return;
            //            //return true;
            //        }
            //    }
            //}));



            //return false;
        }

        private string example_DirectMatch(int userId)
        {
            int ret;
            int[] fingerId = new int[1];

            //等待手指放置
            if (!example_WaitFingerStatus((byte)0x03))
            {
                this.speak("设备异常、等待超时");
                return "设备异常、等待超时";
                //return false;
            }

            //手指直接比对
            ret = BioVeinCaller.FV_DirectMatch(g_devId, 0, 0, fingerId);
            if (ret != 0)
            {
                Console.WriteLine("比对失败: " + ret);
                //ShowTips("比对失败: " + ret);
                this.speak("指静脉不匹配，点击指纹图标继续");

                return "指静脉不匹配，点击指纹图标继续";
                //return false;

            }

            Console.WriteLine("比对成功，编号: " + fingerId[0]);
            //ShowTips("比对成功，编号: " + fingerId[0]);
            if (fingerId[0] == userId)
            {
                this.speak("指静脉验证成功，人脸指静脉验证一致");
                return "指静脉验证成功，人脸指静脉验证一致";
                //return true;
            }
            else
            {
                this.speak("指静脉不匹配，点击指纹图标继续");
                return "指静脉不匹配，点击指纹图标继续";
            }

            //label2.Text = "比对成功，编号: " + fingerId[0];
            //this.BeginInvoke(new EventHandler(delegate
            //{
            //    for (int i = 0; i < this.listViewVein.Items.Count; i++)
            //    {
            //        if (fingerId[0] == Convert.ToInt32(this.listViewVein.Items[i].SubItems[1].Text))
            //        {
            //            Console.WriteLine("比对一致，用户ID: " + userId);
            //            //label2.Text = "比对一致，用户ID: " + userId;
            //            ShowTips("比对一致，用户ID: " + userId);
            //            return;
            //            //return true;
            //        }
            //    }
            //}));



            //return false;
        }

        private bool example_DeleteFinger(int veinId)
        {
            if (veinId == 0)
                return false;
            //删除指定的
            int ret;
            byte flg = 3;
            int fingerId = veinId;
            //byte flg = 0;
            //int fingerId = 0;

            ret = BioVeinCaller.FV_DeleteFinger(g_devId, flg, veinId);
            Logger.Singleton.Debug(veinId.ToString() + "指静脉删除返回值" + ret.ToString());
            if (ret != 0)
            {
                ShowTips("删除手指失败: " + ret);
                //Console.WriteLine("删除手指失败: " + ret);
                return false;
            }
            //VeinService.DeleteVeinDataById(veinId);
            ShowTips("删除手指成功，编号: " + ret);
            //Console.WriteLine("删除手指成功，编号: " + veinId);
            return true;
        }

        #endregion
    }
}
