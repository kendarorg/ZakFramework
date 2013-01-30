using System.Collections.Generic;

namespace ZakDb.Utils
{
	public class DictionaryExternalParameters : IExternalParameters
	{
		private readonly Dictionary<string, object> _defaults;
		private static readonly object _lockObject = new object();
		private static Dictionary<string, object> _session;

		public void ResetSession()
		{
			lock (_lockObject)
			{
				_session = new Dictionary<string, object>();
			}
		}

		public DictionaryExternalParameters(Dictionary<string, object> defaults)
		{
			_defaults = defaults;
			if (_session == null)
			{
				lock (_lockObject)
				{
					_session = new Dictionary<string, object>();
				}
			}
		}

		public T GetAs<T>(string index)
		{
			var toret = this[index];
			if (toret == null)
			{
				return default(T);
			}
			return (T)toret;
		}


		public T GetAs<T>(string index,T defaultValue)
		{
			var toret = this[index];
			return (toret == null) ? defaultValue : (T)toret;
		}

		public object this[string index]
		{
			get
			{
				object toret = null;
				if (!_session.ContainsKey(index))
				{
					if (_defaults.ContainsKey(index))
					{
						toret = _defaults[index];
						_session.Add(index, toret);
					}
				}
				else
				{
					toret = _session[index];
				}
				return toret;
			}
			set
			{
				if (!_session.ContainsKey(index))
				{
					_session.Add(index, value);
				}
				else
				{
					_session[index] = value;
				}
			}
		}
	}
}