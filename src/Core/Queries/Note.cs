using System;

namespace Trezorix.Sparql.Api.Core.Queries {
  public class Note {
    public DateTime CreationDate { get; set; }

    public DateTime ModificationDate { get; set; }

    // TODO: AccountId objectId toevoegen

    public string Content { get; set; }
  }
}
