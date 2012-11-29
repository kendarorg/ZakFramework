using System;

namespace ZakCore.Utils.Logging
{
	public static class LoggerFormatter
	{
		public static string Format(DateTime dateTime,LogLevels level,string toWrite)
		{
			var levelString = level.ToString().Substring(3).ToUpper();
			return string.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}:{5:00} ({6}) {7}",
													 dateTime.Year,
													 dateTime.Month,
													 dateTime.Day,
													 dateTime.Hour,
													 dateTime.Minute,
													 dateTime.Second,
													 levelString,
													 toWrite
				);
		}
	}
}
