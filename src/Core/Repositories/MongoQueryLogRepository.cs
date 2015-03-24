namespace Trezorix.Sparql.Api.Core.Repositories {
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using MongoDB.Bson;
	using MongoDB.Bson.Serialization;
	using MongoDB.Driver;
	using MongoDB.Driver.Builders;

	using MongoRepository;

	using Trezorix.Sparql.Api.Core.Queries;

	using Query = MongoDB.Driver.Builders.Query;
	
	public class MongoQueryLogRepository : MongoRepository<QueryLogItem>, IQueryLogRepository {

		public MongoQueryLogRepository() {
			this.CreateIndexesIfDoNotExist();
		}


		public IEnumerable<QueryLogItem> All() {
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

		public IList<QueryLogItem> GetStartingFromDate(DateTime startDate) {
			var queryItemsColl = this.Collection;

			var query = Query<QueryLogItem>.GT(p => p.DateTime, startDate);
			var results = queryItemsColl.Find(query).ToList();

			return results;
		}

		public IList<QueryLogItem> GetStartingFromDateForAccount(DateTime startDate, string accountApiKey)
		{
			var queryItemsColl = this.Collection;

			var query = Query.And( 
				Query<QueryLogItem>.GT(p => p.DateTime, startDate),
				Query<QueryLogItem>.EQ(p => p.AccountId, accountApiKey)
				);

			var results = queryItemsColl.Find(query).ToList();

			return results;
		}

		public IList<QueryStatistics> GetQueryStatistics() {
			var match = new BsonDocument(
				"$match",
				new BsonDocument { 					
					{ "Name", new BsonRegularExpression(".*", "i") },
					{ "DateTime", new BsonDocument { { "$gte", DateTime.MinValue } }  }
				});

			var groupByName = new BsonDocument(
				"$group",
				new BsonDocument {
					{ "_id", new BsonDocument { { "Name", "$Name" } } },
					{ "DateTime", new BsonDocument { { "$min", "$DateTime" } } }
				});

			var project = new BsonDocument(
				"$project",
				new BsonDocument { 
							{"_id", 0}, 
							{"QueryName","$_id.Name"},
							{"First", "$DateTime"}, 
				});

			var pipeline = new[]{ match, groupByName, project };

			var args = new AggregateArgs() {
				Pipeline = pipeline, 
			};

			var coll = this.Collection;			

			var aggregationResult = coll.Aggregate(args);
			
			var result = aggregationResult.Select(BsonSerializer.Deserialize<QueryStatistics>).ToList();

			return result;
		}


		public void CreateIndexesIfDoNotExist()
		{
			var coll = this.Collection;

			var indexes = coll.GetIndexes().ToList();

			var queryStatisticsIndexExists = indexes.Exists(ind => ind.Name == "QueryStatisticsIndex");
			if (!queryStatisticsIndexExists) {
				var queryStatisticsIndexKeys = IndexKeys.Ascending("Name", "DateTime");
				var queryStatisticsIndexOptions = IndexOptions.SetName("QueryStatisticsIndex");
				coll.CreateIndex(queryStatisticsIndexKeys, queryStatisticsIndexOptions);
			}
		}

	}

	public class QueryStatistics
	{
		public string QueryName { get; set; }

		public DateTime First { get; set; }
	}		
}