using System.Web.Mvc;
using ZakCms.MVC3.Controllers;

namespace ZakCmsBE.Controllers
{
	public class HomeController : ZakCmsController
	{
		public ActionResult Index()
		{
			ViewBag.Message = "Welcome to ZakCms BackEnd!";

			if (!IsAuthenticated)
			{
				return RedirectToAction("LogOn", "Account");
			}
			return RedirectToAction("Index", "Articles");
		}

		public ActionResult About()
		{
			return View();
		}
	}
}