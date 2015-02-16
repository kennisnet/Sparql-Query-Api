using System.Web.Mvc;
using Trezorix.Sparql.Api.Admin.Controllers.Core;
using Trezorix.Sparql.Api.Core.Configuration;

namespace Trezorix.Sparql.Api.Admin.Controllers
{

  public class SettingsController : BaseController
	{
		public ActionResult Index() 
    {
			return View(ApiConfiguration.Current);
		}
	}
}