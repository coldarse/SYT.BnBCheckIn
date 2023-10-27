using System;
using Abp.Application.Services.Dto;

namespace SYT.BnBCheckIn.Usages.Dto
{
	public class PagedUpdatedUsageResultRequestDto : PagedResultRequestDto
    {
        public string Unit { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}

