using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client.Indexes;
using Trezorix.Sparql.Api.Core.Queries;

namespace Trezorix.Sparql.Api.Admin.Indexes
{
	public class QueryStatistics : AbstractIndexCreationTask<QueryLogItem, QueryStatistics.Result>
	{
		public class Result
		{
			public string QueryName { get; set; }
			public DateTime First { get; set; }
		}

		public QueryStatistics()
		{
			Map = queryLogItems => from queryLogItem in queryLogItems
												select new
												{
													QueryName = queryLogItem.Name,
													First = queryLogItem.DateTime
												};

			Reduce = results => from result in results
													group result by result.QueryName into g
													select new
													{
														QueryName = g.Key,
														First = g.Min(x => x.First)
													};
		}

	}
}