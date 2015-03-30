namespace Trezorix.Sparql.Api.Application.MongoRepositories {
  using System.Collections.Generic;
  using System.Diagnostics.CodeAnalysis;

  using MongoRepository;

  using Trezorix.Sparql.Api.Core.Queries;
  using Trezorix.Sparql.Api.Core.Repositories;

  [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", 
    Justification = "Reviewed. Suppression is OK here.")]
  public abstract class MongoQueryRepositoryDecoratorBase : MongoRepository<Query>, IQueryRepository {
    protected readonly IQueryRepository QueryRepository;

    protected MongoQueryRepositoryDecoratorBase(IQueryRepository queryRepository) {
      this.QueryRepository = queryRepository;
    }

    public virtual Query Get(string id) {
      return this.QueryRepository.Get(id);
    }

    public virtual Query GetByAlias(string alias)
    {
      return this.QueryRepository.GetByAlias(alias);
    }

    public virtual IEnumerable<Query> All()
    {
      return this.QueryRepository.All();
    }

    public virtual Query Save(Query query)
    {
      return this.QueryRepository.Save(query);
    }

    public override void Delete(Query query) {
      this.QueryRepository.Delete(query);
    }
  }
}