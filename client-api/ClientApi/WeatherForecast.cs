namespace ClientApi
{
    /// <summary>随机生成的天气预报示例数据。</summary>
    public class WeatherForecast
    {
        /// <summary>预报日期，基于服务器本地时间生成。</summary>
        public DateTime Date { get; set; }

        /// <summary>摄氏温度，单位为摄氏度。</summary>
        public int TemperatureC { get; set; }

        /// <summary>由摄氏温度换算的华氏温度，单位为华氏度。</summary>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /// <summary>天气摘要，可为空。</summary>
        public string? Summary { get; set; }
    }
}
