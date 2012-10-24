using ZakDb.Repositories;

namespace ZakCms.Repositories
{
	public interface IBEUsersRepository : IRepository
	{
		bool ValidateUser(string uid, string pwd);
		object GetUserByUserId(string uid);
	}
}