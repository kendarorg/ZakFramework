// -----------------------------------------------------------------------
// <copyright file="RepositoryTypeDescriptor.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text.RegularExpressions;

namespace ZakDb.Repositories
{
	public class RepositoryTypeDescriptor
	{
		public Type DataType { get; set; }
		public int MinLength { get; set; }
		public int MaxLength { get; set; }

		public object MinValue { get; set; }
		public object MaxValue { get; set; }
		public object Default { get; set; }
		public Regex[] ValidationRegex { get; set; }
	}
}
