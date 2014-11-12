using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Trezorix.Sparql.Api.Admin.App_Start;
using Trezorix.Sparql.Api.Core.Accounts;
using Trezorix.Sparql.Api.Core.Configuration;

namespace Trezorix.Sparql.Api.Admin
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			var cfg = ApiConfiguration.Init((HostingEnvironment.SiteName == "Admin") ? "API" : null);
			ApiConfiguration.Current = cfg;

			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);

			AutofacConfig.SetAsDependencyResolver();

		}
    protected void Application_AuthenticateRequest(Object sender, EventArgs e) {
      HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

      if (authCookie != null) {
        FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
        if (authTicket != null) {
          string username = authTicket.Name;
          try {
            OperatingAccount.SetByAccountId(username);
          }
          catch (Exception) {
          //catch (AccountException) {
            // swallowing any account exceptions, not wanting a malicious user to be able to test for username existance
          }
        }
      }

      // need to sync these two values, framework doesn't do so properly.
      Context.User = Thread.CurrentPrincipal;
    }

  }
}