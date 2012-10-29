namespace ZakThread.Threading.Enums
{
	/// <summary>
	/// Thread status.
	/// </summary>
	public enum RunningStatus
	{
		/// <summary>
		/// Thread created or resetted
		/// </summary>
		None = 0,

		/// <summary>
		/// Thread initialized
		/// </summary>
		Initialized = 1,

		/// <summary>
		/// Thread running
		/// </summary>
		Running = 2,

		/// <summary>
		/// Thread scheduled for halting
		/// </summary>
		Halting = 3,

		/// <summary>
		/// Thread halted
		/// </summary>
		Halted = 4,

		/// <summary>
		/// Thread aborted
		/// </summary>
		Aborted = 5,

		/// <summary>
		/// Some exception had been thrown
		/// </summary>
		ExceptionThrown = 6
	}
}