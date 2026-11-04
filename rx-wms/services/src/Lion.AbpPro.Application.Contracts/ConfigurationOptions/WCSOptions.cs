using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lion.AbpPro.ConfigurationOptions
{
    public class WCSOptions
    {
        /// <summary>
        /// WCS服务器地址
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 是否使用WCS盘点结果模拟数据（本地调试场景）
        /// </summary>
        public bool WCSSimulation { get; set; }
    }
}
