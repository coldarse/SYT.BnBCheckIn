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
using SYT.BnBCheckIn.Picos;

namespace SYT.BnBCheckIn.RFIDS
{
    public class RFIDAppService : CrudAppService<RFID, RFIDDto, Guid, PagedRFIDResultRequestDto>
    {
        private readonly IRepository<Unit, Guid> _unitRepository;

        public RFIDAppService(IRepository<RFID, Guid> repository, IRepository<Unit, Guid> unitRepository
            ) : base(repository)
        {
            _unitRepository = unitRepository;
        }

        protected override IQueryable<RFID> CreateFilteredQuery(PagedRFIDResultRequestDto input)
        {
            var rfids = Repository.GetAll();
            var units = _unitRepository.GetAll();

            var joinedQuery = from rfid in rfids
                              join unit in units
                              on rfid.UnitId equals unit.Id
                              select new
                              {
                                  rfid.Id,
                                  rfid.Value,
                                  rfid.UnitId,
                                  unit.UnitNo
                              };

            var orderedQuery = joinedQuery.OrderBy(x => x.UnitNo);

            if (!input.Keyword.IsNullOrWhiteSpace())
            {
                var filteredQuery = joinedQuery.Where(x =>
                    x.Value.ToLower().Contains(input.Keyword.ToLower()) ||
                    x.UnitNo.ToLower().Contains(input.Keyword.ToLower())
                    );

                orderedQuery = filteredQuery.OrderBy(x => x.UnitNo);
                
            }

            var mappQuery = orderedQuery.Select(a => new RFID()
            {
                Id = a.Id,
                Value = a.Value,
                UnitId = a.UnitId
            });

            return mappQuery.AsQueryable();
            
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
