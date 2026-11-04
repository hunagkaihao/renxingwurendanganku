using WarehouseManagement.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace WarehouseManagement;

public abstract class WarehouseManagementController : AbpControllerBase
{
    protected WarehouseManagementController()
    {
        LocalizationResource = typeof(WarehouseManagementResource);
    }
}
