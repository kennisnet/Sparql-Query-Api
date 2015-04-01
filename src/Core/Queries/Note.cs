using System;

namespace Trezorix.Sparql.Api.Core.Queries {

  public class Note {
    public DateTime CreationDate { get; set; }

    public DateTime ModificationDate { get; set; }

    public string AccountId { get; set; }

    public string Content { get; set; }
  }
}
