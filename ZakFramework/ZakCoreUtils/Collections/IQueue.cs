using System;
using System.Collections.Generic;

namespace ZakCore.Utils.Collections
{
	public interface IQueue<T>
	{
		long Count { get; }

		/// <summary>
		/// Dequeue a single element
		/// </summary>
		/// <param name="t">The element to dequeue</param>
		/// <returns></returns>
		Boolean Dequeue(ref T t);

		/// <summary>
		/// Enqueue a single element
		/// </summary>
		/// <param name="t"></param>
		void Enqueue(T t);

		/// <summary>
		/// Dequeu multiple elements
		/// </summary>
		/// <param name="count">The maximum number of elements to dequeue (default Int64.MaxValue) </param>
		/// <returns></returns>
		IEnumerable<T> Dequeue(Int64 count = Int64.MaxValue);

		T DequeueSingle();

		/// <summary>
		/// Enqueue a list of values
		/// </summary>
		/// <param name="toInsert"></param>
		void Enqueue(List<T> toInsert);

		void Clear();
	}
}