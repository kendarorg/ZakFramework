using System.Collections.Generic;
using ZakDb.Repositories;

namespace ZakDb.Plugins
{
	public interface IJoinRepositoryPlugin : IRepositoryPlugin
	{
		IRepository JoinedRepository { get; set; }

		/// <summary>
		/// The fields on the JoinedRepository table that will be used to build the join
		/// ( TableA JOIN TableB ON JoinableOnFather=JoinableFields)
		/// </summary>
		List<string> JoinableFields { get; }

		List<string> JoinableFieldsLocal { get; }
	}
}