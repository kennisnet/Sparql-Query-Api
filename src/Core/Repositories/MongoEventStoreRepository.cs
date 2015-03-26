namespace Trezorix.Sparql.Api.Core.Repositories
{
  using System.Linq;

  using MongoDB.Bson;

  using Trezorix.Sparql.Api.Core.EventSourcing;
  using MongoRepository;

  using Query = MongoDB.Driver.Builders.Query;

  public class MongoEventStoreRepository: MongoRepository<EventStore>, IEventStoreRepository
  {
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
  }
}
