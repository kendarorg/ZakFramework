using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ZakCms.Factories;
using ZakCms.MVC3.Controllers;
using ZakCms.Models;
using ZakCms.Models.Entitites;
using ZakCms.Repositories;
using ZakDb.Models;
using ZakDb.Utils;
using ZakWeb.Utils.Renderers;

namespace ZakCmsFE.Controllers
{
	public class HomeController : ZakCmsController
	{
		private readonly IArticlesRepository _articleRepository;
		private readonly IRenderer _renderer;
		private readonly IExternalParameters _externalParameters;
		private readonly ILanguagesRepository _languagesRepository;
		private readonly IArticlesToTagsRepository _articlesToTagsRepository;

		public HomeController() :
			this(
			SimpleFakeFactory.Create<IArticlesRepository>(),
			SimpleFakeFactory.Create<ILanguagesRepository>(),
			SimpleFakeFactory.Create<IArticlesToTagsRepository>(),
			SimpleFakeFactory.Create<IRenderer>(),
			SimpleFakeFactory.Create<IExternalParameters>())
		{
		}

		public HomeController(
			IArticlesRepository articleRepository,
			ILanguagesRepository languagesRepository,
			IArticlesToTagsRepository articlesToTagsRepository,
			IRenderer renderer,
			IExternalParameters externalParameters)
		{
			_articleRepository = articleRepository;
			_languagesRepository = languagesRepository;
			_articlesToTagsRepository = articlesToTagsRepository;
			_renderer = renderer;
			_externalParameters = externalParameters;
		}

		public ActionResult Index(string id = "", string langId = "")
		{
			Int64 pageId;
			Int64 numericLangId = _languagesRepository.GetIdFromCode(langId);
			if (numericLangId > 0)
			{
				_externalParameters["LanguageId"] = numericLangId;
			}
			var paavm = new PageArticleViewModel {Article = (ArticleModel) _articleRepository.GetBySeoTitle(id)};

			if (paavm.Article != null)
			{
				pageId = paavm.Article.Id;
			}
			else
			{
				if (!Int64.TryParse(id, out pageId))
				{
					pageId = 0;
				}
				else
				{
					paavm.Article = (ArticleModel) _articleRepository.GetById(pageId);
					if (paavm.Article == null) pageId = 0;
				}
			}

			var result = _articleRepository.ExecuteAction("GetTree", pageId);
			if (result != null && result.Result != null)
			{
				foreach (var item in (List<object>) result.Result)
				{
					var am = (ArticleModel) item;
					paavm.Articles.Add(am);
				}
			}

			if (paavm.Article == null)
			{
				if (paavm.Articles.Count > 0)
				{
					paavm.Article = paavm.Articles[0];
				}
				else
				{
					paavm.Article = new ArticleModel();
				}
			}
			var tags = new List<string>();
			if (paavm.Article.Id > 0)
			{
				var res = _articlesToTagsRepository.GetByOwner(paavm.Article.Id);
				if (res != null)
				{
					foreach (var item in res)
					{
						var tm = (TagModel) ((ManyToManyModel) item).Content;
						tags.Add(tm.Code);
					}
				}
			}
			ViewBag.MetaKeywords = string.Join(",", tags);
			paavm.Article.Content = _renderer.Render(paavm.Article.Content, SiteRoot, ImagesRoot);
			return View(paavm);
		}

		public ActionResult ChangeLanguage(string langId)
		{
			Int64 id = _languagesRepository.GetIdFromCode(langId);
			_externalParameters["LanguageId"] = id;
			return RedirectToAction("Index");
		}
	}
}