using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ZakCms.Factories;
using ZakCms.MVC3.Controllers;
using ZakCms.Models.Entitites;
using ZakCms.Repositories;
using ZakCmsFE.Models;

namespace ZakCmsFE.Controllers
{
	public class FeedsController : ZakCmsController
	{
		private readonly IFeedsRepository _feedsRepository;
		private readonly IFeedsContentRepository _feedsContentRepository;

		public FeedsController() :
			this(SimpleFakeFactory.Create<IFeedsRepository>(), SimpleFakeFactory.Create<IFeedsContentRepository>())
		{
		}

		public FeedsController(IFeedsRepository feedsRepository, IFeedsContentRepository feedsContentRepository)
		{
			_feedsRepository = feedsRepository;
			_feedsContentRepository = feedsContentRepository;
		}

		public ActionResult Details(string id, string langId)
		{
			int idx = id.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase);
			if (idx > 0)
			{
				id = id.Substring(0, idx);
			}

			var facvm = new FeedAndContentViewModel {Feed = (FeedModel) _feedsRepository.GetBySeoTitle(id)};
			if (facvm.Feed == null)
			{
				Int64 feedId;
				facvm.Feed = new FeedModel();
				if (Int64.TryParse(id, out feedId))
				{
					facvm.Feed = (FeedModel) _feedsRepository.GetById(feedId);
					if (facvm.Feed == null) facvm.Feed = new FeedModel();
				}
			}
			if (facvm.Feed.Id != 0)
			{
				var allCnts = _feedsContentRepository.GetByFeedId(facvm.Feed.Id);
				foreach (var cnt in allCnts)
				{
					facvm.FeedContent.Add((FeedContentModel) cnt);
				}
			}
			if (facvm.FeedContent.Count > 0)
			{
				facvm.LastBuildDate = facvm.FeedContent[0].UpdateTime;
			}
			else
			{
				facvm.LastBuildDate = DateTime.MinValue;
			}
			return View(facvm);
		}

		public ActionResult Index()
		{
			var feeds = _feedsRepository.GetAll();
			var feedsList = new List<FeedModel>();
			foreach (var feed in feeds)
			{
				feedsList.Add((FeedModel) feed);
			}
			return View(feedsList);
		}
	}
}