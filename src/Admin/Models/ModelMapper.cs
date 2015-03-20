namespace Trezorix.Sparql.Api.Admin.Models {
  using System.Diagnostics.CodeAnalysis;

  using AutoMapper;

  using Trezorix.Sparql.Api.Admin.Models.Queries;
  using Trezorix.Sparql.Api.Core.Queries;

  [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
    Justification = "Reviewed. Suppression is OK here.")]
  public static class ModelMapper {
    public static void ConfigureMapper() {
      Mapper.CreateMap<Note, NoteModel>().ReverseMap();
      Mapper.CreateMap<Query, QueryModel>().ReverseMap();
    }
  }
}