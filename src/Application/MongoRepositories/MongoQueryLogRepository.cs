﻿namespace Trezorix.Sparql.Api.Application.MongoRepositories {
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using MongoDB.Bson;
  using MongoDB.Bson.Serialization;
  using MongoDB.Driver;
  using MongoDB.Driver.Builders;

  using MongoRepository;

  using Trezorix.Sparql.Api.Core.Queries;
  using Trezorix.Sparql.Api.Core.Repositories;
  
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

    /// <summary>
    /// Logs
    /// </summary>
    /// <param name="startDate"></param>
    /// <returns></returns>
		public IList<QueryLogItem> GetStartingFromDate(DateTime startDate) {
			var queryItemsColl = this.Collection;

			var query = Query<QueryLogItem>.GT(p => p.DateTime, startDate);
			var results = queryItemsColl.Find(query).ToList();

			return results;
		}

    /// <summary>
    /// Accounts > Logs
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="accountApiKey"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Queries > Logs
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="queryAlias"></param>
    /// <returns></returns>
    public IList<QueryLogItem> GetStartingFromDateForQuery(DateTime startDate, string queryAlias)
    {
      var queryItemsColl = this.Collection;

      var query = Query.And(
        Query<QueryLogItem>.GT(p => p.DateTime, startDate),
        Query<QueryLogItem>.EQ(p => p.Name, queryAlias)
        );

      var results = queryItemsColl.Find(query).ToList();

      return results;
    }

    /// <summary>
    /// Queries > Logs
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="queryAlias"></param>
    /// <param name="accountApiKey"></param>
    /// <param name="columns"></param>
    /// <returns></returns>
    public IList<QueryLogStatisticsByColumn> GetQueryLogStatistics(DateTime startDate, string queryAlias, string accountApiKey, IEnumerable<string> columns)
    {
      BsonDocument matchCondition;

      if (!string.IsNullOrEmpty(accountApiKey))
      {
        matchCondition = new BsonDocument {
          { "DateTime", new BsonDocument { { "$gt", startDate } }  },
          { "AccountId", accountApiKey },
          { "Name", queryAlias },
        };

      }
      else
      {
        matchCondition = new BsonDocument {
          { "DateTime", new BsonDocument { { "$gt", startDate } }  },
          { "Name", queryAlias },
        };
      }

      var match = new BsonDocument(
        "$match",
        matchCondition); ;

      var columnList = columns as IList<string> ?? columns.ToList();

      var groups = new BsonDocument();
      foreach (var column in columnList)
      {
        groups.Add(column, "$" + column);
      }

      var group = new BsonDocument(
        "$group",
        new BsonDocument {
          { "_id", groups },
					{ "AverageTime", new BsonDocument { { "$avg", "$ExecutionTime" } } },
          { "TotalHits", new BsonDocument { { "$sum", 1 } } },
          { "NoCacheSumTime", new BsonDocument { { "$sum", new BsonDocument { { "$cond", new BsonArray { "$CacheHit", 0, "$ExecutionTime" } } } } } },
          { "NoCacheTotalHits", new BsonDocument { { "$sum", new BsonDocument { { "$cond", new BsonArray { "$CacheHit", 0, 1 } } } } } },
          { "CacheSumTime", new BsonDocument { { "$sum", new BsonDocument { { "$cond", new BsonArray { "$CacheHit", "$ExecutionTime", 0 } } } } } },
          { "CacheTotalHits", new BsonDocument { { "$sum", new BsonDocument { { "$cond", new BsonArray { "$CacheHit", 1, 0 } } } } } }
				});

      var project = new BsonDocument(
        "$project",
        new BsonDocument { 
          {"_id", 0}, 
          {"MasterColumn", "$_id." + columnList.First()},
          {"Endpoint", "$_id.Endpoint"},
          {"AcceptFormat", "$_id.AcceptFormat"},
          {"RemoteIp", "$_id.RemoteIp"},
          {"AverageTime", "$AverageTime"},
          {"TotalHits", "$TotalHits"},
          {"NoCacheSumTime", "$NoCacheSumTime"},
          {"NoCacheTotalHits", "$NoCacheTotalHits"},
          {"CacheSumTime", "$CacheSumTime"},
          {"CacheTotalHits", "$CacheTotalHits"}
        });

      var pipeline = new[] { match, group, project };

      var args = new AggregateArgs()
      {
        Pipeline = pipeline,
      };

      var coll = this.Collection;

      var aggregationResult = coll.Aggregate(args);

      var result = aggregationResult.Select(BsonSerializer.Deserialize<QueryLogStatisticsByColumn>).ToList();

      return result;
    }


		public IList<QueryStatistics> GetQueryStatisticsForDownloads() {
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

      // Drop old index if applicable
      var queryStatisticsIndexExists = indexes.Exists(ind => ind.Name == "QueryStatisticsIndex");
      if (queryStatisticsIndexExists) {
        coll.DropIndexByName("QueryStatisticsIndex");
      }

      // Create new index if applicable
      var queryLogStatisticsIndexExists = indexes.Exists(ind => ind.Name == "QueryLogStatisticsIndex");
      if (!queryLogStatisticsIndexExists)
      {
        var queryLogStatisticsIndexKeys = IndexKeys.Ascending("AccountId", "Name", "DateTime", "Endpoint", "AcceptFormat", "RemoteIp");
        var queryLogStatisticsIndexOptions = IndexOptions.SetName("QueryLogStatisticsIndex");
        coll.CreateIndex(queryLogStatisticsIndexKeys, queryLogStatisticsIndexOptions);
      }

		}

	}
}