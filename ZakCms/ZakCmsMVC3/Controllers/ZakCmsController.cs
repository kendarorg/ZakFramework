using System.Configuration;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ZakCms.MVC3.Controllers
{
	public class ZakCmsController : Controller
	{
		public void SetAuthenticationStatus(bool authenticated)
		{
			System.Web.HttpContext.Current.Session["IsAuthenticated"] = authenticated &&
			                                                            HttpContext.Request.IsAuthenticated;
		}

		public ZakCmsController()
		{
			SiteRoot = ConfigurationManager.AppSettings["CmsSiteRoot"];
			ImagesRoot = ConfigurationManager.AppSettings["CmsImagesRoot"];
		}

		protected override void OnActionExecuted(ActionExecutedContext ctx)
		{
			base.OnActionExecuted(ctx);
			IsAuthenticated = HttpContext.Request.IsAuthenticated;
			if (NeedAuthentication && !IsAuthenticated)
			{
				throw new HttpException((int) HttpStatusCode.Unauthorized, "Forbidden");
			}
		}

		public virtual bool NeedAuthentication
		{
			get { return false; }
		}

		public bool IsAuthenticated { get; private set; }
		public string SiteRoot { get; set; }
		public string ImagesRoot { get; set; }
	}
}