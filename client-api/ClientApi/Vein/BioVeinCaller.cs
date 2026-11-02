using System.Runtime.InteropServices;

namespace ClientApi.Vein
{
    public class BioVeinCaller
    {
        //////////////////////////////////////////////////
        /// 如果使用32位平台，
        /// 将 [DllImport(@"Vein\BioVein.x64.dll")]
        /// 改为 [DllImport(@"BioVein.Win32.dll")]
        //////////////////////////////////////////////////
        /**
        * @fun() FV_LoadingDevice 装载串口设备
        * @param devName //I：自定义设备名称（64字节）
        * @param ttyName //I：串口映射在系统中的设备文件名（32字节）
        * @param baudRate //I：串口波特率
        * @param libIndex //I：设备通信协议版本（1：1101、1100，2：320）
        * @state 装载一个本地微盾指静脉设备，使用时请确保设备正确连接，
        *        且插入端口和设备文件名相对应。如果只使用USB设备时可以不用调用该接口
        * @return
        */
        //private const string dllname = @"BioVein.Win32.dll";//32位
        private const string dllname = @"Vein\BioVein.x64.dll";//64位
        //将调用的dllname写入变量，还为测试过。
        [DllImport(dllname)]
        public static extern int FV_LoadingDevice(byte[] devName, byte[] ttyName, int baudRate, int libIndex);

        /**
        * @fun() FV_EnumDevice 枚举出系统中链接的usb设备以及手动装载的串口设备
        * @param devNameList //O：设备名称列表（n*64字节，n为连接的微盾设备数量）
        * @state 最大操控设备数量为20个
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_EnumDevice(byte[] devNameList);

        /**
        * @fun() FV_RemoveDevice 卸载指定设备
        * @param devName //I：设备名称（64字节）
        * @state 卸载设备可以释放占用资源
        * @return
        */
        [DllImport(dllname)]
        public static extern int FV_RemoveDevice(byte[] devName);

        /**
        * @fun() FV_InitDevice 设备指定初始化
        * @param devName //I：设备名称（64字节）
        * @state 用于设备打开前初始化资源
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_InitDevice(byte[] devName);

        /**
        * @fun() FV_OpenDevice 打开指定设备
        * @param devName //I：设备名称（64字节）
        * @param flg //I：预留参数
        * @state 打开设备前要先枚举设备并初始化设备，串口则要先手动装载
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_OpenDevice(byte[] devName, int flg);

        /**
        * @fun() FV_CloseDevice 关闭指定设备
        * @param devName //I：设备名称（64字节）
        * @state 关闭设备后设备资源不会被删除，再次打开时不需要重新枚举
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_CloseDevice(byte[] devName);

        /**
        * @fun() FV_GetDevSerialNum 获取设备序列号
        * @param devName //I：设备名称（64字节）
        * @param serialNum //O：设备序列号（32字节）
        * @state 唯一出厂序列
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_GetDevSerialNum(byte[] devName, byte[] serialNum);

        /**
        * @fun() FV_GetSdkVersion 获取SDK版本
        * @param version //O：设备版本（32字节）
        * @state 如“2.00.2E82D.14C12”，"2.00"表示版本，
        *        "2E82D.14C12"表示16进制的编译日期和时间“190509.085010”
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_GetSdkVersion(byte[] version);

        /**
        * @fun() FV_SetDevParam 设置设备参数
        * @param devName //I：设备名称（64字节）
        * @param param //I：设备参数（512字节）
        * @state 设备参数为软参数，不会被记录到设备中，重新启动时恢复默认值
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_SetDevParam(byte[] devName, byte[] param);

        /**
        * @fun() FV_GetDevParam 获取设备参数
        * @param devName //I：设备名称（64字节）
        * @param param //O：设备参数（512字节）
        * @state 
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_GetDevParam(byte[] devName, byte[] param);

        /**
        * @fun() FV_GrabImage 获取手指静脉图像
        * @param devName //I：设备名称（64字节）
        * @param imageData //O：图像数据（分配内存 85558 字节）
        * @param imageSize //O：图像数据实际大小
        * @param imageWidth //O：图像的宽度
        * @param imageHeight //O：图像的高度
        * @state 图像数据为完整的jpg格式图片，可直接保存到文件中。调用时要先保证手指已正确放置
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_GrabImage(byte[] devName, byte[] imageData, int[] imageSize,
            int[] imageWidth, int[] imageHeight);

        /**
        * @fun() FV_FingerDetect 检测设备上手指放置的状态
        * @param devName //I：设备名称（64字节）
        * @param fingerStatus //O：手指状态值
        * @state 状态值中“0”表示移开，“3”表示放置，其他表示手指未正确放置或移开。 
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_FingerDetect(byte[] devName, byte[] fingerStatus);

        /**
        * @fun() FV_GrabFeature 读取一个指静脉特征
        * @param devName //I：设备名称（64字节）
        * @param featureData //O：手指特征静脉数据（512字节）
        * @param flg //I：默认值0
        * @state 特征为二进制数据。调用时要先保证手指已正确放置
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_GrabFeature(byte[] devName, byte[] featureData, byte flg);

        /**
        * @fun() FV_GrabFeatureAndImage 读取指静脉特征和图片
        * @param devName //I：设备名称（64字节）
        * @param featureData //O：手指特征静脉数据（512字节）
        * @param imageData //O：图像数据（分配内存 85558 字节）
        * @param imageSize //O：图像数据实际大小
        * @param imageWidth //O：图像的宽度
        * @param imageHeight //O：图像的高度
        * @param flg //I：指定图像格式，“0x06”输出为jpg格式， “0x07”输出为bmp格式
        * @param quality //O：图像质量
        * @state 调用时要先保证手指已正确放置
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_GrabFeatureAndImage(byte[] devName, byte[] featureData, byte[] imageData,
            int[] imageSize, int[] imageWidth, int[] imageHeight, byte flg, short[] quality);

        /**
        * @fun() FV_ExtractFeature 把图片转为特征值
        * @param devName //I：设备名称（64字节）
        * @param imageData //I：微盾图像数据
        * @param imageType //I：图像类型，bmp或jpg
        * @param imageLen //I：图像数据长度
        * @param featureData //O：转化的特征（至少分配512字节）
        * @param featureLen //I：转换的特征数据长度
        * @state 输入图片要求是FV_GrabFeatureAndImage接口读取的原始图片
        * @return 
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_ExtractFeature(byte[] devName, byte[] imageData, byte imageType,
            int imageLen, byte[] featureData, int featureLen);

        /**
        * @fun() FV_IsSameFinger 简要判断是否为同一根手指
        * @param matchFeature //I：本次读取的特征（512字节）
        * @param regFeature //I：已注册特征（n*512字节，n为已采集的注册特征个数）
        * @param regCnt //I：已采集的注册特征个数
        * @param flg //I：固定值3
        * @state 该接口用于注册时判断，不可用于识别。当注册采集第二枚及以上特征时，应调用接口与已经采集特征
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_IsSameFinger(byte[] matchFeature, byte[] regFeature, byte regCnt, byte flg);

        /**
        * @fun() FV_MatchFeature 指静脉特征比对
        * @param matchFeature  //I：本次读取的待比对特征（512字节）
        * @param allFeature //I：某根手指的特征，注册特征与学习特征的组合（allCnt *512字节）
        * @param allCnt //I：参数allFeature中包含的特征数，每个特征的长度为512字节
        * @param flg //I:固定值0x03
        * @param securityLevel //I:安全级别， 0 - 10， 数字越小越安全，同时对放置手指的要求越高；
        *                      /////1:N：N个用户中循环调用本接口比对查找匹配的用户时，建议数字为4
        *                      /////1:1：已经通过其它方式确定某一个特定用户，调用本接口确定是否为其本人，建议数字为6
        * @param diff //O:比对返回的差异度。在n个用户中，当通过2个用户时当取diff最小的用户为通过用户
        * @param aiFeatureBuf //O：用于比对后输出学习模板的缓冲区。（aiDataLen字节）
        * @param aiDataLen //I：ai缓冲区可用大小，512的倍数
        *                  //O：比对通过时，返回的动态学习特征数据的长度，当验证通过时应将aiFeatureBuf中的AIDataLen字节数据保存
        * @state 该接口默认前三个为注册特征，其余的为动态特征。推荐使用FV_MatchFeatureEx接口
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_MatchFeature(byte[] matchFeature, byte[] allFeature, byte allCnt, byte flg,
            byte securityLevel, int[] diff, byte[] aiFeatureBuf, int[] aiDataLen);

        /**
        * @fun() FV_MatchFeatureEx 指静脉特征比对
        * @param matchFeature //I:待验证特征（1个，共512字节）
        * @param regFeature //I:某用户某根手指的所有注册特征集合（n个，共n*512字节，n为注册时单指获取特征的个数）
        * @param regCnt //I:注册时单指获取特征的枚数
        * @param aiFeature //I:某用户某根手指的所有学习特征集合（n个，共n*512字节，n为学习特征数，至少分配512字节）
        *                  //O:本次验证成功后产生的学习特征，不符合生成条件或验证失败时，该内存区域不被操作
        * @param aiCnt //I:集合中学习特征数
        *              //O:本次验证产生的学习特征个数，符合生成条件或验证成功时值为1，否则为0
        * @param flag //I:0x03
        * @param securityLevel //I:安全级别， 0 - 10， 数字越小越安全，同时对放置手指的要求越高；
        *                      /////1:N：N个用户中循环调用本接口比对查找匹配的用户时，建议数字为4
        *                      /////1:1：已经通过其它方式确定某一个特定用户，调用本接口确定是否为其本人，建议数字为6
        * @param diff			//O:比对返回的差异度，差异度越小说明指静脉越相似，比对时建议便利所有手指，在验证通过的手指中取差异度最小的为最终验证结果
        * @param aiBufLen		//I：ai特征输出时缓存可用长度，
        *                       //O：ai特征实际输出长度，">0"时应将featureAI的aiBufLen字节回写数据库
        * @state
        * @return 
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_MatchFeatureEx(byte[] matchFeature, byte[] regFeature, int regCnt,
            byte[] aiFeature, int aiCnt, byte flag, byte securityLevel, int[] diff, int[] aiBufLen);

        /**
        * @fun() FV_SetLedBeep 设置LED与蜂鸣器动作
        * @param devName //I：设备名称（64字节）
        * @param blinkType //I：LED和BEEP的点灭的类型。（0-绿，1-红，2-绿+beep， 3-红+beep， 4-红绿，5-红绿+beep，6-beep）
        * @param ledTime //I：LED点亮的持续时间，单位毫秒, 范围 0 - 3000
        * @param beepTime //I：BEEP响声的持续时间，单位毫秒, 范围 0 - 3000
        * @state 是否能控制成功，由设备硬件是否支持决定。WDH-1300专用，WDH-1100序列没有LED
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_SetLedBeep(byte[] devName, int blinkType, int ledTime, int beepTime);


        /**
        * @fun() FV_DownloadUser 下发用户的手指数据
        * @param devName //I：设备名称（64字节）
        * @param fingerId //I：手指编号（字符串16字节）
        * @param featureData //I：手指注册特征（featureCnt*512字节）
        * @param featureCnt //I：手指注册特征数量
        * @param fingerGroup //：手指分组(短短整型)
        * @param extend //I：手指的其他数据（最大14字节）
        * @state 设备中以手指为单位存储特征数据，因此手指编号在所有用户中应是唯一的标识，手指分组可以在比对时缩小范围
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_DownloadUser(byte[] devName, byte[] fingerId, byte[] featureData,
            byte featureCnt, byte fingerGroup, byte[] extend);
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_DownloadFinger(byte[] devName, int fingerId, byte[] featureData,
            byte featureCnt, byte fingerGroup, byte[] extend);

        /**
        * @fun() FV_UploadUser 上传用户的手指数据
        * @param devName //I：设备名称（64字节）
        * @param fingerId //I：手指编号（字符串16字节）
        * @param featureData //O：手指注册特征（该编号手指下发时的特征数量*512字节）
        * @param featureCnt //O：该编号手指下发时的特征数量
        * @param fingerGroup //O：该编号手指的分组（短短整型取地址）
        * @param extend //O：手指其他数据（最大14字节）
        * @state 上传的编号是手指编号而不是用户的编号，上传成功的先决条件是设备中已存在该手指编号的相关数据
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_UploadUser(byte[] devName, byte[] fingerId, byte[] featureData,
            byte[] featureCnt, byte[] fingerGroup, byte[] extend);
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_UploadFinger(byte[] devName, int fingerId, byte[] featureData,
            byte[] featureCnt, byte[] fingerGroup, byte[] extend);

        /**
        * @fun() FV_DeviceMatch 下发指静脉特征到设备中验证
        * @param devName //I：设备名称（64字节）
        * @param featureData //I：待比对特征（512字节）
        * @param flg //I：比对标志
        * @param number //I：对应值（最大16字节）
        * @state
        *        [flg]                           [number]  [FV_DeviceMatch] [FV_DownloadMatch]
        * 	0: 特征与所有用户匹配                   NULL
        *	1: 特征与指定手指分组                   gid     短短整型取地址      整型
        *	3: 特征与指定手指编号                   fid     字符串              整型
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_DeviceMatch(byte[] devName, byte[] featureData, byte flg, byte[] number);
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_DownloadMatch(byte[] devName, byte[] featureData, byte flg, int number, int[] fingerId);

        /**
        * @fun() FV_DirectMatch 设备端直接拍照提取特征在模块内比对
        * @param devName //I：设备名称（64字节）
        * @param flg //I：比对标志
        * @param number //I：对应值
        * @param fingerId //O：通过时表示手指编号
        * @state
        *        [flg]                           [number]
        * 	0: 特征与所有用户匹配                   0
        *	1: 特征与指定手指分组                   gid     整型  //1101不支持
        *	3: 特征与指定手指编号                   fid     整型  //1101不支持
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_DirectMatch(byte[] devName, byte flg, int number, int[] fingerId);

        /**
        * @fun() FV_DeleteFinger 删除设备中的手指特征数据
        * @param devName //I：设备名称（64字节）
        * @param flg //I：删除标志
        * @param fingerId //I：对应值（最大16字节）
        * @state
        *        [flg]                           [number]
        * 	0: 删除全部                             0      整型
        *	3: 删除指定                             fid     整型
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_DeleteFinger(byte[] devName, byte flg, int fingerId);



        /**
        * @fun()FV_ReadUUID 读取设备的板级序列号
        * @param devName //I：设备名称（64字节）
        * @param boardId //O：板级ID（最少32字节）
        * @param boardIdLen //O：板级ID实际长度
        * @param opt //I：读取校验码
        * @state
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_ReadUUID(byte[] devName, byte[] boardId, int[] boardIdLen, int opt);

        /**
        * @fun() FV_GetDevSign 读取设备的签名信息
        * @param devName //I：设备名称（64字节）
        * @param devsign //O：签名数据（至少512字节）
        * @param devsignLen //O：实际数据长度
        * @state 接口用于客户端BioVeinSDK与服务端VeinMatchSDK签名授权，签名过程请参考程序设计说明文档
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_GetDevSign(byte[] devName, byte[] devsign, int[] devsignLen);

        /**
        * @fun() FV_SetDevSign 把服务器返回给设备的签名信息设置到设备中去
        * @param devName //I：设备名称（64字节）
        * @param devsign //I：签名数据
        * @param devsignLen //I： 固定值512
        * @state 接口用于客户端BioVeinSDK与服务端VeinMatchSDK签名授权，签名过程请参考程序设计说明文档
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_SetDevSign(byte[] devName, byte[] srvsign, int srvsignLen);

        /**
        * @fun() FV_ReadDevVersion 读取设备的固件版本号
        * @param devName //I：设备名称（64字节）
        * @param version //O：固件版本号（32字节）
        * @param flag //I：固定值0
        * @state
        * @return
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_ReadDevVersion(byte[] devName, byte[] version, int flag);

        /**
        * @Function() 批量比对资源初始化
        * @Param pd //O：描述符
        * @Param count //I：手指总数
        * @Param reg_count //I：单指注册数
        * @Param dyn_count //I：单指学习数
        * @state 批量比对配套步骤一
        * @return 
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_BatchMatchInit(int[] pb, int count, int reg_count, int dyn_count);

        /**
        * @Function() 批量比对设置被比对参数
        * @Param pb //I：描述符
        * @Param fid //I：手指id ，请确保唯一性，否则影响比对结果
        * @Param reg_freature //I：注册特征，个数等于初始时设置数（reg_count *512字节）
        * @Param dyn_feature //I：动态特征，（<=dyn_count *512字节 && dyn_count%512=0）
        * @Param dyn_count //I：动态特征个数，最大不超过初始时设置数,
        * @state 批量比对配套步骤二
        * @return 
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_BatchMatchAddFinger(int pb, int fid, byte[] reg_feature, byte[] dyn_feature, int dyn_count);

        /**
        * @Function() 批量比对运行比对
        * @Param pb //I：描述符
        * @Param match_feature //I：待比对特征（512字节）
        * @param flag  /I：指静脉模板数据为指静脉模块直接采集的数据时为0x03，经过VM_PreProcess/VM_PreProcessEx处理的数据是为0x00
        * @Param fid //O：比对成功的手指编号
        * @Param diff //O：比对成功的差异度
        * @Param dyn_featurn //O：动态特征，（初始时设置数动态特征数dyn_count*512字节）
        * @Param dyn_feature_len//O：ai特征实际输出长度，">0"时应将featureAI的aiBufLen字节回写数据库
        * @state 批量比对配套步骤三
        * @return 
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_BatchMatchRun(int pb, byte[] match_feature, int flag, int[] fid, int[] diff, byte[] dyn_feature, int[] dyn_feature_len);

        /**
        * @Function() 批量比对资源释放
        * @Param pd //I：描述符
        * @state 批量比对配套步骤四
        * @return 
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_BatchMatchRelease(int pb);

        /**
        * @Function() 新格式转旧格式
        * @Param srcBuff //I：新格式特征
        * @Param desBuff //I：旧格式特征
        * @Param cnt //I：特征个数
        * @state 用于服务器未升级情况
        * @return 
        */
        [DllImport(dllname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FV_Coding(byte[] srcBuff, byte[] desBuff, int cnt);


    }
}
