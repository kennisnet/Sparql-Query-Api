namespace Trezorix.Sparql.Api.Core.Repositories {
  using System;
  using System.Collections.Generic;
  using System.Diagnostics.CodeAnalysis;
  using System.Linq;

  using MongoDB.Driver.Linq;

  using MongoRepository;

  using Trezorix.Sparql.Api.Core.Accounts;

  [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", 
    Justification = "Reviewed. Suppression is OK here.")]
  public class MongoAccountRepository : MongoRepository<Account>, IAccountRepository {
    public Account Get(string id) {
      return this.AsEnumerable().SingleOrDefault(a => a.ApiKey == id);
    }

	  public Account GetByApiKey(string apiKey) {
			return this.AsQueryable().SingleOrDefault(a => a.ApiKey == apiKey);
	  }

		public IEnumerable<Account> GetByApiKeys(IEnumerable<string> apiKeys)
		{	
			return this.AsQueryable().Where(a => a.ApiKey.In(apiKeys));
	  }

	  public Account GetByUserName(string userName) {
      return this.AsQueryable().SingleOrDefault(a => a.UserName == userName);
    }

    public Account Save(Account account) {
      Account accountResult;
      var newAccount = account.Id == null;

      if (newAccount)
      {
        accountResult = this.Add(account);
      }
      else
      {
        accountResult = this.Update(account);        
      }

      return accountResult;
    }

    public IEnumerable<Account> All() {
      return this.AsEnumerable();
    }
  }
}