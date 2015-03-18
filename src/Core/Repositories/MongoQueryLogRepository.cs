namespace Trezorix.Sparql.Api.Core.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using MongoDB.Driver.Builders;

	using Trezorix.Sparql.Api.Core.Queries;
	using MongoRepository;

	using Query = MongoDB.Driver.Builders.Query;

	public class MongoQueryLogRepository : MongoRepository<QueryLogItem>, IQueryLogRepository
	{
		public IEnumerable<QueryLogItem> All()
		{
			return this.AsEnumerable();
		}

		public IEnumerable<QueryLogItem> GetByDateRange(DateTime startDate, DateTime endDate) {

			var queryItemsColl = this.Collection;
			
			var query = Query.And(
				Query<QueryLogItem>.GTE(p => p.DateTime, startDate),
				Query<QueryLogItem>.LTE(p => p.DateTime, endDate.AddHours(24)));

			var results = queryItemsColl.Find(query).AsEnumerable();

			return results;
		}
	}
}
