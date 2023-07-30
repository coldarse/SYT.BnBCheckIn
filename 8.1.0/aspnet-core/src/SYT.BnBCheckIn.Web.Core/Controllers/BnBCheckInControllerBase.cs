using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace SYT.BnBCheckIn.Controllers
{
    public abstract class BnBCheckInControllerBase: AbpController
    {
        protected BnBCheckInControllerBase()
        {
            LocalizationSourceName = BnBCheckInConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
