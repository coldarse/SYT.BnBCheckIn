using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using SYT.BnBCheckIn.Configuration.Dto;

namespace SYT.BnBCheckIn.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : BnBCheckInAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
