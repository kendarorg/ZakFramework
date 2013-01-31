
using System;

namespace ZakDb.Descriptors
{
	public class KeyDescriptor
	{
		public bool Ascending { get; private set; }
		public bool Unique { get; internal set; }
		public bool AutoIncrement { get; private set; }
		public string[] Fields { get; private set; }

		public KeyDescriptor(params string[] fields)
		{
			Fields = fields;
		}

		public void SetType(bool unique,bool autoincrement,bool ascending)
		{
			Ascending = ascending;
			Unique = unique;
			AutoIncrement = autoincrement;
		}
	}
}
