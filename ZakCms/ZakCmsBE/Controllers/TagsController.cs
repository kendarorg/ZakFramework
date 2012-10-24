using System;
using System.Web.Mvc;
using ZakCms.Factories;
using ZakCms.MVC3.Controllers;
using ZakCms.Models;
using ZakCms.Models.Entitites;
using ZakCms.Repositories;
using ZakDb.Repositories.Queries;

namespace ZakCmsBE.Controllers
{
	public class TagsController : ZakCmsController
	{
		private readonly ITagsRepository _tagsRepository;

		public override bool NeedAuthentication
		{
			get { return true; }
		}

		public TagsController() :
			this(SimpleFakeFactory.Create<ITagsRepository>())
		{
		}

		public TagsController(ITagsRepository articleRepository)
		{
			_tagsRepository = articleRepository;
		}

		public ActionResult Index(Int64 id = 0, string view = "Details")
		{
			var paavm = new PageTagAdminViewModel
				{
					View = view
				};
			var result = _tagsRepository.GetAll();
			if (result != null)
			{
				foreach (var item in result)
				{
					paavm.Tags.Add((TagModel) item);
				}
			}
			paavm.Tag = (TagModel) _tagsRepository.GetById(id);
			if (paavm.Tag == null)
			{
				if (paavm.Tags.Count > 0)
				{
					paavm.Tag = paavm.Tags[0];
				}
				else
				{
					paavm.Tag = new TagModel {Id = 0};
				}
			}
			return View(paavm);
		}

		[HttpPost]
		public ActionResult Create(TagModel item)
		{
			try
			{
				if (item.Description == null) item.Description = string.Empty;
				Int64 created = _tagsRepository.Create(item);
				return RedirectToAction("Index", new {id = created, view = "Details"});
			}
			catch
			{
				return View();
			}
		}


		[HttpPost]
		public ActionResult Edit(TagModel item)
		{
			try
			{
				if (item.Description == null) item.Description = string.Empty;
				var prv = _tagsRepository.GetById(item.Id, new QueryObject {UseJoins = false}) as TagModel;
				if (prv != null) item.Code = prv.Code;
				_tagsRepository.Update(item);
				return RedirectToAction("Index", new {id = item.Id});
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
				_tagsRepository.Delete(id);
				return RedirectToAction("Index");
			}
			catch
			{
				return View("Error");
			}
		}

		public ActionResult Edit(Int64 id)
		{
			return View(_tagsRepository.GetById(id));
		}
	}
}