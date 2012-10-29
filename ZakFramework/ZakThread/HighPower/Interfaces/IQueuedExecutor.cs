namespace ZakThread.HighPower.Interfaces
{
	public interface IQueuedExecutor
	{
		object EnqueTask(AsyncTask newTask,int msTimeout = 5000);
	}
}