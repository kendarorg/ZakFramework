using System.Collections.Generic;
using System.Web.Mvc;
using ZakCms.Factories;
using ZakCms.MVC3.Controllers;
using ZakCms.Models.Entitites;
using ZakCms.Repositories;

namespace ZakCmsFE.Controllers
{
	public class LanguagesController : ZakCmsController
	{
		private readonly ILanguagesRepository _languagesRepository;

		public LanguagesController() :
			this(SimpleFakeFactory.Create<ILanguagesRepository>())
		{
		}

		private LanguagesController(ILanguagesRepository languagesRepository)
		{
			_languagesRepository = languagesRepository;
		}

		public ActionResult Index()
		{
			var languages = new List<LanguageModel>();
			var result = _languagesRepository.GetAll();
			if (result != null)
			{
				foreach (var item in result)
				{
					if (((LanguageModel) item).Code != "defau")
					{
						languages.Add((LanguageModel) item);
					}
				}
			}
			return PartialView("Index", languages);
		}
	}
}