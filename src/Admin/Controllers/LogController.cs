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

  using Trezorix.Sparql.Api.Application.Accounts;

  public class LogController : BaseController
	{
		private readonly IDocumentSession _session;

		public LogController(IDocumentSession session)
		{
			_session = session;
		}

		[HttpGet]
		public ActionResult Index() {
			return View(ApiConfiguration.Current);
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

  }

}