namespace Trezorix.Sparql.Api.Core.Queries {
  public class QueryLogStatisticsByQueryName {
    public string QueryName { get; set; }

    public string Endpoint { get; set; }

    public string Format { get; set; }

    public string RemoteIp { get; set; }

    public double AverageTime { get; set; }

    public double SumTime { get; set; }
  }
}