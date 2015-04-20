namespace Trezorix.Sparql.Api.Core.Queries
{
  public class QueryLogStatisticsByAccount
  {
    public string AccountId { get; set; }
    public string Endpoint { get; set; }
    public string Format { get; set; }
    public double AverageTime { get; set; }
    //public int AverageCachedTime { get; set; }
    //public int Hits { get; set; }
    //public int CacheHits { get; set; }
  }
}
