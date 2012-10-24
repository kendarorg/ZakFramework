using System.Web.Mvc;
using ZakDb.Repositories.Exceptions;

namespace ZakCms.MVC3.Filters
{
	public class ZakCmsExceptionFilter : HandleErrorAttribute
	{
		public override void OnException(ExceptionContext filterContext)
		{
			if (filterContext.Exception is RepositoryValidationException)
			{
				var validationException = (RepositoryValidationException) filterContext.Exception;
				filterContext.ExceptionHandled = true;
				filterContext.Controller.ViewData.ModelState.AddModelError(validationException.Field, validationException.Error);
				filterContext.Controller.ViewData.ModelState.AddModelError(string.Empty, validationException.Message);
			}
			else if (filterContext.Exception is RepositoryDuplicateKeyException)
			{
				var duplicateException = (RepositoryDuplicateKeyException) filterContext.Exception;
				filterContext.ExceptionHandled = true;
				filterContext.Controller.ViewData.ModelState.AddModelError(string.Empty, duplicateException.Message);
			}
			else
			{
				filterContext.ExceptionHandled = true;
				filterContext.Controller.ViewData.ModelState.AddModelError(string.Empty, filterContext.Exception.Message);
			}
		}
	}
}