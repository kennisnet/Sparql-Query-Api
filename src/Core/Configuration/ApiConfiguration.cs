using System.Collections.Generic;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Trezorix.Sparql.Api.Core.Sparql;

namespace Trezorix.Sparql.Api.Core.Configuration
{

	public class ApiConfiguration
	{
		[JsonIgnoreAttribute]
		public string RepositoryRoot { get; private set; }
		public string Database { get; set; }
		public string ApplicationTitle { get; set; }
		public string Description { get; set; }
		public IEnumerable<SparqlEndpoint> SparqlEndpoints { get; set; }
		public string QueryApiUrl { get; set; }
		public string AdminApiUrl { get; set; }
		public bool AllowAnonymous { get; set; }

		public static ApiConfiguration Current { get; set; }

		public ApiConfiguration()
		{
			//switch (hostName.ToLower())
			//{
			//	case "obk.edustandaard":
			//		_sparqlEndpointUrl = "http://obk.edustandaard.nl/sparql";
			//		break;
			//	case "staging-obk.edustandaard":
			//		_sparqlEndpointUrl = "http://staging-obk.edustandaard.nl/sparql";
			//		break;
			//	case "pld-obk.edustandaard":
			//		_sparqlEndpointUrl = "http://pld3.rnatoolset.net/sparql";
			//		break;
			//	default:
			//		_sparqlEndpointUrl = "http://pld3.rnatoolset.net/sparql";
			//		break;
			//}

		}

		public static ApiConfiguration Init(string hostName)
		{
			string root = HttpContext.Current.Server.MapPath("~");
			if (root.EndsWith("Query") || root.EndsWith("Admin") || root.EndsWith("Api"))
			{
				root += "..\\";
			}
			string repositoryRoot = root + "..\\Data\\";

			string json = File.ReadAllText(repositoryRoot + "settings.json");
			var configuration = JsonConvert.DeserializeObject<ApiConfiguration>(json);
			configuration.RepositoryRoot = repositoryRoot;

			return configuration;
		}

		public static void Save(ApiConfiguration configuration)
		{
			string json = JsonConvert.SerializeObject(configuration, Formatting.Indented);
			File.WriteAllText(Current.RepositoryRoot + "settings.json", json);

			// always assign repository root back (dont know another way?)
			configuration.RepositoryRoot = Current.RepositoryRoot; 
			Current = configuration;
		}

	}
}