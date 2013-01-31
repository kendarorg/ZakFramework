using System;
using System.Collections.Generic;
using ZakDb.Descriptors;
using ZakDb.Queries;

namespace ZakDb.Creators
{
	public abstract class Sql99QueryCreator : IQueryCreator
	{
		protected string Pad(string toPad)
		{
			return string.Format(" {0} ", toPad.Trim());
		}

		public Dictionary<Type, Action<TypeCreatorAction>> TypeCreatorAction { get; set; }
		public Dictionary<Type, Action<TypeConversionAction>> FromDbConversionAction { get; set; }
		public Dictionary<Type, Action<TypeConversionAction>> ToDbConversionAction { get; set; }

		public virtual string CreateTable { get { return Pad("CREATE TABLE"); } }
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

		public T CreateQuery<T>(QueryTable table)
		{
#if DEBUG
			if (!table.Validate(true)) throw new Exception("Invalid query!");
#else
			if (!table.Validate()) throw new Exception("Invalid query!");
#endif

			var queryList = new List<string> { Select };

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
			object toret = string.Join(" ", queryList);
			return (T)toret;
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
			switch (queryOperation)
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
				return string.Format("{0} {1} '{2}'", condition.DotFieldName, compare, value);
			}
			if (!string.IsNullOrEmpty(condition.ComparandFieldName))
			{
				return string.Format("{0} {1} {2}", condition.DotFieldName, compare, condition.ComparandFieldName);
			}
			throw new NotImplementedException();
		}

		private string ParseConditionNullOrNot(QueryCondition condition, QueryOperation queryOperation)
		{
			return string.Format("{0} {1}", condition.DotFieldName, queryOperation == QueryOperation.IsNull ? IsNull : IsNotNull);
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
				return string.Format("{0} IS NULL", condition.DotFieldName);
			}
			if (condition.IsComparandSet)
			{
				var value = condition.ComparandValue;
				return string.Format("{0} = '{1}'", condition.DotFieldName, value);
			}
			if (!string.IsNullOrEmpty(condition.ComparandFieldName))
			{
				return string.Format("{0} = {1}", condition.DotFieldName, condition.ComparandFieldName);
			}
			throw new NotImplementedException();
		}


		private bool ExceptionOnError(bool exceptionOnError, bool value = false)
		{
			if (exceptionOnError && !value) throw new Exception();
			return value;
		}

		public virtual bool Validate(TableDescriptor tableDescriptor,bool exceptionOnError = false)
		{
			int autoIncrementCount = 0;
			foreach (var key in tableDescriptor.Keys)
			{
				if (key.AutoIncrement)
				{
					if (key.Fields.Length != 1) return ExceptionOnError(exceptionOnError);
					autoIncrementCount++;
				}
			}
			if (autoIncrementCount > 1) return ExceptionOnError(exceptionOnError);
			return true;
		}

		public T CreateTableQuery<T>(TableDescriptor tableDescriptor)
		{
			var toret = string.Format("{0} {1} {2} ", CreateTable, tableDescriptor.FullName, SubOpen);
			var fieldsList = new List<string>();
			foreach (var field in tableDescriptor.Fields)
			{
				fieldsList.Add(CreateDataTypeQuery<string>(field.FieldDescriptor,field.FieldName));
			}
			toret += string.Join(",", fieldsList);
			toret += SubClose;
			object obj = toret;
			return (T)obj;
		}

		public abstract T CreateDatabaseQuery<T>(DatabaseDescriptor dbDescriptor);
		public abstract T CreateDataTypeQuery<T>(FieldDescriptor descriptor, string fieldName);
	}
}
