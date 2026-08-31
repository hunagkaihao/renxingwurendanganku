using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wcs.DahSpecss;
using Wcs.DahSpecss.Models;
using Wcs.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Wms.EntityFrameworkCore.Repositories.ArchiveBoxs
{
    public class EfCoreDahSpecsRepository : EfCoreRepository<WcsDbContext, DahSpecs, int>, IDahSpecsRepository
    {
        public EfCoreDahSpecsRepository(IDbContextProvider<WcsDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<DahSpecs> FindBySpecsCodeAsync(string specsCode, CancellationToken cancelToken = default)
        {
            var dbSet = await GetDbSetAsync().ConfigureAwait(false);
            var specsList = await dbSet.AsNoTracking().Where(o => o.SpecCode == specsCode).ToListAsync(cancelToken);
            if(specsList.Count > 1)
                throw new Exception($"规格号为{specsCode}的规格数量不止1个，数据错误");
            if(specsList.Count == 1) return specsList[0];
            else return null;
        }

        public async Task<DahSpecs> FindBySpecsNameAsync(string specsName, CancellationToken cancelToken = default)
        {
            var dbSet = await GetDbSetAsync().ConfigureAwait(false);
            var specsList = await dbSet.AsNoTracking().Where(o => o.SpecName == specsName).ToListAsync(cancelToken);
            if(specsList.Count > 1)
                throw new Exception($"规格名为{specsName}的规格数量不止1个，数据错误");
            if(specsList.Count == 1) return specsList[0];
            else return null;
        }

        public async Task<List<DahSpecs>> GetAllDahSpecsAsync(CancellationToken cancelToken = default)
        {
            var dbSet = await GetDbSetAsync().ConfigureAwait(false);
            return await dbSet.AsNoTracking().OrderBy(o => o.Id).ToListAsync(cancelToken);
        }
    }
}