using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ZakCms.Factories;
using ZakCms.MVC3.Filters;

namespace ZakCmsFE
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : HttpApplication
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new ZakCmsExceptionFilter());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Default", // Route name
				"Feeds/{langId}/{id}", // URL with parameters
				new
					{
						controller = "Feeds",
						action = "Index",
						id = "none",
						langId = (string) SimpleFakeFactory.Create("CmsLanguageString")
					}); // Parameter defaults*/

			routes.MapRoute(
				"Pages", // Route name
				"{langId}/{id}", // URL with parameters
				new
					{
						controller = "Home",
						action = "Index",
						id = "none",
						langId = (string) SimpleFakeFactory.Create("CmsLanguageString")
					}); // Parameter defaults

			routes.MapRoute(
				"LanguagesSelection", // Route name
				"{langId}/{controller}/{action}/{id}", // URL with parameters
				new
					{
						controller = "Home",
						action = "Index",
						id = "none",
						langId = (string) SimpleFakeFactory.Create("CmsLanguageString")
					}); // Parameter defaults
		}

		protected void Application_Start()
		{
			SimpleFakeFactory.InitializeSimpleFakeFactory(false);

			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}
	}
}