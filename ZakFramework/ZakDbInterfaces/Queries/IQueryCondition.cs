namespace ZakDb.Queries
{
	public interface IQueryCondition
	{
		QueryCondition And(params QueryCondition[] querySelects);
		QueryCondition Or(params QueryCondition[] querySelects);
		QueryCondition Not(object value = null, bool asNull = false);
		QueryCondition Eq(object value = null, bool asNull = false);
		QueryCondition Neq(object value = null, bool asNull = false);
		QueryCondition Gt(object value);
		QueryCondition Gte(object value);
		QueryCondition Lt(object value);
		QueryCondition Lte(object value);
		QueryCondition In(params object[] values);
		QueryCondition IsNull();
		QueryCondition IsNotNull();
	}
}