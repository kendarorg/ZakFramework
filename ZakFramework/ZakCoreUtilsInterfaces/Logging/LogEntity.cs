using System;

// ReSharper disable CoVariantArrayConversion

namespace ZakCore.Utils.Logging
{
	public class LogEntity
	{
		public ushort Level;
		public string CallerId;
		public string FormatParameter;
		public string[] Parameters;
		public DateTime Timestamp;

		public override string ToString()
		{
			if (Parameters != null && Parameters.Length > 0)
			{
				FormatParameter = string.Format(FormatParameter, Parameters);
			}
			FormatParameter = FormatParameter.Replace("\n", "\t\n");


			return string.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}:{5:00} ({6}-{7}) {8}",
			                     Timestamp.Year,
			                     Timestamp.Month,
			                     Timestamp.Day,
			                     Timestamp.Hour,
			                     Timestamp.Minute,
			                     Timestamp.Second,
			                     Level,
			                     CallerId.PadLeft(16, ' '),
			                     FormatParameter
				);
		}
	}
}

// ReSharper restore CoVariantArrayConversion