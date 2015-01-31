namespace Trezorix.Sparql.Api.Admin.Controllers.Api {
  using System.Net;
  using System.Web.Http;

  using Trezorix.Sparql.Api.Admin.Controllers.Attributes;
  using Trezorix.Sparql.Api.Core.Configuration;

  [RoutePrefix("Api/Settings")]
  [AuthenticateUser]
  [Authorize]
  public class SettingsController : ApiController
  {
    [HttpGet]
    public dynamic Get() 
    {
      return ApiConfiguration.Current;
    }

    [HttpGet]
    [Route("{id}")]
    public dynamic Get(string id)
    {
      return ApiConfiguration.Current;
    }

    [HttpPost]
    [Route("{id}")]
    public dynamic Post(string id, ApiConfiguration model)
    {
			ApiConfiguration.Save(model);

			var webclient = new WebClient();
			webclient.OpenRead(ApiConfiguration.Current.QueryApiUrl + "/Home/Reload");
			return Ok();
		}

		[HttpPost]
    [Route("{id}/ClearCache")]
		public dynamic ClearCache(string id) 
    {
			var webclient = new WebClient();
			webclient.OpenRead(ApiConfiguration.Current.QueryApiUrl + "/Home/ClearCache");
			return Ok();
		}
  }
}