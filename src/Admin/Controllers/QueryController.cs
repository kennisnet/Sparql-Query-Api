using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Trezorix.Sparql.Api.Admin.Controllers.Core;
using Trezorix.Sparql.Api.Admin.Models.Queries;
using Trezorix.Sparql.Api.Core.Queries;
using Trezorix.Sparql.Api.Core.Repositories;

namespace Trezorix.Sparql.Api.Admin.Controllers
{
  using Trezorix.Sparql.Api.Application.Accounts;

  public class QueryController : BaseController
	{
		private readonly IQueryRepository _queryRepository;
		private readonly IAccountRepository _accountRepository;

		public QueryController(IQueryRepository queryRepository, IAccountRepository accountRepository)
		{
			_queryRepository = queryRepository;
			_accountRepository = accountRepository;
		}

		[HttpGet]
		public ActionResult Index() 
    {
			try 
			{
        ViewBag.Account = OperatingAccount.Current(_accountRepository);
			}
			catch (ApplicationException e) 
			{
				return RedirectToAction("Login", "Home");
			}

			var list = _queryRepository.All().ToList();
			var model = new GroupedQueryModel
				{
					Groups = new List<QueryGroup>()
				};
			foreach (string group in list.Select(q => q.Group).Distinct())
			{
				string safeId = ((!string.IsNullOrEmpty(group)) ? group.Replace("'", "_") : "");
				string thisGroup = group;
				model.Groups.Add(new QueryGroup { Id = safeId, Label = ((!string.IsNullOrEmpty(group)) ? group : "Algemeen"), Items = list.Where(q => q.Group == thisGroup)});
			}

			if (model.Groups.Count == 0)
			{
				model.Groups.Add(new QueryGroup { Label = "Algemeen", Items = new List<Query>()});
			}
			return View(model);
		}

		[HttpGet]
		public ActionResult Item(string alias, string group)
		{
      ViewBag.Account = OperatingAccount.Current(_accountRepository);

			var query = (alias == "new") ? new Query() { Group = group } : _queryRepository.GetByAlias(alias);

			return View(query);
		}
	
	}
}