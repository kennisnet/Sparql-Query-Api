namespace Trezorix.Sparql.Api.QueryApi.Filters 
{
  public interface IApiKeyAuthorizer 
	{
    bool IsAuthorized(string apiKey);
		bool IsAuthorized(string apiKey, string ipAddress, string referrer);
		bool IsAuthorized(string apiKey, string[] roles);
  }
}