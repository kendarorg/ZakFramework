using System.Collections.Generic;
using ZakDb.Utils;

namespace ZakCmsSetup.Utils
{
	public class DictionaryExternalParameters : IExternalParameters
	{
		private readonly Dictionary<string, object> _defaults;
		private static readonly object _lockObject = new object();
		private static Dictionary<string, object> _session;

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

		public object this[string index]
		{
			get
			{
				if (!_session.ContainsKey(index) && _defaults.ContainsKey(index))
				{
					_session.Add(index, _defaults[index]);
				}
				if (!_session.ContainsKey(index))
				{
					return null;
				}
				return _session[index];
			}
			set
			{
				if (_session.ContainsKey(index))
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