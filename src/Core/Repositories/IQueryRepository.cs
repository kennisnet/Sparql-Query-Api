using System.Collections.Generic;
using Trezorix.Sparql.Api.Core.Queries;

namespace Trezorix.Sparql.Api.Core.Repositories
{
	public interface IQueryRepository
	{
		Query Get(string name);
		Query GetByAlias(string alias);
		Query GetById(string id);
    Query Save(Query query);
		IEnumerable<Query> All();
		void Delete(Query query);
	}
}