using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Hosting;
using System.Web.Mvc;
using Trezorix.Sparql.Api.Core.Configuration;
using Trezorix.Sparql.Api.Core.Repositories;

namespace Trezorix.Sparql.Api.QueryApi.Controllers
{
	public class HomeController : Controller
	{
		private readonly IQueryRepository _queryRepository;

		public HomeController(IQueryRepository queryRepository)
		{
			_queryRepository = queryRepository;
		}

		public ActionResult Index()
		{
			var apiKey = System.Web.HttpContext.Current.Request.Params["api_key"];

			var queries = (!string.IsNullOrEmpty(apiKey))
				              ? _queryRepository.All().Where(q => q.ApiKeys.Any(k => k == apiKey) || q.AllowAnonymous)
				              : _queryRepository.All().Where(q => q.AllowAnonymous);

			ViewBag.ApiKey = apiKey;

			return View(queries);
		}

		public ActionResult ClearCache(string queryName = "")
		{
			var sb = new List<string>();
			List<string> cacheKeys = MemoryCache.Default.Where(kvp => kvp.Key.StartsWith(queryName)).Select(kvp => kvp.Key).ToList();
			foreach (string cacheKey in cacheKeys)
			{
				sb.Add(cacheKey);
				MemoryCache.Default.Remove(cacheKey);
			} 
			// does not work?
			//MemoryCache.Default.Trim(100);

			return Json(new { Action = "ClearCache", Succes = true, Data = sb }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Reload()
		{
			//var cfg = ApiConfiguration.Init(HostingEnvironment.SiteName);
			//ApiConfiguration.Current = cfg;
			
			return Json(new {Action = "Reload", Succes = true}, JsonRequestBehavior.AllowGet);
		}

	}
}
