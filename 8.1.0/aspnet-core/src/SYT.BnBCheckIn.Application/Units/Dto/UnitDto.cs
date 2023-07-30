using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;

namespace SYT.BnBCheckIn.Units.Dto
{

    [AutoMap(typeof(Unit))]
    public class UnitDto : EntityDto<Guid>
    {
        public Guid BuildingId { get; set; }
        public string UnitNo { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }
}
