using System;
using ZakDb.Query;

namespace ZakDb.Queries
{
	public class QueryCondition : IQueryable, IQueryCondition
	{
		public IQueryable Parent { get; set; }
		private bool _comparandSet;
		private bool _shouldUseValue;
		public QueryOperationType OperationType { get; private set; }
		public IQueryCondition[] SubQueries { get; private set; }
		public object ComparandValue { get; private set; }
		public QueryField FieldName { get; private set; }
		public string FullFieldName { get { return string.Format("{0}_{1}", TableAlias, FieldName); } }
		public string DotFieldName { get { return string.Format("{0}.{1}", TableAlias, FieldName); } }
		public QueryTable TableAlias { get; internal set; }
		public QueryField ComparandFieldName { get; private set; }
		public QueryOperation Operation { get; private set; }
		public bool IsComparandNull
		{
			get { return (_comparandSet && ComparandValue == null); }
		}
		public bool IsComparandSet
		{
			get { return _comparandSet; }
		}

		internal QueryCondition(QueryField fieldName):
			this()
		{
			FieldName = fieldName;
			TableAlias = FieldName.Table;
		}

		internal QueryCondition()
		{
			_shouldUseValue = false;
			_comparandSet = false;
			SubQueries = null;
			ComparandValue = null;
			ComparandFieldName = null;
			Operation = QueryOperation.None;
			OperationType = QueryOperationType.None;
		}

		public QueryCondition SetFieldName(QueryField fieldName)
		{
			FieldName = fieldName;
			return this;
		}

		public QueryCondition SetFieldName(string fieldName)
		{
			var queryField = TableAlias.GetField(fieldName);
			FieldName = queryField;
			return this;
		}

		public QueryCondition SetComparandFieldName(QueryField comparandFieldName)
		{
			ComparandFieldName = comparandFieldName;
			return this;
		}

		public QueryCondition SetComparandValue(object comparandValue)
		{
			ComparandValue = comparandValue;
			_comparandSet = true;
			AssignParent(comparandValue);
			return this;
		}


		public QueryCondition And(params QueryCondition[] querySelects)
		{
			SubQueries = querySelects;
			foreach (var cond in SubQueries)
			{
				AssignParent(cond);
			}
			Operation = QueryOperation.And;
			OperationType = QueryOperationType.Multiple;
			return this;
		}

		public QueryCondition Or(params QueryCondition[] querySelects)
		{
			SubQueries = querySelects;
			foreach (var cond in SubQueries)
			{
				AssignParent(cond);
			}
			Operation = QueryOperation.Or;
			OperationType = QueryOperationType.Multiple;
			return this;
		}

		private void SetComparandEvenAsNullIfRequired(object value, bool asNull)
		{
			if ((asNull && value == null) || value != null)
			{
				ComparandValue = value;
				_comparandSet = true;
				_shouldUseValue = true;
			}
		}

		public QueryCondition Not(object value = null, bool asNull = false)
		{
			Operation = QueryOperation.Not;
			SetComparandEvenAsNullIfRequired(value, asNull);
			OperationType = QueryOperationType.Dual;
			AssignParent(value);
			return this;
		}

		public QueryCondition Eq(object value = null, bool asNull = false)
		{
			Operation = QueryOperation.Eq;
			SetComparandEvenAsNullIfRequired(value, asNull);
			OperationType = QueryOperationType.Dual;
			AssignParent(value);
			return this;
		}
		public QueryCondition Neq(object value = null, bool asNull = false)
		{
			Operation = QueryOperation.Neq;
			SetComparandEvenAsNullIfRequired(value, asNull);
			OperationType = QueryOperationType.Dual;
			AssignParent(value);
			return this;
		}

		public QueryCondition Gt(object value)
		{
			Operation = QueryOperation.Gt;
			SetComparandIfNotNull(value);
			OperationType = QueryOperationType.Dual;
			_shouldUseValue = true;
			AssignParent(value);
			return this;
		}

		public QueryCondition Gte(object value)
		{
			Operation = QueryOperation.Gte;
			SetComparandIfNotNull(value);
			OperationType = QueryOperationType.Dual;
			_shouldUseValue = true;
			AssignParent(value);
			return this;
		}

		public QueryCondition Lt(object value)
		{
			Operation = QueryOperation.Lt;
			SetComparandIfNotNull(value);
			OperationType = QueryOperationType.Dual;
			_shouldUseValue = true;
			AssignParent(value);
			return this;
		}

		public QueryCondition Lte(object value)
		{
			Operation = QueryOperation.Lte;
			SetComparandIfNotNull(value);
			OperationType = QueryOperationType.Dual;
			_shouldUseValue = true;
			AssignParent(value);
			return this;
		}

		private void SetComparandIfNotNull(object value)
		{
			if (value != null)
			{
				ComparandValue = value;
				_comparandSet = true;
				AssignParent(value);
			}
		}

		public QueryCondition In(params object[] values)
		{
			Operation = QueryOperation.In;
			if (values != null && values.Length > 0)
			{
				ComparandValue = values;
				_comparandSet = true;
				foreach (var item in values)
				{
					AssignParent(item);
				}
			}
			OperationType = QueryOperationType.Dual;
			_shouldUseValue = true;
			return this;
		}

		private void AssignParent(object item)
		{
			if (item == null) return;
			var cond = item as IQueryable;
			if (cond != null) cond.Parent = this;
		}

		public QueryCondition IsNull()
		{
			Operation = QueryOperation.IsNull;
			OperationType = QueryOperationType.Unary;
			return this;
		}

		public QueryCondition IsNotNull()
		{
			Operation = QueryOperation.IsNotNull;
			OperationType = QueryOperationType.Unary;
			return this;
		}

		private bool ExceptionOnError(bool exceptionOnError, bool value = false)
		{
			if (exceptionOnError && !value) throw new Exception();
			return value;
		}

		private bool NeedsFieldName
		{
			get { return Operation != QueryOperation.Or && Operation != QueryOperation.And; }
		}

		public bool Validate(bool exceptionOnError = false)
		{
			if (Operation == QueryOperation.None) return ExceptionOnError(exceptionOnError);
			if (FieldName == null && NeedsFieldName) return ExceptionOnError(exceptionOnError);
			if (TableAlias == null) return ExceptionOnError(exceptionOnError);

			switch (OperationType)
			{
				case (QueryOperationType.Unary):
					return ExceptionOnError(exceptionOnError,
						!_comparandSet && ComparandFieldName == null);
				case (QueryOperationType.Dual):
					if (_comparandSet && ComparandFieldName != null) return ExceptionOnError(exceptionOnError);
					if (_shouldUseValue && !_comparandSet) return ExceptionOnError(exceptionOnError);
					if (!_shouldUseValue && _comparandSet) return ExceptionOnError(exceptionOnError);
					return true;
				case (QueryOperationType.Multiple):
					if (_comparandSet || ComparandFieldName != null) return ExceptionOnError(exceptionOnError);
					if (SubQueries != null && SubQueries.Length > 0)
					{
						foreach (var subQuery in SubQueries)
						{
							var qc = subQuery as QueryCondition;
							var qt = subQuery as QueryTable;
							if (qc != null)
							{
								if (!qc.Validate(exceptionOnError)) return ExceptionOnError(exceptionOnError);
							}
							else if (qt != null)
							{
								if (!qt.Validate(exceptionOnError)) return ExceptionOnError(exceptionOnError);
							}
						}
					}
					return ExceptionOnError(exceptionOnError,
						SubQueries != null && SubQueries.Length > 0);
				default:
					return ExceptionOnError(exceptionOnError);
			}
		}
	}
}
