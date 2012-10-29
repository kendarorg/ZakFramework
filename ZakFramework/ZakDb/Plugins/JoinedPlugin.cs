using System.Collections.Generic;
using ZakDb.Repositories;
using ZakDb.Repositories.Utils;

namespace ZakDb.Plugins
{
	public abstract class JoinedPlugin : IJoinRepositoryPlugin
	{
		public virtual IRepository Repository { get; set; }
		protected readonly object _lockObj = new object();

		protected List<string> _joinableFields;
		protected List<string> _updatableFields;
		protected List<string> _selectableFields;
		protected List<string> _joinableFieldsLocal;

		public abstract List<string> JoinableFieldsLocal { get; }
		public abstract List<string> UpdatableFields { get; }

		public virtual List<string> SelectableFields
		{
			get { return RepositoryUtils.FillStringArray(ref _selectableFields, _lockObj, JoinedRepository.SelectableFields); }
		}

		public virtual List<string> JoinableFields
		{
			get { return RepositoryUtils.FillStringArray(ref _joinableFields, _lockObj, new[] {"Id"}); }
		}

		protected JoinedPlugin(IRepository joinedRepository)
		{
			JoinedRepository = joinedRepository;
		}

		public abstract void FillFromDb(ZakDataReader reader, object item);
		public abstract void ConvertToDb(object item, Dictionary<string, object> toFill);
		/*
		public void FillFromDb(ZakDataReader reader, object item)
		{
			((IJoinedLanguageModel)item).LanguageId = (Int64)reader["LanguageId"];
			((IJoinedLanguageModel)item).LanguageCode = (string)reader[JoinedRepository.TableName+ "_Code"];
			((IJoinedLanguageModel)item).LanguageDescription = (string)reader[JoinedRepository.TableName + "_Description"];
		}

		public void ConvertToDb(object item, Dictionary<string, object> toFill)
		{
			toFill.Add("LanguageId", ((IJoinedLanguageModel)item).LanguageId);
		}*/

		/// <summary>
		/// MainRepository left join JoinedRepository
		/// </summary>
		public IRepository JoinedRepository { get; set; }

		public void RegisterActions()
		{
		}
	}
}