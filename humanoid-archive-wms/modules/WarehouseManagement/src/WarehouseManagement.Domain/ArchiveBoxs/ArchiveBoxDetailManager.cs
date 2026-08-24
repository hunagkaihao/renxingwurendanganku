using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using WarehouseManagement.ArchiveBoxs.Aggregates;
using WarehouseManagement.ArchiveBoxs.Dto;
using WarehouseManagement.Goodss;
using WarehouseManagement.Archives;
using WarehouseManagement.Archives.Aggregates;

namespace WarehouseManagement.ArchiveBoxs
{
    public class ArchiveBoxDetailManager : ArchiveBoxDetailDomainService
    {
        private readonly IArchiveBoxDetailRepository _archiveBoxDetailRepository;
        private readonly IArchiveBoxRepository _archiveBoxRepository;
        private readonly IArchiveRepository _archiveRepository;

        public ArchiveBoxDetailManager(IArchiveBoxDetailRepository archiveBoxDetailRepository,IArchiveBoxRepository archiveBoxRepository,IArchiveRepository archiveRepository)
        {
            _archiveBoxDetailRepository = archiveBoxDetailRepository;
            _archiveBoxRepository = archiveBoxRepository;
            _archiveRepository = archiveRepository;
        }

        public async Task<ArchiveBoxDetail> GetDetailByArchiveId(int Id)
        {
            return await _archiveBoxDetailRepository.FindByArchiveIdAsync(Id);
        }

        public async Task<List<ArchiveBoxDetail>> GetAll()
        {
            return await _archiveBoxDetailRepository.GetListAsync();
        }

        public async Task<PagedResultDto<ArchiveBoxDetailDto>> GetDetailAsync(PagingArchiveBoxDetailInput input)
        {
            var queryable = await _archiveBoxDetailRepository.GetQueryableAsync();

            var query = from archiveBoxDetail in queryable
                        where archiveBoxDetail.ArchiveBoxId == input.ArchiveBoxId
                        join ArchiveBox in await _archiveBoxRepository.GetQueryableAsync() on archiveBoxDetail.ArchiveBoxId equals ArchiveBox.Id
                        join Archive in await _archiveRepository.GetQueryableAsync() on archiveBoxDetail.ArchiveId equals Archive.Id
                        select new { archiveBoxDetail, ArchiveBox, Archive };

            //Paging
            query = query
                .OrderByDescending(f => f.archiveBoxDetail.Id)
                .Skip(input.SkipCount)
                .Take(1000);

            var queryResult = await AsyncExecuter.ToListAsync(query);

            var archiveBoxDetailDtos = queryResult.Select(x =>
            {
                var archiveBoxDetailDtos = ObjectMapper.Map<ArchiveBoxDetail, ArchiveBoxDetailDto>(x.archiveBoxDetail);
                archiveBoxDetailDtos.ArchiveName = x.Archive.ArchivesName;
                archiveBoxDetailDtos.ArchiveCode = x.Archive.ArchivesCode;

                return archiveBoxDetailDtos;
            }).Take(input.PageSize).ToList();

            var totalCount = queryResult.Count() + input.SkipCount;

            return new PagedResultDto<ArchiveBoxDetailDto>(
                totalCount,
                archiveBoxDetailDtos
            );
        }
    }
}
