using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;

namespace SYT.BnBCheckIn.Usages.Dto
{

  [AutoMap(typeof(Usage))]
  public class UsageDto : EntityDto<Guid>
  {
      public string Unit { get; set; }
      public string Pico { get; set; }
      public string RFID { get; set; }
      public string Building { get; set; }
      public DateTime StartTime { get; set; }
      public DateTime EndTime { get; set; }
      public string CheckInRef { get; set; }
  }
}
