using System.IO;
using System.Runtime.Caching;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Xsl;

namespace Trezorix.Sparql.Api.Core.Repositories
{
	public static class XsltRepository
	{
		private static readonly ObjectCache _xsltCache = MemoryCache.Default;

		public static XslCompiledTransform Get(string name)
		{
			XslCompiledTransform result = (_xsltCache != null) ? (XslCompiledTransform)_xsltCache[name] : null;
			if (result == null)
			{
				string filePathname = HostingEnvironment.MapPath(@"~/Xslt/" + name);
				if (!File.Exists(filePathname))
				{
					filePathname = HostingEnvironment.MapPath(@"~/bin/Xslt/" + name);
				}
				result = CompileXslt(filePathname);
				if (_xsltCache != null)
				{
					_xsltCache[name] = result;
				}
			}
			return result;
		}

		private static XslCompiledTransform CompileXslt(string fullFileName)
		{
			var xsltSettings = new XsltSettings(true, false);

			XslCompiledTransform xslTransform;
			using (XmlReader xmlReader = XmlReader.Create(fullFileName,
			                                              new XmlReaderSettings
			                                              	{
			                                              		DtdProcessing = DtdProcessing.Parse
			                                              	}))
			{
				xslTransform = new XslCompiledTransform();
				xslTransform.Load(xmlReader, xsltSettings, new XmlUrlResolver());
			}
			return xslTransform;
		}
	}

}