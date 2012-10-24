using ZakDb.Repositories;

namespace ZakCms.Repositories
{
	public interface ITagsRepository : IRepository
	{
		object GetByTagCode(string code);
	}
}