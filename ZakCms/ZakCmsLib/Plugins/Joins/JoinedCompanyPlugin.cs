using ZakCms.Models.Entitites;
using ZakCms.Models.Joins;
using ZakDb.Models;
using ZakDb.Plugins;
using ZakDb.Repositories;

namespace ZakCms.Plugins.Joins
{
	public class JoinedCompanyPlugin : JoinedLovPlugin
	{
		public JoinedCompanyPlugin(IRepository joinedRepository) :
			base(joinedRepository, "CompanyId")
		{
		}

		protected override long GetIdFromOwnerModel(object item)
		{
			var cm = (IJoinedCompanyModel) item;
			return cm.Company.Id;
		}

		protected override void SetOnOwnerModel(object item, ILovModel newJoinedModel)
		{
			var cm = (IJoinedCompanyModel) item;
			cm.Company = (CompanyModel) newJoinedModel;
		}
	}
}