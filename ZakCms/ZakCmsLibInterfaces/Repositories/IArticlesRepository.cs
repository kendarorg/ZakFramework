using ZakDb.Repositories;

namespace ZakCms.Repositories
{
	public interface IArticlesRepository : IRepository
	{
		object GetBySeoTitle(string seoTitle);
	}
}