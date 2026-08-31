using Wcs.Localization;
using Volo.Abp.Application.Services;

namespace Wcs;

public abstract class WcsAppService : ApplicationService
{
    protected WcsAppService()
    {
        LocalizationResource = typeof(WcsResource);
    }
}