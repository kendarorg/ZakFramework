using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ZakCms.Factories;
using ZakCms.MVC3.Controllers;
using ZakCms.Models;
using ZakCms.Models.Entitites;
using ZakCms.Repositories;
using ZakDb.Linq;
using ZakDb.Models;
using ZakDb.Repositories.Queries;
using ZakDb.Utils;
using ZakWeb.Utils.Commons;
using ZakWeb.Utils.Renderers;

namespace ZakCmsBE.Controllers
{
	public class ArticlesController : ZakCmsController
	{
		private readonly IArticlesRepository _articleRepository;
		private readonly ILanguagesRepository _languagesRepository;
		private readonly IRenderer _renderer;
		private readonly IExternalParameters _externalParameters;
		private readonly ICompaniesRepository _companiesRepository;
		private readonly IArticlesToTagsRepository _articlesToTagsRepository;
		private readonly ITagsRepository _tagsRepository;

		public override bool NeedAuthentication
		{
			get { return true; }
		}

		public ArticlesController() :
			this(
			SimpleFakeFactory.Create<IArticlesRepository>(),
			SimpleFakeFactory.Create<ILanguagesRepository>(),
			SimpleFakeFactory.Create<ICompaniesRepository>(),
			SimpleFakeFactory.Create<IArticlesToTagsRepository>(),
			SimpleFakeFactory.Create<ITagsRepository>(),
			SimpleFakeFactory.Create<IRenderer>(),
			SimpleFakeFactory.Create<IExternalParameters>())
		{
		}

		public ArticlesController(
			IArticlesRepository articleRepository,
			ILanguagesRepository languagesRepository,
			ICompaniesRepository companiesRepository,
			IArticlesToTagsRepository articlesToTagsRepository,
			ITagsRepository tagsRepository,
			IRenderer renderer, IExternalParameters externalParameters)
		{
			_articleRepository = articleRepository;
			_languagesRepository = languagesRepository;
			_renderer = renderer;
			_externalParameters = externalParameters;
			_companiesRepository = companiesRepository;
			_articlesToTagsRepository = articlesToTagsRepository;
			_tagsRepository = tagsRepository;
		}

		public ActionResult Index(Int64 id = 0, string view = "Details", string generatedSeoTitle = "")
		{
			var paavm = new PageArticleAdminViewModel
				{
					View = view,
					Article = (ArticleModel) _articleRepository.GetById(id),
					Languages = new List<LanguageModel>(_languagesRepository.GetAll().ConvertAll(k => (LanguageModel) k)),
					Companies = new List<CompanyModel>(_companiesRepository.GetAll().ConvertAll(k => (CompanyModel) k))
				};
			if (paavm.Article == null)
			{
				paavm.Article = new ArticleModel
					{
						Id = 0,
						Language = new LanguageModel((long) _externalParameters["LanguageId"]),
						Company = new CompanyModel((long) _externalParameters["CompanyId"])
					};
			}

			var result = _articleRepository.ExecuteAction("GetTree", id);
			if (result != null && result.Result != null)
			{
				foreach (var item in (List<object>) result.Result)
				{
					var el = (ArticleModel) item;
					if (paavm.Article == null)
					{
						paavm.Article = el;
					}
					paavm.Articles.Add((ArticleModel) item);
				}
			}


			if (view == "Edit")
			{
				if (!string.IsNullOrWhiteSpace(generatedSeoTitle))
				{
					paavm.Article.SeoTitle = generatedSeoTitle;
				}
				var avaialableTags = new List<TagModel>();
				var resat = _tagsRepository.GetAll();
				foreach (var item in resat)
				{
					avaialableTags.Add((TagModel) item);
				}
				var resut = _articlesToTagsRepository.GetByOwner(paavm.Article.Id);
				if (resut != null)
				{
					foreach (var item in resut)
					{
						var tm = (TagModel) ((ManyToManyModel) item).Content;
						paavm.Tags.Add(tm);
					}
				}

				paavm.AvailableTags = new List<TagModel>(avaialableTags.ExceptModel(paavm.Tags));
			}
			if (view == "Details")
			{
				paavm.Article.Content = _renderer.Render(paavm.Article.Content, SiteRoot, ImagesRoot);
				if (paavm.Article.Id > 0)
				{
					var res = _articlesToTagsRepository.GetByOwner(paavm.Article.Id);
					if (res != null)
					{
						foreach (var item in res)
						{
							var tm = (TagModel) ((ManyToManyModel) item).Content;
							paavm.Tags.Add(tm);
						}
					}
				}
			}
			return View(paavm);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult AddChild(ArticleModel article)
		{
			try
			{
				if (article.Content == null) article.Content = string.Empty;
				if (article.Title == null) article.Title = string.Empty;
				article.Company = new CompanyModel((Int64) _externalParameters["CompanyId"]);
				article.Language = new LanguageModel((Int64) _externalParameters["LanguageId"]);
				Int64 created = _articleRepository.Create(article);
				return RedirectToAction("Index", new {id = created, view = "Details"});
			}
			catch
			{
				return View();
			}
		}

		public ActionResult Edit(Int64 id)
		{
			return View(_articleRepository.GetById(id));
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult Edit(ArticleModel article)
		{
			try
			{
				if (article.Content == null) article.Content = string.Empty;
				if (article.Title == null) article.Title = string.Empty;
				article.Company = new CompanyModel((Int64) _externalParameters["CompanyId"]);
				article.Language = new LanguageModel((Int64) _externalParameters["LanguageId"]);
				_articleRepository.Update(article);
				return RedirectToAction("Index", new {id = article.Id});
			}
			catch
			{
				return View();
			}
		}

		public ActionResult MoveUp(Int64 id)
		{
			Int64 elId = id;
			var am = (ArticleModel) _articleRepository.GetById(elId, new QueryObject {UseJoins = false});
			_articleRepository.ExecuteAction("MoveUp", am);
			return RedirectToAction("Index", new {id = elId, view = "Details"});
		}

		public ActionResult MoveDown(Int64 id)
		{
			Int64 elId = id;
			var am = (ArticleModel) _articleRepository.GetById(elId, new QueryObject {UseJoins = false});
			_articleRepository.ExecuteAction("MoveDown", am);
			return RedirectToAction("Index", new {id = elId, view = "Details"});
		}

		public ActionResult Delete(Int64 id)
		{
			try
			{
				_articleRepository.Delete(id);
				return RedirectToAction("Index");
			}
			catch
			{
				return View("Error");
			}
		}

		public ActionResult ChooseLanguage(PageArticleAdminViewModel model)
		{
			_externalParameters["LanguageId"] = model.Article.Language.Id;
			return RedirectToAction("Index");
		}

		public ActionResult ChooseCompany(PageArticleAdminViewModel model)
		{
			_externalParameters["CompanyId"] = model.Article.Company.Id;
			return RedirectToAction("Index");
		}

		public ActionResult GenerateSeoTitle(Int64 id, string generateSeoTitleOriginal)
		{
			Int64 idValue = id;
			string newTitle = SeoUtils.BuildSeoFriendlyName(generateSeoTitleOriginal);
			return RedirectToAction("Index", new {id = idValue, view = "Edit", generatedSeoTitle = newTitle});
		}

		public ActionResult RemoveTag(long articleid, long tagid)
		{
			_articlesToTagsRepository.Delete(articleid, tagid);
			return RedirectToAction("Index", new {id = articleid, view = "Edit"});
		}

		public ActionResult AddTag(Int64 articleId, Int64 tagid)
		{
			_articlesToTagsRepository.Create(articleId, tagid);
			return RedirectToAction("Index", new {id = articleId, view = "Edit"});
		}
	}
}