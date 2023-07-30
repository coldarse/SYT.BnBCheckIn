using Abp.Domain.Entities;
using System;

namespace SYT.BnBCheckIn.RFIDS
{

  public class RFID : Entity<Guid>
  {
      public string Value { get; set; }
      public Guid UnitId { get; set; }
  }
}
