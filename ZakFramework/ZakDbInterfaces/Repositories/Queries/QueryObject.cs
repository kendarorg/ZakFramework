namespace ZakDb.Repositories.Queries
{
	public class QueryObject
	{
		public QueryObject()
		{
			Limit = -1;
			Skip = -1;
			WhereCondition = string.Empty;
			OrderByCondition = string.Empty;
			UseJoins = true;
			Action = QueryAction.Select;
			SelectFields = string.Empty;
			QueryBody = string.Empty;
			TypeOfQuery = QueryType.Query;
			ForceSelectField = false;
		}

		public QueryType TypeOfQuery { get; set; }
		public string QueryBody { get; set; }
		public string SelectFields { get; set; }
		public QueryAction Action { get; set; }
		public int Limit { get; set; }
		public int Skip { get; set; }
		public string WhereCondition { get; set; }
		public string OrderByCondition { get; set; }
		public bool UseJoins { get; set; }
		public bool ForceSelectField { get; set; }
	}
}