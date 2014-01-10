using System;
using System.IO;
using System.Web.Mvc;

namespace Trezorix.Sparql.Api.QueryApi.Controllers
{
	public class LogController : Controller
	{

		public ActionResult Index(DateTime? date = null)
		{
			if (date == null)
			{
				date = DateTime.Now;
			}
			string logFilename = Server.MapPath(string.Format("~/logs/{0:yyyy-MM-dd}_log.txt", date));
			var log = (System.IO.File.Exists(logFilename)) ? System.IO.File.ReadAllText(logFilename) : "Geen log items gevonden.";

			return Json(new { Lines = log.Replace("\r", "").Split(new string[] { "\n" }, StringSplitOptions.None) }, JsonRequestBehavior.AllowGet);
		}
	}
}