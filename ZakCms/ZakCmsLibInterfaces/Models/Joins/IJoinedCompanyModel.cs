using ZakCms.Models.Entitites;
using ZakDb.Models;

namespace ZakCms.Models.Joins
{
	public interface IJoinedCompanyModel : IModel
	{
		CompanyModel Company { get; set; }
	}
}