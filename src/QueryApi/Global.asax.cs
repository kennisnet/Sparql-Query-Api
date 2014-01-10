using System.Net.Http.Formatting;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Trezorix.Sparql.Api.Core.Configuration;
using Trezorix.Sparql.Api.QueryApi.App_Start;
using Trezorix.Sparql.Api.QueryApi.MediaTypeFormatters;

namespace Trezorix.Sparql.Api.QueryApi
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class WebApiApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			var cfg = ApiConfiguration.Init(HostingEnvironment.SiteName);
			ApiConfiguration.Current = cfg;

		  WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			// clear the default xml support (so now it's json)
			GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
			//// add output parameter to control format
			GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings.Add(new QueryStringMapping("format", "json", "application/json"));
			GlobalConfiguration.Configuration.Formatters.XmlFormatter.MediaTypeMappings.Add(new QueryStringMapping("format", "xml", "application/xml"));

			RdfHtmlMediaTypeFormatter htmlMediaTypeFormatter  = new RdfHtmlMediaTypeFormatter();
			htmlMediaTypeFormatter.MediaTypeMappings.Add(new QueryStringMapping("format", "html", "text/html"));
			GlobalConfiguration.Configuration.Formatters.Add(htmlMediaTypeFormatter);

			RdfCsvMediaTypeFormatter csvMediaTypeFormatter = new RdfCsvMediaTypeFormatter();
			csvMediaTypeFormatter.MediaTypeMappings.Add(new QueryStringMapping("format", "csv", "text/csv"));
			GlobalConfiguration.Configuration.Formatters.Add(csvMediaTypeFormatter);

			AutofacConfig.SetAsDependencyResolver();

		}
	}
}