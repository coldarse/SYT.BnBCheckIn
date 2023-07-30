using Abp.Domain.Entities;
using System;

namespace SYT.BnBCheckIn.Usages
{

  public class Usage : Entity<Guid>
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
