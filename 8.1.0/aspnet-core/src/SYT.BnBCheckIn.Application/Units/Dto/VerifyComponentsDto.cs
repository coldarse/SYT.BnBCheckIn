using System;
namespace SYT.BnBCheckIn.Units.Dto
{
	public class VerifyComponentsDto
	{
		public Guid picoId { get; set; }
		public string RFID { get; set; }
	}

    public class VerifyComponentsStringDto
    {
        public string picoId { get; set; }
        public string RFID { get; set; }
    }

}

