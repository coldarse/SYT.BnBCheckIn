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

namespace SYT.BnBCheckIn.RFIDS
{
    public class RFIDAppService : CrudAppService<RFID, RFIDDto, Guid, PagedRFIDResultRequestDto>
    {

        public RFIDAppService(IRepository<RFID, Guid> repository) : base(repository)
        {
        }
        protected override IQueryable<RFID> CreateFilteredQuery(PagedRFIDResultRequestDto input)
        {
            return (IQueryable<RFID>)Repository.GetAllIncluding()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => 
                    x.Value.Contains(input.Keyword));
        }

        public async Task<RFID> getRFID(string value)
        {
            return await Repository.FirstOrDefaultAsync(x => x.Value == value);
        }

        public List<string> getUnitRFIDs(Guid unitId)
        {
            List<RFID> rfids = Repository.GetAllIncluding().Where(r => r.UnitId.Equals(unitId)).ToList();
            List<string> unitrfids = new List<string>();
            foreach(var rfid in rfids)
            {
                unitrfids.Add(rfid.Value);
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
    }
}
