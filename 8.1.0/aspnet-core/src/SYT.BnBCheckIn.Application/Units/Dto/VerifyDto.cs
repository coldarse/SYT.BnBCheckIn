using System;
using System.Collections.Generic;

namespace SYT.BnBCheckIn.Units.Dto
{
	public class VerifyDto
	{
		public bool Validity { get; set; }
		public Guid UsageId { get; set; }
		public List<string> UnitRFIDs { get; set; }
		public string Error { get; set; }

		public VerifyDto()
		{
			Validity = false;
			UsageId = Guid.Empty;
			UnitRFIDs = new List<string>();
			Error = "";
		}
	}
}

