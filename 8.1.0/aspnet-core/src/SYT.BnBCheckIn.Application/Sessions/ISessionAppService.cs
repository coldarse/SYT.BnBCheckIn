using System.Threading.Tasks;
using Abp.Application.Services;
using SYT.BnBCheckIn.Sessions.Dto;

namespace SYT.BnBCheckIn.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
