using Abp.Domain.Entities;
using System;

namespace SYT.BnBCheckIn.Units
{

    public class Unit : Entity<Guid>
    {
        public Guid BuildingId { get; set; }
        public string UnitNo { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }
}
