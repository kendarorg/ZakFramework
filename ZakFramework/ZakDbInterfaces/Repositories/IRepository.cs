using System;
using System.Collections.Generic;
using ZakDb.Queries;

namespace ZakDb.Repositories
{
	public interface IMainRepository<T>
	{
		IDataPlugin DataPlugin { get; set; }
		IEnumerable<RepositoryTypeDescriptor> Rows { get; }
		bool Add(T item);
		bool Update(T item);
		bool Delete(T item);
		T FindById(Guid id);
		IEnumerable<T> Find(QueryTable query = null);
	}
}
