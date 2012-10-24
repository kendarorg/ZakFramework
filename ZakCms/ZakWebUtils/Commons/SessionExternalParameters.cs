using System.Collections.Generic;
using System.Web;
using ZakDb.Utils;

namespace ZakWeb.Utils.Commons
{
	public class SessionExternalParameters : IExternalParameters
	{
		private readonly Dictionary<string, object> _defaults;

		public SessionExternalParameters(Dictionary<string, object> defaults)
		{
			_defaults = defaults;
		}

		public object this[string index]
		{
			get
			{
				if (HttpContext.Current.Session[index] == null && _defaults.ContainsKey(index))
				{
					HttpContext.Current.Session[index] = _defaults[index];
				}
				return HttpContext.Current.Session[index];
			}
			set { if (HttpContext.Current != null && HttpContext.Current.Session != null) HttpContext.Current.Session[index] = value; }
		}
	}
}