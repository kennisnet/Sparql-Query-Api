using System;
using System.IO;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;
using Trezorix.Sparql.Api.Core.Repositories;

namespace Trezorix.Sparql.Api.QueryApi.MediaTypeFormatters
{
	public class RdfHtmlMediaTypeFormatter : MediaTypeFormatter
	{
		public RdfHtmlMediaTypeFormatter()
		{
			// ToDo: Support N3 and Turtle?
			SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
		}

		public override bool CanReadType(Type type)
		{
			// ToDo: receive Rdf?
			return false;
		}

		public override bool CanWriteType(Type type)
		{

			return typeof(XmlNode).IsAssignableFrom(type);
		}
		
		public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, System.Net.Http.HttpContent content, System.Net.TransportContext transportContext)
		{
			var taskSource = new TaskCompletionSource<object>();
			try
			{
				var xml = (XmlNode) value;

				var parameters = new XsltArgumentList();
				var transform = XsltRepository.Get("RdfToHtml.xslt");
				transform.Transform(xml, parameters, writeStream);

				//((XmlDocument)xml).Save(writeStream);
				
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