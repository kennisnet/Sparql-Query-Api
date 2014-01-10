using System;
using System.Collections.Generic;

namespace Trezorix.Sparql.Api.Core.Queries
{
	public class Query
	{
		private string _label;
		public string Id { get; set; }
		public string Label
		{
			get { return (string.IsNullOrEmpty(_label)) ? Id : _label; }
			set { _label = value; }
		}

		public string SparqlQuery { get; set; }
		public string Description { get; set; }
		public string Group { get; set; }
		public string Endpoint { get; set; }
		public bool AllowAnonymous { get; set; }
		public IList<Guid> ApiKeys { get; set; }
		public IEnumerable<QueryParameter> Parameters { get; set; }
	}
}