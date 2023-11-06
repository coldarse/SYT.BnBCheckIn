using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace SYT.BnBCheckIn.Usages.Dto
{

	public class DayUsage
	{
		public string name { get; set; }
		public double value { get; set; }
		public string start { get; set; }
		public string end { get; set; }
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
		public double Duration { get; set; }
	}

	public class DayUsageWithUnits
	{
		public string Name { get; set; }
		public string Building { get; set; }
		public List<DayUsage> Series { get; set; }
	}

	public class UnitsWithInfo
	{
		public string Unit { get; set; }
		public List<tempDaysUsage> Infos { get; set; }
    }

	public class DayUsageWithUnitsNotNested
	{
        public string Building { get; set; }
        public string Unit { get; set; }
		public string StartTime { get; set; }
		public string EndTime { get; set; }
        public string Duration { get; set; }
	}

	public class DayUsageWithAndWithoutNested
	{
        public List<DayUsageWithUnits> nested { get; set; }
		public List<DayUsageWithUnitsNotNested> notNested { get; set; }
    }

	public class UsageDataTable
	{
		public PagedResultDto<DayUsageWithUnitsNotNested> pagedNotNested { get; set; }
		public List<DayUsageWithUnits> nested { get; set; }
        public List<DayUsageWithUnitsNotNested> notNested { get; set; }
        public string Duration { get; set; }
	}


}

