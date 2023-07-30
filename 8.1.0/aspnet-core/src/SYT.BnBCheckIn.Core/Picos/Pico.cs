using Abp.Domain.Entities;
using System;

namespace SYT.BnBCheckIn.Picos
{

  public class Pico : Entity<Guid>
  {
      public string Name { get; set; }
      public Guid UnitId { get; set; }
  }
}
