namespace Trezorix.Sparql.Api.Admin.Models.Queries
{
	using System;
	using System.Collections.Generic;

	using Trezorix.Sparql.Api.Core.Authorization;
	using Trezorix.Sparql.Api.Core.Queries;

	public class QueryModel
	{
		public QueryModel() {
			this.Notes = new List<Note>();
			this.ApiKeys = new List<string>();
			this.Parameters = new List<QueryParameter>();
      Authorization = new List<AuthorizationSettings>();
    }

		public string Id { get; set; }
		public string Alias { get; set; }
		public string Label { get; set; }

		public string SparqlQuery { get; set; }
		public string Description { get; set; }
		public string Group { get; set; }
		public string Endpoint { get; set; }		
		public bool AllowAnonymous { get; set; }

		public IList<Note> Notes { get; set; }
		public IList<string> ApiKeys { get; set; }
		public IList<QueryParameter> Parameters { get; set; }
    public IList<AuthorizationSettings> Authorization { get; set; }
	}
}