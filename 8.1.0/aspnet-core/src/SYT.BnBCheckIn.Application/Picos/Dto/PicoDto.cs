using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;

namespace SYT.BnBCheckIn.Picos.Dto
{

  [AutoMap(typeof(Pico))]
  public class PicoDto : EntityDto<Guid>
  {
      public string Name { get; set; }
      public Guid UnitId { get; set; }
  }
}
