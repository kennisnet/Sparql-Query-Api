using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Trezorix.Sparql.Api.Admin.Controllers.Attributes;

namespace Trezorix.Sparql.Api.Admin.Controllers.Core
{
	[AuthenticateUser]
	public abstract class BaseController : Controller
	{
		internal const int RNA_HTTP_ERROR_CODE = 409;

		// shadow Json result helper to always allow Get.
		protected new ActionResult Json(dynamic model)
		{
			// Note: the default datetime convertion has been changed (breaking change). See: http://james.newtonking.com/default.aspx?PageIndex=2
			string json = JsonConvert.SerializeObject(model, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), Converters = { new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd\\THH:mm:ssK" }}});
			return Content(json, "application/json", Encoding.UTF8);
		}

		protected HttpStatusCodeResult HttpModelError()
		{
			string errors = String.Join("; ", ModelState.SelectMany(e => e.Value.Errors).Select(item => item.ErrorMessage).ToList());
			return new HttpStatusCodeResult(RNA_HTTP_ERROR_CODE, errors);
		}

		protected HttpStatusCodeResult HttpRnaError(string message)
		{
			return new HttpStatusCodeResult(RNA_HTTP_ERROR_CODE, message);
		}
		
		protected HttpStatusCodeResult HttpBadRequestError(string message)
		{
			return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest, message);
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);

			//UserAccount userAccount = OperatingUserAccount.Current();
			//if (userAccount != null)
			//{
			//	CultureInfo userAccountCulture = CultureInfo.GetCultureInfo(userAccount.Language);
			//	Thread.CurrentThread.CurrentCulture = userAccountCulture;
			//	Thread.CurrentThread.CurrentUICulture = userAccountCulture;
			//}
		}

	}
}
