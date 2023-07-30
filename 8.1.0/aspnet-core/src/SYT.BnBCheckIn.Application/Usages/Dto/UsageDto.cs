using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;

namespace SYT.BnBCheckIn.Usages.Dto
{

  [AutoMap(typeof(Usage))]
  public class UsageDto : EntityDto<Guid>
  {
      public Guid Unit { get; set; }
      public Guid Pico { get; set; }
      public Guid RFID { get; set; }
      public Guid Building { get; set; }
      public DateTime StartTime { get; set; }
      public DateTime EndTime { get; set; }
      public string CheckInRef { get; set; }
  }
}
