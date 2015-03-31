using System.Web.Http;

namespace Trezorix.Sparql.Api.Admin.App_Start
{
  using System.Linq;
  using System.Net.Http.Formatting;

  using Newtonsoft.Json.Serialization;

  public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
      //// for json output we prefer camelCase
      //var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
      //jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

      // clear the default xml support (so now it's json)
      config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

      // add output parameter to control format
      config.Formatters.JsonFormatter.MediaTypeMappings.Add(
        new QueryStringMapping("format", "json", "application/json"));
      config.Formatters.XmlFormatter.MediaTypeMappings.Add(new QueryStringMapping("format", "xml", "application/xml"));

      config.MapHttpAttributeRoutes();
      
      config.Routes.MapHttpRoute(
					name: "DefaultApi",
					routeTemplate: "api/{controller}/{id}",
					defaults: new { id = RouteParameter.Optional }
			);
		}
	}
}
