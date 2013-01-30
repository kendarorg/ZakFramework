using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ZakDb.Repositories.Utils
{
	public class ZakDataReader
	{
		private readonly SqlDataReader _reader;
		private readonly Dictionary<string, string> _fields;

		public ZakDataReader(SqlDataReader reader)
		{
			_reader = reader;
			int fieldsCount = _reader.FieldCount - 1;
			_fields = new Dictionary<string, string>();
			while (fieldsCount >= 0)
			{
				string fieldName = _reader.GetName(fieldsCount);
				if (!_fields.ContainsKey(fieldName))
				{
					_fields.Add(fieldName, string.Empty);
				}

				fieldsCount--;
			}
		}

		public object this[int i]
		{
			get { return _reader[i]; }
		}

		public object this[string i]
		{
			get
			{
				if (_fields.ContainsKey(i))
				{
					object ob = _reader[i];
					if (ob != DBNull.Value) return ob;
				}
				return null;
			}
		}

		public bool Read()
		{
			return _reader.Read();
		}
	}
}