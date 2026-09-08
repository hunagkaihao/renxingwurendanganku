using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using WarehouseManagement.Goodss;
using WarehouseManagement.Material;
using Material = WarehouseManagement.Material.Aggregates.Material;
using WarehouseManagement.MaterialBoxs.Aggregates;
using WarehouseManagement.MaterialBoxs.Dto;

namespace WarehouseManagement.MaterialBoxs
{
    public class MaterialBoxDetailManager : MaterialBoxDetailDomainService
    {
        private readonly IArchiveBoxDetailRepository _archiveBoxDetailRepository;
        private readonly IMaterialBoxRepository _materialBoxRepository;
        private readonly IMaterialRepository _archiveRepository;

        public MaterialBoxDetailManager(IArchiveBoxDetailRepository archiveBoxDetailRepository,IMaterialBoxRepository materialBoxRepository,IMaterialRepository archiveRepository)
        {
            _archiveBoxDetailRepository = archiveBoxDetailRepository;
            _materialBoxRepository = materialBoxRepository;
            _archiveRepository = archiveRepository;
        }

        public async Task<MaterialBoxDetail> GetDetailByArchiveId(int Id)
        {
            return await _archiveBoxDetailRepository.FindByArchiveIdAsync(Id);
        }

        public async Task<List<MaterialBoxDetail>> GetAll()
        {
            return await _archiveBoxDetailRepository.GetListAsync();
        }

        public async Task<PagedResultDto<MaterialBoxDetailDto>> GetDetailAsync(PagingMaterialBoxDetailInput input)
        {
            var queryable = await _archiveBoxDetailRepository.GetQueryableAsync();

            var query = from archiveBoxDetail in queryable
                        where archiveBoxDetail.MaterialBoxId == input.MaterialBoxId
                        join ArchiveBox in await _materialBoxRepository.GetQueryableAsync() on archiveBoxDetail.MaterialBoxId equals ArchiveBox.Id
                        join Material in await _archiveRepository.GetQueryableAsync() on archiveBoxDetail.MaterialId equals Material.Id
                        select new { archiveBoxDetail, ArchiveBox, Material };

            //Paging
            query = query
                .OrderByDescending(f => f.archiveBoxDetail.Id)
                .Skip(input.SkipCount)
                .Take(1000);

            var queryResult = await AsyncExecuter.ToListAsync(query);

            var archiveBoxDetailDtos = queryResult.Select(x =>
            {
                var archiveBoxDetailDtos = ObjectMapper.Map<MaterialBoxDetail, MaterialBoxDetailDto>(x.archiveBoxDetail);
                archiveBoxDetailDtos.MaterialName = x.Material.MaterialName;
                archiveBoxDetailDtos.MaterialCode = x.Material.MaterialCode;

                return archiveBoxDetailDtos;
            }).Take(input.PageSize).ToList();

            var totalCount = queryResult.Count() + input.SkipCount;

            return new PagedResultDto<MaterialBoxDetailDto>(
                totalCount,
                archiveBoxDetailDtos
            );
        }
    }
}
