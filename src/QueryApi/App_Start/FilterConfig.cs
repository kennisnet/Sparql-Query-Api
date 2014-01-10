using System.Web.Mvc;

namespace Trezorix.Sparql.Api.QueryApi.App_Start
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}