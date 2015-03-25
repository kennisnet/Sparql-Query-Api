using System.Linq;
using System.Web.Mvc;
using Trezorix.Sparql.Api.Admin.Controllers.Core;
using Trezorix.Sparql.Api.Admin.Models.Accounts;
using Trezorix.Sparql.Api.Admin.Services;
using Trezorix.Sparql.Api.Core.Accounts;
using Trezorix.Sparql.Api.Core.Repositories;

namespace Trezorix.Sparql.Api.Admin.Controllers {
  using Trezorix.Sparql.Api.Application.Accounts;

  public class HomeController : BaseController
	{
		private readonly IAccountRepository _accountRepository;
		private readonly IFormsAuthenticationService _formsService;

    public HomeController(IAccountRepository accountRepository, IFormsAuthenticationService formsService)
    {
	    _accountRepository = accountRepository;
	    _formsService = formsService;
    }

		[HttpGet]
    public ActionResult Index() {
      ViewBag.Account = OperatingAccount.Current(_accountRepository);
      return View();
    }

		[HttpGet]
    public ActionResult Overview() {
      ViewBag.Account = OperatingAccount.Current(_accountRepository);
      return View();
    }

		[AllowAnonymous]
		[HttpGet]
		public ActionResult Login(string returnUrl)
		{
			var model = new LoginModel
			{
				ReturnUrl = returnUrl,
				//Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
			};

			ViewData.Model = model;

			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		//[ValidateAntiForgeryToken]
		public ActionResult Login(string userName, string password, bool rememberMe = false, string returnUrl = "")
		{
			if (string.IsNullOrEmpty(returnUrl))
			{
				returnUrl = "~/Home";
			}
			Account account = _accountRepository.GetByUserName(userName);

			if (account != null && account.Password != null)
			{
				var hashedPassword = account.ComputeSaltedHash(password);
				if (hashedPassword == account.Password)
				{
					_formsService.SignIn(account.Id, rememberMe);
					return Redirect(returnUrl);
				}
			}
				
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		public ActionResult Logout()
		{
			_formsService.SignOut();
			OperatingAccount.SetAnonymous();
			return Redirect("~/Home/Login");
		}
	
	}

}
