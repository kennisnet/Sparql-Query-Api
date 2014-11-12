using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Mvc;

using Trezorix.Sparql.Api.Admin.Controllers.Core;
using Trezorix.Sparql.Api.Admin.Models.Queries;
using Trezorix.Sparql.Api.Core.Accounts;
using Trezorix.Sparql.Api.Core.Configuration;
using Trezorix.Sparql.Api.Core.Queries;
using Trezorix.Sparql.Api.Core.Repositories;

namespace Trezorix.Sparql.Api.Admin.Controllers
{
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
		public ActionResult Index() {
			try 
			{
				ViewBag.Account = OperatingAccount.Current();
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
		public ActionResult Item(string id, string group)
		{
			ViewBag.Account = OperatingAccount.Current();

			var query = (id == "new") ? new Query() { Group = group } : _queryRepository.Get(id);

			return View(query);
		}

		[HttpGet]
		public ActionResult Details(string id)
		{
			ViewBag.Account = OperatingAccount.Current();

			var query = (id == "new") ? new Query { ApiKeys = new List<Guid> { OperatingAccount.Current().ApiKey } } : _queryRepository.Get(id);

			var model = new QueryModel();
			model.MapFrom(query, _accountRepository.All(), _queryRepository.All().Select(q => q.Group).Distinct());

			model.Link = ApiConfiguration.Current.QueryApiUrl + model.Link.Replace("$$apikey", OperatingAccount.Current().ApiKey.ToString());
			
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult Details(QueryModel model)
		{
			if (string.IsNullOrEmpty(model.Id))
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Query moet een naam hebben.");
			}
			var query = new Query();
			model.MapTo(query);
			_queryRepository.Save(query.Id, query);

			ClearCacheInQueryApi(query);

			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		[HttpPut]
		public ActionResult Details(string id, QueryModel model)
		{
			var query = _queryRepository.Get(id);
		
			if (query == null)
			{
				return new HttpNotFoundResult();
			}

			if (model.Id != id)
			{
				_queryRepository.Delete(query);
				ClearCacheInQueryApi(query);
			}

			model.MapTo(query);

			_queryRepository.Save(model.Id, query);

			ClearCacheInQueryApi(query);
		
			return new HttpStatusCodeResult(HttpStatusCode.OK); 
		}

		[HttpPost]
		public ActionResult Delete(string id)
		{
			var query = _queryRepository.Get(id);

			_queryRepository.Delete(query);

			return Redirect("~/Query");
		}


		[HttpGet]
		public ActionResult Preview(string id) {
			var query = _queryRepository.Get(id);

			if (query == null) {
				return new HttpNotFoundResult();
			}

			var model = new QueryModel();
			model.MapFrom(query, _accountRepository.All(), _queryRepository.All().Select(q => q.Group).Distinct());

			model.Link = ApiConfiguration.Current.QueryApiUrl + model.Link.Replace("$$apikey", OperatingAccount.Current().ApiKey.ToString());

			var webclient = new WebClient();
			var response = webclient.OpenRead(model.Link + "&debug=true");


			return new FileStreamResult(response, "application/json");
		}

    [HttpGet]
    public ActionResult PreviewQuery(string id) {
      var query = _queryRepository.Get(id);

      if (query == null) {
        return new HttpNotFoundResult();
      }

      var model = new QueryModel();
      model.MapFrom(query, _accountRepository.All(), _queryRepository.All().Select(q => q.Group).Distinct());

      model.Link = ApiConfiguration.Current.QueryApiUrl + model.Link.Replace("$$apikey", OperatingAccount.Current().ApiKey.ToString());

      var webclient = new WebClient();
      var response = webclient.OpenRead(model.Link + "&debug=true&showQuery=true&format=json");


      return new FileStreamResult(response, "application/json");
    }
    
    private void ClearCacheInQueryApi(Query query)
		{
			var webclient = new WebClient();
			webclient.OpenRead(ApiConfiguration.Current.QueryApiUrl + "/Home/ClearCache?queryName=" + query.Id);
		}
	
	}
}