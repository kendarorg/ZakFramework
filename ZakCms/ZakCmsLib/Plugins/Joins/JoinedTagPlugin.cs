using System;
using ZakCms.Models.Entitites;
using ZakDb.Models;
using ZakDb.Plugins;
using ZakDb.Repositories;
using ZakDb.Repositories.Utils;

namespace ZakCms.Plugins.Joins
{
	public class JoinedTagPlugin : JoinedLovPlugin
	{
		public JoinedTagPlugin(IRepository joinedRepository) :
			base(joinedRepository, "LeftId", "RightId")
		{
		}


		protected override long GetIdFromOwnerModel(object item)
		{
			var cm = (ManyToManyModel) item;
			return cm.LeftId;
		}

		protected override void SetOnOwnerModel(object item, ILovModel newJoinedModel)
		{
			var cm = (ManyToManyModel) item;
			cm.Content = newJoinedModel;
		}

		public override void FillFromDb(ZakDataReader reader, object item)
		{
			var cm = (ManyToManyModel) item;
			if (cm.Content == null)
			{
				cm.Content = (IModel) JoinedRepository.CreateItem();
			}
			((TagModel) cm.Content).Id = (Int64) reader["RightId"];
			((TagModel) cm.Content).Code = (string) reader[JoinedRepository.TableName + "_Code"];
			((TagModel) cm.Content).Description = (string) reader[JoinedRepository.TableName + "_Description"];
		}
	}
}