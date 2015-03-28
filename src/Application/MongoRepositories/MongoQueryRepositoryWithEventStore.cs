namespace Trezorix.Sparql.Api.Application.MongoRepositories {
  using System;
  using System.Collections.Generic;
  using System.Diagnostics.CodeAnalysis;

  using MongoRepository;

  using Trezorix.Sparql.Api.Core.Queries;
  using Trezorix.Sparql.Api.Core.EventSourcing;
  using Trezorix.Sparql.Api.Core.Repositories;

  [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", 
    Justification = "Reviewed. Suppression is OK here.")]
  public class MongoQueryRepositoryWithEventStore : MongoRepository<Query>, IQueryRepository {
    private readonly IQueryRepository queryRepository;

    private readonly IEventStoreRepository eventStoreRepository;

    public MongoQueryRepositoryWithEventStore(IQueryRepository queryRepository, IEventStoreRepository eventStoreRepository) {
      this.queryRepository = queryRepository;
      this.eventStoreRepository = eventStoreRepository;
    }

    public Query Get(string id) {
      return this.queryRepository.Get(id);
    }

    public Query GetByAlias(string alias) {
      return this.queryRepository.GetByAlias(alias);
    }

    public IEnumerable<Query> All() {
      return this.queryRepository.All();
    }

    public Query Save(Query query) {
      var isNewQuery = query.Id == null;

      var queryResult = this.queryRepository.Save(query);

      var eventStore = new EventStore()
      {
        Date = DateTime.UtcNow,
        EventName = isNewQuery ? Events.CreateAccount : Events.UpdateAccount,
        Payload = queryResult
      };

      if (isNewQuery)
      {
        this.eventStoreRepository.Add(eventStore);
      }
      else
      {
        this.eventStoreRepository.Add(eventStore);
      }

      return queryResult;
    }

    public override void Delete(Query query) {

      var eventStore = new EventStore()
      {
        Date = DateTime.UtcNow,
        EventName = Events.DeleteAccount,
        Payload = query
      };

      this.eventStoreRepository.Add(eventStore);
      this.queryRepository.Delete(query);
    }
  }
}