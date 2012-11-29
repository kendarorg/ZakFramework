using System;
using System.Collections;
using System.Collections.Generic;

namespace ZakCore.Utils.Commons
{

	public sealed class CommandLineParser
	{
		private readonly string _helpMessage;
		private readonly Dictionary<string, string> _commandLineValues;
		private static Dictionary<string, string> _kvps;


		public static string GetEnv(string envVar)
		{
			if (_kvps.ContainsKey(envVar))
			{
				return _kvps[envVar];
			}
			return null;
		}

		public CommandLineParser(string[] args, string helpMessage)
		{
			//_kvps = kvps == null ? new Dictionary<string, string>() : kvps;
			IDictionary environmentVariables = Environment.GetEnvironmentVariables();
			_kvps = new Dictionary<string, string>();
			foreach (DictionaryEntry de in environmentVariables)
			{
				_kvps.Add((string)de.Key, (string)de.Value);
			}
			_helpMessage = helpMessage;
			_commandLineValues = new Dictionary<string, string>();
			for (int index = 0; index < args.Length; index++)
			{
				var item = args[index];
				if (item.StartsWith("-"))
				{
					_commandLineValues.Add(item.Substring(1), string.Empty);
				}
				if (index < (args.Length - 1))
				{
					var nextItem = args[index + 1];
					if (!nextItem.StartsWith("-"))
					{
						_commandLineValues[item.Substring(1)] = nextItem;
					}
				}
			}
			if (IsSet("help"))
			{
				ShowHelp();
			}
		}

		public string this[string index]
		{
			get
			{
				if (IsSet(index))
					return _commandLineValues[index];
				return null;
			}
			set
			{
				if (IsSet(index))
					_commandLineValues[index] = value;
				else
					_commandLineValues.Add(index, value);
			}
		}

		public bool IsSet(string index)
		{
			return _commandLineValues.ContainsKey(index);
		}

		public bool Has(params string[] vals)
		{
			foreach (var item in vals)
			{
				if (!IsSet(item)) return false;
			}
			return true;
		}

		public bool HasAllOrNone(params string[] vals)
		{
			int setted = 0;
			foreach (var item in vals)
			{
				if (IsSet(item)) setted++;
			}
			if (setted == 0 || setted == vals.Length) return true;
			return false;
		}

		public bool HasOneAndOnlyOne(params string[] vals)
		{
			bool setted = false;
			foreach (var item in vals)
			{
				if (IsSet(item))
				{
					if (setted)
					{
						return false;
					}
					setted = true;
				}
			}
			return setted;
		}

		public void ShowHelp()
		{
			Console.WriteLine(_helpMessage);
			Console.ReadKey();
			Environment.Exit(0);
		}
	}
}