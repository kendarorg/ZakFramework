using System;
using System.Collections.Generic;
using ZakCms.Models.AdditionalBehaviours;
using ZakDb.Repositories;
using ZakDb.Repositories.Queries;
using ZakDb.Repositories.Utils;

namespace ZakCms.Plugins.AdditionalBehaviours
{
	public class ElementTimestampPlugin : IElementTimestampPlugin
	{
		public IRepository Repository { get; set; }

		public List<string> UpdatableFields
		{
			get { return new List<string>(); }
		}

		public void FillFromDb(ZakDataReader reader, object item)
		{
			((IElementTimestampModel) item).UpdateTime = (DateTime) reader["UpdateTime"];
			((IElementTimestampModel) item).CreateTime = (DateTime) reader["CreateTime"];
		}

		public List<string> SelectableFields
		{
			get { return new List<string> {"UpdateTime", "CreateTime"}; }
		}

		private bool OnPreSelect(object source, EventArgs args)
		{
			var qo = ((OnPreSelectEventArgs) args).Query;
			if (qo.Action == QueryAction.Update)
			{
				qo.QueryBody = RepositoryUtils.AddSetParameter(qo.QueryBody,
				                                               string.Format("{0}.UpdateTime=GETDATE()", Repository.TableName));
			}
			return true;
		}

		public void RegisterActions()
		{
			Repository.PreSelectHandler += OnPreSelect;
		}


		public void ConvertToDb(object source, Dictionary<string, object> toFill)
		{
		}
	}
}