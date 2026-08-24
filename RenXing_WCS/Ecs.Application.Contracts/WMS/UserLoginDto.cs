using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecs.WMS
{
    public class UserLoginDto
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string  Name { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
