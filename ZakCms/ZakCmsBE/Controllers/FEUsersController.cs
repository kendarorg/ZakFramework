using ZakCms.Factories;
using ZakCms.Repositories;

namespace ZakCmsBE.Controllers
{
	public class FEUsersController : BEUsersController
	{
		public FEUsersController() :
			this(SimpleFakeFactory.Create<IFEUsersRepository>())
		{
		}

		public FEUsersController(IFEUsersRepository userRepository) :
			base(userRepository)
		{
		}
	}
}