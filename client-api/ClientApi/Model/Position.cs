using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApi.Model
{
    public class Position
    {
        public string point_1 { get; set; }
        public string point_2 { get; set; }
        public Position()
        {
            point_1 = "";
            point_2 = "";

        }
        //public Position(string userId, double _score)
        //{
        //    user_id = userId;
        //    score = _score;

        //}
    }
}
