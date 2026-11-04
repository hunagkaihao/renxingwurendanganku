using WarehouseManagement.Localization;
using Volo.Abp.Application.Services;

namespace WarehouseManagement;

public abstract class WarehouseManagementAppService : ApplicationService
{
    protected WarehouseManagementAppService()
    {
        LocalizationResource = typeof(WarehouseManagementResource);
        ObjectMapperContext = typeof(WarehouseManagementApplicationModule);
    }
}
