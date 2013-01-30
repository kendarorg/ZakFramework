namespace ZakDb.Queries
{
	public enum QueryOperation
	{
		None = 0,
		And,
		Or,
		Not,
		Eq,
		Neq,
		Gt,
		Gte,
		Lt,
		Lte,
		In,
		IsNull,
		IsNotNull
	}
}
