using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Net;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Trezorix.Sparql.Api.QueryApi.Filters;

namespace Trezorix.Sparql.Api.Application.Filters
{
  using NLog;

  public class ApiKeyAuthAttribute : AuthorizationFilterAttribute
	{

		private static readonly string[] _emptyArray = new string[0];
		private const string _apiKeyAuthorizerMethodName = "IsAuthorized";

		private readonly string _apiKeyQueryParameter;
		private string _roles;
		private readonly Type _apiKeyAuthorizerType;
		private string[] _rolesSplit = _emptyArray;

		public string Roles
		{
			get
			{
				return _roles ?? string.Empty;
			}
			set
			{
				_roles = value;
				_rolesSplit = SplitString(value);
			}
		}

		public ApiKeyAuthAttribute(string apiKeyQueryParameter, Type apiKeyAuthorizerType)
		{
			if (string.IsNullOrEmpty(apiKeyQueryParameter))
				throw new ArgumentNullException("apiKeyQueryParameter");

			if (apiKeyAuthorizerType == null)
				throw new ArgumentNullException("apiKeyAuthorizerType");

			if (!IsTypeOfIApiKeyAuthorizer(apiKeyAuthorizerType))
			{
				throw new ArgumentException(
						string.Format(
								"{0} type has not implemented the TugberkUg.Web.Http.IApiKeyAuthorizer interface",
								apiKeyAuthorizerType.ToString()
						),
						"apiKeyAuthorizerType"
				);
			}

			_apiKeyQueryParameter = apiKeyQueryParameter;
			_apiKeyAuthorizerType = apiKeyAuthorizerType;
		}

		public override void OnAuthorization(HttpActionContext actionContext)
		{
			if (actionContext == null)
				throw new ArgumentNullException("actionContext");

			if (this.SkipAuthorization(actionContext))
				return;

			if (!AuthorizeCore(actionContext.Request))
				HandleUnauthorizedRequest(actionContext);
		}

		protected virtual void HandleUnauthorizedRequest(HttpActionContext actionContext)
		{
			if (actionContext == null)
			{
				throw new ArgumentNullException("actionContext");
			}
			actionContext.Response = actionContext.ControllerContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
		}

		//private helpers
		private bool IsTypeOfIApiKeyAuthorizer(Type type)
		{
			foreach (Type interfaceType in type.GetInterfaces())
			{
				if (interfaceType == typeof(IApiKeyAuthorizer))
					return true;
			}

			return false;
		}

		private bool SkipAuthorization(HttpActionContext actionContext)
		{
			return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any<AllowAnonymousAttribute>() ||
					actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any<AllowAnonymousAttribute>();
		}

		private bool AuthorizeCore(HttpRequestMessage request)
		{
			var apiKey = HttpUtility.ParseQueryString(request.RequestUri.Query)[_apiKeyQueryParameter];

			string remoteIp = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
			string remoteForwardedIp = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
			if (!String.IsNullOrEmpty(remoteForwardedIp))
			{
				remoteIp = remoteForwardedIp;
			}
			string referrer = HttpContext.Current.Request.ServerVariables["REFERER"];

			return IsAuthorized(apiKey, remoteIp, referrer);
		}

		private bool IsAuthorized(string apiKey, string ipAddress, string referrer)
		{
      object result = null;
      try {
        object apiKeyAuthorizerClassInstance = Activator.CreateInstance(_apiKeyAuthorizerType);

			  if (_rolesSplit == _emptyArray)
			  {
				  result = _apiKeyAuthorizerType.GetMethod(_apiKeyAuthorizerMethodName, new Type[] { typeof(string), typeof(string), typeof(string) }).
							  Invoke(apiKeyAuthorizerClassInstance, new object[] { apiKey, ipAddress, referrer });
			  }
			  else
			  {
				  result = _apiKeyAuthorizerType.GetMethod(_apiKeyAuthorizerMethodName, new Type[] { typeof(string), typeof(string[]) }).
						  Invoke(apiKeyAuthorizerClassInstance, new object[] { apiKey, ipAddress, referrer, _rolesSplit });
			  }
      }
      catch (Exception e) {
        LogManager.GetCurrentClassLogger().Error("Exception during execution of apiKey authorizer", e);
      }

      return (bool)result;
    }

		private static string[] SplitString(string original)
		{
			if (string.IsNullOrEmpty(original))
				return _emptyArray;

			IEnumerable<string> source =
				from piece in original.Split(new char[] { ',' })
				let trimmed = piece.Trim()
				where !string.IsNullOrEmpty(trimmed)
				select trimmed;
			return source.ToArray<string>();
		}
	}
}