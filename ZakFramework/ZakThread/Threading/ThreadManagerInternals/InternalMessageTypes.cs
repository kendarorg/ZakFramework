namespace ZakThread.Threading.ThreadManagerInternals
{
	public enum InternalMessageTypes
	{
		AddThread = 0,
		RemoveThread = 1,
		Terminate = 2,
		RunThread,
		RegisterMessageType
	}
}