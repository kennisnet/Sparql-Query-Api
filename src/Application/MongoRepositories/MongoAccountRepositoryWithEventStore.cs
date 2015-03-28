namespace Trezorix.Sparql.Api.Application.MongoRepositories {
  using System;
  using System.Collections.Generic;
  using System.Diagnostics.CodeAnalysis;

  using MongoRepository;

  using Trezorix.Sparql.Api.Core.Accounts;
  using Trezorix.Sparql.Api.Core.EventSourcing;
  using Trezorix.Sparql.Api.Core.Repositories;

  [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", 
    Justification = "Reviewed. Suppression is OK here.")]
  public class MongoAccountRepositoryWithEventStore : MongoRepository<Account>, IAccountRepository {
    private readonly IAccountRepository accountRepository;

    private readonly IEventStoreRepository eventStoreRepository;

    public MongoAccountRepositoryWithEventStore(IAccountRepository accountRepository, IEventStoreRepository eventStoreRepository) {
      this.accountRepository = accountRepository;
      this.eventStoreRepository = eventStoreRepository;
    }

    public Account Get(string id) {
      return this.accountRepository.Get(id);
    }

	  public Account GetByApiKey(string apiKey) {
      return this.accountRepository.GetByApiKey(apiKey);
	  }

    public IEnumerable<Account> GetByApiKeys(IEnumerable<string> apiKeys) {
      return this.accountRepository.GetByApiKeys(apiKeys);
    }

    public Account GetByUserName(string userName) {
      return this.accountRepository.GetByUserName(userName);
    }

    public Account Save(Account account) {
      var newAccount = account.Id == null;

      Account accountResult = this.accountRepository.Save(account);

      var eventStore = new EventStore()
      {
        Date = DateTime.UtcNow,
        EventName = newAccount ? Events.CreateAccount : Events.UpdateAccount,
        Payload = account
      };

      if (newAccount)
      {
        this.eventStoreRepository.Add(eventStore);
      }
      else
      {
        this.eventStoreRepository.Add(eventStore);
      }

      return accountResult;
    }

    public IEnumerable<Account> All() {
      return this.accountRepository.All();
    }

    public override void Delete(Account account) {

      var eventStore = new EventStore()
      {
        Date = DateTime.UtcNow,
        EventName = Events.DeleteAccount,
        Payload = account
      };

      this.eventStoreRepository.Add(eventStore);
      this.accountRepository.Delete(account);
    }
  }
}