using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Trezorix.Sparql.Api.Admin.Controllers.Core;
using Trezorix.Sparql.Api.Admin.Models.Queries;
using Trezorix.Sparql.Api.Core.Queries;
using Trezorix.Sparql.Api.Core.Repositories;

namespace Trezorix.Sparql.Api.Admin.Controllers {
	using Trezorix.Sparql.Api.Application.Accounts;

	public class TestController : BaseController {
		private readonly IQueryRepository _queryRepository;
		private readonly IAccountRepository _accountRepository;

		public TestController(IQueryRepository queryRepository, IAccountRepository accountRepository) {
			_queryRepository = queryRepository;
			_accountRepository = accountRepository;
		}

		[HttpGet]
		public ActionResult Index() {
			try {
				ViewBag.Account = OperatingAccount.Current(_accountRepository);
			}
			catch (ApplicationException e) {
				return RedirectToAction("Login", "Home");
			}

			return View();
		}
	}
}