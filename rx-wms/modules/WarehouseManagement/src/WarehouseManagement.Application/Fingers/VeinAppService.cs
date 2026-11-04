using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarehouseManagement.Fingers.Aggregates;
using WarehouseManagement.Fingers.Dto;

namespace WarehouseManagement.Fingers
{
    public class VeinAppService : WarehouseManagementAppService, IVeinAppService
    {
        private readonly IVeinRepository _veinRepository;

        public VeinAppService(IVeinRepository veinRepository) { 
            _veinRepository = veinRepository;
        
        }

        public async Task<List<AddVeinDto>> GetVeinListByUserIdAsync(string userid)
        {

            /*
            List<Vein> veinLis = await _veinRepository.GetVeinsByUserId(userid);
            List<AddVeinDto> addVeinDtoLis = new();

            for (int i = 0; i < veinLis.Count(); i++)
            {
                addVeinDtoLis.Add(new AddVeinDto { FingerId = veinLis[i].FingerId, UserId = veinLis[i].UserId });
            }
            return addVeinDtoLis;
            */

            
            List<Vein> veinList = await _veinRepository.GetVeinsByUserId(userid);
            
            List<AddVeinDto> addVeinDtoList = veinList.Select(vein => new AddVeinDto
            {
                UserId = vein.UserId,
                FingerId = vein.FingerId
            }).ToList();

            return addVeinDtoList;
            
        }

        public async Task<VeinDto> VeinAddAsync(AddVeinDto vein)
        {


            try
            {
                List<Vein> veinlist = await _veinRepository.GetVeinsByUserId(vein.UserId);

                if (veinlist.Any(v => v.FingerId == vein.FingerId))
                {
                    return new VeinDto { Success = false, Error = "该用户已有相同的指静脉数据。" };
                }
                else
                {
                    Vein vein1 = new Vein
                    {
                        UserId = vein.UserId,
                        FingerId = vein.FingerId
                    };
                    await _veinRepository.InsertAsync(vein1).ConfigureAwait(false);
                    return new VeinDto { Success = true, Error = "" };
                }


            }
            catch (Exception ex)
            {
                return new VeinDto { Success = false, Error = ex.Message};
            }

            


        }

        /*
        public async Task<VeinDto> VeinAddAsync(AddVeinDto veins)
        {
            try
            {
                Vein vein =new Vein();

                List<Vein> veinsLis = await _veinRepository.GetVeinsByUserId(veins.UserId);

                if (veinsLis.Count == 0)
                {
                    vein.VeinNo = 1;
                }
                else if (veinsLis.Count >= 6)
                {
                    throw new UserFriendlyException("用户存储指静脉数据已超过最大限制！");
                }
                else
                {
                    vein.VeinNo = veinsLis.Count + 1;
                }

                vein.UserId= veins.UserId;
                vein.FeatureData = veins.FeatureData;
                vein.FeatureCnt = veins.FeatureCnt;


                await _veinRepository.InsertAsync(vein);

                return new VeinDto() { Success = true, Error = "" };
            }
            catch (Exception ex)
            {
                return new VeinDto() { Success = false, Error = ex.Message };
            }
        }


        */

        public async Task<VeinDto> VeinDeleteAsync(string fingerid)
        {
            try
            {
                await _veinRepository.DeleteAsync(o => o.FingerId == fingerid);

                return new VeinDto() { Success = true, Error = "" };
            }
            catch (Exception ex)
            {
                return new VeinDto() { Success = false, Error = ex.Message };
            }
        }
    }
}
