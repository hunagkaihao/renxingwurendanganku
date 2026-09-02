using WarehouseManagement.Cells.Dto;
using WarehouseManagement.Permissions;
using Lion.AbpPro.Extension.Customs.Dtos;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using WarehouseManagement.Cells;
using System.Net.Http;
using Lion.AbpPro.Extension.Customs.Http;

namespace WarehouseManagement.Cells
{
    [Authorize(WarehouseManagementPermissions.CellManagement.Default)]
    public class CellAppService : WarehouseManagementAppService,
         ICellAppService //implement the ICellAppService
    {
        //private readonly IRepository<Cell, Guid> _cellRepository;
        /// <summary>
        ///  注意 为了快速直接注入仓库层 规范上是不允许的
        ///  这里注入仓储也只是为了查询分页
        ///  如果是其他的操作全部通过对应manger进行操作
        /// </summary>
        private readonly ICellRepository _cellRepository;
        private readonly CellManager _cellManagement;
        //private readonly IHttpClientFactory _httpClientFactory;
        public CellAppService(ICellRepository cellRepository,
            CellManager cellManagement)
        {
            _cellRepository = cellRepository;
            _cellManagement = cellManagement;
            //_cellManager = cellManager;
            //GetPolicyName = CellStorePermissions.Cells.Default;
            //GetListPolicyName = CellStorePermissions.Cells.Default;
            //CreatePolicyName = CellStorePermissions.Cells.Create;
            //UpdatePolicyName = CellStorePermissions.Cells.Edit;
            //DeletePolicyName = CellStorePermissions.Cells.Delete;
            //_httpClientFactory = httpClientFactory;
        }
        [Authorize(WarehouseManagementPermissions.CellManagement.Create)]
        public async Task<CellDto> CreateAsync(CreateCellDto input)
        {
            var cell = await _cellManagement.CreateAsync(input.CellCode,input.CellType,input.CellName,input.WarehouseId);
            return  base.ObjectMapper.Map<Cell, CellDto>(cell);
        }

        public async Task InitCreateCell(CellInitDto input)
        {
            List<Cell> cells = await _cellRepository.GetListAsync(f => f.CellType == CellType.Cell);
            if (cells.Count != 0)
            {
                throw new UserFriendlyException("已有库位信息，无法进行库位未初始化。");
            }
            for (int z = 1; z <= input.Cell_z; z++)
            {
                for (int y = 1; y <= input.Cell_y; y++)
                {
                    for (int x = 1; x <= input.Cell_x; x++)
                    {
                        CreateCellDto cell = new CreateCellDto();
                        cell.CellType = "Cell";

                        // WMS 与 WCS 统一使用“排D2-列D3-层D2”的库位码协议。
                        // 坐标字段含义保持为 z=排、x=列、y=层，例如第1排、第1列、第1层生成 01-001-01。
                        // 列号必须补齐三位，否则 WCS 按自身标准生成盘点结果时会返回 01-001-01，
                        // 而 WMS 若保存成 01-01-01，将无法按 CellCode 匹配出入库库位和盘点冻结快照。
                        cell.CellCode = z.ToString().PadLeft(2, '0') + '-' +
                                        x.ToString().PadLeft(3, '0') + '-' +
                                        y.ToString().PadLeft(2, '0');
                        cell.CellName = cell.CellCode;
                        cell.WarehouseId = 1;
                        await _cellManagement.CreateAsync(cell.CellCode, cell.CellType, cell.CellName, cell.WarehouseId);
                    }

                }
            }
 
        }

        [AllowAnonymous]
        public async Task<CellDto> CustomCreateAsync(CustomCreateCellDto input)
        {
            //var cellEntity = base.ObjectMapper.Map<CreateCellDto, Cell>(input);
            //var cell=  await _cellRepository.InsertAsync(cellEntity);
            var cell = await _cellManagement.CustomCreateAsync(input.CellCode, input.CellType, input.CellName
                , input.CellGroup, input.Cell_z, input.Cell_x, input.Cell_y, input.CellStorageType, input.DeviceCode
                , input.CustomCode, input.CellModel, input.WarehouseId);
            return base.ObjectMapper.Map<Cell, CellDto>(cell);
        }
        [Authorize(WarehouseManagementPermissions.CellManagement.Create)]
        public async Task<CellDto> CreateStationAsync(CreateStationDto input)
        {
            //var cellEntity = base.ObjectMapper.Map<CreateCellDto, Cell>(input);
            //var cell=  await _cellRepository.InsertAsync(cellEntity);
            var cell = await _cellManagement.CreateAsync(input.CellCode, input.CellType, input.CellName,input.WarehouseId);
            return base.ObjectMapper.Map<Cell, CellDto>(cell);
        }        
        [Authorize(WarehouseManagementPermissions.CellManagement.Create)]
        public async Task<CellDto> GetByCodeAsync(CreateCellDto input)
        {
            //var cellEntity = base.ObjectMapper.Map<CreateCellDto, Cell>(input);
            //var cell=  await _cellRepository.InsertAsync(cellEntity);
            var cell = await _cellManagement.GetByCodeAsync(input.CellCode,true);
            if (cell == null)
            {
                throw new UserFriendlyException(message: "库位不存在或不可用");
            }
            return base.ObjectMapper.Map<Cell, CellDto>(cell);
        }
        public async Task<PagedResultDto<CellDto>> GetPagingListAsync(PagingCellListInput input)
        {

            // 通过access token 获取用户信息
            //Dictionary<string, string> headers = new Dictionary<string, string>
            //    { { "Authorization", $"Bearer {accessToken}" } };
            //var response =
            //    await _httpClientFactory.PostAsync<PagingCellListInput, PagedResultDto<CellDto>>("agv", "http://localhost:44315/Cells/stationpage", new PagingCellListInput() { PageIndex=1,PageSize=10});


            var result = new PagedResultDto<CellDto>();
            var totalCount = await _cellRepository.GetPagingCountAsync(input.Filter, input.WarehouseId, input.CellType);
            result.TotalCount = totalCount;
            if (totalCount <= 0) return result;

            var entities = await _cellRepository.GetPagingListAsync(input.Filter,input.WarehouseId,input.CellType, input.PageSize,
                input.SkipCount, false);
            result.Items = ObjectMapper.Map<List<Cell>, List<CellDto>>(entities);

            return result;
        }


        public async Task<ListResultDto<CellDto>> GetCellListByZAsync(PagingCellListInput input)
        {
            var result = new ListResultDto<CellDto>();
            var entities = await _cellRepository.GetCellListByZAsync(input.CellZ, false);
            result.Items = ObjectMapper.Map<List<Cell>, List<CellDto>>(entities);

            return result;
        }
        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(WarehouseManagementPermissions.CellManagement.Update)]
        public virtual async Task<CellDto> UpdateAsync(UpdateCellDto input)
        {
            var cell= await _cellManagement.UpdateAsync(input.Id,input.CellName,input.CellCode,input.CellType);
            return base.ObjectMapper.Map<Cell, CellDto>(cell);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        [Authorize(WarehouseManagementPermissions.CellManagement.Delete)]
        public virtual async Task DeleteAsync(IdIntInput input)
        {
            await _cellManagement.DeleteAsync(input.Id);
            //await _cellRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// 获取所有库位
        /// </summary>
        /// <returns></returns>
        public async Task<ListResultDto<CellDto>> AllListAsync()
        {
            List<Cell> source =
                await _cellRepository.GetListAsync()
                    .ConfigureAwait(continueOnCapturedContext: false);
            return new ListResultDto<CellDto>(
                base.ObjectMapper.Map<List<Cell>, List<CellDto>>(source));
        }

        public async Task SetCellEnable(UpdateCellDto input)
        {
            await _cellManagement.SetAsEnableAsync(input.CellCode);
        }

        public async Task SetCellDisable(UpdateCellDto input)
        {
            await _cellManagement.SetDisableAsync(input.CellCode);
        }

    }
}
