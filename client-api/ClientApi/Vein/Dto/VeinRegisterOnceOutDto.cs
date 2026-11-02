namespace ClientApi.Vein.Dto
{
    /// <summary>指静脉单次采集或最终注册的结果。</summary>
    public class VeinRegisterOnceOutDto
    {
        public VeinRegisterOnceOutDto(string msg, bool success, string veinFeature)
        {
            Msg = msg;
            Success = success;
            VeinFeature = veinFeature;
        }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string? Msg { get; set; }
        /// <summary>
        /// 本次采集或注册是否成功；true 不代表已经完成全部六次采集。
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// Base64 累计特征，供下一次采集传回；失败时可能返回原特征或空值。
        /// </summary>
        public string? VeinFeature { get; set; }
    }
}
