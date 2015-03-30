namespace Trezorix.Sparql.Api.Application.MongoRepositories
{
  using System.Collections.Generic;
  using System.Linq;

  using MongoRepository;

  using Trezorix.Sparql.Api.Core.Queries;
  using Trezorix.Sparql.Api.Core.Repositories;

  public class MongoQueryRepository: MongoRepository<Query>, IQueryRepository 
	{
		public Query Get(string name)
		{
			return this.AsEnumerable().SingleOrDefault(q => q.Label == name);
		}

		public Query GetByAlias(string alias) {
			return this.AsEnumerable().SingleOrDefault(q => q.Alias == alias);
		}

		public IEnumerable<Query> All() {
			return this.AsEnumerable();
		}

		public Query Save(Query query)
		{
			if (query.Id == null)
			{
				this.Add(query);
			}
			else
			{
				this.Update(query);
			}
		  return query;
		}
	}
}