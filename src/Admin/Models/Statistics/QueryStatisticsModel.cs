namespace Trezorix.Sparql.Api.Admin.Models.Statistics
{
	public class QueryStatisticsModel
	{
		public string Name { get; set; }
		public int AverageTime { get; set; }
		public int AverageExecutionTime { get; set; }
		public int AverageCachedTime { get; set; }
		public int Hits { get; set; }
		public int CacheHits { get; set; }
	}
}