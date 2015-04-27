namespace Trezorix.Sparql.Api.Core.Queries {
  public class QueryLogStatisticsByColumn {
    public string MasterColumn { get; set; }

    public string Endpoint { get; set; }

    public string AcceptFormat { get; set; }

    public string RemoteIp { get; set; }

    public double AverageTime { get; set; }

    public long TotalHits { get; set; }

    public double NoCacheSumTime { get; set; }

    public long NoCacheTotalHits { get; set; }

    public double CacheSumTime { get; set; }

    public long CacheTotalHits { get; set; }
  }
}