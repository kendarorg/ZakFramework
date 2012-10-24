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
	public class BEUsersController : ZakCmsController
	{
		private readonly IBEUsersRepository _usersRepository;

		public override bool NeedAuthentication
		{
			get { return true; }
		}

		public BEUsersController() :
			this(SimpleFakeFactory.Create<IBEUsersRepository>())
		{
		}

		public BEUsersController(IBEUsersRepository userRepository)
		{
			_usersRepository = userRepository;
		}

		public ActionResult Index(Int64 id = 0, string view = "Details")
		{
			var paavm = new PageUserAdminViewModel
				{
					View = view
				};
			var result = _usersRepository.GetAll();
			if (result != null)
			{
				foreach (var item in result)
				{
					paavm.Users.Add((UserModel) item);
				}
			}
			paavm.User = (UserModel) _usersRepository.GetById(id);
			if (paavm.User == null)
			{
				if (paavm.Users.Count > 0)
				{
					paavm.User = paavm.Users[0];
				}
				else
				{
					paavm.User = new UserModel {Id = 0};
				}
			}
			return View(paavm);
		}

		[HttpPost]
		public ActionResult Create(UserModel item)
		{
			try
			{
				Int64 created = _usersRepository.Create(item);
				return RedirectToAction("Index", new {id = created, view = "Details"});
			}
			catch
			{
				return View();
			}
		}


		[HttpPost]
		public ActionResult Edit(UserModel item)
		{
			try
			{
				var prv = _usersRepository.GetById(item.Id, new QueryObject {UseJoins = false}) as UserModel;
				if (prv != null) item.UserId = prv.UserId;
				_usersRepository.Update(item);
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
				_usersRepository.Delete(id);
				return RedirectToAction("Index");
			}
			catch
			{
				return View("Error");
			}
		}

		public ActionResult Edit(Int64 id)
		{
			return View(_usersRepository.GetById(id));
		}
	}
}