namespace ZakDb.Plugins
{
	public class PluginOutcome
	{
		public PluginOutcome()
		{
			Result = null;
			Success = false;
		}

		public object Result { get; set; }
		public bool Success { get; set; }
	}
}