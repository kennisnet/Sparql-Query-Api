namespace Trezorix.Sparql.Api.Core.Repositories
{
	using System.Collections.Generic;

	using Trezorix.Sparql.Api.Core.Queries;

	public interface IQueryLogRepository
	{
		QueryLogItem Add(QueryLogItem queryLogItem);
		IEnumerable<QueryLogItem> All();
	}
}
