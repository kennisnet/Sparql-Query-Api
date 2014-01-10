using System.Collections.Generic;
using Trezorix.Sparql.Api.Core.Queries;

namespace Trezorix.Sparql.Api.Admin.Models.Queries
{
	public class QueryGroup
	{
		public string Id { get; set; }
		public string Label { get; set; }
		public IEnumerable<Query> Items { get; set; }
	}
}