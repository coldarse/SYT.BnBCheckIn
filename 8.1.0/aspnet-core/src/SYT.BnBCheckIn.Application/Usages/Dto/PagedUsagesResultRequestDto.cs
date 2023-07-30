using Abp.Application.Services.Dto;
using System;

namespace SYT.BnBCheckIn.Usages
{

  public class PagedUsageResultRequestDto : PagedResultRequestDto
  {
      public string Keyword { get; set; }
      public Guid? Unit { get; set; }
      public Guid? Pico { get; set; }
      public Guid? RFID { get; set; }
      public Guid? Building { get; set; }
      public DateTime? StartTime { get; set; }
      public DateTime? EndTime { get; set; }
      public string CheckInRef { get; set; }
  }
}
