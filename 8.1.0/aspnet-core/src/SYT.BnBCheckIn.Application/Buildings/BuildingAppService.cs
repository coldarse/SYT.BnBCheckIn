using Abp;
using Abp.Application.Services;
using Abp.Extensions;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SYT.BnBCheckIn.Buildings.Dto;
using SYT.BnBCheckIn.Units;
using SYT.BnBCheckIn.Units.Dto;
using System.Net.NetworkInformation;

namespace SYT.BnBCheckIn.Buildings
{
    public class BuildingAppService : CrudAppService<Building, BuildingDto, Guid, PagedBuildingResultRequestDto>
    {


        public BuildingAppService(IRepository<Building, Guid> repository) : base(repository)
        {
        }
        protected override IQueryable<Building> CreateFilteredQuery(PagedBuildingResultRequestDto input)
        {
            return (IQueryable<Building>)Repository.GetAllIncluding()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => 
                    x.Name.Contains(input.Keyword) ||
                    x.Address.Contains(input.Keyword) ||
                    x.City.Contains(input.Keyword) ||
                    x.State.Contains(input.Keyword) ||
                    x.Postcode.Contains(input.Keyword) ||
                    x.Remark.Contains(input.Keyword));
        }

        public async Task<List<Building>> GetAllBuildings()
        {
            List<Building> temp = await Repository.GetAllListAsync();
            return temp;
        }

        public async Task<Building> getBuilding(Guid id)
        {
            return await Repository.FirstOrDefaultAsync(x => x.Id == id);
        }

    }
}
