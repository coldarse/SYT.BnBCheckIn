using System.Threading.Tasks;
using SYT.BnBCheckIn.Configuration.Dto;

namespace SYT.BnBCheckIn.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
