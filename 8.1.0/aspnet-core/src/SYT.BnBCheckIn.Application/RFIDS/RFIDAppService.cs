using Abp;
using Abp.Application.Services;
using Abp.Extensions;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SYT.BnBCheckIn.RFIDS.Dto;
using SYT.BnBCheckIn.Units;
using Abp.UI;

namespace SYT.BnBCheckIn.RFIDS
{
    public class RFIDAppService : CrudAppService<RFID, RFIDDto, Guid, PagedRFIDResultRequestDto>
    {
        //private readonly UnitAppService _unitAppService;

        public RFIDAppService(IRepository<RFID, Guid> repository
            //,UnitAppService unitAppService
            ) : base(repository)
        {
            //_unitAppService = unitAppService;
        }

        protected override IQueryable<RFID> CreateFilteredQuery(PagedRFIDResultRequestDto input)
        {
            //if (!input.Keyword.IsNullOrWhiteSpace())
            //{
            //    var unit = _unitAppService.GetUnitbyName(input.Keyword);
            //    if (unit is not null)
            //    {
            //        return Repository.GetAllIncluding()
            //            .Where(x => x.UnitId.Equals(unit.Id));
            //    }

            //    return Repository.GetAllIncluding()
            //        .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
            //            x.Value.Contains(input.Keyword)).AsQueryable();
            //}

            //return Repository.GetAll();

            return Repository.GetAllIncluding()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Value.Contains(input.Keyword)).AsQueryable();
        }

        public async Task<RFID> getRFID(string value)
        {
            return await Repository.FirstOrDefaultAsync(x => x.Value == value);
        }

        public List<string> getUnitRFIDs(Guid unitId, Guid masterunit)
        {
            List<RFID> rfids = Repository.GetAllIncluding().Where(r => r.UnitId.Equals(unitId)).ToList();

            List<RFID> masterrfids = Repository.GetAllIncluding().Where(m => m.UnitId.Equals(masterunit)).ToList();

            List<string> unitrfids = new List<string>();

            foreach(var rfid in rfids)
            {
                unitrfids.Add(rfid.Value);
            }

            foreach (var m_rfid in masterrfids)
            {
                unitrfids.Add(m_rfid.Value);
            }

            return unitrfids;
        }

        public async Task<bool> registerRFID(regRFID input)
        {
            var temp_rfid = await Repository.FirstOrDefaultAsync(x => x.Value == input.Value);

            if (temp_rfid is not null) return false;

            RFID temp = new RFID()
            {
                Value = input.Value,
                UnitId = input.UnitId
            };

            try
            {
                var rfid = await Repository.InsertAsync(temp);
                if (rfid is not null) return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> getIsExist(string value)
        {
            var rfid = await Repository.FirstOrDefaultAsync(x => x.Value == value);
            if (rfid is null) return true;
            return false;
        }
    }
}
