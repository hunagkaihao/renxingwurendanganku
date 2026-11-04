using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Faces.Dto;
using WarehouseManagement.Fingers.Dto;

namespace WarehouseManagement.Fingers
{
    public interface  IVeinAppService 
    {
        
        //添加指静脉
        Task<VeinDto> VeinAddAsync(AddVeinDto veins);


        
        //删除指静脉
        Task<VeinDto> VeinDeleteAsync(string fingerId);

        //根据id获取指静脉
        Task<List<AddVeinDto>> GetVeinListByUserIdAsync(string userId);


    }
}
