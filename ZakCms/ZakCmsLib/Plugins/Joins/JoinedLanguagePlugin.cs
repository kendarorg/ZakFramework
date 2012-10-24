using ZakCms.Models.Entitites;
using ZakCms.Models.Joins;
using ZakDb.Models;
using ZakDb.Plugins;
using ZakDb.Repositories;

namespace ZakCms.Plugins.Joins
{
	public class JoinedLanguagePlugin : JoinedLovPlugin
	{
		public JoinedLanguagePlugin(IRepository joinedRepository) :
			base(joinedRepository, "LanguageId")
		{
		}

		protected override long GetIdFromOwnerModel(object item)
		{
			var cm = (IJoinedLanguageModel) item;
			return cm.Language.Id;
		}

		protected override void SetOnOwnerModel(object item, ILovModel newJoinedModel)
		{
			var cm = (IJoinedLanguageModel) item;
			cm.Language = (LanguageModel) newJoinedModel;
		}
	}
}