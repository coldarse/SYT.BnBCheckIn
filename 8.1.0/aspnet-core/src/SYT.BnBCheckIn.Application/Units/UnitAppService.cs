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
using SYT.BnBCheckIn.Picos;
using SYT.BnBCheckIn.RFIDS;
using SYT.BnBCheckIn.Usages;
using SYT.BnBCheckIn.Buildings;

namespace SYT.BnBCheckIn.Units
{
    public class UnitAppService : CrudAppService<Unit, UnitDto, Guid, PagedUnitResultRequestDto>
    {
        private readonly PicoAppService _picoAppService;
        private readonly RFIDAppService _rFIDAppService;
        private readonly UsageAppService _usageAppService;
        private readonly BuildingAppService _buildingAppService;

        public UnitAppService(IRepository<Unit, Guid> repository, PicoAppService picoAppService, RFIDAppService rFIDAppService, UsageAppService usageAppService, BuildingAppService buildingAppService) : base(repository)
        {
            _picoAppService = picoAppService;
            _rFIDAppService = rFIDAppService;
            _usageAppService = usageAppService;
            _buildingAppService = buildingAppService;
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

        public async Task<VerifyDto> Verify(VerifyComponentsDto input)
        {
            VerifyDto verify = new VerifyDto();

            var pico = await _picoAppService.getPico(input.PicoId);
            var rfid = await _rFIDAppService.getRFID(input.RFID);

            if (pico.UnitId != rfid.UnitId) return verify;

            var unit = await Repository.FirstOrDefaultAsync(x => x.Id == pico.UnitId);
            var building = await _buildingAppService.getBuilding(unit.BuildingId);

            var usage = _usageAppService.Create(new Usages.Dto.UsageDto
            {
                Unit = unit.UnitNo,
                Pico = pico.Name,
                RFID = rfid.Value,
                Building = building.Name,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.MinValue,
                CheckInRef = ""
            });

            verify.Validity = true;
            verify.UsageId = usage.Id;

            return verify;
        }
    }
}
