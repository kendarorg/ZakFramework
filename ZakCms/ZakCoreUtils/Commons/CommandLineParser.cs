using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZakCore.Utils.Commons
{
	/// <summary>
	/// Class to manage command line arguments
	/// </summary>
	public abstract class CommandLineParser
	{
		/// <summary>
		/// Return true if can continue with elaboration
		/// </summary>
		/// <param name="pars"></param>
		/// <returns></returns>
		public delegate object ArgumentHandler(params object[] pars);

		public static ArgumentHandler MakeArgumentHandler(object target, MethodInfo method)
		{
			return par => method.Invoke(target, par);
		}

		private readonly List<CommandDescriptor> _availableCommands = new List<CommandDescriptor>();
		//private Dictionary<>

		private static CommandDescriptor SetupParam(List<CommandDescriptor> command, object instance, string function,
		                                            string key, string descritpion, params CommandArg[] pars)
		{
			var cd = new CommandDescriptor
				{
					Command = key,
					Description = descritpion,
					Function = function,
					Target = instance,
					Arguments = new List<CommandArg>(pars)
				};
			command.Add(cd);
			return cd;
		}

		public CommandDescriptor AddParam(object instance, string function, string key, string descritpion,
		                                  params CommandArg[] pars)
		{
			CommandDescriptor cd = SetupParam(_availableCommands, instance, function, key, descritpion, pars);
			return cd;
		}

		public class CommandArg
		{
			public CommandArg(string ot, string desc)
			{
				ObjectType = ot.ToUpper().Trim();
				Description = desc.Trim();
			}

			public String ObjectType;
			public String Description;
			public String Shortcut;
		}

		public class CommandDescriptor
		{
			internal string Command;
			internal string Description;
			internal string Function;
			internal object Target;
			internal List<CommandArg> Arguments = new List<CommandArg>();
			internal List<CommandDescriptor> Dependant = new List<CommandDescriptor>();

			public CommandDescriptor AddParam(object instance, string function, string key, string descritpion,
			                                  params CommandArg[] pars)
			{
				return SetupParam(Dependant, instance, function, key, descritpion, pars);
			}
		}

		public string Initialize()
		{
			return Initialize(Environment.GetCommandLineArgs());
		}

		public string Initialize(string[] args)
		{
			throw new NotImplementedException();
		}

#if NONE
		public class CommandResult
		{
			public String Command;
			public Int64 CmdId;
			public List<object> Results = new List<object>();
			internal CommandSpecification Specfication;

		}

		internal class CommArgument
		{
			public string PType;
			public string PDescription;
		}

		internal class CommandSpecification
		{
			internal String Command;
			internal String Help;
			internal List<CommArgument> AllowedArgs = new List<CommArgument>();
			internal List<CommandSpecification> DependantCommands = new List<CommandSpecification>();
			internal string Delegate;
			internal object DelegateObject;
			internal CommandLineParser Parser;
			public CommandSpecification AddArgument(object target, string handler, string cmd, string help, params string[] pars)
			{
				return Parser.AddArgument(this, target, handler, cmd, help, pars);
			}
		}

		private List<CommandSpecification> _availableCommands = new List<CommandSpecification>();
		private List<CommandResult> _commandResult = new List<CommandResult>();
		private string[] _arguments;

		protected abstract string BaseHelp { get; }

		public CommandLineParser()
		{
			AddArgument(this, "HandleHelp", "-help", "Program Help");
			AddArgument(this, "HandleIni", "-iniFile", "Loads an ini file with all the parameters (all tab sv)", "Ini file path", INI_TYPE_EXISTING_FILE);
			InitializeArguments();
			_arguments = Environment.GetCommandLineArgs();
		}

		public CommandLineParser(string[] args)
		{
			AddArgument(this, "HandleHelp", "-help", "Program Help");
			AddArgument(this, "HandleIni", "-iniFile", "Loads an ini file with all the parameters (all tab sv)", "Ini file path", INI_TYPE_EXISTING_FILE);
			InitializeArguments();
			_arguments = args;
		}

		private string _lastHelpRequired = string.Empty;

		public void Parse()
		{
			Parse(_arguments);
		}

		private void LoadIniFile(CommandResult commandResult)
		{
			int lineCount = 0;
			string fileName = (string)commandResult.Results[0];
			string[] lines = File.ReadAllLines(fileName);
			foreach (string res in lines)
			{
				string es = res.Trim();
				if (!string.IsNullOrEmpty(es) && !es.StartsWith("#"))
				{
					try
					{
						string[] spl = es.Split('\t');

						string cmd = spl[0].ToUpper().Trim();

						List<string> cmdPars = new List<string>();
						for (int i = 1; i < spl.Length; i++)
						{
							cmdPars.Add(spl[i]);
						}
						SetupCommand(cmd, cmdPars);

					}
					catch (Exception)
					{
						Console.WriteLine(string.Format("Error reading file {0} line {1}.\n", fileName, lineCount));
					}
				}
			}

		}


		/// <summary>
		/// Setup the values allowed as command line params
		/// </summary>
		protected abstract void InitializeArguments();


		protected CommandSpecification AddArgument(object target, string handler, string cmd, string help, params string[] pars)
		{
			return AddArgument(null, target, handler, cmd, help, pars);
		}

		internal CommandSpecification AddArgument(CommandSpecification commandSpecification, object target, string handler, string cmd, string help, params string[] pars)
		{
			cmd = cmd.ToUpper();
			if (!cmd.StartsWith("-"))
			{
				cmd = "-" + cmd;
			}
			if (pars.Length % 2 != 0)
			{
				throw new IndexOutOfRangeException();
			}
			CommandSpecification cs = new CommandSpecification()
			{
				Command = cmd,
				Help = help,
				Delegate = handler,
				DelegateObject = target
			};
			if (commandSpecification == null)
			{
				_availableCommands.Add(cs);
			}
			else
			{
				commandSpecification.DependantCommands.Add(cs);
			}
			for (int i = 0; i < pars.Length; i += 2)
			{
				cs.AllowedArgs.Add(new CommArgument() { PDescription = pars[i].Trim(), PType = pars[i + 1].ToUpper().Trim() });
			}
			return cs;
		}

		public bool ContainsResult(string command)
		{
			command = command.ToUpper();
			if (!command.StartsWith("-"))
			{
				command = "-" + command;
			}
			for (int i = 0; i < _commandResult.Count; i++)
			{
				if (_commandResult[i].Command.CompareTo(command) == 0)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasMultipleResults(string command)
		{
			command = command.ToUpper();
			if (!command.StartsWith("-"))
			{
				command = "-" + command;
			}
			int cnt = 0;
			for (int i = 0; i < _commandResult.Count; i++)
			{
				if (_commandResult[i].Command.CompareTo(command) == 0)
				{
					cnt++;
				}
			}
			return cnt > 1;
		}

		public List<CommandResult> GetResults(string command)
		{
			List<CommandResult> toret = new List<CommandResult>();
			command = command.ToUpper();
			if (!command.StartsWith("-"))
			{
				command = "-" + command;
			}
			for (int i = 0; i < _commandResult.Count; i++)
			{
				if (_commandResult[i].Command.CompareTo(command) == 0)
				{
					toret.Add(_commandResult[i]);
				}
			}
			return toret;
		}

		public CommandResult GetResult(string command, int idx = 0)
		{

			command = command.ToUpper();
			if (!command.StartsWith("-"))
			{
				command = "-" + command;
			}
			if (HasMultipleResults(command))
			{
				throw new IndexOutOfRangeException();
			}
			if (!ContainsResult(command))
			{
				return null;
			}
			return GetResults(command)[idx];
		}

		private String _lastKey;

		private bool Parse(string[] pars)
		{
			for (int i = 0; i < pars.Length; i++)
			{
				string cmd = pars[i].ToUpper().Trim();
				//Get the parameters of the command
				List<String> cmdPars = new List<string>();
				for (i++; i < pars.Length; i++)
				{
					if (pars[i].StartsWith("-"))
					{
						i--;
						break;
					}
					cmdPars.Add(pars[i]);
				}
				if (!SetupCommand(cmd, cmdPars))
				{
					return false;
				}
			}
			return true;
		}

		private bool SetupCommand(string cmd, List<string> cmdPars)
		{
			_lastKey = cmd;
			//Find all arguments
			for (int i = 0; i < _availableCommands.Count; i++)
			{
				CommandSpecification ci = _availableCommands[i];
				if (ci.AllowedArgs.Count == cmdPars.Count && ci.Command.CompareTo(cmd) == 0)
				{
					if (IsAllowed(ci, cmdPars))
					{
						ArgumentHandler f2 = MakeArgumentHandler(
								ci.DelegateObject,
								ci.DelegateObject.GetType().GetMethod(ci.Delegate));
						f2(cmdPars.ToArray());
						return true;
					}
				}
			}
			return false;
		}

		private bool IsAllowed(CommandSpecification ci, List<string> cmdPars)
		{
			try
			{
				CommandResult cr = new CommandResult();
				for (int i = 0; i < ci.AllowedArgs.Count; i++)
				{
					CommArgument ca = ci.AllowedArgs[i];
					object val = ConvertToVal(ca, cmdPars[i]);
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private object ConvertToVal(CommArgument ca, string v)
		{
			object toret = null;
			switch (ca.PType)
			{
				case (INI_TYPE_INT):
					toret = int.Parse(v);
					break;
				case (INI_TYPE_GUID):
					{
						Guid outguid = Guid.Empty;
						if (!Guid.TryParse(v, out outguid))
						{
							Int128Value val = Int128Value.Parse(v);
							outguid = val.ConvertToGuid();
						}
					}
					break;
				case (INI_TYPE_DATE):
					toret = DateTime.ParseExact(v, "yyyyMMdd-hhmmtt", null);
					break;
				case (INI_TYPE_INTERNET_ADDRESS):
					{
						IPAddress ipa = null;
						if (!IPAddress.TryParse(v, out ipa))
						{
							IPHostEntry ihe = Dns.GetHostEntry(v);
							toret = ihe.AddressList[0];
						}
					}
					break;
				case (INI_TYPE_EXISTING_FILE):
					if (!File.Exists(v))
					{
						throw new ArgumentNullException();
					}
					toret = v;
					break;
				case (INI_TYPE_DIR):
				case (INI_TYPE_FILE):
					toret = v;
					break;
				case (INI_TYPE_EXISTING_DIR):
					if (!Directory.Exists(v))
					{
						throw new ArgumentNullException();
					}
					toret = v;
					break;
				case (INI_TYPE_STRING):
				default:
					toret = v.ToString();
					break;
			}

			return toret;
		}

		private string BuildHelp()
		{
			string toret = BaseHelp + "\n";
			foreach (var e in _availableCommands)
			{
				toret += BuildHelp(e);
			}
			return toret;
		}

		private string BuildHelp(CommandSpecification cs)
		{
			string bas = "  ";
			string toret = string.Empty;
			toret += bas + cs.Command.ToLower() + "\n";
			toret += bas + " " + cs.Help + "\n";
			for (int i = 0; i < cs.AllowedArgs.Count; i++)
			{
				CommArgument ar = cs.AllowedArgs[i];
				String typeHelp = TypeHelp(ar);
				if (string.IsNullOrEmpty(typeHelp))
				{
					toret += bas + " " + " " + ar.PDescription + "\r\n";
				}
				else
				{
					toret += bas + " " + " " + ar.PDescription + " ( " + typeHelp + ")\r\n";
				}
			}
			return toret;
		}

		public const string INI_TYPE_INT = "INT";
		public const string INI_TYPE_DATE = "DATE";
		public const string INI_TYPE_INTERNET_ADDRESS = "INTERNETADDRESS";
		public const string INI_TYPE_INT128 = "GUID";
		public const string INI_TYPE_GUID = "GUID";
		public const string INI_TYPE_EXISTING_FILE = "PATHFILEEX";
		public const string INI_TYPE_FILE = "PATHFILENEX";
		public const string INI_TYPE_EXISTING_DIR = "PATHDIREX";
		public const string INI_TYPE_DIR = "PATHDIRNEX";
		public const string INI_TYPE_STRING = "STRING";

		private string TypeHelp(CommArgument ca)
		{
			switch (ca.PType)
			{
				case (INI_TYPE_INT):
					return "Integer value";
				case (INI_TYPE_DATE):
					return "DateTime value, yyyyMMdd-hhmmtt";
				case (INI_TYPE_INTERNET_ADDRESS):
					return "Internet address or DNS name";
				case (INI_TYPE_GUID):
					return "Standard windows Guid or Unsigned int128 value";
				case (INI_TYPE_EXISTING_FILE):
					return "Existing file path";
				case (INI_TYPE_DIR):
					return "Directory path";
				case (INI_TYPE_FILE):
					return "File path";
				case (INI_TYPE_EXISTING_DIR):
					return "Existing directory path";
				case (INI_TYPE_STRING):
				default:
					return "Generic value";
			}
		}

		public bool HandleHelp(CommandResult commandResult, params object[] pars)
		{
			Console.Write(BuildHelp());
			return false;
		}


		public bool HandleIni(CommandResult commandResult, params object[] pars)
		{
			LoadIniFile(commandResult);

			return false;
		}
#endif
	}
}