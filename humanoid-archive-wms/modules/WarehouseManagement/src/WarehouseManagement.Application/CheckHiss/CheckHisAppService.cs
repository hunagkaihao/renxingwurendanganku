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


            query = query
                .OrderByDescending(f => f.checkHis.Id)
                .Skip(input.SkipCount)
                .Take(1000);

            var queryResult = await AsyncExecuter.ToListAsync(query);


            var checkHisDtos = queryResult.Select(x =>
            {
                var checkHisDtos = ObjectMapper.Map<CheckHis, CheckHisDto>(x.checkHis);
                //archiveBoxDtos.CellCode = x.cell.CellCode;

                return checkHisDtos;
            }).Take(input.PageSize).ToList();

            var totalCount = queryResult.Count() + input.SkipCount;

            return new PagedResultDto<CheckHisDto>(
                totalCount,
                checkHisDtos
            );
        }
        public async Task<PagedResultDto<CheckDetailHisDto>> GetPagingDetailListAsync(PagingCheckDetailHisDto input)
        {

            var queryable = await _checkDetailHisRepository.GetQueryableAsync();

            //Prepare a query to join books and authors
            var query = from checkDetailHis in queryable
                        where (checkDetailHis.CheckId == input.CheckId)
                        select new { checkDetailHis };


            query = query
                .OrderByDescending(f => f.checkDetailHis.Id)
                .Skip(input.SkipCount)
                .Take(1000);

            var queryResult = await AsyncExecuter.ToListAsync(query);


            var checkHisDtos = queryResult.Select(x =>
            {
                var checkHisDtos = ObjectMapper.Map<CheckDetailHis, CheckDetailHisDto>(x.checkDetailHis);
                //archiveBoxDtos.CellCode = x.cell.CellCode;

                return checkHisDtos;
            }).Take(input.PageSize).ToList();

            var totalCount = queryResult.Count() + input.SkipCount;

            return new PagedResultDto<CheckDetailHisDto>(
                totalCount,
                checkHisDtos
            );
        }

    }
}
