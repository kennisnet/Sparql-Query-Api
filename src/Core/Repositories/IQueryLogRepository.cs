﻿namespace Trezorix.Sparql.Api.Core.Repositories {
  using System;
  using System.Collections.Generic;

  using Trezorix.Sparql.Api.Core.Queries;

  public interface IQueryLogRepository {
    QueryLogItem Add(QueryLogItem queryLogItem);

    IEnumerable<QueryLogItem> All();

    IEnumerable<QueryLogItem> GetByDateRange(DateTime startDate, DateTime endDate);

    IList<QueryLogItem> GetStartingFromDate(DateTime startDate);

    IList<QueryLogItem> GetStartingFromDateForAccount(DateTime startDate, string accountApiKey);

    IList<QueryLogItem> GetStartingFromDateForQuery(DateTime startDate, string queryAlias);

    IList<QueryStatistics> GetQueryStatisticsForDownloads();

    IList<QueryLogStatisticsByAccount> GetQueryLogStatisticsByAccount(
      DateTime startDate, 
      string queryAlias, 
      string accountId, 
      IEnumerable<string> columns);

    IList<QueryLogStatisticsByQueryName> GetQueryLogStatisticsByQueryName(
      DateTime startDate, 
      string accountApiKey, 
      string queryName,
      IEnumerable<string> columns);
  }
}