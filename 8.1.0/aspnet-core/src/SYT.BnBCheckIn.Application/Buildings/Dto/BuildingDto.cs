using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;

namespace SYT.BnBCheckIn.Buildings.Dto
{

  [AutoMap(typeof(Building))]
  public class BuildingDto : EntityDto<Guid>
  {
      public string Name { get; set; }
      public string Address { get; set; }
      public string City { get; set; }
      public string State { get; set; }
      public string Postcode { get; set; }
      public string Remark { get; set; }
  }
}
