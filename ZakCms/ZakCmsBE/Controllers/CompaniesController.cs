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
	public class CompaniesController : ZakCmsController
	{
		private readonly ICompaniesRepository _companiesRepository;

		public override bool NeedAuthentication
		{
			get { return true; }
		}

		public CompaniesController() :
			this(SimpleFakeFactory.Create<ICompaniesRepository>())
		{
		}

		public CompaniesController(ICompaniesRepository articleRepository)
		{
			_companiesRepository = articleRepository;
		}

		public ActionResult Index(Int64 id = 0, string view = "Details")
		{
			var paavm = new PageCompanyAdminViewModel
				{
					View = view
				};
			var result = _companiesRepository.GetAll();
			if (result != null)
			{
				foreach (var item in result)
				{
					paavm.Companies.Add((CompanyModel) item);
				}
			}
			paavm.Company = (CompanyModel) _companiesRepository.GetById(id);
			if (paavm.Company == null)
			{
				if (paavm.Companies.Count > 0)
				{
					paavm.Company = paavm.Companies[0];
				}
				else
				{
					paavm.Company = new CompanyModel {Id = 0};
				}
			}
			return View(paavm);
		}

		[HttpPost]
		public ActionResult Create(CompanyModel item)
		{
			try
			{
				if (item.Description == null) item.Description = string.Empty;
				Int64 created = _companiesRepository.Create(item);
				return RedirectToAction("Index", new {id = created, view = "Details"});
			}
			catch
			{
				return View();
			}
		}


		[HttpPost]
		public ActionResult Edit(CompanyModel item)
		{
			try
			{
				if (item.Description == null) item.Description = string.Empty;
				var prv = _companiesRepository.GetById(item.Id, new QueryObject {UseJoins = false}) as CompanyModel;
				if (prv != null) item.Code = prv.Code;
				_companiesRepository.Update(item);
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
				_companiesRepository.Delete(id);
				return RedirectToAction("Index");
			}
			catch
			{
				return View("Error");
			}
		}

		public ActionResult Edit(Int64 id)
		{
			return View(_companiesRepository.GetById(id));
		}
	}
}