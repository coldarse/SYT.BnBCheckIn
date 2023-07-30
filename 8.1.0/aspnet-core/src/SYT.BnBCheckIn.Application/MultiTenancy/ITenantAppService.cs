using Abp.Application.Services;
using SYT.BnBCheckIn.MultiTenancy.Dto;

namespace SYT.BnBCheckIn.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

