using Wcs.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Wcs.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class WcsController : AbpControllerBase
{
    protected WcsController()
    {
        LocalizationResource = typeof(WcsResource);
    }
}
