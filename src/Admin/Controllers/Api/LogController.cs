﻿namespace Trezorix.Sparql.Api.Admin.Controllers.Api {
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web.Http;

  using Trezorix.Sparql.Api.Admin.Controllers.Attributes;
  using Trezorix.Sparql.Api.Admin.Models.Statistics;
  using Trezorix.Sparql.Api.Application.Attributes;
  using Trezorix.Sparql.Api.Core.Queries;
  using Trezorix.Sparql.Api.Core.Repositories;

  [RoutePrefix("Api/Log")]
  [NLogWebApi]
  [AuthenticateUser]
  [Authorize]
  public class LogController : ApiController
  {
    private readonly IQueryLogRepository _queryLogRepository;
    private readonly IAccountRepository _accountRepository;

		public LogController(IQueryLogRepository queryLogRepository, IAccountRepository accountRepository) {
			_queryLogRepository = queryLogRepository;
			_accountRepository = accountRepository;
		}

		[HttpGet]
    [Route("Statistics")]
		public dynamic Statistics(string timespan, string accountApiKey = null, string queryAlias = null, string columns = null)
		{
      DateTime startTime;
      DateTime endTime = DateTime.UtcNow;
      switch (timespan) {
        case "last-month": {
            startTime = endTime.AddMonths(-1);
            break;
          }
        case "last-week": {
            startTime = endTime.AddDays(-7);
            break;
          }
        case "last-hour": {
            startTime = endTime.AddHours(-1);
            break;
          }
        case "last-24":
        default: {
            startTime = endTime.AddHours(-24);
            break;
          }
      }

		  var optionalColumns = (columns != null) ? columns.Split(',') : new string[] {}; 
	    return !string.IsNullOrEmpty(queryAlias) ?
        this.GetLogStatsForQuery(queryAlias, startTime, optionalColumns) :
        this.GetLogStatsForAccountOrAllLogsQueryItems(accountApiKey, startTime, optionalColumns);


	    //var sum = list.Aggregate((acc, cur) => acc + cur);
      //var average = list.Aggregate((acc, cur) => acc + cur) / list.Count;

      //var webclient = new WebClient();
      //string s = webclient.DownloadString(ApiConfiguration.Current.QueryApiUrl + "/Log/Index");

      //return Content(s);// Json(ApiConfiguration.Current, JsonRequestBehavior.AllowGet);
    }

		private dynamic GetLogStatsForQuery(string queryAlias, DateTime startTime, string [] columns) {
			var logItems = this._queryLogRepository.GetStartingFromDateForQuery(startTime, queryAlias);


			var queryStatistics = new List<QueryStatisticsModel>();
			var accountIds = logItems.AsQueryable().Select(q => q.AccountId).Distinct().ToList();

			if (!accountIds.Any()) {
				return null;
			}

			var accounts = _accountRepository.GetByApiKeys(accountIds).Where(a => a != null).ToList();

			foreach (var accountId in accountIds) {

        var groupedSet = this._queryLogRepository.GetQueryLogStatistics(startTime, queryAlias, accountId, (new[] { "AccountId" }).Concat(columns));        

        foreach (var item in groupedSet)
        {
          queryStatistics.Add(
            new QueryStatisticsModel
            {
              Name = accounts.First(a => a.ApiKey == accountId).FullName,
              AverageTime = Convert.ToInt32(item.AverageTime),
              AverageExecutionTime = (item.NoCacheTotalHits != 0) ? Convert.ToInt32(item.NoCacheSumTime / item.NoCacheTotalHits) : 0,
              AverageCachedTime = (item.CacheTotalHits != 0) ? Convert.ToInt32(item.CacheSumTime / item.CacheTotalHits) : 0,
              AcceptFormat = (!string.IsNullOrEmpty(item.AcceptFormat) && item.AcceptFormat.Contains("text/html")) ? "text/html" : item.AcceptFormat,
              Endpoint = item.Endpoint,
              RemoteIp = item.RemoteIp,
              Hits = Convert.ToInt32(item.CacheTotalHits + item.NoCacheTotalHits),
              CacheHits = Convert.ToInt32(item.CacheTotalHits)
            });
        }
			}

			return queryStatistics;
		}

    private dynamic GetLogStatsForAccountOrAllLogsQueryItems(string accountApiKey, DateTime startTime, string[] columns) {
			var logItems = !string.IsNullOrEmpty(accountApiKey)
				                 ? this._queryLogRepository.GetStartingFromDateForAccount(startTime, accountApiKey)
				                 : this._queryLogRepository.GetStartingFromDate(startTime);

			var queryStatistics = new List<QueryStatisticsModel>();

			var queryNames = logItems.AsQueryable().Select(q => q.Name).Distinct().ToList();
     
			foreach (var queryName in queryNames) {

        var groupedSet = this._queryLogRepository.GetQueryLogStatistics(startTime, queryName, accountApiKey, (new[] { "Name" }).Concat(columns));

        foreach (var item in groupedSet) {
          queryStatistics.Add(
            new QueryStatisticsModel
            {
              Name = queryName,
              AverageTime = Convert.ToInt32(item.AverageTime),
              AverageExecutionTime = (item.NoCacheTotalHits != 0) ? Convert.ToInt32(item.NoCacheSumTime / item.NoCacheTotalHits) : 0,
              AverageCachedTime = (item.CacheTotalHits != 0) ? Convert.ToInt32(item.CacheSumTime / item.CacheTotalHits) : 0,
              AcceptFormat = (!string.IsNullOrEmpty(item.AcceptFormat) && item.AcceptFormat.Contains("text/html")) ? "text/html" : item.AcceptFormat,
              Endpoint = item.Endpoint,
              RemoteIp = item.RemoteIp,
              Hits = Convert.ToInt32(item.CacheTotalHits + item.NoCacheTotalHits),
              CacheHits = Convert.ToInt32(item.CacheTotalHits)
            });
        }
			}

			return queryStatistics;
		}

		[HttpGet]
		[Route("Downloads")]
		public dynamic Downloads(int start = 0, int count = 10, string timespan = "month")
		{
			LogDownloadListModel downloadList;
			switch (timespan)
			{
				case "week":
					downloadList = ListWeeklyDownloads(start, count);
					break;
				default:
					downloadList = ListMonthlyDownloads(start, count);
					break;
			}
			return downloadList;
		}

		private LogDownloadListModel ListWeeklyDownloads(int start, int count)
		{
			// get the min dates per query			
			var queryStatistics = _queryLogRepository.GetQueryStatisticsForDownloads();

			// get the min date from all queries
			var minTimestamp = queryStatistics.Where(q => q.First > DateTime.MinValue).Min(q => q.First).Date;

			//calculate total weeks from first log item
			var today = DateTime.UtcNow;
			var dayOfWeek = (int)today.DayOfWeek;
			var timespan = (today.AddDays(-dayOfWeek) - minTimestamp);
			int weeks = (int)Math.Ceiling(timespan.TotalDays / 7);

			var weeklyDownloads = new LogDownloadListModel
			{
				TotalCount = weeks,
				Start = start,
				End = Math.Min(start + count, weeks),
				Items = new List<LogDownLoadModel>()
			};
			for (var i = weeklyDownloads.Start; i < weeklyDownloads.End; i++)
			{
				var endOfWeek = today.AddDays(-dayOfWeek - i * 7);
				weeklyDownloads.Items.Add(new LogDownLoadModel
				{
					Label = string.Format("{0:yyyy-MM-dd} / {1:yyyy-MM-dd}", endOfWeek.AddDays(-6), endOfWeek),
					Start = string.Format("{0:yyyy-MM-dd}", endOfWeek.AddDays(-6)),
					End = string.Format("{0:yyyy-MM-dd}", endOfWeek),
				}
					);
			}
			return weeklyDownloads;
		}

		private LogDownloadListModel ListMonthlyDownloads(int start, int count)
		{
			// get the min dates per query
			var queryStatistics = _queryLogRepository.GetQueryStatisticsForDownloads();

			if (queryStatistics.Count == 0)
				return null;

			// get the min date from all queries
			var minTimestamp = queryStatistics.Where(q => q.First > DateTime.MinValue).Min(q => q.First).Date;

			//calculate total weeks from first log item
			var today = DateTime.UtcNow;
			int offsetDays = DateTime.DaysInMonth(today.Year, today.Month) - (int)today.Day;
			int months = ((today.Year - minTimestamp.Year) * 12) + today.Month - minTimestamp.Month + 1;

			var montlyDownloads = new LogDownloadListModel
			{
				TotalCount = months,
				Start = start,
				End = Math.Min(start + count, months),
				Items = new List<LogDownLoadModel>()
			};

			for (var i = montlyDownloads.Start; i < montlyDownloads.End; i++)
			{
				var endOfMonth = today.AddDays(offsetDays).AddMonths(-i).Date;
				// AddMonths is not exact (seems to add 31 days?)
				var startOfMonth = endOfMonth.AddDays(1 - DateTime.DaysInMonth(endOfMonth.AddDays(-1).Year, endOfMonth.AddDays(-1).Month));
				montlyDownloads.Items.Add(new LogDownLoadModel
				{
					Label = string.Format("{0:yyyy-MM-dd} / {1:yyyy-MM-dd}", (startOfMonth < minTimestamp) ? minTimestamp : startOfMonth, (endOfMonth > today) ? today : endOfMonth),
					Start = string.Format("{0:yyyy-MM-dd}", (startOfMonth < minTimestamp) ? minTimestamp : startOfMonth),
					End = string.Format("{0:yyyy-MM-dd}", (endOfMonth > today) ? today : endOfMonth),
				}
					);
			}
			return montlyDownloads;
		}

  }
}