using System.Collections.Generic;
using ZakDb.Repositories;
using ZakDb.Repositories.Utils;

namespace ZakDb.Plugins
{
	public interface IRepositoryPlugin
	{
		IRepository Repository { get; set; }
		List<string> UpdatableFields { get; }
		void FillFromDb(ZakDataReader reader, object article);
		void ConvertToDb(object source, Dictionary<string, object> toFill);
		List<string> SelectableFields { get; }
		void RegisterActions();
	}
}