namespace Trezorix.Sparql.Api.Admin.Models.Queries {
  using System;

  public class NoteModel {

    public DateTime CreationDate { get; set; }

    public DateTime ModificationDate { get; set; }

    public string AccountId { get; set; }

    public string Content { get; set; }
  }
}
