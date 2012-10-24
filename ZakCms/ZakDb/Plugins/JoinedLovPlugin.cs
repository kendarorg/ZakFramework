using System;
using System.Collections.Generic;
using ZakDb.Models;
using ZakDb.Repositories;
using ZakDb.Repositories.Utils;

namespace ZakDb.Plugins
{
	public abstract class JoinedLovPlugin : JoinedPlugin
	{
		private readonly string _ownerIdField;
		private readonly string _ownedIdField;

		protected JoinedLovPlugin(IRepository joinedRepository, string ownerIdField, string ownedIdField = null) :
			base(joinedRepository)
		{
			_ownerIdField = ownerIdField;
			_ownedIdField = ownedIdField == null ? _ownerIdField : ownedIdField;
		}

		public override List<string> JoinableFieldsLocal
		{
			get { return RepositoryUtils.FillStringArray(ref _joinableFieldsLocal, _lockObj, new[] {_ownedIdField}); }
		}

		public override List<string> UpdatableFields
		{
			get { return RepositoryUtils.FillStringArray(ref _updatableFields, _lockObj, new[] {_ownerIdField}); }
		}

		public override void FillFromDb(ZakDataReader reader, object item)
		{
			var newJoinedModel = (ILovModel) JoinedRepository.CreateItem();
			newJoinedModel.Id = (Int64) reader[_ownerIdField];
			newJoinedModel.Code = (string) reader[JoinedRepository.TableName + "_Code"];
			newJoinedModel.Description = (string) reader[JoinedRepository.TableName + "_Description"];
			SetOnOwnerModel(item, newJoinedModel);
		}

		public override void ConvertToDb(object item, Dictionary<string, object> toFill)
		{
			toFill.Add(_ownerIdField, GetIdFromOwnerModel(item));
		}

		protected abstract Int64 GetIdFromOwnerModel(object item);

		protected abstract void SetOnOwnerModel(object item, ILovModel newJoinedModel);
	}
}