using ZakDb.Queries;

namespace ZakDb.Repositories
{
	public interface IDataPlugin
	{
		string ToDbDataType(RepositoryTypeDescriptor descriptor);
		bool ExecuteUpdate(object q);
	}
}