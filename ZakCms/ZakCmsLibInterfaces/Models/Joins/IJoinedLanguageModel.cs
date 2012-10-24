using ZakCms.Models.Entitites;
using ZakDb.Models;

namespace ZakCms.Models.Joins
{
	public interface IJoinedLanguageModel : IModel
	{
		LanguageModel Language { get; set; }
	}
}