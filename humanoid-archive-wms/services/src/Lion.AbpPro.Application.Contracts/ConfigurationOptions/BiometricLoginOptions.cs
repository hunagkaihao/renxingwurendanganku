namespace Lion.AbpPro.ConfigurationOptions
{
    /// <summary>一体机生物识别服务的连接配置；账号绑定由数据库维护。</summary>
    public class BiometricLoginOptions
    {
        /// <summary>WMS 可以访问的 ClientApi HTTP(S) 根地址，必须对应当前一体机。</summary>
        public string ServiceUrl { get; set; }

        /// <summary>识别请求超时，单位为秒，有效范围为 5～120。</summary>
        public int TimeoutSeconds { get; set; } = 60;

    }
}
