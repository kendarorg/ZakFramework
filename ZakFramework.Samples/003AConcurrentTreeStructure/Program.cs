

namespace _003AConcurrentTreeStructure
{
	internal class Program
	{
		// ReSharper disable UnusedParameter.Local
		private static void Main(string[] args)
		// ReSharper restore UnusedParameter.Local
		{
			SingleThreadProgram.SingleThreadAccess();
			MultiThreadProgram.MultiThreadAccess(args[0], 50);
		}
	}
}