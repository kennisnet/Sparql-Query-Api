namespace Trezorix.Sparql.Api.Core.EventSourcing
{
  using System;

  using MongoRepository;

  public class EventStore : Entity
  {
    public string AccountId { get; set; }
    public Events EventName { get; set; }
    public DateTime Date { get; set; }    
    public dynamic Payload { get; set; }
  }
}
