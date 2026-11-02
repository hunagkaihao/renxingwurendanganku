namespace ClientApi.Model
{
    /// <summary>人脸录入和删除接口的兼容响应。</summary>
    public class FaceManagerResultDto
    {
        public FaceManagerResultDto(int errno, string msg, string log_id)
        {
            Errno=errno;
            Msg=msg;
            Data = new FaceManagerResultDataDto(log_id);
        }
        /// <summary>结果代码：0 成功，99 业务或服务失败。</summary>
        public int Errno { get; set; }
        /// <summary>操作结果或失败原因的中文说明。</summary>
        public string Msg { get; set; }
        /// <summary>请求追踪信息。</summary>
        public FaceManagerResultDataDto Data { get; set; }
    }
    /// <summary>人脸操作的请求追踪数据。</summary>
    public class FaceManagerResultDataDto
    {
        public FaceManagerResultDataDto(string log_id)
        {
            Log_id =log_id;
        }
        /// <summary>当前 HTTP 请求的追踪标识。</summary>
        public string Log_id { get; set; }
    }
}
