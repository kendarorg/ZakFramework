using System;
using System.Collections.Generic;
using ZakCms.Models.Joins;
using ZakDb.Plugins;
using ZakDb.Repositories;
using ZakDb.Repositories.Utils;
using ZakDb.Utils;

namespace ZakCms.Plugins.Selectors
{
	public class NumericLanguageSelectorPlugin : IExternalParametersPlugin, ILanguageSelectorPlugin
	{
		private readonly IExternalParameters _externalParameters;
		private readonly Int64 _defaultLanguageId;
		private readonly string[] _fields;

		public NumericLanguageSelectorPlugin(IExternalParameters externalParameters, Int64 defaultLanguageId,
		                                     params string[] fields)
		{
			_externalParameters = externalParameters;
			_defaultLanguageId = defaultLanguageId;
			_fields = fields;
			_externalParameters["LanguageId"] = _defaultLanguageId;
		}

		private bool OnPostInstantiate(object sender, EventArgs eventArgs)
		{
			var ea = (OnPostInstantiateEventArgs) eventArgs;
			object item = ea.Item;
			var ob = item as IJoinedLanguageModel;
			if (ob != null)
			{
				var val = _externalParameters["LanguageId"];
				if (val != null) ob.Language.Id = (Int64) val;
			}
			return true;
		}

		public IRepository Repository { get; set; }

		public List<string> UpdatableFields
		{
			get { return new List<string>(); }
		}

		private bool OnVerify(object sender, EventArgs eventArgs)
		{
			var ea = (OnVerifyEventArgs) eventArgs;
			object item = ea.Item;
			var ob = item as IJoinedLanguageModel;
			if (ob != null)
			{
				var val = _externalParameters["LanguageId"];
				if (val != null)
				{
					return ob.Language.Id == (Int64) val;
				}
			}
			return true;
		}

		public List<string> SelectableFields
		{
			get { return new List<string>(); }
		}


		private bool OnPreSelect(object source, EventArgs args)
		{
			var qo = ((OnPreSelectEventArgs) args).Query;
			var val = _externalParameters["LanguageId"];
			if (val != null)
			{
				qo.WhereCondition = RepositoryUtils.AddWhereParameter(qo.WhereCondition, string.Format("{0}={1}", _fields[0], val));
			}
			return true;
		}

		public void RegisterActions()
		{
			Repository.VerifyHandler += OnVerify;
			Repository.PostInstantiateHandler += OnPostInstantiate;
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