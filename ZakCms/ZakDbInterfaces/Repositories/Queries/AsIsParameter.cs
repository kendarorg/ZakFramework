namespace ZakDb.Repositories.Queries
{
	public class AsIsParameter
	{
		public string Content { get; set; }

		public override string ToString()
		{
			return Content;
		}
	}
}