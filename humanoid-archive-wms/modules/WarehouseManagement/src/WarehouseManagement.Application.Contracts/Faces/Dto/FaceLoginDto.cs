using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Faces.Dto
{
    public class FaceLoginDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public string Token { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
    }
}
