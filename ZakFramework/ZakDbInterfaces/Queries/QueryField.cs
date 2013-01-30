namespace ZakDb.Queries
{
	public class QueryField
	{
		public string Name { get; private set; }
		public QueryTable Table { get; private set; }
		public string FullName { get { return string.Format("{0}_{1}", Table.Alias, Name); } }

		public QueryField(string name, QueryTable table)
		{
			Name = name;
			Table = table;
		}
	}
}
