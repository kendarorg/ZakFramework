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
	public class LanguagesController : ZakCmsController
	{
		private readonly ILanguagesRepository _languagesRepository;

		public override bool NeedAuthentication
		{
			get { return true; }
		}

		public LanguagesController() :
			this(SimpleFakeFactory.Create<ILanguagesRepository>())
		{
		}

		public LanguagesController(ILanguagesRepository articleRepository)
		{
			_languagesRepository = articleRepository;
		}

		public ActionResult Index(Int64 id = 0, string view = "Details")
		{
			var paavm = new PageLanguageAdminViewModel
				{
					View = view
				};
			var result = _languagesRepository.GetAll();
			if (result != null)
			{
				foreach (var item in result)
				{
					paavm.Languages.Add((LanguageModel) item);
				}
			}
			paavm.Language = (LanguageModel) _languagesRepository.GetById(id);
			if (paavm.Language == null)
			{
				if (paavm.Languages.Count > 0)
				{
					paavm.Language = paavm.Languages[0];
				}
				else
				{
					paavm.Language = new LanguageModel {Id = 0};
				}
			}
			return View(paavm);
		}

		[HttpPost]
		public ActionResult Create(LanguageModel item)
		{
			try
			{
				if (item.Description == null) item.Description = string.Empty;
				Int64 created = _languagesRepository.Create(item);
				return RedirectToAction("Index", new {id = created, view = "Details"});
			}
			catch
			{
				return View();
			}
		}


		[HttpPost]
		public ActionResult Edit(LanguageModel item)
		{
			try
			{
				if (item.Description == null) item.Description = string.Empty;
				var prv = _languagesRepository.GetById(item.Id, new QueryObject {UseJoins = false}) as LanguageModel;
				if (prv != null) item.Code = prv.Code;
				_languagesRepository.Update(item);
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
				_languagesRepository.Delete(id);
				return RedirectToAction("Index");
			}
			catch
			{
				return View("Error");
			}
		}

		public ActionResult Edit(Int64 id)
		{
			return View(_languagesRepository.GetById(id));
		}
	}
}