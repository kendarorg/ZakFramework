using System;
using ZakCore.Utils.Logging;
using ZakThread.Threading.Enums;

namespace ZakThread.Threading
{
	public interface IBaseThread
	{
		ILogger Logger { get; }

		/// <summary>
		/// String identifier of the thread
		/// </summary>
		String ThreadName { get; }

		/// <summary>
		/// Integer identifier of the thread
		/// </summary>
		UInt16 ThreadId { get; }

		/// <summary>
		/// Status of the object.
		/// </summary>
		RunningStatus Status { get; }

		/// <summary>
		/// Rerieve the last errors. When an error is parsed, the exception is lost.
		/// </summary>
		Exception LastError { get; }

		/// <summary>
		/// Start the thread
		/// </summary>
		void RunThread(int timeoutMs = 1000);

		/// <summary>
		/// Terminate the thread
		/// </summary>
		/// <param name="force">If true, abort!!!</param>
		void Terminate(Boolean force = false);

		/// <summary>
		/// Wait for the thread to terminate
		/// </summary>
		/// <param name="timeoutMs">ms to wait for the thread termination.</param>
		void WaitTermination(Int64 timeoutMs);
	}
}