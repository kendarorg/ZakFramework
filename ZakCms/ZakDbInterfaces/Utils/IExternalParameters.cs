namespace ZakDb.Utils
{
	public interface IExternalParameters
	{
		object this[string index] { get; set; }
	}
}