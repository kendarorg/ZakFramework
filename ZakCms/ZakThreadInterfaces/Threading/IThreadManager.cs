namespace ZakThread.Threading
{
	public interface IThreadManager:IBaseMessageThread
	{
		void AddThread(IBaseMessageThread messageThread);
		void RemoveThread(IBaseMessageThread messageThread,bool forceHalt = false);
		void RemoveThread(string messageThreadName, bool forceHalt = false);
		void RunThread(string messageThreadName);
	}
}
