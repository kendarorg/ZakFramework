using System;
using System.Collections;
using System.Collections.Generic;

namespace ZakCore.Utils.Commons
{

	public class CommandLineParser
	{
		private readonly string _helpMessage;
		private readonly ICommandLineParserExitBehaviour _exitBehaviour;
		private readonly Dictionary<string, string> _commandLineValues;
		private static Dictionary<string, string> _kvps;

		public string Help {get { return _helpMessage; }}

		private static object _lockObject = new object();

		public static string GetEnv(string envVar)
		{
			if (_kvps == null)
			{
				lock (_lockObject)
				{
					IDictionary environmentVariables = Environment.GetEnvironmentVariables();
					_kvps = new Dictionary<string, string>();
					foreach (DictionaryEntry de in environmentVariables)
					{
						_kvps.Add((string)de.Key, (string)de.Value);
					}
				}
			}
			if (_kvps.ContainsKey(envVar))
			{
				return _kvps[envVar];
			}
			return null;
		}

		public static void SetEnv(string envVar,string val)
		{
			if (_kvps == null)
			{
				lock (_lockObject)
				{
					IDictionary environmentVariables = Environment.GetEnvironmentVariables();
					_kvps = new Dictionary<string, string>();
					foreach (DictionaryEntry de in environmentVariables)
					{
						_kvps.Add((string)de.Key, (string)de.Value);
					}
				}
			}
			if (_kvps.ContainsKey(envVar))
			{
				_kvps[envVar] = val;
			}
			else
			{
				_kvps.Add(envVar, val);
			}
		}

		public CommandLineParser(string[] args, string helpMessage,ICommandLineParserExitBehaviour exitBehaviour=null)
		{
			//_kvps = kvps == null ? new Dictionary<string, string>() : kvps;
			IDictionary environmentVariables = Environment.GetEnvironmentVariables();
			_kvps = new Dictionary<string, string>();
			foreach (DictionaryEntry de in environmentVariables)
			{
				_kvps.Add((string)de.Key, (string)de.Value);
			}
			_helpMessage = helpMessage;
			_exitBehaviour = exitBehaviour;
			_commandLineValues = new Dictionary<string, string>();
			for (int index = 0; index < args.Length; index++)
			{
				var item = args[index];
				if (item.StartsWith("-"))
				{
					_commandLineValues.Add(item.Substring(1).ToLower(), string.Empty);
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
				index = index.ToLower();
				if (IsSet(index))
					return _commandLineValues[index];
				return null;
			}
			set
			{
				index = index.ToLower();
				if (IsSet(index))
					_commandLineValues[index] = value;
				else
					_commandLineValues.Add(index, value);
			}
		}

		public bool IsSet(string index)
		{
			index = index.ToLower();
			return _commandLineValues.ContainsKey(index);
		}

		public bool Has(params string[] vals)
		{
			foreach (var item in vals)
			{
				var index = item.ToLower();
				if (!IsSet(index)) return false;
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
			if(_exitBehaviour!=null) _exitBehaviour.HandleApplicationExit();
		}
	}
}