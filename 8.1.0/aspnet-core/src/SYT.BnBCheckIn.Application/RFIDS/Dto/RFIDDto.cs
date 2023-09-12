using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;

namespace SYT.BnBCheckIn.RFIDS.Dto
{

    [AutoMap(typeof(RFID))]
    public class RFIDDto : EntityDto<Guid>
    {
        public string Value { get; set; }
        public Guid UnitId { get; set; }
    }

    public class regRFID
    {
        public string Value { get; set; }
        public Guid UnitId { get; set; }
    }

}
