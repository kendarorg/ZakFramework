using System;
using System.Collections.Generic;
using System.Transactions;
using ZakDb.Models;
using ZakDb.Plugins;
using ZakDb.Repositories.Queries;
using ZakDb.Repositories.Utils;

namespace ZakDb.Repositories
{
	public abstract class ManyToManyRepository : BaseRepository, IManyToManyRepository
	{
		protected ManyToManyRepository(string tableName, string cmsDb, List<IRepositoryPlugin> pluginsList)
			: base(tableName, cmsDb, pluginsList)

		{
		}


		protected abstract bool UpdateMainOnChange { get; }

		protected IRepository DataRepository { get; set; }

		protected ManyToManyRepository(string tableName, string connectionString,
		                               IEnumerable<IRepositoryPlugin> repositoryPlugins,
		                               IRepository dataRepository) :
			                               base(tableName, connectionString, repositoryPlugins)
		{
			DataRepository = dataRepository;
		}

		public override void FillFromDb(ZakDataReader reader, object item)
		{
			base.FillFromDb(reader, item);
			((ManyToManyModel) item).LeftId = (Int64) reader["LeftId"];
			((ManyToManyModel) item).RightId = (Int64) reader["RightId"];
		}

		protected override Dictionary<string, object> ConvertToDb(object item)
		{
			var toret = base.ConvertToDb(item);
			//toret.Add("LeftId", ((ManyToManyModel)item).LeftId);
			toret.Add("RightId", ((ManyToManyModel) item).RightId);
			return toret;
		}

		protected override void InitializeUpdatableFields(List<string> updatableFields)
		{
			base.InitializeUpdatableFields(updatableFields);
			//updatableFields.Add("LeftId");
			updatableFields.Add("RightId");
		}

		protected override void InitializeSelectableFields(List<string> selectableFields)
		{
			base.InitializeSelectableFields(selectableFields);
			selectableFields.Add("LeftId");
			selectableFields.Add("RightId");
		}

		#region ManyToMAnyFields

		public List<object> GetByOwner(IModel model)
		{
			return GetByOwner(model.Id);
		}

		public List<object> GetByOwner(Int64 modelId)
		{
			return
				GetAll(new QueryObject
					{WhereCondition = string.Format("{0}.LeftId={1} AND {0}.RightId IS NOT NULL ", TableName, modelId)});
		}

		public object Create(IModel leftModel, IModel rightModel)
		{
			return Create(leftModel.Id, rightModel.Id);
		}

		public void Delete(IModel leftModel, IModel rightModel = null)
		{
			Delete(leftModel.Id, rightModel == null ? 0 : rightModel.Id);
		}

		public void DeleteAll(IModel leftModel)
		{
			DeleteAll(leftModel.Id);
		}

		public object Create(Int64 leftModelId, Int64 rightModelId)
		{
			using (var scope = new TransactionScope())
			{
				var element = (ManyToManyModel) InstantiateNewItem();
				element.LeftId = leftModelId;
				element.RightId = rightModelId;
				var result = base.Create(element);
				UpdateOnChange(leftModelId);
				scope.Complete();
				return result;
			}
		}

		public void Delete(Int64 leftModelId, Int64 rightModelId = 0)
		{
			using (var scope = new TransactionScope())
			{
				var qo = new QueryObject();
				if (rightModelId == 0)
				{
					qo.WhereCondition = string.Format("{0}.LeftId={1}", TableName, leftModelId);
				}
				else
				{
					qo.WhereCondition = string.Format("{0}.LeftId={1} AND {0}.RightId={2}", TableName, leftModelId, rightModelId);
				}
				var ob = (ManyToManyModel) GetFirst(qo);
				if (ob != null)
				{
					base.Delete(ob.Id);
					UpdateOnChange(leftModelId);
				}
				scope.Complete();
			}
		}

		public void DeleteAll(Int64 leftModelId)
		{
			var obs = GetAll(new QueryObject {WhereCondition = string.Format("{0}.LeftId={1}", TableName, leftModelId)});
			using (var scope = new TransactionScope())
			{
				foreach (var ob in obs)
				{
					base.Delete(((ManyToManyModel) ob).Id);
				}
				UpdateOnChange(leftModelId);
				scope.Complete();
			}
		}

		protected virtual void UpdateOnChange(Int64 modelId)
		{
			if (!UpdateMainOnChange) return;
			var element = (IModel) DataRepository.GetById(modelId, new QueryObject {UseJoins = false});
			DataRepository.Update(element);
		}

		#endregion

		public List<object> GetByOwned(IModel model)
		{
			return GetByOwned(model.Id);
		}

		public List<object> GetByOwned(long modelId)
		{
			return GetAll(new QueryObject {WhereCondition = string.Format("{0}.RightId={1}", TableName, modelId)});
		}

		protected override object InstantiateNewItem()
		{
			return new ManyToManyModel();
		}
	}
}