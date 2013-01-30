
using System;
using System.Collections.Generic;

namespace ZakDb.Queries
{
	public class Sql99QueryCreator
	{
		protected string Pad(string toPad)
		{
			return string.Format(" {0} ", toPad.Trim());
		}
		public virtual string FieldAs { get { return Pad("AS"); } }
		public virtual string TableAs { get { return Pad("AS"); } }
		public virtual string Select { get { return Pad("SELECT"); } }
		public virtual string From { get { return Pad("FROM"); } }
		public virtual string Where { get { return Pad("WHERE"); } }
		public virtual string SubOpen { get { return Pad("("); } }
		public virtual string SubClose { get { return Pad(")"); } }
		public virtual string And { get { return Pad("AND"); } }
		public virtual string Or { get { return Pad("OR"); } }
		public virtual string IsNull { get { return Pad("IS NULL"); } }
		public virtual string IsNotNull { get { return Pad("IS NOT NULL"); } }
		public virtual string Lt { get { return Pad("<"); } }
		public virtual string Lte { get { return Pad("<="); } }
		public virtual string Gt { get { return Pad(">"); } }
		public virtual string Gte { get { return Pad(">="); } }

		public string CreateQuery(QueryTable table)
		{
#if DEBUG
			if (!table.Validate(true)) throw new Exception("Invalid query!");
#else
			if (!table.Validate()) throw new Exception("Invalid query!");
#endif

			var queryList = new List<string>();
			queryList.Add(Select);
			var tomerge = new List<string>();
			foreach (var field in table.Fields)
			{
				tomerge.Add(string.Format("{0}.{1} {2} {3}", table.Alias, field.Name, FieldAs, field.FullName));
			}
			queryList.Add(string.Join(",", tomerge.ToArray()));
			queryList.Add(From);
			queryList.Add(string.Format("{0} {1} {2}", table.Table, TableAs, table.Alias));

			if (table.HasConditions)
			{
				queryList.Add(Where);
				foreach (var condition in ParseCondition(table.Conditions))
				{
					queryList.Add(condition);
				}

			}
			return string.Join(" ", queryList);
		}

		private IEnumerable<string> ParseCondition(QueryCondition condition)
		{
			switch (condition.Operation)
			{
				case (QueryOperation.Eq):
					yield return ParseConditionEq(condition);
					break;
				case (QueryOperation.Neq):
				case (QueryOperation.Lt):
				case (QueryOperation.Lte):
				case (QueryOperation.Gt):
				case (QueryOperation.Gte):
					yield return ParseConditionCompare(condition, condition.Operation);
					break;
				case (QueryOperation.And):
				case (QueryOperation.Or):
					foreach (var item in ParseConditionAndOr(condition, condition.Operation)) yield return item;
					break;
				case (QueryOperation.IsNull):
				case (QueryOperation.IsNotNull):
					yield return ParseConditionNullOrNot(condition, condition.Operation);
					break;
			}
		}

		private string ParseConditionCompare(QueryCondition condition, QueryOperation queryOperation)
		{
			var compare = string.Empty;
			switch (condition.Operation)
			{
				case (QueryOperation.Lt):
					compare = Lt;
					break;
				case (QueryOperation.Lte):
					compare = Lte;
					break;
				case (QueryOperation.Gt):
					compare = Gt;
					break;
				case (QueryOperation.Gte):
					compare = Gte;
					break;
			}
			if (condition.IsComparandSet)
			{
				var value = condition.ComparandValue;
				return string.Format("{0} {1} '{2}'", condition.FullFieldName, compare, value);
			}
			if (!string.IsNullOrEmpty(condition.ComparandFieldName))
			{
				return string.Format("{0} {1} {2}", condition.FullFieldName,compare, condition.ComparandFieldName);
			}
			throw new NotImplementedException();
		}

		private string ParseConditionNullOrNot(QueryCondition condition, QueryOperation queryOperation)
		{
			return string.Format("{0} {1}", condition.FullFieldName, queryOperation == QueryOperation.IsNull?IsNull:IsNotNull);
		}

		private IEnumerable<string> ParseConditionAndOr(QueryCondition condition, QueryOperation operation)
		{
			var newList = new List<string>();
			foreach (var item in condition.SubQueries)
			{
				newList.AddRange(ParseCondition(item));
			}
			yield return SubOpen + string.Join(operation == QueryOperation.And ? And : Or, newList) + SubClose;
		}

		private string ParseConditionEq(QueryCondition condition)
		{
			if (condition.IsComparandNull)
			{
				return string.Format("{0} IS NULL", condition.FullFieldName);
			}
			if (condition.IsComparandSet)
			{
				var value = condition.ComparandValue;
				return string.Format("{0} = '{1}'", condition.FullFieldName, value);
			}
			if(!string.IsNullOrEmpty(condition.ComparandFieldName))
			{
				return string.Format("{0} = {1}", condition.FullFieldName, condition.ComparandFieldName);
			}
			throw new NotImplementedException();
		}
	}
}
