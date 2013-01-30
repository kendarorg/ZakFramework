using System.Collections.Generic;
using System.Linq;
using ZakDb.Models;

namespace ZakDb.Linq
{
	public class ModelComparer<T> : IEqualityComparer<T>
		where T : IModel
	{
		public bool Equals(T x, T y)
		{
			return x.Id == y.Id;
		}

		public int GetHashCode(T obj)
		{
			return 1;
		}
	}

	public static class LinqExtensions
	{
		public static IEnumerable<T> ExceptModel<T>
			(this IEnumerable<T> first, IEnumerable<T> second)
			where T : IModel
		{
			return first.Except(second, new ModelComparer<T>());
		}
	}
}