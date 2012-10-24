using System;
using System.Collections.Generic;
using ZakDb.Models;
using ZakDb.Repositories;
using ZakDb.Repositories.Utils;

namespace ZakDb.Plugins
{
	public abstract class ManyToManyPlugin : JoinedPlugin
	{
		private ManyToManyRepository _repository;

		public override IRepository Repository
		{
			get { return _repository; }
			set { _repository = (ManyToManyRepository) value; }
		}

		protected ManyToManyPlugin(IRepository joinedRepository) :
			base(joinedRepository)
		{
		}

		public override List<string> JoinableFieldsLocal
		{
			get { return RepositoryUtils.FillStringArray(ref _joinableFieldsLocal, _lockObj, new[] {"RightId"}); }
		}

		public override List<string> UpdatableFields
		{
			get { return RepositoryUtils.FillStringArray(ref _updatableFields, _lockObj, null); }
		}

		public override void FillFromDb(ZakDataReader reader, object item)
		{
			var mtm = (ManyToManyModel) item;
			mtm.Content = (IModel) JoinedRepository.CreateItem();
			mtm.Content.Id = (Int64) reader["RightId"];
			//Fill the joined element from the 
			FillJoinedFromDb(reader, mtm.Content, JoinedRepository.TableName);
		}

		protected abstract void FillJoinedFromDb(ZakDataReader reader, IModel modelToFill, string fieldsNamesPrefix);

		public override void ConvertToDb(object item, Dictionary<string, object> toFill)
		{
			var mtm = (ManyToManyModel) item;
			toFill.Add("RightId", mtm.RightId);
		}

		protected abstract Int64 GetIdFromOwnerModel(object item);

		protected abstract void SetOnOwnerModel(object item, ILovModel newJoinedModel);
	}
}