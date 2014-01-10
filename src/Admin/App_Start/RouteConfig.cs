using System.Web.Mvc;
using System.Web.Routing;

namespace Trezorix.Sparql.Api.Admin.App_Start
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "AccountItem",
				url: "Account/{id}",
				defaults: new { controller = "Account", action = "Item" }
			);

			routes.MapRoute(
				name: "QueryItem",
				url: "Query/{id}",
				defaults: new { controller = "Query", action = "Item" }
			);

			routes.MapRoute(
					name: "Default",
					url: "{controller}/{action}/{id}",
					defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
	
		}
	}
}