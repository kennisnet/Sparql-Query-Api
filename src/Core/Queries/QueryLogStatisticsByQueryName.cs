namespace Trezorix.Sparql.Api.Core.Queries {
  public class QueryLogStatisticsByQueryName {
    public string QueryName { get; set; }

    public string Endpoint { get; set; }

    public string Format { get; set; }

    public string RemoteIp { get; set; }

    public double AverageTime { get; set; }

    public long TotalHits { get; set; }

    public double NoCacheSumTime { get; set; }

    public long NoCacheTotalHits { get; set; }

    public double CacheSumTime { get; set; }

    public long CacheTotalHits { get; set; }
  }
}