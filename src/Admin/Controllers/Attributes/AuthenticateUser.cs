using System;
using System.Web;
using System.Web.Mvc;

namespace Trezorix.Sparql.Api.Admin.Controllers.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	internal sealed class AuthenticateUser : AuthorizeAttribute
	{
		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (httpContext.User.Identity.IsAuthenticated) return true;
			
			return false;
		}
	}
}