namespace Trezorix.Sparql.Api.QueryApi.MediaTypeFormatters {
  using System;
  using System.IO;
  using System.Net.Http.Formatting;
  using System.Net.Http.Headers;
  using System.Threading.Tasks;
  using System.Xml;

  public class RdfXmlMediaTypeFormatter : MediaTypeFormatter
  {
    public RdfXmlMediaTypeFormatter()
    {
      // ToDo: Support N3 and Turtle?
      this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/rdf+xml"));
      this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/xml"));
    }

    public override bool CanReadType(Type type)
    {
      // ToDo: receive Rdf?
      return false;
    }

    public override bool CanWriteType(Type type)
    {
      return true;
      //return typeof(XmlNode).IsAssignableFrom(type);
    }
		
    public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, System.Net.Http.HttpContent content, System.Net.TransportContext transportContext)
    {
      var taskSource = new TaskCompletionSource<object>();
      try
      {
        var xml = (XmlNode) value;

        var documentElement = ((XmlDocument)value).DocumentElement;
        if (documentElement != null) {
          ((XmlDocument)xml).Save(writeStream);
        }
				
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