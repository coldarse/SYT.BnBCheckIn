using Abp.Application.Services.Dto;
using System;

namespace SYT.BnBCheckIn.Usages
{

  public class PagedUsageResultRequestDto : PagedResultRequestDto
  {
      public string Keyword { get; set; }
      public string Unit { get; set; }
      public string Pico { get; set; }
      public string RFID { get; set; }
      public string Building { get; set; }
      public DateTime? StartTime { get; set; }
      public DateTime? EndTime { get; set; }
      public string CheckInRef { get; set; }
  }
}
