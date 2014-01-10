using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Trezorix.Sparql.Api.QueryApi.MediaTypeFormatters
{
	public class RdfCsvMediaTypeFormatter : MediaTypeFormatter
	{
		public RdfCsvMediaTypeFormatter()
		{
			SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));
		}

		public override bool CanReadType(Type type)
		{
			return false;
		}

		public override bool CanWriteType(Type type)
		{
			return typeof(XmlNode).IsAssignableFrom(type);
		}

		public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
		{
			base.SetDefaultContentHeaders(type, headers, mediaType);
			string name = HttpContext.Current.Request.Url.Segments[HttpContext.Current.Request.Url.Segments.Length-1];
			headers.Add("Content-Disposition", "attachment; filename=" + name + ".csv");
		}
		
		public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, System.Net.Http.HttpContent content, System.Net.TransportContext transportContext)
		{
			var taskSource = new TaskCompletionSource<object>();
			try
			{
				var sb = new StringBuilder();

				var xml = (XmlNode) value;
				XmlDocument document = (xml.OwnerDocument) ?? (XmlDocument)xml;
				var namespaceManager = new XmlNamespaceManager(document.NameTable);
				namespaceManager.AddNamespace("sparql", "http://www.w3.org/2005/sparql-results#");
				var variables = new List<string>();
				var variableNodes = xml.SelectNodes("sparql:sparql/sparql:head/sparql:variable", namespaceManager);
				
				foreach (XmlElement node in variableNodes)
				{
					variables.Add(node.GetAttribute("name"));
					sb.Append(node.GetAttribute("name") + ";");
				}
				sb.AppendLine();

				XmlNodeList resultNodes = xml.SelectNodes("sparql:sparql/sparql:results/sparql:result", namespaceManager);
				foreach (XmlElement node in resultNodes)
				{
					foreach (string variable in variables)
					{
						var binding = node.SelectSingleNode("sparql:binding[@name='" + variable + "']", namespaceManager);
						if (binding != null)
						{
							sb.Append(binding.FirstChild.InnerText);
						}
						sb.Append(";");
					}
					sb.AppendLine();
				}

				var streamWriter = new StreamWriter(writeStream);
        streamWriter.Write(sb.ToString());
		
				taskSource.SetResult(null);
			}
			catch (Exception e)
			{
				taskSource.SetException(e);
			}
			return taskSource.Task;
		}
	}

}