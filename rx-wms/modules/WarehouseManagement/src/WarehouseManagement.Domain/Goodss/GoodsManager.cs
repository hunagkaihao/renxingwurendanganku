using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using WarehouseManagement.Goodss.Aggregates;
using Volo.Abp.Uow;

namespace WarehouseManagement.Goodss
{
    public class GoodsManager : GoodsDomainService
    {
        private readonly IGoodsRepository _goodsRepository;
        //private readonly IDistributedCache<Goods> _cache;//设置缓存

        //    public GoodsManager(
        //IGoodsRepository GoodsRepository,
        //IDistributedCache<GoodsDto> cache)
        //    {
        //        _GoodsRepository = GoodsRepository;
        //        _cache = cache;
        //    }

        public GoodsManager(
            IGoodsRepository goodsRepository)
        {
            _goodsRepository = goodsRepository;
        }

        /// <summary>
        /// 创建物料
        /// </summary>
        /// <param name="code"></param>
        /// <param name="displayText"></param>
        /// <param name="description"></param>
        public async Task<Goods> CreateAsync(string goodsCode, string goodsName, string goodsSpec, string goodsBand, string goodsUnits)
        {
            var existingGoods = await _goodsRepository.FindAsync(f => f.GoodsCode == goodsCode);
            if (existingGoods != null)
            {
                throw new UserFriendlyException(message: "创建物料失败，物料编码重复");
            }
            var entity = new Goods(goodsCode, goodsName, goodsSpec, goodsBand, goodsUnits);
            return await _goodsRepository.InsertAsync(entity);
        }
        public async Task<Goods> CreateAsync(string matCode, string ownerCode, string matText, string matUnit, double groWet, string matTypCode, string matGrpCode
            , string abcFlag, double minStkQty, double maxStkQty, string picUrl, string abolishFlag
            , string matStr1, string matStr2, string matStr3, string matStr4, string matStr5, string matStr6
            , string matStr7, string matStr8, string matStr9
            , string validateFlag, string validateRule, int validatePeriod, int expireWarnTime, int outPriorTime)
        {
            var existingGoods = await _goodsRepository.FindAsync(f => f.GoodsCode == matCode);
            if (existingGoods != null)
            {
                throw new UserFriendlyException(message: "创建物料失败，物料编码重复");
            }
            var entity = new Goods(matCode, ownerCode, matText, matUnit, groWet,matTypCode,matGrpCode,abcFlag,minStkQty,maxStkQty,picUrl
                ,abolishFlag,matStr1,matStr2,matStr3,matStr4,matStr5,matStr6,matStr7,matStr8,matStr9
                ,validateFlag,validateRule,validatePeriod,expireWarnTime,outPriorTime);
            return await _goodsRepository.InsertAsync(entity);
        }
        public async Task<Goods> CreateAsync(string goodsCode, string goodsName, string goodsSpec, string goodsBand)
        {
            var existingGoods = await _goodsRepository.FindAsync(f => f.GoodsCode == goodsCode);
            if (existingGoods != null)
            {
                throw new UserFriendlyException(message: "创建物料失败，物料编码重复");
            }
            var entity = new Goods(goodsCode, goodsName, goodsSpec, goodsBand);
            return await _goodsRepository.InsertAsync(entity,true);
        }
        /// <summary>
        /// EXCEL批量导入物料
        /// </summary>
        /// <param name="goodsBaseDtos"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [UnitOfWork]
        public async Task CreateManyAsync(List<GoodsBaseDto> goodsBaseDtos)
        {
            List<string> goodsCodes = goodsBaseDtos.Select(d => d.GoodsCode).ToList();
            var existingGoodss = await _goodsRepository.GetListAsync(f => goodsCodes.Contains(f.GoodsCode));
            if (existingGoodss.Count > 0)
            {
                throw new UserFriendlyException(message: "创建物料失败，存在重复条码");
            }
            List<Goods> goodss = new List<Goods>();
            for (int i = 0; i < goodsBaseDtos.Count; i++)
            {
                if (goodss.Any(f=>f.GoodsCode == goodsBaseDtos[i].GoodsCode))
                    throw new UserFriendlyException(message: "创建物料失败，导入数据中存在重复条码");
                var entity = new Goods(goodsBaseDtos[i].GoodsCode, goodsBaseDtos[i].GoodsName, goodsBaseDtos[i].GoodsSpec, goodsBaseDtos[i].GoodsBand);
                goodss.Add(entity);
            }
            await _goodsRepository.InsertManyAsync(goodss,true);
        }

        public async Task DeleteAsync(int goodsId)
        {
            var entity = await _goodsRepository.FindByIdAsync(goodsId);
            if (entity == null)
                throw new UserFriendlyException(message: "物品不存在");
            await _goodsRepository.DeleteAsync(entity);
        }
        public async Task<Goods> UpdateAsync(int id, string goodsCode, string goodsName, string goodsSpec, string goodsBand, string goodsUnits)
        {
            var entity = await _goodsRepository.FindByIdAsync(id);
            if (entity == null)
                throw new UserFriendlyException(message: "物品不存在");
            entity.Update(goodsCode, goodsName, goodsSpec, goodsBand, goodsUnits);
            return await _goodsRepository.UpdateAsync(entity);
        }
        public async Task<Goods> GetByIdAsync(int goodsId)
        {
            return await _goodsRepository.FindByIdAsync(goodsId);
        }

        public async Task<List<Goods>> GetListByIdsAsync(List<int> goodsIds)
        {
            return await _goodsRepository.GetListAsync(f => goodsIds.Contains(f.Id));
        }
        /// <summary>
        /// 通过编码获取物料
        /// </summary>
        /// <param name="goodsCode"></param>
        /// <returns></returns>
        public async Task<Goods> GetByCodeAsync(string goodsCode)
        {
            return await _goodsRepository.FindByCodeAsync(goodsCode);
        }
        /// <summary>
        /// 通过编码清单获取物料清单
        /// </summary>
        /// <param name="goodsCodes"></param>
        /// <returns></returns>
        public async Task<List<Goods>> GetByCodeListAsync(List<string> goodsCodes)
        {
            return await _goodsRepository.GetListAsync(f=> goodsCodes.Contains(f.GoodsCode));
        }
    }
}
