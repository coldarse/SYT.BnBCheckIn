using System.Threading.Tasks;
using Abp.Application.Services;
using SYT.BnBCheckIn.Authorization.Accounts.Dto;

namespace SYT.BnBCheckIn.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
