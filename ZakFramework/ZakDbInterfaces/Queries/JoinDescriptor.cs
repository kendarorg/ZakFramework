
using ZakDb.Query;

namespace ZakDb.Queries
{
	public class JoinDescriptor:IQueryable
	{
		public QueryTable JoinTable { get; private set; }
		public QueryCondition JoinCondition { get; private set; }

		public JoinDescriptor(QueryTable joinTable,QueryCondition joinCondition)
		{
			JoinTable = joinTable;
			JoinCondition = joinCondition;
		}

		public bool Validate(bool exceptonOnError = false)
		{
			return JoinTable.Validate(exceptonOnError) && JoinCondition.Validate(exceptonOnError);
		}
	}
}
