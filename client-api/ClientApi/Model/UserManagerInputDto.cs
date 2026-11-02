using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApi.Model
{
    /// <summary>旧版用户管理兼容请求。</summary>
    public class UserManagerInputDto
    {
        /// <summary>操作码：2817 新增用户、2819 删除用户、2824 查询用户列表。</summary>
        public int RequestValue { get; set; }
        /// <summary>旧版密码字段；当前接口未使用。</summary>
        public string pass { get; set; }
        /// <summary>新增用户时解码的 Base64 图片内容。</summary>
        public string image_content { get; set; }
        /// <summary>旧版图片类型字段；当前接口未使用。</summary>
        public string image_type { get; set; }
        /// <summary>旧版图片质量控制字段；当前接口未使用。</summary>
        public string quality_control { get; set; }
        /// <summary>业务用户编号，删除兼容响应中返回此值。</summary>
        public string user_id { get; set; }
        /// <summary>旧版用户附加信息；当前接口未使用。</summary>
        public object user_info { get; set; }

    }
}
