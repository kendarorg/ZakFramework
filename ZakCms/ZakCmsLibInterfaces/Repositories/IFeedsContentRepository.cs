using System.Collections.Generic;
using ZakDb.Repositories;

namespace ZakCms.Repositories
{
	public interface IFeedsContentRepository : IRepository
	{
		List<object> GetByFeedId(long p);
	}
}