using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApi.Model
{
    public class UserList
    {
        public double score { get; set; }
        public string user_id { get; set; }
        public UserInfo user_info { get; set; }
        public UserList()
        {

        }
        public UserList(string userId, double _score)
        {
            user_id = userId;
            score = _score;
            user_info = new UserInfo();

        }
    }
}
