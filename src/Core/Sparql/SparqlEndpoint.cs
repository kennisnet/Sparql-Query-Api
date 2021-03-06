﻿using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Trezorix.Sparql.Api.Core.Helpers;

namespace Trezorix.Sparql.Api.Core.Sparql
{
	using System.Diagnostics;

	public class SparqlEndpoint
	{
		public string Name { get; set; }
		public string Url { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public int Timeout { get; set; }
		public string Namespaces { get; set; }

		public T Query<T>(string query, bool debug = false) where T : class
		{
			if (typeof (T) == typeof (SparqlResponse))
			{
				var stream = (MemoryStream)ExecuteSparqlQuery(query, "json", debug);

				var data = Encoding.UTF8.GetString(stream.ToArray());
				var result = JsonConvert.DeserializeObject<SparqlResponse>(data);
				return result as T;
			}
			if (typeof(T) == typeof(XmlDocument))
			{
				var stream = (MemoryStream)ExecuteSparqlQuery(query, "xml", debug);
				var result = new XmlDocument();
				result.Load(stream);
				return result as T;
			}
			if (typeof(T) == typeof(object)) {
				var stream = (MemoryStream)ExecuteSparqlQuery(query, "json", debug);

				var data = Encoding.UTF8.GetString(stream.ToArray());
				var result = JsonConvert.DeserializeObject<dynamic>(data);
				return result as T;
			}
			return null;
		}

		private Stream ExecuteSparqlQuery(string query, string output, bool debug)
		{
			MemoryStream stream = null;

			var client = new WebClientWithTimeout(Math.Max(Timeout, 90*1000));
			if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
			{
				var uri = new Uri(Url);
				client.Credentials = new NetworkCredential(Username, Password, "");
			}
			switch (output)
			{
				case "json":
				client.Headers.Add(
					HttpRequestHeader.Accept,
					query.ToLowerInvariant().Contains("construct")
					? "application/sparql-results+json, application/rdf+json, application/ld+json"
					: "application/sparql-results+json");
					break;
				case "xml":
				client.Headers.Add(
					HttpRequestHeader.Accept,
					query.ToLowerInvariant().Contains("construct")
					? "application/rdf+xml, application/sparql-results+xml"
					: "application/sparql-results+xml");
				break;
			}
			client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
			client.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-16");

			string sparqlQuery = query;
			string url = Url + (debug ? (Url.Contains("?") ? "&" : "?") +  "debug=true" : "");
			byte[] result = client.UploadValues(url, new NameValueCollection
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