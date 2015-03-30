namespace Trezorix.Sparql.Api.Application.MongoRepositories {
  using System.Collections.Generic;
  using System.Diagnostics.CodeAnalysis;

  using MongoRepository;

  using Trezorix.Sparql.Api.Core.Accounts;
  using Trezorix.Sparql.Api.Core.Repositories;

  [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", 
    Justification = "Reviewed. Suppression is OK here.")]
  public abstract class MongoAccountRepositoryDecoratorBase : MongoRepository<Account>, IAccountRepository {
    protected readonly IAccountRepository AccountRepository;

    protected MongoAccountRepositoryDecoratorBase(IAccountRepository accountRepository) {
      this.AccountRepository = accountRepository;
    }

    public virtual Account Get(string id) {
      return this.AccountRepository.Get(id);
    }

    public virtual Account GetByApiKey(string apiKey)
    {
      return this.AccountRepository.GetByApiKey(apiKey);
    }

    public virtual IEnumerable<Account> GetByApiKeys(IEnumerable<string> apiKeys)
    {
      return this.AccountRepository.GetByApiKeys(apiKeys);
    }

    public virtual Account GetByUserName(string userName)
    {
      return this.AccountRepository.GetByUserName(userName);
    }

    public virtual Account Save(Account account)
    {
      return this.AccountRepository.Save(account);
    }

    public IEnumerable<Account> All() {
      return this.AccountRepository.All();
    }

    public override void Delete(Account account) {
      this.AccountRepository.Delete(account);
    }
  }
}