using System;
using System.Collections.Generic;

namespace Trezorix.Sparql.Api.Core.Queries
{
	using MongoRepository;

	using Trezorix.Sparql.Api.Core.Authorization;

  public class Query : Entity
	{
		public Query() {
			Notes = new List<Note>();
			ApiKeys = new List<string>();
			Parameters = new List<QueryParameter>();
		  Authorization = new List<AuthorizationSettings>();
		}
		private string label;

		public string Alias { get; set; }
		public string Label
		{
			get { return (string.IsNullOrEmpty(this.label)) ? Alias : this.label; }
			set { this.label = value; }
		}

		public string SparqlQuery { get; set; }
		public string Description { get; set; }
		public string Group { get; set; }
		public string Endpoint { get; set; }		
		public bool AllowAnonymous { get; set; }

		public IList<Note> Notes { get; set; }
    public IList<AuthorizationSettings> Authorization { get; set; }
		public IList<string> ApiKeys { get; set; }
		public IList<QueryParameter> Parameters { get; set; }
	}
}