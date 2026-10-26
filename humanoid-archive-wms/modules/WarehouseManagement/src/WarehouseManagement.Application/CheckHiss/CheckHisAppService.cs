using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using WarehouseManagement.CheckHiss.Aggregates;
using WarehouseManagement.CheckHiss.Dto;

namespace WarehouseManagement.CheckHiss
{
    public class CheckHisAppService : WarehouseManagementAppService,ICheckHisAppService
    {
        private readonly ICheckHisRepository _checkHisRepository;
        private readonly ICheckDetailHisRepository _checkDetailHisRepository;

        public CheckHisAppService(ICheckHisRepository checkHisRepository, ICheckDetailHisRepository checkDetailHisRepository)
        {
            _checkHisRepository = checkHisRepository;
            _checkDetailHisRepository = checkDetailHisRepository;
        }

        public async Task<PagedResultDto<CheckHisDto>> GetPagingListAsync(PagingCheckHisDto input)
        {

            var queryable = await _checkHisRepository.GetQueryableAsync();

            //Prepare a query to join books and authors
            var query = from checkHis in queryable
                        where checkHis.CheckCode.Contains(input.Filter.IsNullOrEmpty() ? "" : input.Filter.Trim())
                        select new { checkHis };

            var pageSize = input.PageSize > 0 ? input.PageSize : 10;
            var skipCount = (input.PageIndex - 1) * pageSize;

            query = query
                .OrderByDescending(f => f.checkHis.Id)
                .Skip(skipCount)
                .Take(pageSize);

            var queryResult = await AsyncExecuter.ToListAsync(query);


            var checkHisDtos = queryResult.Select(x =>
            {
                var checkHisDtos = ObjectMapper.Map<CheckHis, CheckHisDto>(x.checkHis);
                //archiveBoxDtos.CellCode = x.cell.CellCode;

                return checkHisDtos;
            }).ToList();

            var totalCount = checkHisDtos.Count;

            return new PagedResultDto<CheckHisDto>(
                totalCount,
                checkHisDtos
            );
        }
        /// <summary>
        /// 按盘点计划编号查询历史明细，保留时间筛选和分页规则。
        /// </summary>
        /// <param name="input">必填的盘点计划编号，以及可选的明细创建时间范围和分页参数。</param>
        /// <returns>对应计划的盘点结果总数和当前页明细；编号不存在时返回空页。</returns>
        public async Task<PagedResultDto<CheckDetailHisDto>> GetPagingDetailListAsync(PagingCheckDetailHisDto input)
        {
            var checkCode = input.CheckCode?.Trim();
            if (string.IsNullOrWhiteSpace(checkCode))
            {
                throw new UserFriendlyException("盘点计划编号不能为空");
            }

            var queryable = await _checkDetailHisRepository.GetQueryableAsync();

            // 对外使用业务编号定位计划，明细仍通过历史主记录 ID 关联，避免混入未关联的旧明细。
            var query = from checkDetailHis in queryable
                        join checkHis in await _checkHisRepository.GetQueryableAsync()
                            on checkDetailHis.CheckId equals checkHis.Id
                        where checkHis.CheckCode == checkCode
                              && (!input.StartCreationTime.HasValue ||
                               checkDetailHis.CreationTime >= input.StartCreationTime.Value)
                              && (!input.EndCreationTime.HasValue ||
                                  checkDetailHis.CreationTime <= input.EndCreationTime.Value)
                        select new { checkDetailHis };

            var pageSize = input.PageSize > 0 ? input.PageSize : 10;
            var pageIndex = input.PageIndex > 0 ? input.PageIndex : 1;
            var skipCount = (pageIndex - 1) * pageSize;
            var totalCount = await AsyncExecuter.CountAsync(query);

            query = query
                .OrderByDescending(f => f.checkDetailHis.Id)
                .Skip(skipCount)
                .Take(pageSize);

            var queryResult = await AsyncExecuter.ToListAsync(query);


            var checkHisDtos = queryResult.Select(x =>
            {
                var checkHisDtos = ObjectMapper.Map<CheckDetailHis, CheckDetailHisDto>(x.checkDetailHis);
                checkHisDtos.MaterialBoxBarcode = string.IsNullOrWhiteSpace(x.checkDetailHis.BoxBarcode)
                    ? x.checkDetailHis.StockBarcode
                    : x.checkDetailHis.BoxBarcode;
                //archiveBoxDtos.CellCode = x.cell.CellCode;

                return checkHisDtos;
            }).ToList();

            return new PagedResultDto<CheckDetailHisDto>(
                totalCount,
                checkHisDtos
            );
        }

    }
}
