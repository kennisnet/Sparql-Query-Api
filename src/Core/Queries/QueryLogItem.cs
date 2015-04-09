using System;
using System.Web;

namespace Trezorix.Sparql.Api.Core.Queries
{
	using MongoRepository;

	public class QueryLogItem : Entity
	{
		public string Name { get; set; }
		public string AccountId { get; set; }
		public string RemoteIp { get; set; }
		public string Referrer { get; set; }
		public bool CacheHit { get; set; }
		public string Endpoint { get; set; }
		public string AcceptFormat { get; set; }
		public DateTime DateTime { get; set; }
		public long ExecutionTime { get; set; }

		public static QueryLogItem FromRequest(string name, HttpRequest request)
		{
			string remoteForwardedIp = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

			var queryLogItem = new QueryLogItem()
				{
					Name = name,
					DateTime = DateTime.UtcNow,
					AccountId = request.Params["api_key"], 
					RemoteIp = (!String.IsNullOrEmpty(remoteForwardedIp)) ? remoteForwardedIp : request.ServerVariables["REMOTE_ADDR"],
					Referrer = request.ServerVariables["REFERER"],			
				};
			return queryLogItem;
		}
	}
}