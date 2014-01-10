using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trezorix.Sparql.Api.Core.Sparql
{
	public class SparqlResponse
	{
		public SparqlHead Head { get; set; }
		public SparqlResults Results { get; set; }
	}

	public class SparqlResults
	{
		public bool Distinct { get; set; }
		public bool Ordered { get; set; }
		public IEnumerable<Dictionary<string, SparqlBinding>> Bindings { get; set; }

	}

	public class SparqlBinding
	{
		public string Type { get; set; }
		public string Value { get; set; }
	}

	public class SparqlHead
	{
		public IEnumerable<string> Link { get; set; }
		public IEnumerable<string> Vars { get; set; }
	}

	//Dictionnary<string,Album>
}
