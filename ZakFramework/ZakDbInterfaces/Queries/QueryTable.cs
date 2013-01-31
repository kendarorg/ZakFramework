using System;
using System.Collections.Generic;
using System.Threading;
using ZakDb.Query;

namespace ZakDb.Queries
{
	public class QueryTable : IQueryCondition,IQueryable
	{
		private static long _aliasIndex;
		private List<JoinDescriptor> _joinTables;
		private Dictionary<string, QueryField> _fields;
		public IQueryable Parent { get; set; }
		public List<JoinDescriptor> JoinTables
		{
			get
			{
				return new List<JoinDescriptor>(_joinTables);
			}
		}

		public List<QueryField> Fields
		{
			get
			{
				return new List<QueryField>(_fields.Values);
			}
		}

		public QueryTable(string table, string alias = null)
		{
			var aliasIndex = Interlocked.Read(ref _aliasIndex);
			Conditions = null;
			Table = table;
			Alias = string.IsNullOrWhiteSpace(alias) ? string.Format("{0}_A{1}", Table, aliasIndex) : alias;
		}

		public string Table { get; private set; }
		public string Alias { get; private set; }
		public QueryCondition Conditions { get; private set; }

		public bool HasConditions { get { return Conditions != null; } }

		public QueryTable AddField(string fieldName)
		{
			if (_fields == null) _fields = new Dictionary<string, QueryField>();
			_fields.Add(fieldName, new QueryField(fieldName, this));
			return this;
		}

		public QueryTable AddField(QueryField field)
		{
			if (_fields == null) _fields = new Dictionary<string, QueryField>();
			_fields.Add(field.Name, field);
			return this;
		}

		public QueryField GetField(string fieldName)
		{
			if (_fields == null || !_fields.ContainsKey(fieldName)) return null;
			return _fields[fieldName];
		}

		public QueryTable AddFields(params string[] fieldNames)
		{
			foreach (var field in fieldNames)
			{
				AddField(field);
			}
			return this;
		}

		public QueryTable AddFields(params QueryField[] fields)
		{
			foreach (var field in fields)
			{
				AddField(field);
			}
			return this;
		}

		public QueryTable Join(QueryTable table, QueryCondition condition)
		{
			var jqt = Join(new JoinDescriptor(table, condition));
			jqt.Parent = this;
			return jqt;
		}

		
		public QueryTable Join(JoinDescriptor joinDescriptor)
		{
			if (_joinTables == null) _joinTables = new List<JoinDescriptor>();
			_joinTables.Add(joinDescriptor);
			return this;
		}

		public QueryTable Query(QueryCondition conditions)
		{
			Conditions = conditions;
			Conditions.Parent = this;
			return this;
		}

		private bool ExceptionOnError(bool exceptionOnError, bool value = false)
		{
			if (exceptionOnError && !value) throw new Exception();
			return value;
		}

		public bool Validate(bool exceptionOnError = false)
		{
			if (_joinTables != null)
			{
				foreach (var joinTable in _joinTables)
				{
					if (!joinTable.Validate(exceptionOnError)) return ExceptionOnError(exceptionOnError);
				}
			}
			if (Conditions != null)
			{
				if (!Conditions.Validate(exceptionOnError)) return ExceptionOnError(exceptionOnError);
			}
			if (string.IsNullOrWhiteSpace(Table)) return ExceptionOnError(exceptionOnError);
			if (string.IsNullOrWhiteSpace(Alias)) return ExceptionOnError(exceptionOnError);

			var aliasesDictionary = new Dictionary<string, string>();
			foreach (var alias in GetAliases())
			{
				var testAlias = alias.ToLowerInvariant();
				if (aliasesDictionary.ContainsKey(testAlias)) return ExceptionOnError(exceptionOnError);
				aliasesDictionary.Add(testAlias, alias);
			}

			return true;
		}

		private IEnumerable<string> GetAliases()
		{
			yield return Alias;
			if (_joinTables != null)
			{
				foreach (var joinTable in _joinTables)
				{
					foreach (var alias in joinTable.JoinTable.GetAliases())
					{
						yield return alias;
					}
				}
			}
		}

		/*public QueryCondition CreateCondition(string fieldName =null)
		{
			var toret = CreateEmptyCondition(fieldName);
			if (Conditions == null)
			{
				Conditions = toret;
			}
			return toret;
		}

		public QueryCondition CreateEmptyCondition(string fieldName = null)
		{
			if (string.IsNullOrWhiteSpace(fieldName))
			{
				return new QueryCondition(null, Alias);
			}
			QueryCondition toret = null;
			if (_fields != null && _fields.ContainsKey(fieldName))
			{
				toret = new QueryCondition(fieldName, Alias);
			}
			return toret;
		}*/

		public QueryCondition And(params QueryCondition[] querySelects)
		{
			var qc = new QueryCondition(null, Alias);
			Conditions = qc;
			Conditions.Parent = this;
			return qc.And(querySelects);
		}

		public QueryCondition Or(params QueryCondition[] querySelects)
		{
			var qc = new QueryCondition(null, Alias);
			Conditions = qc;
			Conditions.Parent = this;
			return qc.Or(querySelects);
		}

		public QueryCondition Not(object value = null, bool asNull = false)
		{
			var qc = new QueryCondition(null, Alias);
			Conditions = qc;
			Conditions.Parent = this;
			return qc.Not(value, asNull);
		}

		public QueryCondition Eq(object value = null, bool asNull = false)
		{
			var qc = new QueryCondition(null, Alias);
			Conditions = qc;
			Conditions.Parent = this;
			return qc.Eq(value, asNull);
		}

		public QueryCondition Neq(object value = null, bool asNull = false)
		{
			var qc = new QueryCondition(null, Alias);
			Conditions = qc;
			Conditions.Parent = this;
			return qc.Neq(value, asNull);
		}

		public QueryCondition Gt(object value)
		{
			var qc = new QueryCondition(null, Alias);
			Conditions = qc;
			Conditions.Parent = this;
			return qc.Gt(value);
		}

		public QueryCondition Gte(object value)
		{
			var qc = new QueryCondition(null, Alias);
			Conditions = qc;
			Conditions.Parent = this;
			return qc.Gte(value);
		}

		public QueryCondition Lt(object value)
		{
			var qc = new QueryCondition(null, Alias);
			Conditions = qc;
			Conditions.Parent = this;
			return qc.Lt(value);
		}

		public QueryCondition Lte(object value)
		{
			var qc = new QueryCondition(null, Alias);
			Conditions = qc;
			Conditions.Parent = this;
			return qc.Lte(value);
		}

		public QueryCondition In(params object[] values)
		{
			var qc = new QueryCondition(null, Alias);
			Conditions = qc;
			Conditions.Parent = this;
			return qc.In(values);
		}

		public QueryCondition IsNull()
		{
			var qc = new QueryCondition(null, Alias);
			Conditions = qc;
			Conditions.Parent = this;
			return qc.IsNull();
		}

		public QueryCondition IsNotNull()
		{
			var qc = new QueryCondition(null, Alias);
			Conditions = qc;
			Conditions.Parent = this;
			return qc.IsNotNull();
		}
	}
}
