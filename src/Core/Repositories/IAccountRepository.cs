using System.Collections.Generic;

using Trezorix.Sparql.Api.Core.Accounts;

namespace Trezorix.Sparql.Api.Core.Repositories
{
  public interface IAccountRepository
  {
    Account Get(string id);
    Account GetById(string id);
    Account GetByApiKey(string apiKey);
    IEnumerable<Account> GetByApiKeys(IEnumerable<string> apiKeys);
    Account GetByUserName(string userName);
    Account Save(Account account);
    void Delete(Account account);
    IEnumerable<Account> All();
  }
}