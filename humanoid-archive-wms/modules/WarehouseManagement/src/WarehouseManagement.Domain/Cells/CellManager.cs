using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lion.AbpPro.ConfigurationOptions;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Settings;
using Lion.AbpPro.Settings.Dtos;
using Lion.AbpPro.Settings;

namespace WarehouseManagement.Cells
{
    public class CellManager : CellDomainService
    {
        private readonly ICellRepository _cellRepository;
        private readonly ISettingAppService _settingDefinitionManager;
        //private readonly IDistributedCache<Cell> _cache;//设置缓存

        //    public CellManager(
        //ICellRepository CellRepository,
        //IDistributedCache<CellDto> cache)
        //    {
        //        _CellRepository = CellRepository;
        //        _cache = cache;
        //    }
        public bool ZBigToSmall { get; set; }
        public bool YBigToSmall { get; set; }
        public bool XBigToSmall { get; set; }
        //public bool WCSEnable { get; set; }
        public CellManager(
            ICellRepository cellRepository , IOptionsSnapshot<AllocationRulesOptions> options , ISettingAppService settingDefinitionManager)
        {
            _cellRepository = cellRepository;
            ZBigToSmall = options.Value.ZBigToSmall;
            YBigToSmall = options.Value.YBigToSmall;
            XBigToSmall = options.Value.XBigToSmall;
            _settingDefinitionManager = settingDefinitionManager;
        }

        /// <summary>
        /// 创建字典类型
        /// </summary>
        /// <param name="code"></param>
        /// <param name="displayText"></param>
        /// <param name="description"></param>
        public async Task<Cell> CreateAsync(string cellCode, string cellType, string cellName, int warehouseId = 1)
        {
            if (await IsExistCode(cellCode))
            {
                throw new UserFriendlyException(message: "库位编码已存在");
            }
            var entity = new Cell(cellCode, cellType, cellName, warehouseId);
            return await _cellRepository.InsertAsync(entity);
        }
        /// <summary>
        /// 自定义创建
        /// </summary>
        /// <returns></returns>
        public async Task<Cell> CustomCreateAsync(string cellCode, string cellType, string cellName, string cellGroup
            , int cell_z, int cell_x, int cell_y, string cellStorageType, string deviceCode, string customCode, string cellModel, int warehouseId = 0)
        {
            if (await IsExistCode(cellCode))
            {
                throw new UserFriendlyException(message: "库位编码已存在");
            }
            var entity = new Cell(cellCode, cellType, cellName, cellGroup, cell_z,
                cell_x, cell_y, cellStorageType, deviceCode, customCode, cellModel, warehouseId);
            return await _cellRepository.InsertAsync(entity);
        }

        public async Task DeleteAsync(int cellId)
        {
            var entity = await _cellRepository.FindByIdAsync(cellId);
            if (entity == null)
                throw new UserFriendlyException(message: "物品不存在");
            await _cellRepository.DeleteAsync(entity);
        }
        public async Task<Cell> UpdateAsync(int id,string cellName, string cellCode, string cellType)
        {
            var entity = await _cellRepository.FindByIdAsync(id);
            if (entity == null)
                throw new UserFriendlyException(message: "物品不存在");
            entity.Update(cellName, cellCode, cellType);
            return await _cellRepository.UpdateAsync(entity);
        }

        /// <summary>
        /// 通过code获取对象
        /// </summary>
        /// <param name="cellCode"></param>
        /// <returns></returns>
        public async Task<Cell> GetByCodeAsync(string cellCode,bool checkEnable=false)
        {
            
            var cellEntity= await _cellRepository.FindByCodeAsync(cellCode);
            if (checkEnable)
            {
                if (cellEntity == null)
                {
                    return null; 
                }
                if (cellEntity.RunStatus != CellRunStatus.Enable || cellEntity.CellStatus != CellStatus.Nohave)
                {
                    return null;
                }
            }
            return cellEntity;
        }

        public async Task<Cell> GetByIdAsync(int cellId, bool checkEnable = false)
        {

            var cellEntity = await _cellRepository.FindByIdAsync(cellId);
            if (checkEnable)
            {
                if (cellEntity == null)
                {
                    return null;
                }
                if (cellEntity.RunStatus != CellRunStatus.Enable || cellEntity.CellStatus != CellStatus.Nohave)
                {
                    return null;
                }
            }
            return cellEntity;
        }

        
        

        public async Task<Cell> UpdateShelfAsync(string cellCode)
        {
            var cell= await _cellRepository.FindByCodeAsync(cellCode);
            if (cell == null)
                throw new UserFriendlyException(message: "库位不存在");
            cell.SetRunStatus(CellRunStatus.Enable.ToString());
            cell.SetCellStatus(CellStatus.Have.ToString());
            return await _cellRepository.UpdateAsync(cell);
        }

        public async Task SetAsSelectedAsync(string cellCode)
        {
            var cell = await _cellRepository.FindByCodeAsync(cellCode);
            if (cell == null)
                throw new UserFriendlyException(message: "库位不存在");
            cell.SetRunStatus(CellRunStatus.Selected.ToString());
            await _cellRepository.UpdateAsync(cell);
            //Log.Warning($"库位:{cellCode}的状态设置为Selected。方法名：{System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }
        public async Task SetAsEnableAsync(string cellCode)
        {
            var cell = await _cellRepository.FindByCodeAsync(cellCode);
            if (cell == null)
                throw new UserFriendlyException(message: "库位不存在");
            cell.SetRunStatus(CellRunStatus.Enable.ToString());
            await _cellRepository.UpdateAsync(cell);
            //Log.Warning($"库位:{cellCode}的状态设置为Enable。方法名：{System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }

        public async Task SetSelectedAsync(int cellId)
        {
            var cell = await _cellRepository.FindByIdAsync(cellId);
            if (cell == null)
                throw new UserFriendlyException(message: "库位不存在");
            cell.SetRunStatus(CellRunStatus.Selected.ToString());
            await _cellRepository.UpdateAsync(cell);
            //Log.Warning($"库位:{cell.CellCode}的状态设置为Selected。方法名：{System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }
        public async Task SetAsEnableAsync(int cellId)
        {
            var cell = await _cellRepository.FindByIdAsync(cellId);
            if (cell == null)
                throw new UserFriendlyException(message: "库位不存在");
            cell.SetRunStatus(CellRunStatus.Enable.ToString());
            await _cellRepository.UpdateAsync(cell);
            //Log.Warning($"库位:{cell.CellCode}的状态设置为Enable。方法名：{System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }
        public async Task SetDisableAsync(string cellCode)
        {
            var cell = await _cellRepository.FindByCodeAsync(cellCode);
            if (cell == null)
                throw new UserFriendlyException(message: "库位不存在");
            cell.SetRunStatus(CellRunStatus.Disable.ToString());
            await _cellRepository.UpdateAsync(cell);
            //Log.Warning($"库位:{cell.CellCode}的状态设置为Selected。方法名：{System.Reflection.MethodBase.GetCurrentMethod().Name}");
        }
        /// <summary>
        /// 入库库位设置
        /// </summary>
        /// <param name="cellName"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<Cell> SetAsStockInAsync(string cellName)
        {
            var cell = await _cellRepository.FindByCodeAsync(cellName);
            if (cell == null)
                throw new UserFriendlyException(message: "库位不存在");
            cell.SetRunStatus(CellRunStatus.Enable.ToString());
            cell.SetCellStatus(CellStatus.Have.ToString());
            return await _cellRepository.UpdateAsync(cell);
        }
        public async Task<Cell> SetAsStockInAsync(int cellId)
        {
            var cell = await _cellRepository.FindByIdAsync(cellId);
            if (cell == null)
                throw new UserFriendlyException(message: "库位不存在");
            cell.SetRunStatus(CellRunStatus.Enable.ToString());
            cell.SetCellStatus(CellStatus.Have.ToString());
            return await _cellRepository.UpdateAsync(cell);
        }
        /// <summary>
        /// 出库库位设置
        /// </summary>
        /// <param name="cellName"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        ///
        public async Task<Cell> SetAsStockOutAsync(string cellName)
        {
            var cell = await _cellRepository.FindByCodeAsync(cellName);
            if (cell == null)
                throw new UserFriendlyException(message: "库位不存在");
            cell.SetRunStatus(CellRunStatus.Enable.ToString());
            cell.SetCellStatus(CellStatus.Nohave.ToString());
            return await _cellRepository.UpdateAsync(cell);
        }
        public async Task<Cell> SetAsStockOutAsync(int cellId)
        {
            var cell = await _cellRepository.FindByIdAsync(cellId);
            if (cell == null)
                throw new UserFriendlyException(message: "库位不存在");
            cell.SetRunStatus(CellRunStatus.Enable.ToString());
            cell.SetCellStatus(CellStatus.Nohave.ToString());
            return await _cellRepository.UpdateAsync(cell);
        }

        /// <summary>
        /// 根据区域码获取库位清单
        /// </summary>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<List<int>> GetCellidsByAreaCode(string areaCode)
        {
            //areaCode 编码规则 与库位编码一致  不补位  z-x-y
            List<int> iLists = null;
            if (areaCode == "0-0-0")
            {
                //iLists = _cellRepository.GetAll().Where(x => x.CellType == "Cell").Select(x => x.Id).ToList();
                List<Cell> cellsAll = await _cellRepository.GetListAsync(x => x.CellType == CellType.Cell);
                iLists = new List<int>();
                for (int ii = 0; ii < cellsAll.Max(x => x.Cell_z); ii++)
                {
                    for (int i = 1; i < cellsAll.Max(x => x.Cell_y) + 1; i++)
                    {
                        if (Convert.ToBoolean(i % 2))
                        {
                            iLists.AddRange(cellsAll.Where(x => x.Cell_y == i & x.Cell_z == ii).OrderBy(y => y.Cell_x).Select(x => x.Id).ToList());
                        }
                        else
                        {
                            iLists.AddRange(cellsAll.Where(x => x.Cell_y == i & x.Cell_z == ii).OrderByDescending(y => y.Cell_x).Select(x => x.Id).ToList());
                        }
                    }
                }
                return iLists;
            }
            try
            {
                string[] areaCodes = areaCode.Split('-');
                if (areaCodes[0] != "0")
                {
                    if (areaCodes[2] != "0")
                    {
                        if (areaCodes[1] == "0")
                        {
                            iLists = (await _cellRepository.GetListAsync(x => x.CellType == CellType.Cell & x.Cell_z == Convert.ToInt32(areaCodes[0]) & x.Cell_y == Convert.ToInt32(areaCodes[2]))).OrderBy(y => y.Cell_x).Select(x => x.Id).ToList();
                        }
                        else
                        {
                            iLists = (await _cellRepository.GetListAsync(x => x.CellType == CellType.Cell & x.Cell_z == Convert.ToInt32(areaCodes[0]) & x.Cell_x == Convert.ToInt32(areaCodes[1]) & x.Cell_y == Convert.ToInt32(areaCodes[2]))).Select(x => x.Id).ToList();
                        }
                    }
                    else
                    {
                        if (areaCodes[1] != "0")
                        {
                            iLists = (await _cellRepository.GetListAsync(x => x.CellType == CellType.Cell & x.Cell_z == Convert.ToInt32(areaCodes[0]) & x.Cell_x == Convert.ToInt32(areaCodes[1]))).OrderBy(y => y.Cell_y).Select(x => x.Id).ToList();
                        }
                        else
                        {
                            List<Cell> cells = await _cellRepository.GetListAsync(x => x.CellType == CellType.Cell & x.Cell_z == Convert.ToInt32(areaCodes[0]));
                            iLists = new List<int>();
                            for (int i = 1; i < cells.Max(x => x.Cell_y) + 1; i++)
                            {
                                if (Convert.ToBoolean(i % 2))
                                {
                                    iLists.AddRange(cells.Where(x => x.Cell_y == i).OrderBy(y => y.Cell_x).Select(x => x.Id).ToList());
                                }
                                else
                                {
                                    iLists.AddRange(cells.Where(x => x.Cell_y == i).OrderByDescending(y => y.Cell_x).Select(x => x.Id).ToList());
                                }
                            }
                            //iLists = _cellRepository.GetAll().Where(x => x.CellType == "Cell" & x.Cell_z == Convert.ToInt32(areaCodes[0])).Select(x => x.Id).ToList();
                            //增加排序进行S扫描
                        }
                    }

                }
                return iLists;
            }
            catch (Exception)
            {

                throw new UserFriendlyException("操作失败", "区域编码错误");
            }

        }

        public async Task<List<int>> OrderCellidsByIds(List<int> ids)
        {
            List<int> iLists = (await _cellRepository.GetListAsync(x => ids.Contains(x.Id) && x.CellType == CellType.Cell)).OrderBy(o => o.Cell_z).ThenBy(o => o.Cell_y).ThenBy(o => o.Cell_x).Select(x => x.Id).ToList();
            return iLists;
        }
        /// <summary>
        /// 根据区域码获取库位清单
        /// </summary>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<List<Cell>> GetCellsByAreaCode(string areaCode)
        {
            //areaCode 编码规则 与库位编码一致  不补位  z-x-y
            List<Cell> iLists = null;
            if (areaCode == "0-0-0")
            {
                //iLists = _cellRepository.GetAll().Where(x => x.CellType == "Cell").Select(x => x.Id).ToList();
                List<Cell> cellsAll = await _cellRepository.GetListAsync(x => x.CellType == CellType.Cell);
                iLists = new List<Cell>();
                for (int ii = 0; ii < cellsAll.Max(x => x.Cell_z); ii++)
                {
                    for (int i = 1; i < cellsAll.Max(x => x.Cell_y) + 1; i++)
                    {

                        iLists.AddRange(cellsAll.Where(x => x.Cell_y == i & x.Cell_z == ii).OrderBy(y => y.Cell_x));

                    }
                }
                return iLists;
            }
            try
            {
                string[] areaCodes = areaCode.Split('-');
                if (areaCodes[0] != "0")
                {
                    if (areaCodes[2] != "0")
                    {
                        if (areaCodes[1] == "0")
                        {
                            iLists = (await _cellRepository.GetListAsync(x => x.CellType == CellType.Cell & x.Cell_z == Convert.ToInt32(areaCodes[0]) & x.Cell_y == Convert.ToInt32(areaCodes[2]))).OrderBy(y => y.Cell_x).ToList();
                        }
                        else
                        {
                            iLists = (await _cellRepository.GetListAsync(x => x.CellType == CellType.Cell & x.Cell_z == Convert.ToInt32(areaCodes[0]) & x.Cell_x == Convert.ToInt32(areaCodes[1]) & x.Cell_y == Convert.ToInt32(areaCodes[2]))).ToList();
                        }
                    }
                    else
                    {
                        if (areaCodes[1] != "0")
                        {
                            iLists = (await _cellRepository.GetListAsync(x => x.CellType == CellType.Cell & x.Cell_z == Convert.ToInt32(areaCodes[0]) & x.Cell_x == Convert.ToInt32(areaCodes[1]))).OrderBy(y => y.Cell_y).ToList();
                        }
                        else
                        {
                            List<Cell> cells = await _cellRepository.GetListAsync(x => x.CellType == CellType.Cell & x.Cell_z == Convert.ToInt32(areaCodes[0]));
                            iLists = new List<Cell>();
                            for (int i = 1; i < cells.Max(x => x.Cell_y) + 1; i++)
                            {
                                iLists.AddRange(cells.Where(x => x.Cell_y == i).OrderBy(y => y.Cell_x).ToList());
                            }
                            //iLists = _cellRepository.GetAll().Where(x => x.CellType == "Cell" & x.Cell_z == Convert.ToInt32(areaCodes[0])).Select(x => x.Id).ToList();
                            //增加排序进行S扫描
                        }
                    }

                }
                return iLists;
            }
            catch (Exception)
            {

                throw new UserFriendlyException("操作失败", "区域编码错误");
            }

        }

        public async Task<Cell> GetByNameAsync(string cellName, bool checkEnable = false)
        {

            var cellEntity = await _cellRepository.FindByNameAsync(cellName);
            if (checkEnable)
            {
                if (cellEntity == null)
                {
                    return null;
                }
                if (cellEntity.RunStatus != CellRunStatus.Enable || cellEntity.CellStatus != CellStatus.Nohave)
                {
                    return null;
                }
            }
            return cellEntity;
        }
        public async Task<List<Cell>> GetByCellTypeAsync(CellType cellType)
        {

            var cellEntity = await _cellRepository.GetListAsync(x => x.CellType == cellType);

            return cellEntity;
        }



        public async Task<bool> IsExistCode(string cellCode)
        {
            var cell = await _cellRepository.FindByCodeAsync(cellCode);
            if (cell != null)
                return true;
            return false;
        }
        //自动分配出入档口
        public async Task<Cell> GetEmptyStation(int warehouseId,string cellModel)
        {
            //获取可用的空库位  符合料箱类型
            var cellList = await _cellRepository
                .GetListAsync(f => f.WarehouseId == warehouseId & f.CellStatus == CellStatus.Nohave & f.RunStatus == CellRunStatus.Enable
               & f.CellModel == cellModel & f.CellType == CellType.Station)
                ;
            // 低层优先  通道优先
            return cellList.OrderBy(s => s.Cell_y).ThenBy(s => s.Cell_x).FirstOrDefault();
        }

        //自动分配密集架库位
        public async Task<Cell> GetEmptyCell(int warehouseId, string cellModel)
        {
            //获取分拨墙配置
            //var allSettings = _settingDefinitionManager.GetAll().ToList(); GetSetting

            //var settings = allSettings.Where(e => e.Properties.ContainsKey("BigToSmall")).ToList();
            ZBigToSmall = Convert.ToBoolean(await _settingDefinitionManager.GetSetting("ZBigToSmall")); 
            XBigToSmall = Convert.ToBoolean(await _settingDefinitionManager.GetSetting("XBigToSmall"));
            YBigToSmall = Convert.ToBoolean(await _settingDefinitionManager.GetSetting("YBigToSmall"));
            //获取可用的空库位  符合料箱类型
            var cellList = await _cellRepository
                .GetListAsync(f => f.WarehouseId == warehouseId & f.CellStatus == CellStatus.Nohave & f.RunStatus == CellRunStatus.Enable
               & f.CellModel == cellModel & f.CellType == CellType.Cell)
                ;
            // 低层优先  通道优先
            //return cellList.OrderBy(s => s.Cell_y).ThenBy(s => s.Cell_x).FirstOrDefault();
            //按配置规则执行
            if(ZBigToSmall)
            {
                if (YBigToSmall)
                {
                    if (XBigToSmall)
                    {
                        return cellList.OrderByDescending(s => s.Cell_z).ThenByDescending(s => s.Cell_y).ThenByDescending(s => s.Cell_x).FirstOrDefault();
                    }
                    return cellList.OrderByDescending(s => s.Cell_z).ThenByDescending(s => s.Cell_y).ThenBy(s => s.Cell_x).FirstOrDefault();
                }
                else
                {
                    if (XBigToSmall)
                    {
                        return cellList.OrderByDescending(s => s.Cell_z).ThenBy(s => s.Cell_y).ThenByDescending(s => s.Cell_x).FirstOrDefault();
                    }
                    return cellList.OrderByDescending(s => s.Cell_z).ThenBy(s => s.Cell_y).ThenBy(s => s.Cell_x).FirstOrDefault();
                }
            }
            else
            {
                if (YBigToSmall)
                {
                    if (XBigToSmall)
                    {
                        return cellList.OrderBy(s => s.Cell_z).ThenByDescending(s => s.Cell_y).ThenByDescending(s => s.Cell_x).FirstOrDefault();
                    }
                    return cellList.OrderBy(s => s.Cell_z).ThenByDescending(s => s.Cell_y).ThenBy(s => s.Cell_x).FirstOrDefault();
                }
                else
                {
                    if (XBigToSmall)
                    {
                        return cellList.OrderBy(s => s.Cell_z).ThenBy(s => s.Cell_y).ThenByDescending(s => s.Cell_x).FirstOrDefault();
                    }
                    return cellList.OrderBy(s => s.Cell_z).ThenBy(s => s.Cell_y).ThenBy(s => s.Cell_x).FirstOrDefault();
                }
            }
        }


    }
}
