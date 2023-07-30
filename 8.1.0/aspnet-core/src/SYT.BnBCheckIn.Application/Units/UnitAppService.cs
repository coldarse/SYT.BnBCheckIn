using Abp;
using Abp.Application.Services;
using Abp.Extensions;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SYT.BnBCheckIn.Units.Dto;

namespace SYT.BnBCheckIn.Units
{
    public class UnitAppService : CrudAppService<Unit, UnitDto, Guid, PagedUnitResultRequestDto>
    {

        public UnitAppService(IRepository<Unit, Guid> repository) : base(repository)
        {
        }
        protected override IQueryable<Unit> CreateFilteredQuery(PagedUnitResultRequestDto input)
        {
            return (IQueryable<Unit>)Repository.GetAllIncluding()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => 
                    x.UnitNo.Contains(input.Keyword) ||
                    x.Status.Contains(input.Keyword) ||
                    x.Remark.Contains(input.Keyword));
        }

        public async Task<List<Unit>> GetAllUnits()
        {
            List<Unit> temp = await Repository.GetAllListAsync();
            return temp;
        }
    }
}
