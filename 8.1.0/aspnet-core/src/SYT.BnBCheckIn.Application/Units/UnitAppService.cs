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
            IQueryable<Unit> units = Repository.GetAllIncluding()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => 
                    x.UnitNo.Contains(input.Keyword) ||
                    x.Status.Contains(input.Keyword) ||
                    x.Remark.Contains(input.Keyword)).AsQueryable();

            units = units.Where(x => !x.UnitNo.ToLower().Contains("master"));
            return units;
        }

        public async Task<List<Unit>> GetAllUnits()
        {
            List<Unit> temp = await Repository.GetAllListAsync();
            return temp;
        }

        public async Task<Unit> GetUnitbyName(string unit)
        {
            Unit temp = await Repository.FirstOrDefaultAsync(x => x.UnitNo.ToLower() == unit.ToLower());
            return temp;
        }

        public async Task<VerifyDto> Verify(VerifyComponentsDto input)
        {
            VerifyDto verify = new VerifyDto();

            try
            {
                var rfid = await _rFIDAppService.getRFID(input.RFID);
                var pico = await _picoAppService.getPicoByID(input.picoId);

                Unit picounit = Repository.FirstOrDefault(u => u.Id.Equals(pico.UnitId));
                Unit rfidunit = Repository.FirstOrDefault(u => u.Id.Equals(rfid.UnitId));

                if (picounit is null) return verify;
                if (rfidunit is null) return verify;

                var unit = await Repository.FirstOrDefaultAsync(x => x.Id == pico.UnitId);
                var building = await _buildingAppService.getBuilding(unit.BuildingId);

                //--- Getting Building ID for Master to match with Unit's Building Id ---//
                var units = await Repository.GetAllListAsync(x => x.BuildingId == unit.BuildingId);
                var master_unit = units.Find(x => x.UnitNo.ToLower().Contains("master"));

                List<string> rfids = _rFIDAppService.getUnitRFIDs(picounit.Id, master_unit.Id);

                if (rfidunit.UnitNo.ToLower().Contains("master"))
                {
                    if (rfidunit.BuildingId != unit.BuildingId) return verify;
                    
                    verify.Validity = true;
                    verify.UnitRFIDs = rfids;

                    return verify;
                }

                if (pico.UnitId != rfid.UnitId) return verify;

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
                verify.UnitRFIDs = rfids;

                return verify;
            }
            catch(Exception ex)
            {
                verify.Error = ex.ToString();
                return verify;
            }
        }

        public async Task<bool> updateMasterUnitName(UpdateDeleteMasterUnitDto input)
        {
            var tempUnit = await Repository.FirstOrDefaultAsync(x => x.BuildingId.Equals(input.BuildingId));

            tempUnit.UnitNo = input.Name + " Master";

            var updateUnit = await Repository.UpdateAsync(tempUnit);

            if (updateUnit != null) return true;

            return false;
        }

        public async Task deleteMasterUnit(string input)
        {
            await Repository.DeleteAsync(x => x.UnitNo.Equals(input + " Master"));
        }

        public async Task<bool> getAreThereAssignedUnits(Guid buildingId)
        {
            var tempUnits = await Repository.GetAllListAsync(x => x.BuildingId.Equals(buildingId));

            //tempUnits = tempUnits.Where(x => !x.UnitNo.ToLower().Contains("master")).ToList();

            if (tempUnits.Count != 0) return true;
            return false;
        }

        public async Task<bool> updateNewBuilding(UpdateBuildingIds input)
        {
            try
            {
                var tempUnits = await Repository.GetAllListAsync(x => x.BuildingId.Equals(input.buildingId));

                foreach (var unit in tempUnits)
                {
                    unit.BuildingId = input.newBuildingId;
                    await Repository.UpdateAsync(unit);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
