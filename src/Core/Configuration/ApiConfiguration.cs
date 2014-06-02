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

		public static ApiConfiguration Init(string instanceName)
		{
			string root = HttpContext.Current.Server.MapPath("~").ToLowerInvariant();
			if (root.EndsWith("query") || root.EndsWith("admin") || root.EndsWith("api"))
			{
				root += "..\\";
			}
			string repositoryRoot = string.Format("{0}..\\Data\\{1}\\", root, instanceName);

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