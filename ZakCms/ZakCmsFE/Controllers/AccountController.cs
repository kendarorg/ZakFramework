using System;
using System.Web.Mvc;
using System.Web.Security;
using ZakCms.Factories;
using ZakCms.MVC3.Controllers;
using ZakCms.Models.Entitites;
using ZakCms.Repositories;
using ZakCmsFE.Models;

namespace ZakCmsFE.Controllers
{
	public class AccountController : ZakCmsController
	{
		private readonly IFEUsersRepository _usersRepository;

		public AccountController() :
			this(SimpleFakeFactory.Create<IFEUsersRepository>())
		{
		}

		public AccountController(IFEUsersRepository usersRepository)
		{
			_usersRepository = usersRepository;
		}

		public ActionResult LogOn()
		{
			if (IsAuthenticated)
			{
				SetAuthenticationStatus(true);
				return RedirectToAction("Index", "Home");
			}
			return View();
		}

		[HttpPost]
		public ActionResult LogOn(LogOnModel model, string returnUrl)
		{
			if (IsAuthenticated)
			{
				SetAuthenticationStatus(true);
				return RedirectToAction("Index", "Home");
			}
			if (ModelState.IsValid)
			{
				if (_usersRepository.ValidateUser(model.UserName, model.Password))
				{
					FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
					SetAuthenticationStatus(true);
					if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
					    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
					{
						return Redirect(returnUrl);
					}
					return RedirectToAction("Index", "Home");
				}
				ModelState.AddModelError("", "The user name or password provided is incorrect.");
			}

			// If we got this far, something failed, redisplay form
			SetAuthenticationStatus(false);
			return View(model);
		}

		public ActionResult LogOff()
		{
			FormsAuthentication.SignOut();
			SetAuthenticationStatus(false);
			return RedirectToAction("Index", "Home");
		}

		public ActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Register(RegisterModel model)
		{
			// Attempt to register the user
			if (model.Password != model.ConfirmPassword)
			{
				ModelState.AddModelError("ConfirmPassword", "ConfirmPassword non matching");
			}
			if (ModelState.IsValid)
			{
				var um = new UserModel
					{
						UserId = model.UserName,
						UserPassword = model.Password
					};

				try
				{
					_usersRepository.Create(um);
					FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
					SetAuthenticationStatus(true);
					return RedirectToAction("Index", "Home");
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", ex.Message);
				}
			}

			// If we got this far, something failed, redisplay form
			SetAuthenticationStatus(false);
			return View(model);
		}

		[Authorize]
		public ActionResult ChangePassword()
		{
			return View();
		}

		[Authorize]
		[HttpPost]
		public ActionResult ChangePassword(ChangePasswordModel model)
		{
			if (model.NewPassword != model.ConfirmPassword)
			{
				ModelState.AddModelError("ConfirmPassword", "ConfirmPassword non matching");
			}

			var user = (UserModel) _usersRepository.GetUserByUserId(User.Identity.Name);
			if (model.OldPassword != user.UserPassword)
			{
				ModelState.AddModelError("ConfirmPassword", "Password non matching");
			}

			if (ModelState.IsValid)
			{
				user.UserPassword = model.NewPassword;
				try
				{
					_usersRepository.Update(user);
					// ChangePassword will throw an exception rather
					// than return false in certain failure scenarios.
					return RedirectToAction("ChangePasswordSuccess");
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", ex.Message);
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		public ActionResult ChangePasswordSuccess()
		{
			return View();
		}
	}
}