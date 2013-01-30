using System;
using System.Collections.Generic;
using ZakDb.Models;

namespace ZakDb.Repositories
{
	public interface IManyToManyRepository : IRepository
	{
		List<object> GetByOwner(IModel model);
		List<object> GetByOwned(IModel model);
		object Create(IModel leftModel, IModel rightModel);
		void Delete(IModel leftModel, IModel rightModel);
		void DeleteAll(IModel leftModel);

		List<object> GetByOwner(Int64 modelId);
		List<object> GetByOwned(Int64 modelId);
		object Create(Int64 leftModelId, Int64 rightModelId);
		void Delete(Int64 leftModelId, Int64 rightModelId);
		void DeleteAll(Int64 leftModelId);
	}
}