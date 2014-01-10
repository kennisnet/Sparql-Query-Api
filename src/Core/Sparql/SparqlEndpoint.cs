using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Trezorix.Sparql.Api.Core.Helpers;

namespace Trezorix.Sparql.Api.Core.Sparql
{

	public class SparqlEndpoint
	{
		public string Name { get; set; }
		public string Url { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public int Timeout { get; set; }
		public string Namespaces { get; set; }

		public T Query<T>(string query) where T : class
		{
			if (typeof (T) == typeof (SparqlResponse))
			{
				var stream = (MemoryStream)ExecuteSparqlQuery(query, "json");

				var data = Encoding.UTF8.GetString(stream.ToArray());
				var result = JsonConvert.DeserializeObject<SparqlResponse>(data);
				return result as T;
			}
			if (typeof(T) == typeof(XmlDocument))
			{
				var stream = (MemoryStream)ExecuteSparqlQuery(query, "xml");
				var result = new XmlDocument();
				result.Load(stream);
				return result as T;
			}
			return null;
		}

		private Stream ExecuteSparqlQuery(string query, string output)
		{
			MemoryStream stream = null;

			var client = new WebClientWithTimeout(Math.Max(Timeout, 90*1000));
			if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
			{
				client.Credentials = new NetworkCredential(Username, Password, new Uri(Url).Host);
			}
			switch (output)
			{
				case "json":
					client.Headers.Add(HttpRequestHeader.Accept, "application/rdf+json, application/sparql-results+json");
					break;
				case "xml":
					client.Headers.Add(HttpRequestHeader.Accept, "application/rdf+xml, application/sparql-results+xml");
					break;
			}
			client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
			client.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-16");

			string sparqlQuery = query;

			byte[] result = client.UploadValues(Url, new NameValueCollection
				{
					{
						"query", sparqlQuery
					}
				});

			if (result != null)
			{
				stream = new MemoryStream(result);
			}
			return stream;
		}

	}
}