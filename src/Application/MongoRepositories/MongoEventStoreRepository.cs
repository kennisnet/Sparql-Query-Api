namespace Trezorix.Sparql.Api.Application.MongoRepositories
{
  using System.Linq;

  using MongoDB.Bson;
  using MongoDB.Driver.Builders;

  using MongoRepository;

  using Trezorix.Sparql.Api.Core;
  using Trezorix.Sparql.Api.Core.EventSourcing;
  using Trezorix.Sparql.Api.Core.Repositories;

  public class MongoEventStoreRepository: MongoRepository<EventStore>, IEventStoreRepository
  {
    private readonly IAccountIdResolver accountIdResolver;

    public MongoEventStoreRepository(IAccountIdResolver accountIdResolver) {
      this.accountIdResolver = accountIdResolver;
    }

    public EventStore Get(string eventStoreId)
    {
      return this.AsEnumerable().SingleOrDefault(a => a.Id == eventStoreId);
    }

    public EventStore GetByAccountId(string accountId) {
      var coll = this.Collection;

      var query = Query.And(
        Query.EQ("Payload._t", "Account"),
        Query.EQ("Payload._id", new ObjectId(accountId)));

      var result = coll.FindOneAs<EventStore>(query);

      return result;
    }

    public override EventStore Add(EventStore entity) {
      entity.AccountId = accountIdResolver.GetAccountId();
      return base.Add(entity);
    }
  }
}
