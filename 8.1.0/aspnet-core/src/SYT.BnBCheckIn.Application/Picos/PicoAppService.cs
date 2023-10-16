using Abp;
using Abp.Application.Services;
using Abp.Extensions;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SYT.BnBCheckIn.Picos.Dto;

namespace SYT.BnBCheckIn.Picos
{
    public class PicoAppService : CrudAppService<Pico, PicoDto, Guid, PagedPicoResultRequestDto>
    {

        public PicoAppService(IRepository<Pico, Guid> repository) : base(repository)
        {
        }
        protected override IQueryable<Pico> CreateFilteredQuery(PagedPicoResultRequestDto input)
        {
            return Repository.GetAllIncluding()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => 
                    x.Name.Contains(input.Keyword)).AsQueryable();
        }

        public async Task<Pico> getPicoByName(string pico)
        {
            return await Repository.FirstOrDefaultAsync(x => x.Name == pico);
        }

        public async Task<Pico> getPicoByID(Guid pico)
        {
            return await Repository.FirstOrDefaultAsync(x => x.Id == pico);
        }
    }
}
