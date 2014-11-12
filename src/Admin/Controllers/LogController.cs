using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CsvHelper;
using Raven.Client;
using Trezorix.Sparql.Api.Admin.Controllers.Core;
using Trezorix.Sparql.Api.Admin.Models.Statistics;
using Trezorix.Sparql.Api.Core.Accounts;
using Trezorix.Sparql.Api.Core.Configuration;
using Trezorix.Sparql.Api.Core.Queries;

namespace Trezorix.Sparql.Api.Admin.Controllers
{
  using Raven.Client.Document;

  public class LogController : BaseController
	{
		private readonly IDocumentSession _session;

		public LogController(IDocumentSession session)
		{
			_session = session;
		}

		[HttpGet]
		public ActionResult Index() {
			ViewBag.Account = OperatingAccount.Current();
			return View(ApiConfiguration.Current);
		}

		
		[HttpGet]
		public ActionResult Statistics(string timespan)
		{
			DateTime startTime;
			DateTime endTime = DateTime.UtcNow;
			switch (timespan)
			{
				case "last-month":
					{
						startTime = endTime.AddMonths(-1);
						break;
					}
				case "last-week":
					{
						startTime = endTime.AddDays(-7);
						break;
					}
				case "last-hour":
					{
						startTime = endTime.AddHours(-1);
						break;
					}
				case "last-24":
				default:
					{
						startTime = endTime.AddHours(-24);
						break;
					}

			}

		  _session.Advanced.MaxNumberOfRequestsPerSession = 100;
			var logItems = _session.Query<QueryLogItem>().Where(q => q.DateTime > startTime);
			var queryNames = logItems.Select(q => q.Name).Distinct().ToList();

			var queryStatistics = new List<QueryStatisticsModel>();

			foreach (string queryName in queryNames)
			{
			  string name = queryName;
			  var set = GetAll(logItems.Where(q => q.Name == name));
				queryStatistics.Add(new QueryStatisticsModel
					{
						//var sum = list.Aggregate((acc, cur) => acc + cur);
						//var average = ;
						Name = queryName,
						//AverageExecutionTime = items.Select(q => q.ExecutionTime).Aggregate((acc, cur) => acc + cur) / items.Count(), 
						AverageTime = Convert.ToInt32(Math.Round(set.Average(ed => ed.ExecutionTime))),
						AverageExecutionTime = (set.Any(q => !q.CacheHit)) ? Convert.ToInt32(Math.Round(set.Where(q => !q.CacheHit).Average(ed => ed.ExecutionTime))) : 0,
						AverageCachedTime = (set.Any(q => q.CacheHit)) ? Convert.ToInt32(Math.Round(set.Where(q => q.CacheHit).Average(ed => ed.ExecutionTime))) : 0,
						Hits = set.Count(),
						CacheHits = set.Count(ed => ed.CacheHit == true)
					});
			}

			return Json(queryStatistics, JsonRequestBehavior.AllowGet);

			//var sum = list.Aggregate((acc, cur) => acc + cur);
			//var average = list.Aggregate((acc, cur) => acc + cur) / list.Count;

			//var webclient = new WebClient();
			//string s = webclient.DownloadString(ApiConfiguration.Current.QueryApiUrl + "/Log/Index");

			//return Content(s);// Json(ApiConfiguration.Current, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public ActionResult Downloads(int start = 0, int count = 10, string timespan = "month")
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
			return Json(downloadList, JsonRequestBehavior.AllowGet);
		}

		private LogDownloadListModel ListWeeklyDownloads(int start, int count)
		{
			// get the min dates per query
			var queryStatistics = _session.Query<Indexes.QueryStatistics.Result, Indexes.QueryStatistics>().ToList();
			// get the min date from all queries
			var minTimestamp = queryStatistics.Where(q => q.First > DateTime.MinValue).Min(q => q.First).Date;

			//calculate total weeks from first log item
			var today = DateTime.UtcNow;
			var dayOfWeek = (int) today.DayOfWeek;
			var timespan = (today.AddDays(-dayOfWeek) - minTimestamp);
			int weeks = (int) Math.Ceiling(timespan.TotalDays/7);

			var weeklyDownloads = new LogDownloadListModel
				{
					TotalCount = weeks,
					Start = start,
					End = Math.Min(start + count, weeks),
					Items = new List<LogDownLoadModel>()
				};
			for (var i = weeklyDownloads.Start; i < weeklyDownloads.End; i++)
			{
				var endOfWeek = today.AddDays(-dayOfWeek - i*7);
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
			var queryStatistics = _session.Query<Indexes.QueryStatistics.Result, Indexes.QueryStatistics>().ToList();

			if (queryStatistics.Count == 0) return null;

			// get the min date from all queries
			var minTimestamp = queryStatistics.Where(q => q.First > DateTime.MinValue).Min(q => q.First).Date;

			//calculate total weeks from first log item
			var today = DateTime.UtcNow;
			int offsetDays = DateTime.DaysInMonth(today.Year, today.Month) - (int)today.Day;
			int months = ((today.Year - minTimestamp.Year)*12) + today.Month - minTimestamp.Month + 1;

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

		public FileContentResult Download(DateTime start, DateTime end)
		{
			var textWriter = new StringWriter();

			var csv = new CsvWriter(textWriter);
			csv.Configuration.Delimiter = ";";
			csv.WriteHeader<QueryLogItem>();

		  var documentStore = _session.Advanced.DocumentStore;

		  var query = documentStore.OpenSession().Query<QueryLogItem>().Where(q => q.DateTime >= start && q.DateTime <= end.AddHours(24));

		  const int BatchSize = 1024;
			int pos = 0;
      var items = query.Skip(pos).Take(BatchSize);
      int requestCounter = 1;
      while (items.Any())
			{
				foreach (QueryLogItem queryLogItem in items)
				{
					csv.WriteRecord(queryLogItem);
				}
        pos += BatchSize;
			  requestCounter++;
        // since RavenDB limits the number of query's per session, we create a new one each 10? query's
        if (requestCounter > 10) {
          query = documentStore.OpenSession().Query<QueryLogItem>().Where(q => q.DateTime >= start && q.DateTime <= end.AddHours(24));
          requestCounter = 1;
        }
        items = query.Skip(pos).Take(BatchSize);

			}
			textWriter.Close();

			return File(new System.Text.UTF8Encoding().GetBytes(textWriter.ToString()), "text/csv", string.Format("log {0:yyyy-MM-dd} -- {1:yyyy-MM-dd}.csv", start, end));
		}


    private IList<T> GetAll<T>(IQueryable<T> queryable) {
      int start = 0;
      var result = new List<T>();
      while (true) {
        var current = queryable.Take(1024).Skip(start).ToList();
        if (current.Count == 0) break;
        start += current.Count;
        result.AddRange(current);
      }
      return result;
    }
  }

}