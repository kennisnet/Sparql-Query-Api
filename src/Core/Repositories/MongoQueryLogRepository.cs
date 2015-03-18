namespace Trezorix.Sparql.Api.Core.Repositories
{
	using System.Collections.Generic;
	using System.Linq;

	using Trezorix.Sparql.Api.Core.Queries;
	using MongoRepository;

	public class MongoQueryLogRepository : MongoRepository<QueryLogItem>, IQueryLogRepository
	{
		public IEnumerable<QueryLogItem> All()
		{
			return this.AsEnumerable();
		}
	}
}
