using System;

namespace Lion.AbpPro.Users.Dtos
{
    /// <summary>指静脉设备采集进度；设备完成后仍须等待录入接口确认 WMS 绑定成功。</summary>
    public class UserBiometricEnrollmentProgressDto
    {
        /// <summary>本次录入操作随机标识。</summary>
        public Guid OperationId { get; set; }
        /// <summary>设备响应中的业务用户 GUID，仅用于校验进度确属当前租户目标人员。</summary>
        public string UserId { get; set; }
        /// <summary>当前阶段：preparing、place、capturing、remove、writing、binding、completed 或 failed。</summary>
        public string Stage { get; set; } = "preparing";
        /// <summary>当前操作提示，以设备实际阶段为准。</summary>
        public string Message { get; set; } = "正在连接设备，请准备使用同一根手指完成六次录入。";
        /// <summary>已经采集的特征次数，有效范围 0～6。</summary>
        public int CompletedSamples { get; set; }
        /// <summary>需要采集的总次数，固定为 6。</summary>
        public int TotalSamples { get; set; } = 6;
        /// <summary>设备采集流程是否结束；不会代替账号绑定接口的成功结果。</summary>
        public bool IsComplete { get; set; }
        /// <summary>设备录入成功或失败，采集中为空。</summary>
        public bool? Success { get; set; }
        /// <summary>失败时设备返回的 SDK 错误码，没有错误码时为空。</summary>
        public int? SdkCode { get; set; }
    }
}
