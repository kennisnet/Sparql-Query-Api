using System.Collections.Generic;

namespace Trezorix.Sparql.Api.Admin.Models.Queries
{
	public class QueryGroupModel
	{
		public string Id { get; set; }
		public string Label { get; set; }
		public IEnumerable<QueryModel> Items { get; set; }
	}
}