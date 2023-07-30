using Abp.Application.Services.Dto;
using System;

namespace SYT.BnBCheckIn.Buildings
{

  public class PagedBuildingResultRequestDto : PagedResultRequestDto
  {
      public string Keyword { get; set; }
      public string Name { get; set; }
      public string Address { get; set; }
      public string City { get; set; }
      public string State { get; set; }
      public string Postcode { get; set; }
      public string Remark { get; set; }
  }
}
