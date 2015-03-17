namespace Trezorix.Sparql.Api.Core.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using MongoRepository;
	using Trezorix.Sparql.Api.Core.Queries;

	public class MongoQueryRepository: MongoRepository<Query>, IQueryRepository 
	{
		public Query Get(string name)
		{
			return this.AsEnumerable().SingleOrDefault(q => q.Label == name);
		}

		public Query GetByAlias(string name) {
			throw new NotImplementedException();
			//return this.AsEnumerable().SingleOrDefault(q => q.Alias == name);
		}

		public IEnumerable<Query> All() {
			return this.AsEnumerable();
		}

		public void Save(string name, Query query)
		{
			if (query.Id == null)
			{
				this.Add(query);
			}
			else
			{
				this.Update(query);
			}
		}
	}
}