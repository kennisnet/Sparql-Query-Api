using System.Net;
using System.Web.Mvc;
using Trezorix.Sparql.Api.Admin.Controllers.Core;
using Trezorix.Sparql.Api.Core.Accounts;
using Trezorix.Sparql.Api.Core.Configuration;

namespace Trezorix.Sparql.Api.Admin.Controllers
{
	public class SettingsController : BaseController
	{

		[HttpGet]
		public ActionResult Index() {
			ViewBag.Account = OperatingAccount.Current();
			return View(ApiConfiguration.Current);
		}
		
		[HttpGet]
		public ActionResult Details(string id)
		{
			return Json(ApiConfiguration.Current, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult Details(ApiConfiguration model)
		{
			ApiConfiguration.Save(model);

			var webclient = new WebClient();
			webclient.OpenRead(ApiConfiguration.Current.QueryApiUrl + "/Home/Reload");
			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		[HttpPost]
		public ActionResult ClearCache() {
			var webclient = new WebClient();
			webclient.OpenRead(ApiConfiguration.Current.QueryApiUrl + "/Home/ClearCache");
			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

	}
}