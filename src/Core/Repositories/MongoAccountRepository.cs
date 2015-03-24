namespace Trezorix.Sparql.Api.Core.Repositories {
  using System;
  using System.Collections.Generic;
  using System.Diagnostics.CodeAnalysis;
  using System.Linq;

  using MongoRepository;

  using Trezorix.Sparql.Api.Core.Accounts;

  [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", 
    Justification = "Reviewed. Suppression is OK here.")]
  public class MongoAccountRepository : MongoRepository<Account>, IAccountRepository {
    public Account Get(string id) {
      return this.AsEnumerable().SingleOrDefault(a => a.ApiKey == Guid.Parse(id));
    }

	  public Account GetByApiKey(string apiKey) {
			var apiKeyGuid = Guid.Parse(apiKey);
			return this.AsQueryable().SingleOrDefault(a => a.ApiKey == apiKeyGuid);
	  }

		public IEnumerable<Account> GetByApiKeys(IEnumerable<string> apiKeys)
		{
			var apiKeyGuids = apiKeys.Select(Guid.Parse).ToList();
			return this.AsQueryable().Where(a => apiKeyGuids.Any(k => k == a.ApiKey));
	  }

	  public Account GetByUserName(string userName) {
      return this.AsQueryable().SingleOrDefault(a => a.UserName == userName);
    }

    public void Save(string name, Account account) {
      if (account.Id == null) {
        this.Add(account);
      }
      else {
        this.Update(account);
      }
    }

    public IEnumerable<Account> All() {
      return this.AsEnumerable();
    }
  }
}