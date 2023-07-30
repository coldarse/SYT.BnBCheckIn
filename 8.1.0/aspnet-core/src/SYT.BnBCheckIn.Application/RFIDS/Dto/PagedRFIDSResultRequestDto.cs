using Abp.Application.Services.Dto;
using System;

namespace SYT.BnBCheckIn.RFIDS
{

  public class PagedRFIDResultRequestDto : PagedResultRequestDto
  {
      public string Keyword { get; set; }
      public string Value { get; set; }
      public Guid? UnitId { get; set; }
  }
}
