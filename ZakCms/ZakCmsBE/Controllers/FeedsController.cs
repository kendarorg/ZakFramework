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
using ZakDb.Utils;
using ZakWeb.Utils.Commons;

namespace ZakCmsBE.Controllers
{
	public class FeedsController : ZakCmsController
	{
		private readonly IFeedsRepository _feedRepository;
		private readonly ILanguagesRepository _languagesRepository;
		private readonly IExternalParameters _externalParameters;
		private readonly ICompaniesRepository _companiesRepository;
		private readonly IFeedsToTagsRepository _feedsToTagsRepository;
		private readonly ITagsRepository _tagsRepository;

		public override bool NeedAuthentication
		{
			get { return true; }
		}

		public FeedsController() :
			this(
			SimpleFakeFactory.Create<IFeedsRepository>(),
			SimpleFakeFactory.Create<ILanguagesRepository>(),
			SimpleFakeFactory.Create<ICompaniesRepository>(),
			SimpleFakeFactory.Create<IFeedsToTagsRepository>(),
			SimpleFakeFactory.Create<ITagsRepository>(),
			SimpleFakeFactory.Create<IExternalParameters>())
		{
		}

		public FeedsController(
			IFeedsRepository feedRepository,
			ILanguagesRepository languagesRepository,
			ICompaniesRepository companiesRepository,
			IFeedsToTagsRepository feedsToTagsRepository,
			ITagsRepository tagsRepository,
			IExternalParameters externalParameters)
		{
			_feedRepository = feedRepository;
			_languagesRepository = languagesRepository;
			_externalParameters = externalParameters;
			_companiesRepository = companiesRepository;
			_feedsToTagsRepository = feedsToTagsRepository;
			_tagsRepository = tagsRepository;
		}

		public ActionResult Index(Int64 id = 0, string view = "Details", string generatedSeoTitle = "")
		{
			var paavm = new PageFeedAdminViewModel
				{
					View = view,
					Languages = new List<LanguageModel>(_languagesRepository.GetAll().ConvertAll(k => (LanguageModel) k)),
					Companies = new List<CompanyModel>(_companiesRepository.GetAll().ConvertAll(k => (CompanyModel) k))
				};
			var result = _feedRepository.GetAll();
			if (result != null)
			{
				foreach (var item in result)
				{
					paavm.Feeds.Add((FeedModel) item);
				}
			}
			paavm.Feed = (FeedModel) _feedRepository.GetById(id);
			if (paavm.Feed == null)
			{
				if (paavm.Feeds.Count > 0)
				{
					paavm.Feed = paavm.Feeds[0];
				}
				else
				{
					paavm.Feed = new FeedModel
						{
							Id = 0,
							Language = new LanguageModel((long) _externalParameters["LanguageId"]),
							Company = new CompanyModel((long) _externalParameters["CompanyId"])
						};
				}
			}


			if (view == "Edit")
			{
				if (!string.IsNullOrWhiteSpace(generatedSeoTitle))
				{
					paavm.Feed.SeoTitle = generatedSeoTitle;
				}
				var avaialableTags = new List<TagModel>();
				var resat = _tagsRepository.GetAll();
				foreach (var item in resat)
				{
					avaialableTags.Add((TagModel) item);
				}
				var resut = _feedsToTagsRepository.GetByOwner(paavm.Feed.Id);
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
				if (paavm.Feed.Id > 0)
				{
					var res = _feedsToTagsRepository.GetByOwner(paavm.Feed.Id);
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
		public ActionResult Create(FeedModel feed)
		{
			try
			{
				if (feed.Title == null) feed.Title = string.Empty;
				feed.Company = new CompanyModel((Int64) _externalParameters["CompanyId"]);
				feed.Language = new LanguageModel((Int64) _externalParameters["LanguageId"]);
				Int64 created = _feedRepository.Create(feed);
				return RedirectToAction("Index", new {id = created, view = "Details"});
			}
			catch
			{
				return View();
			}
		}

		public ActionResult Edit(Int64 id)
		{
			return View(_feedRepository.GetById(id));
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult Edit(FeedModel feed)
		{
			try
			{
				if (feed.Title == null) feed.Title = string.Empty;
				feed.Company = new CompanyModel((Int64) _externalParameters["CompanyId"]);
				feed.Language = new LanguageModel((Int64) _externalParameters["LanguageId"]);
				_feedRepository.Update(feed);
				return RedirectToAction("Index", new {id = feed.Id});
			}
			catch
			{
				return View();
			}
		}

		public ActionResult Delete(Int64 id)
		{
			try
			{
				_feedRepository.Delete(id);
				return RedirectToAction("Index");
			}
			catch
			{
				return View("Error");
			}
		}

		public ActionResult ChooseLanguage(PageFeedAdminViewModel model)
		{
			_externalParameters["LanguageId"] = model.Feed.Language.Id;
			return RedirectToAction("Index");
		}

		public ActionResult ChooseCompany(PageFeedAdminViewModel model)
		{
			_externalParameters["CompanyId"] = model.Feed.Company.Id;
			return RedirectToAction("Index");
		}

		public ActionResult GenerateSeoTitle(Int64 id, string generateSeoTitleOriginal)
		{
			Int64 idValue = id;
			string newTitle = SeoUtils.BuildSeoFriendlyName(generateSeoTitleOriginal);
			return RedirectToAction("Index", new {id = idValue, view = "Edit", generatedSeoTitle = newTitle});
		}

		public ActionResult RemoveTag(long feedid, long tagid)
		{
			_feedsToTagsRepository.Delete(feedid, tagid);
			return RedirectToAction("Index", new {id = feedid, view = "Edit"});
		}

		public ActionResult AddTag(Int64 feedId, Int64 tagid)
		{
			_feedsToTagsRepository.Create(feedId, tagid);
			return RedirectToAction("Index", new {id = feedId, view = "Edit"});
		}
	}
}