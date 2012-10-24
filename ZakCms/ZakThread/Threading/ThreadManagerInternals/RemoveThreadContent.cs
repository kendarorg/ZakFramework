namespace ZakThread.Threading.ThreadManagerInternals
{
	public class RemoveThreadContent
	{
		public bool ForceHalt { get; set; }
		public string ThreadName { get; set; }

		public RemoveThreadContent(IBaseMessageThread messageThread,bool forceHalt)
		{
			ForceHalt = forceHalt;
			ThreadName = messageThread.ThreadName;
		}

		public RemoveThreadContent(string threadName, bool forceHalt)
		{
			ForceHalt = forceHalt;
			ThreadName = threadName;
		}
	}
}
