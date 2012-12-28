
namespace _003AConcurrentTreeStructure
{
	internal class Program
	{
		const string STARTING_PATH = @"D:\Development";

		// ReSharper disable UnusedParameter.Local
		private static void Main(string[] args)
		// ReSharper restore UnusedParameter.Local
		{
			
			SingleThreadProgram.SingleThreadAccess();
			MultiThreadProgram.MultiThreadAccess(STARTING_PATH, 50);
		}
	}
}