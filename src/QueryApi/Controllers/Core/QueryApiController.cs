using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Web.Http;
using System.Xml;
using System.Xml.Xsl;
using NLog;

namespace Trezorix.Sparql.Api.QueryApi.Controllers.Core
{

	public class QueryApiController : ApiController
	{
		protected readonly Logger Logger = LogManager.GetCurrentClassLogger();
		protected readonly Stopwatch Stopwatch = new Stopwatch();
		protected readonly ObjectCache Cache = MemoryCache.Default;

		protected Stream TransformQueryResult(XslCompiledTransform transform, XmlDocument rdfXml, XsltArgumentList parameters)
		{
			var stream = new MemoryStream();
			transform.Transform(rdfXml, parameters, stream);

			stream.Seek(0, SeekOrigin.Begin);
			return stream;
		}

		protected string Version
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version.Major + "." + Assembly.GetExecutingAssembly().GetName().Version.Minor;
			}
		}

		protected string Build
		{
			get
			{
				return "" + Assembly.GetExecutingAssembly().GetName().Version.Build;
			}
		}

		protected bool IsValidUri(string value)
		{
			return value.Trim().StartsWith("bk:");
		}

		protected bool IsValidUriList(string values)
		{
			return values.Split(',').All(value => value.Trim().StartsWith("bk:"));
		}

	}
}
