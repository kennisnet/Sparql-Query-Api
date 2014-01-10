using System.Collections.Generic;

namespace Trezorix.Sparql.Api.Admin.Models.Queries
{
	public class GroupedQueryModel
	{
		public IList<QueryGroup> Groups { get; set; }
	}
}