namespace ZakDb.Models
{
	public interface ILovModel : IModel
	{
		string Code { get; set; }
		string Description { get; set; }
	}
}