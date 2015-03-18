using System;
using System.IO;
using System.Web.Mvc;
using CsvHelper;

using Trezorix.Sparql.Api.Admin.Controllers.Core;
using Trezorix.Sparql.Api.Core.Configuration;

namespace Trezorix.Sparql.Api.Admin.Controllers
{
	using Trezorix.Sparql.Api.Core.Queries;
	using Trezorix.Sparql.Api.Core.Repositories;

	public class LogController : BaseController
	{
		private readonly IQueryLogRepository _queryLogRepository;

		public LogController(IQueryLogRepository queryLogRepository) {
			_queryLogRepository = queryLogRepository;
		}

		[HttpGet]
		public ActionResult Index() {
			return View(ApiConfiguration.Current);
		}

		public FileContentResult Download(DateTime start, DateTime end) {
			var textWriter = new StringWriter();

			var csv = new CsvWriter(textWriter);
			csv.Configuration.Delimiter = ";";
			csv.WriteHeader<QueryLogItem>();

			var queryLogItems = _queryLogRepository.GetByDateRange(start, end);

			foreach (var queryLogItem in queryLogItems)
			{
				csv.WriteRecord(queryLogItem);
			}

			textWriter.Close();

			return File(new System.Text.UTF8Encoding().GetBytes(textWriter.ToString()), "text/csv", string.Format("log {0:yyyy-MM-dd} -- {1:yyyy-MM-dd}.csv", start, end));
		}
  }
}