using Ecs.DahSpecss.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Ecs.DahSpecss;

public interface IDahSpecsRepository : IRepository<DahSpecs, int>
{
    public Task<DahSpecs> FindBySpecsCodeAsync(string specsCode, CancellationToken cancelToken = default);
    public Task<DahSpecs> FindBySpecsNameAsync(string specsName, CancellationToken cancelToken = default);
    public Task<List<DahSpecs>> GetAllDahSpecsAsync(CancellationToken cancelToken = default);
}