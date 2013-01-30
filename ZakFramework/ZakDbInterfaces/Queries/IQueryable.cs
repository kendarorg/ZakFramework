namespace ZakDb.Query
{
	public interface IQueryable
	{
		bool Validate(bool exceptonOnError = false);
	}
}
