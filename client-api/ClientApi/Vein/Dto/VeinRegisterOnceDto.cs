namespace ClientApi.Vein.Dto
{
    /// <summary>指静脉单次采集请求，连续调用六次完成注册。</summary>
    public class VeinRegisterOnceDto
    {
        /// <summary>
        /// 业务用户编号；完成第六次采集时转换为 Int16 后提交后台。
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 兼容保留的指静脉编号，当前接口未使用。
        /// </summary>
        public int FingerId { get; set; } = 0;
        /// <summary>
        /// 调用前已完成的采集次数，正常流程为 0～5，第六次调用传 5。
        /// </summary>
        public int RegisterCount { get; set; }
        /// <summary>
        /// 上次响应中的 Base64 累计特征；首次调用可为空，后续原样传回。
        /// </summary>
        public string? VeinFeature { get; set; }
    }
}
