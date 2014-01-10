using System;
using System.Web;
using System.Web.Mvc;

namespace Trezorix.Sparql.Api.Admin.Controllers.Attributes
{
	public class NoCacheAttribute : ActionFilterAttribute
	{
		public override void OnResultExecuting(ResultExecutingContext filterContext)
		{
			var cacheSetting = filterContext.HttpContext.Response.Cache;

			cacheSetting.SetExpires(DateTime.UtcNow.AddDays(-1));
			cacheSetting.SetValidUntilExpires(false);
			cacheSetting.SetRevalidation(HttpCacheRevalidation.AllCaches);
			cacheSetting.SetCacheability(HttpCacheability.NoCache);
			cacheSetting.SetNoStore();

			base.OnResultExecuting(filterContext);
		}

	}
}