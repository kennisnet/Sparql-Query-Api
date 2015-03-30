namespace Trezorix.Sparql.Api.Application.MongoRepositories {
  using System;
  using System.Diagnostics.CodeAnalysis;

  using Trezorix.Sparql.Api.Core.Accounts;
  using Trezorix.Sparql.Api.Core.EventSourcing;
  using Trezorix.Sparql.Api.Core.Repositories;

  [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", 
    Justification = "Reviewed. Suppression is OK here.")]
  public class MongoAccountRepositoryWithEventStore : MongoAccountRepositoryDecoratorBase
  {
    private readonly IEventStoreRepository eventStoreRepository;

    public MongoAccountRepositoryWithEventStore(IAccountRepository accountRepository, IEventStoreRepository eventStoreRepository)
      : base(accountRepository)
    {
      this.eventStoreRepository = eventStoreRepository;
    }

    public override Account Save(Account account) {
      var newAccount = account.Id == null;

      Account accountResult = this.AccountRepository.Save(account);

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

    public override void Delete(Account account) {

      var eventStore = new EventStore()
      {
        Date = DateTime.UtcNow,
        EventName = Events.DeleteAccount,
        Payload = account
      };

      this.eventStoreRepository.Add(eventStore);
      this.AccountRepository.Delete(account);
    }
  }
}