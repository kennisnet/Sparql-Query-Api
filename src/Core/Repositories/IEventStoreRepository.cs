namespace Trezorix.Sparql.Api.Core.Repositories
{
  using Trezorix.Sparql.Api.Core.EventSourcing;

  public interface IEventStoreRepository {
    EventStore Get(string eventStoreId);
    EventStore GetByAccountId(string accountId);
    EventStore Add(EventStore eventStore);
  }
}
