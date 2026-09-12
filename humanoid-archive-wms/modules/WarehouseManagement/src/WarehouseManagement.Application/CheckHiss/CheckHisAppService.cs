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
        public async Task<PagedResultDto<CheckDetailHisDto>> GetPagingDetailListAsync(PagingCheckDetailHisDto input)
        {

            var queryable = await _checkDetailHisRepository.GetQueryableAsync();

            var query = from checkDetailHis in queryable
                        where (!input.StartCreationTime.HasValue ||
                               checkDetailHis.CreationTime >= input.StartCreationTime.Value)
                              && (!input.EndCreationTime.HasValue ||
                                  checkDetailHis.CreationTime <= input.EndCreationTime.Value)
                        select new { checkDetailHis };

            var pageSize = input.PageSize > 0 ? input.PageSize : 10;
            var skipCount = (input.PageIndex - 1) * pageSize;

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

            var totalCount = checkHisDtos.Count;

            return new PagedResultDto<CheckDetailHisDto>(
                totalCount,
                checkHisDtos
            );
        }

    }
}
