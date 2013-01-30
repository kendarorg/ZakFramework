using System;
using ZakDb.Query;

namespace ZakDb.Queries
{
	public class QueryCondition : IQueryable, IQueryCondition
	{
		private bool _comparandSet;
		private bool _shouldUseValue;
		public QueryOperationType OperationType { get; private set; }
		public QueryCondition[] SubQueries { get; private set; }
		public object ComparandValue { get; private set; }
		public string FieldName { get; private set; }
		public string FullFieldName { get { return string.Format("{0}_{1}", TableAlias, FieldName); } }
		public string TableAlias { get; private set; }
		public string ComparandFieldName { get; private set; }
		public QueryOperation Operation { get; private set; }
		public bool IsComparandNull
		{
			get { return (_comparandSet && ComparandValue == null); }
		}
		public bool IsComparandSet
		{
			get { return _comparandSet; }
		}

		internal QueryCondition(string fieldName,string tableAlias)
		{
			_shouldUseValue = false;
			_comparandSet = false;
			SubQueries = null;
			FieldName = fieldName;
			TableAlias = tableAlias;
			ComparandValue = null;
			ComparandFieldName = null;
			Operation = QueryOperation.None;
			OperationType = QueryOperationType.None;
		}

		public QueryCondition SetFieldName(string fieldName)
		{
			FieldName = fieldName;
			return this;
		}

		public QueryCondition SetComparandFieldName(string comparandFieldName)
		{
			ComparandFieldName = comparandFieldName;
			return this;
		}

		public QueryCondition SetComparandValue(object comparandValue)
		{
			ComparandValue = comparandValue;
			_comparandSet = true;
			return this;
		}


		public QueryCondition And(params QueryCondition[] querySelects)
		{
			SubQueries = querySelects;
			Operation = QueryOperation.And;
			OperationType = QueryOperationType.Multiple;
			return this;
		}

		public QueryCondition Or(params QueryCondition[] querySelects)
		{
			SubQueries = querySelects;
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
			return this;
		}

		public QueryCondition Eq(object value = null, bool asNull = false)
		{
			Operation = QueryOperation.Eq;
			SetComparandEvenAsNullIfRequired(value, asNull);
			OperationType = QueryOperationType.Dual;
			return this;
		}
		public QueryCondition Neq(object value = null, bool asNull = false)
		{
			Operation = QueryOperation.Neq;
			SetComparandEvenAsNullIfRequired(value, asNull);
			OperationType = QueryOperationType.Dual;
			return this;
		}

		public QueryCondition Gt(object value)
		{
			Operation = QueryOperation.Gt;
			SetComparandIfNotNull(value);
			OperationType = QueryOperationType.Dual;
			_shouldUseValue = true;
			return this;
		}

		public QueryCondition Gte(object value)
		{
			Operation = QueryOperation.Gte;
			SetComparandIfNotNull(value);
			OperationType = QueryOperationType.Dual;
			_shouldUseValue = true;
			return this;
		}

		public QueryCondition Lt(object value)
		{
			Operation = QueryOperation.Lt;
			SetComparandIfNotNull(value);
			OperationType = QueryOperationType.Dual;
			_shouldUseValue = true;
			return this;
		}

		public QueryCondition Lte(object value)
		{
			Operation = QueryOperation.Lte;
			SetComparandIfNotNull(value);
			OperationType = QueryOperationType.Dual;
			_shouldUseValue = true;
			return this;
		}

		private void SetComparandIfNotNull(object value)
		{
			if (value != null)
			{
				ComparandValue = value;
				_comparandSet = true;
			}
		}

		public QueryCondition In(params object[] values)
		{
			Operation = QueryOperation.In;
			if (values != null && values.Length>0)
			{
				ComparandValue = values;
				_comparandSet = true;
			}
			OperationType = QueryOperationType.Dual;
			_shouldUseValue = true;
			return this;
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

		private bool ExceptionOnError(bool exceptionOnError,bool value = false)
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
			if (string.IsNullOrWhiteSpace(FieldName) && NeedsFieldName) return ExceptionOnError(exceptionOnError);
			if (string.IsNullOrWhiteSpace(TableAlias)) return ExceptionOnError(exceptionOnError);

			switch (OperationType)
			{
				case (QueryOperationType.Unary):
					return ExceptionOnError(exceptionOnError,
						!_comparandSet && string.IsNullOrWhiteSpace(ComparandFieldName));
				case (QueryOperationType.Dual):
					if (_comparandSet && !string.IsNullOrWhiteSpace(ComparandFieldName)) return ExceptionOnError(exceptionOnError);
					if (_shouldUseValue && !_comparandSet) return ExceptionOnError(exceptionOnError);
					if (!_shouldUseValue && _comparandSet) return ExceptionOnError(exceptionOnError);
					return true;
				case (QueryOperationType.Multiple):
					if (_comparandSet || !string.IsNullOrWhiteSpace(ComparandFieldName)) return ExceptionOnError(exceptionOnError);
					if (SubQueries != null && SubQueries.Length > 0)
					{
						foreach (var subQuery in SubQueries)
						{
							if (!subQuery.Validate(exceptionOnError)) return ExceptionOnError(exceptionOnError);
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
