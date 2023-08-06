using Abp;
using Abp.Application.Services;
using Abp.Extensions;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SYT.BnBCheckIn.Usages.Dto;

namespace SYT.BnBCheckIn.Usages
{
    public class UsageAppService : CrudAppService<Usage, UsageDto, Guid, PagedUsageResultRequestDto>
    {

        public UsageAppService(IRepository<Usage, Guid> repository) : base(repository)
        {
        }
        protected override IQueryable<Usage> CreateFilteredQuery(PagedUsageResultRequestDto input)
        {
            return (IQueryable<Usage>)Repository.GetAllIncluding()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => 
                    x.CheckInRef.Contains(input.Keyword));
        }

        public async Task<Usage> getUsage(Guid id)
        {
            return await Repository.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
