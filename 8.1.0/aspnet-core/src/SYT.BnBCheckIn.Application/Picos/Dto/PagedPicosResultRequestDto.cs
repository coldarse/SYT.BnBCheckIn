using Abp.Application.Services.Dto;
using System;

namespace SYT.BnBCheckIn.Picos
{
    public class PagedPicoResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public string Name { get; set; }
        public Guid? UnitId { get; set; }
        public string Sorting { get; set; }
    }
}
