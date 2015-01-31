using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Trezorix.Sparql.Api.Admin.Controllers.Core;
using Trezorix.Sparql.Api.Admin.Models.Accounts;
using Trezorix.Sparql.Api.Admin.Services;
using Trezorix.Sparql.Api.Core.Accounts;
using Trezorix.Sparql.Api.Core.Repositories;

namespace Trezorix.Sparql.Api.Admin.Controllers {

	public class AccountController : BaseController
	{
		private readonly IQueryRepository _queryRepository;
		private readonly IAccountRepository _accountRepository;
	  private readonly IFormsAuthenticationService _formsService;

    public AccountController(IQueryRepository queryRepository, IAccountRepository accountRepository, IFormsAuthenticationService formsService)
    {
	    _accountRepository = accountRepository;
	    _formsService = formsService;
	    _queryRepository = queryRepository;
    }

		public ActionResult Index()
		{
			ViewBag.Account = OperatingAccount.Current();
			var model = _accountRepository.All().ToList();
			return View(model);
		}

		[HttpGet]
		public ActionResult Item(string id)
		{
			ViewBag.Account = OperatingAccount.Current();
			
			var model = (id == "new") ? new Account() : _accountRepository.Get(id);

			return View(model);
		}

		[AllowAnonymous]
    [HttpGet]
    public ActionResult Signup(string errorString) {
      var model = new SignupModel();
      ViewBag.ErrorMessage = errorString;
      return View(model);
    }
    
    [AllowAnonymous]
    [HttpPost]
    public ActionResult Signup(SignupModel model) {
      if (_accountRepository.All().Any(p => p.UserName == model.Username)) {
        return Signup("Er bestaat al een gebruiker met dezelfde naam.");
      }

      if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Email)) {
        return Signup("Vul a.u.b. alle velden in.");
      }

      var account = new Account {
        UserName = model.Username,
        ApiKey = Guid.NewGuid(),
      };

			_accountRepository.Save(account.Id, account);

	    return null; //Login(account.Id, "", "~/Account");
    }

    [HttpPost]
    public ActionResult Delete(string id) {
			Account account = _accountRepository.Get(id);
			if (account != null) {
        if (OperatingAccount.Current().Id == account.Id) {
          _formsService.SignOut();
          OperatingAccount.SetAnonymous();
        }

				_accountRepository.Delete(account);

      }

      return Redirect("~/Account");
    }

  }

}
