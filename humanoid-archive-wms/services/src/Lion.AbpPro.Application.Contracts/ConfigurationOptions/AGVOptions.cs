namespace Lion.AbpPro.ConfigurationOptions
{
    public class AGVOptions
    {
        /// <summary>
        /// AGV服务器地址
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public string Enable { get; set; }
        /// <summary>
        /// 电梯任务类型
        /// </summary>
        public string LiftTaskType { get; set; }
        /// <summary>
        /// 电梯任务类型
        /// </summary>
        public string LiftEmptyTaskType { get; set; }
        /// <summary>
        /// 平层任务类型
        /// </summary>
        public string FloorTaskType { get; set; }
        /// <summary>
        /// 平层空车任务类型
        /// </summary>
        public string FloorEmptyTaskType { get; set; }
        /// <summary>
        /// CTU入库任务类型
        /// </summary>
        public string CTUInTaskType { get; set; }
        /// <summary>
        /// CTU出库任务类型
        /// </summary>
        public string CTUOutTaskType { get; set; }
    }
}