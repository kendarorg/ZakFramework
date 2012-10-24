using System;
using System.Collections.Generic;
using ZakDb.Plugins;
using ZakDb.Repositories;
using ZakDb.Repositories.Utils;
using ZakDb.Utils;

namespace ZakCms.Plugins.Selectors
{
	public class ArticleAuthenticationSelectorPlugin : IExternalParametersPlugin, IArticleAuthenticationSelectorPlugin
	{
		private readonly IExternalParameters _externalParameters;
		private readonly bool _defaultIsAuthenticated;
		private readonly string[] _fields;

		public ArticleAuthenticationSelectorPlugin(IExternalParameters externalParameters, params string[] fields)
		{
			_externalParameters = externalParameters;
			_defaultIsAuthenticated = false;
			_fields = fields;
			_externalParameters["IsAuthenticated"] = _defaultIsAuthenticated;
		}

		public IRepository Repository { get; set; }

		public List<string> UpdatableFields
		{
			get { return new List<string>(); }
		}

		public List<string> SelectableFields
		{
			get { return new List<string>(); }
		}


		private bool OnPreSelect(object source, EventArgs args)
		{
			var qo = ((OnPreSelectEventArgs) args).Query;
			var val = _externalParameters["IsAuthenticated"];
			if (val != null)
			{
				var isAuthenticated = (bool) val;
				if (!isAuthenticated)
				{
					qo.WhereCondition = RepositoryUtils.AddWhereParameter(qo.WhereCondition, string.Format(
						"{0}.{1}=0", Repository.TableName, _fields[0]));
				}
			}
			return true;
		}

		public void RegisterActions()
		{
			Repository.PreSelectHandler += OnPreSelect;
		}


		public void FillFromDb(ZakDataReader reader, object article)
		{
		}

		public void ConvertToDb(object source, Dictionary<string, object> toFill)
		{
		}
	}
}