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

    IList<QueryLogStatisticsByColumn> GetQueryLogStatistics(
      DateTime startDate, 
      string queryAlias, 
      string accountApiKey, 
      IEnumerable<string> columns);
  }
}