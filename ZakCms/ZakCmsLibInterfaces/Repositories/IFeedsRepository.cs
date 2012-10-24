using ZakDb.Repositories;

namespace ZakCms.Repositories
{
	public interface IFeedsRepository : IRepository
	{
		object GetBySeoTitle(string id);
	}
}