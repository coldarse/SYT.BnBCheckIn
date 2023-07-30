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
  }
}
