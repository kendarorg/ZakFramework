using System;
using System.Collections.Generic;
using System.Globalization;
using ZakCms.Models.Entitites;
using ZakDb.Models;
using ZakDb.Plugins;
using ZakDb.Repositories;
using ZakDb.Repositories.Exceptions;
using ZakDb.Repositories.Queries;
using ZakDb.Repositories.Utils;

namespace ZakCms.Repositories
{
	public class LanguagesRepository : ListOfValuesRepository, ILanguagesRepository
	{
		public LanguagesRepository(string tableName, string connectionString) :
			this(tableName, connectionString, new List<IRepositoryPlugin>())
		{
		}

		public LanguagesRepository(string tableName, string connectionString, IEnumerable<IRepositoryPlugin> repositoryPlugins)
			:
				base(tableName, connectionString, repositoryPlugins)
		{
		}

		#region Abstract Implementations

		public Int64 GetIdFromCode(string id)
		{
			var result = ExecuteSql(new QueryObject
				{
					TypeOfQuery = QueryType.Scalar,
					WhereCondition = string.Format("Code='{0}'", RepositoryUtils.AddSlashes(id)),
					UseJoins = false,
					ForceSelectField = true,
					SelectFields = "Id"
				});

			if (result != null) return (Int64) result;
			return 0;
		}

		protected override object InstantiateNewItem()
		{
			return new LanguageModel();
		}

		public override bool OnVerify(object item, string operation)
		{
			if (string.Compare(((ILovModel) item).Code, "defau", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				return true;
			}
			try
			{
				CultureInfo.CreateSpecificCulture(((ILovModel) item).Code.ToLowerInvariant().Trim());
			}
			catch (Exception)
			{
				throw new RepositoryValidationException(
					GetType().Name,
					operation,
					"Language",
					string.Format(
						"code '{0}' is not a valid",
						((ILovModel) item).Code.ToLowerInvariant().Trim()));
			}
			return true;
		}

		#endregion
	}
}