using System;
using System.Collections.Generic;

namespace ZakCore.Utils.Commons
{
	public class Crontab
	{
		private readonly bool _realCrontab = true;

		private string ReplaceMonths(string p)
		{
			//TODON Month names
			return p;
		}

		private DateTime _baseline;
		private readonly int _mscron;


		private string ReplaceWeekdays(string p)
		{
			//TODON Weekdays names
			return p;
		}

		private enum DateSection
		{
			Sec = 0,
			Min,
			Hour,
			MonthDay,
			Month,
			WeekDay,
			Year
		};

		private class DateRange
		{
			public DateRange()
			{
				Starred = false;
			}

			public readonly Dictionary<int, int> Values = new Dictionary<int, int>();
			public int Periodicity = -1;
			public DateSection Position;
			public bool Starred;
		}

		private readonly String _configLine;
		private readonly bool _considerSeconds;
		private readonly List<DateRange> _ranges = new List<DateRange>();

		/// <summary>
		/// Format string with spaces between the entries
		///  *     *     *    *     *     *    * command to be executed
		///  -     -     -    -     -     -    -
		///  |     |     |    |     |     |    +----- year
		///  |     |     |    |     |     +----- day of week (0 - 6) (Sunday=0)
		///  |     |     |    |     +------- month (1 - 12)
		///  |     |     |    +--------- day of month (1 - 31)
		///  |     |     +----------- hour (0 - 23)
		///  |     +------------- min (0 - 59)
		///  +------------- sec (0 - 59)
		/// </summary>
		public Crontab(String commandLine, bool considerSeconds = false)
		{
			_error = false;
			_considerSeconds = considerSeconds;
			if (string.IsNullOrEmpty(commandLine))
			{
				throw new ArgumentNullException();
			}

			_configLine =
				commandLine.Replace("\t", " ").Replace("  ", " ").Replace("?", "*").Replace("*/", "0/").Trim().ToUpper();

			if (_configLine.StartsWith("@"))
			{
				_configLine = SpecialCommands(_configLine);
			}

			String[] pars = _configLine.Split(' ');
			if (pars.Length != 7)
			{
				throw new IndexOutOfRangeException();
			}

			for (int i = 0; i < pars.Length; i++)
			{
				pars[i] = pars[i].Trim();
				_ranges.Add(SetupElement(pars[i], i));
			}

			if (pars[1] == "*" && pars[0] == "*")
			{
				_ranges[0].Starred = false;
				_ranges[0].Periodicity = 1;
				_ranges[0].Values.Add(0, 0);
				_ranges[1].Starred = false;
				_ranges[1].Periodicity = 1;
				_ranges[1].Values.Add(0, 0);
			}
		}


		public Crontab(DateTime baseLine, int milliseconds)
		{
			_baseline = baseLine;
			_mscron = milliseconds;
			_realCrontab = false;
			_error = false;
		}

		private string SpecialCommands(string configLine)
		{
			switch (configLine.ToLower())
			{
				case ("@yearly"):
				case ("@annually"):
					return "0 0 0 1 1 * *";
				case ("@monthly"):
					return "0 0 0 1 * * *";
				case ("@weekly"):
					return "0 0 0 * * 0 *";
				case ("@daily"):
					return "0 0 0 * * * *";
				case ("@hourly"):
					return "0 0 * * * * *";
				default:
					return configLine;
			}
		}

		private DateRange SetupElement(string p, int i)
		{
			var d = new DateRange {Position = (DateSection) i};
			if (d.Position == DateSection.Month)
			{
				p = ReplaceMonths(p);
			}
			else if (d.Position == DateSection.WeekDay)
			{
				p = ReplaceWeekdays(p);
			}

			if (p == "*")
			{
				d.Starred = true;
				return d;
			}
			string[] tmp;
			if (p.IndexOf("/", StringComparison.Ordinal) > 0)
			{
				tmp = p.Split('/');
				d.Periodicity = int.Parse(tmp[1]);
				p = tmp[0];
			}
			if (p.IndexOf("*", StringComparison.Ordinal) > 0)
			{
				d.Starred = true;
				return d;
			}
			tmp = p.Split(',');
			foreach (string s in tmp)
			{
				if (s.IndexOf("-", StringComparison.Ordinal) > 0)
				{
					string[] tmp2 = s.Split('-');
					int start = int.Parse(tmp2[0]);
					int end = int.Parse(tmp2[1]);
					if (!d.Values.ContainsKey(start))
					{
						d.Values.Add(start, end);
					}
				}
				else
				{
					int val = int.Parse(s);
					d.Values.Add(val, val);
				}
			}

			return d;
		}

		public bool MayRunAt(DateTime dt)
		{
			if (!_realCrontab)
			{
				Int64 msDelta = (dt.Ticks - _baseline.Ticks)/TimeSpan.TicksPerMillisecond;
				return (msDelta%_mscron == 0);
			}
			int allOk = 0;
			for (int i = 0; i < _ranges.Count; i++)
			{
				DateRange d = _ranges[i];
				switch (d.Position)
				{
					case (DateSection.Sec):
						if (_considerSeconds)
						{
							allOk += CheckValue(dt.Second, d);
						}
						else
						{
							allOk++;
						}
						break;
					case (DateSection.Year):
						allOk += CheckValue(dt.Year, d);
						break;
					case (DateSection.WeekDay):
						allOk += CheckValue((int) dt.DayOfWeek, d);
						break;
					case (DateSection.Month):
						allOk += CheckValue(dt.Month, d); ///////
						break;
					case (DateSection.MonthDay):
						allOk += CheckValue(dt.Day, d);
						break;
					case (DateSection.Hour):
						allOk += CheckValue(dt.Hour, d);
						break;
					case (DateSection.Min):
						allOk += CheckValue(dt.Minute, d);
						break;
					default:
						return false;
				}
			}
			return allOk == 7;
		}

		private int CheckValue(int p, DateRange d)
		{
			// "*" everything is ok
			if (d.Starred && d.Values.Count == 0) return 1;
			if (d.Values.Count == 1 && d.Periodicity > 0)
			{
				if (d.Values.ContainsKey(0) && d.Values[0] == 0)
				{
					if (p%d.Periodicity == 0) return 1;
				}
			}

			// 1-2,3,4
			foreach (var e in d.Values)
			{
				if (e.Key == e.Value && e.Key == p)
				{
					// 3,4
					return 1;
				}

				if (e.Key <= p && p <= e.Value)
				{
					// 1-2
					if (d.Periodicity == -1) return 1;
					// 1-10/3
					if ((p - e.Key)%d.Periodicity == 0) return 1;
				}
			}
			return 0;
		}

		private bool _error;
		protected const int SECONDSPERMINUTE = 60;
		protected const int MINUTESPERHOUR = 60;
		protected const int HOURESPERDAY = 24;
		protected const int DAYSPERWEEK = 7;
		protected const int MONTHSPERYEAR = 12;
		protected const int DAYSPERMONTH = 31;

		public DateTime Next(DateTime? srcTime = null)
		{
			DateTime dt = srcTime != null ? srcTime.Value : DateTime.Now;
			if (!_realCrontab)
			{
				Int64 msDelta = (dt.Ticks - _baseline.Ticks)/TimeSpan.TicksPerMillisecond;
				if (msDelta%_mscron == 0) return dt;
				dt = dt + TimeSpan.FromMilliseconds(_mscron - (msDelta%_mscron));
				return dt;
			}
			_error = false;

			var vals = new[] {dt.Second, dt.Minute, dt.Hour, dt.Day, dt.Month + 1, (int) dt.DayOfWeek, dt.Year};
			// ReSharper disable TooWideLocalVariableScope
			// ReSharper disable RedundantAssignment
			int prev = 0;
			int delta = 0;
			// ReSharper restore RedundantAssignment
			// ReSharper restore TooWideLocalVariableScope
			bool doRestart = true;
			while (doRestart)
			{
				vals = new[] {dt.Second, dt.Minute, dt.Hour, dt.Day, dt.Month + 1, (int) dt.DayOfWeek, dt.Year};
				doRestart = false;
				//for (int i = 0; i < _ranges.Count && doRestart==false; i++)
				for (int i = (_ranges.Count - 1); i >= 0 && doRestart == false; i--)
				{
					DateRange d = _ranges[i];
					prev = vals[i];
					delta = 0;
					switch (d.Position)
					{
						case (DateSection.Year):
							{
								vals[i] = GetNearest(vals[i], d, 2100);
								delta = vals[i] - prev;
								if (delta > 0)
								{
									dt = new DateTime(vals[6], vals[4] - 1, vals[3], vals[2], vals[1], vals[0]);
									vals = new[] {0, 0, 0, 1, 1, (int) dt.DayOfWeek, dt.Year};
									dt = new DateTime(vals[6], vals[4] - 1, vals[3], vals[2], vals[1], vals[0]);
								}
								vals = new[] {dt.Second, dt.Minute, dt.Hour, dt.Day, dt.Month + 1, (int) dt.DayOfWeek, dt.Year};
							}
							break;
						case (DateSection.WeekDay):
							{
								vals[i] = GetNearest(vals[i], d, DAYSPERWEEK);
								delta = vals[i] - prev;
								if (delta > 0)
								{
									dt += TimeSpan.FromDays(delta);
									vals = new[] {0, 0, 0, dt.Day, dt.Month + 1, (int) dt.DayOfWeek, dt.Year};
									dt = new DateTime(vals[6], vals[4] - 1, vals[3], vals[2], vals[1], vals[0]);
								}
								vals = new[] {dt.Second, dt.Minute, dt.Hour, dt.Day, dt.Month + 1, (int) dt.DayOfWeek, dt.Year};
							}
							break;
						case (DateSection.Month):
							{
								vals[i] = GetNearest(vals[i], d, MONTHSPERYEAR);

								if (vals[i] > 12)
								{
									vals[6] += vals[i]/12;
									vals[i] = vals[i]%12 + 1;
									delta++;
								}
								dt = new DateTime(vals[6], vals[4] - 1, vals[3], vals[2], vals[1], vals[0]);
								if (delta > 0)
								{
									vals = new[] {0, 0, 0, 1, dt.Month + 1, (int) dt.DayOfWeek, dt.Year};
									dt = new DateTime(vals[6], vals[4] - 1, vals[3], vals[2], vals[1], vals[0]);
								}
								vals = new[] {dt.Second, dt.Minute, dt.Hour, dt.Day, dt.Month + 1, (int) dt.DayOfWeek, dt.Year};
							}
							break;
						case (DateSection.MonthDay):
							{
								vals[i] = GetNearest(vals[i], d, DAYSPERMONTH);
								delta = vals[i] - prev;
								if (delta > 0)
								{
									dt += TimeSpan.FromDays(delta);
									vals = new[] {0, 0, 0, dt.Day, dt.Month + 1, (int) dt.DayOfWeek, dt.Year};
									dt = new DateTime(vals[6], vals[4] - 1, vals[3], vals[2], vals[1], vals[0]);
								}
								vals = new[] {dt.Second, dt.Minute, dt.Hour, dt.Day, dt.Month + 1, (int) dt.DayOfWeek, dt.Year};
							}
							break;
						case (DateSection.Hour):
							{
								vals[i] = GetNearest(vals[i], d, HOURESPERDAY);
								delta = vals[i] - prev;
								if (delta > 0)
								{
									dt += TimeSpan.FromHours(delta);
									vals = new[] {0, 0, dt.Hour, dt.Day, dt.Month + 1, (int) dt.DayOfWeek, dt.Year};
									dt = new DateTime(vals[6], vals[4] - 1, vals[3], vals[2], vals[1], vals[0]);
								}
								vals = new[] {dt.Second, dt.Minute, dt.Hour, dt.Day, dt.Month + 1, (int) dt.DayOfWeek, dt.Year};
							}
							break;
						case (DateSection.Min):
							{
								vals[i] = GetNearest(vals[i], d, MINUTESPERHOUR);
								delta = vals[i] - prev;
								if (delta > 0)
								{
									dt += TimeSpan.FromMinutes(delta);
								}
								vals = new[] {0, dt.Minute, dt.Hour, dt.Day, dt.Month + 1, (int) dt.DayOfWeek, dt.Year};
							}
							break;
						case (DateSection.Sec):
							if (_considerSeconds)
							{
								vals[i] = GetNearest(vals[i], d, SECONDSPERMINUTE);
								delta = vals[i] - prev;
								if (delta > 0)
								{
									dt += TimeSpan.FromSeconds(delta);
								}
								vals = new[] {dt.Second, dt.Minute, dt.Hour, dt.Day, dt.Month + 1, (int) dt.DayOfWeek, dt.Year};
							}
							break;
					}
					doRestart = (delta > 0);
				}
			}

			if (_error)
			{
				_error = false;
				return DateTime.MaxValue;
			}
			return new DateTime(vals[6], vals[4] - 1, vals[3], vals[2], vals[1], vals[0]);
		}

		private int GetNearest(int p, DateRange d, int max)
		{
			while (p < max)
			{
				if (CheckValue(p, d) == 1) return p;
				p++;
			}
			p = 0;
			while (p < max)
			{
				if (CheckValue(p, d) == 1) return p + max;
				p++;
			}
			_error = true;
			return -1;
		}
	}
}