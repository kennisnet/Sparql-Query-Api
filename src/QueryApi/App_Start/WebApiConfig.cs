using System.Web.Http;

namespace Trezorix.Sparql.Api.QueryApi.App_Start
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{

			config.Routes.MapHttpRoute(
					name: "QueryParametervalues",
					routeTemplate: "{name}/ParameterValues/{parameter}",
					defaults: new { controller = "Query", Action = "ParameterValues", name = RouteParameter.Optional, parameter = RouteParameter.Optional }
			);

			config.Routes.MapHttpRoute(
					name: "QueryApi",
					routeTemplate: "{name}",
					defaults: new {controller = "Query", name = RouteParameter.Optional}
			);
		
		}
	}
}
