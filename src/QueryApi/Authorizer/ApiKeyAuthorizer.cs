using System.Collections.Generic;
using Trezorix.Sparql.Api.Core.Configuration;
using Trezorix.Sparql.Api.Core.Repositories;
using NLog;
using Trezorix.Sparql.Api.QueryApi.Filters;

namespace Trezorix.Sparql.Api.QueryApi.Authorizer
{
  using Trezorix.Sparql.Api.Application.MongoRepositories;

  public class ApiKeyAuthorizer : IApiKeyAuthorizer
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly HashSet<string> _apiKeys = new HashSet<string>();

		private readonly IAccountRepository _accountRepository;

		public ApiKeyAuthorizer()
		{
			_accountRepository = new MongoAccountRepository();
			//var accountRepository = new FileAccountRepository(ApiConfiguration.Current.RepositoryRoot + "Account");
			var accounts = _accountRepository.All();

			foreach (var account in accounts)
			{
				_apiKeys.Add(account.ApiKey.ToString());
			}
		}

		public bool IsAuthorized(string apiKey)
		{
			return ApiConfiguration.Current.AllowAnonymous || _apiKeys.Contains(apiKey.ToLowerInvariant());
		}

		public bool IsAuthorized(string apiKey, string ipAddress, string referrer)
		{
			if (ApiConfiguration.Current.AllowAnonymous) return true;

			_logger.Info("access {0} from {1}; {2}", apiKey, ipAddress, referrer);
			if (apiKey == null)
			{
				return false;
			}
			return _apiKeys.Contains(apiKey.ToLowerInvariant());
		}

		public bool IsAuthorized(string apiKey, string[] roles)
		{
			return IsAuthorized(apiKey);
		}
	}
}