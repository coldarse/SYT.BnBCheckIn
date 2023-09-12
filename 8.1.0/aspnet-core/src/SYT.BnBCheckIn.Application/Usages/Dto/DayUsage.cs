using System;
using System.Collections.Generic;

namespace SYT.BnBCheckIn.Usages.Dto
{

	public class DayUsage
	{
		public string name { get; set; }
		public float value { get; set; }
	}

	public class tempDaysUsage
	{
		public Guid Id { get; set; }
		public string Unit { get; set; }
		public string Pico { get; set; }
		public string RFID { get; set; }
		public string Building { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public float Duration { get; set; }
	}

	public class DayUsageWithUnits
	{
		public string Unit { get; set; }
		public List<DayUsage> Usages { get; set; }
	}

	public class UnitsWithInfo
	{
		public string Unit { get; set; }
		public List<tempDaysUsage> Infos { get; set; }
    }
}

