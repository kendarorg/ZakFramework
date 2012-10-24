using System;
using System.Collections.Generic;
using ZakDb.Plugins;
using ZakDb.Repositories.Queries;

namespace ZakDb.Repositories.Utils
{
	public interface IRepositoryEventArg
	{
	}

	public delegate bool BooleanResultEventHandler(object sender, EventArgs e);

	public class FillFromDbEventArgs : EventArgs, IRepositoryEventArg
	{
		public ZakDataReader Reader { get; set; }
		public object Article { get; set; }

		public FillFromDbEventArgs(ZakDataReader reader, object article)
		{
			Reader = reader;
			Article = article;
			//void FillFromDb(ZakDataReader reader, object article);
		}
	}

	public class ConvertToDbEventArgs : EventArgs, IRepositoryEventArg
	{
		public object Source { get; set; }
		public Dictionary<string, object> ToFill { get; set; }

		public ConvertToDbEventArgs(object source, Dictionary<string, object> toFill)
		{
			Source = source;
			ToFill = toFill;
			//void ConvertToDb(object source, Dictionary<string, object> toFill);
		}
	}

	public class ObjectItemEventArgs : EventArgs, IRepositoryEventArg
	{
		public object Item { get; set; }

		protected ObjectItemEventArgs(object item)
		{
			Item = item;
			//bool OnPreCreate(object item);
		}
	}

	public class Int64ItemEventArgs : EventArgs, IRepositoryEventArg
	{
		public Int64 Id { get; set; }

		protected Int64ItemEventArgs(Int64 id)
		{
			Id = id;
			//bool OnPreCreate(object item);
		}
	}

	public class OnPreCreateEventArgs : ObjectItemEventArgs
	{
		public OnPreCreateEventArgs(object item) : base(item)
		{
		}
	}

	public class OnPostCreateEventArgs : ObjectItemEventArgs
	{
		public OnPostCreateEventArgs(object item) : base(item)
		{
		}
	}

	public class OnPreUpdateEventArgs : ObjectItemEventArgs
	{
		public OnPreUpdateEventArgs(object item) : base(item)
		{
		}
	}

	public class OnPostUpdateEventArgs : ObjectItemEventArgs
	{
		public OnPostUpdateEventArgs(object item) : base(item)
		{
		}
	}

	public class OnPostInstantiateEventArgs : ObjectItemEventArgs
	{
		public OnPostInstantiateEventArgs(object item) : base(item)
		{
		}
	}

	public class OnPreDeleteEventArgs : Int64ItemEventArgs
	{
		public OnPreDeleteEventArgs(Int64 id) : base(id)
		{
		}
	}

	public class OnPostDeleteEventArgs : Int64ItemEventArgs
	{
		public OnPostDeleteEventArgs(Int64 id) : base(id)
		{
		}
	}

	public class OnVerifyEventArgs : EventArgs, IRepositoryEventArg
	{
		public object Item { get; set; }
		public string Operation { get; set; }

		public OnVerifyEventArgs(object item, string operation)
		{
			Item = item;
			Operation = operation;
			//bool OnVerify(object item, string operation);
		}
	}

	public class OnPreSelectEventArgs : EventArgs, IRepositoryEventArg
	{
		public QueryObject Query { get; set; }

		public OnPreSelectEventArgs(QueryObject queryObject)
		{
			Query = queryObject;
			//bool OnVerify(object item, string operation);
		}
	}

	public delegate PluginOutcome ExecuteActionEventHandler(object sender, EventArgs e);

	public class OnExecuteActionEventArgs : EventArgs, IRepositoryEventArg
	{
		public string ActionName { get; set; }
		public object[] Parameters { get; set; }

		public OnExecuteActionEventArgs(string actionName, params object[] parameters)
		{
			ActionName = actionName;
			Parameters = parameters;
			//PluginOutcome ExecuteAction(string actionName, params object[] pars);
		}
	}
}