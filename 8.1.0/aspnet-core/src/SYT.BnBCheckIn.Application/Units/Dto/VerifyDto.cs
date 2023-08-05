using System;
namespace SYT.BnBCheckIn.Units.Dto
{
	public class VerifyDto
	{
		public bool Validity { get; set; }
		public Guid UsageId { get; set; }

		public VerifyDto()
		{
			Validity = false;
			UsageId = Guid.Empty;
		}
	}
}

