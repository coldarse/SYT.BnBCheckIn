using Abp.Application.Services.Dto;
using System;

namespace SYT.BnBCheckIn.Units
{

    public class PagedUnitResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public Guid BuildingId { get; set; }
        public string UnitNo { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }
}
