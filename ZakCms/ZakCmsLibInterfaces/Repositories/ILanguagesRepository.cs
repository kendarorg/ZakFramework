using System;
using ZakDb.Repositories;

namespace ZakCms.Repositories
{
	public interface ILanguagesRepository : IRepository
	{
		Int64 GetIdFromCode(string id);
	}
}