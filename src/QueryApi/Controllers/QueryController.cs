using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Xml;

using Trezorix.Sparql.Api.Application.Filters;
using Trezorix.Sparql.Api.Application.TimeTracking;
using Trezorix.Sparql.Api.Core.Configuration;
using Trezorix.Sparql.Api.Core.Queries;
using Trezorix.Sparql.Api.Core.Repositories;
using Trezorix.Sparql.Api.Core.Sparql;
using Trezorix.Sparql.Api.QueryApi.Authorizer;
using Trezorix.Sparql.Api.QueryApi.Controllers.Core;

namespace Trezorix.Sparql.Api.QueryApi.Controllers
{
	public class QueryController : QueryApiController
	{		
		private readonly IQueryRepository _queryRepository;
		private readonly IQueryLogRepository _queryLogRepository;
		private readonly IAccountRepository _accountRepository;

		public QueryController(IQueryRepository queryRepository, IQueryLogRepository queryLogRepository, IAccountRepository accountRepository)
		{
			_queryRepository = queryRepository;
			_queryLogRepository = queryLogRepository;
			_accountRepository = accountRepository;
		}

		public HttpResponseMessage Get()
		{
			var response = this.Request.CreateResponse(HttpStatusCode.Redirect);
			var apiKey = HttpContext.Current.Request.Params["api_key"];
			var url = ApiConfiguration.Current.QueryApiUrl + "/Home/Index";
			response.Headers.Location = new Uri(url + "?api_key=" + apiKey);
			return response;
		}

		[ApiKeyAuth("api_key", typeof(ApiKeyAuthorizer))]
		public dynamic Get(string name, bool debug = false, bool showQuery = false)
		{
			var type = GetOutputType();
			if (type.Name == "XmlDocument" && !showQuery)
			{
				return Execute<XmlDocument>(name, HttpContext.Current.Request.Params, debug, showQuery);
			}

			return Execute<object>(name, HttpContext.Current.Request.Params, debug, showQuery);
		}

		[ApiKeyAuth("api_key", typeof(ApiKeyAuthorizer))]
		public dynamic Post(string name, [FromBody]dynamic model, bool debug = false, bool showQuery = false)
		{
			var type = GetOutputType();
			if (type.Name == "XmlDocument" && !showQuery) {
				return Execute<XmlDocument>(name, model, debug, showQuery);
			}

			return Execute<object>(name, model, debug, showQuery);
		}

		[HttpGet]
		[ApiKeyAuth("api_key", typeof(ApiKeyAuthorizer))]
		public IEnumerable<dynamic> ParameterValues(string name, string parameter, bool debug = false)
		{
			var query = _queryRepository.GetByAlias(name);

			if (query == null)
			{
				var notFoundMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
				throw new HttpResponseException(notFoundMessage);
			}

			var endpoint = FindEndpointForQuery(query);

			if (!IsAuthorized(query))
			{
				var message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
				throw new HttpResponseException(message);
			}

			// get the requested parameter
			var queryParameter = query.Parameters.FirstOrDefault(p => p.Name == parameter);

			if (queryParameter == null)
			{
				var notFoundMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
				throw new HttpResponseException(notFoundMessage);
			}

			var sq = new Parameterized‎SparqlQuery(endpoint.Namespaces, queryParameter.ValuesQuery);

			var labelUriList = new List<dynamic>();

			if (!string.IsNullOrEmpty(queryParameter.ValuesQuery))
			{
				var result = ExecuteCachedJsonQuery(string.Format("{0}_{1}", name, parameter), true, sq, endpoint, null, null);

				foreach (var binding in result.Results.Bindings)
				{
					labelUriList.Add(new { Label = binding.ElementAt(1).Value.Value, Value = binding.ElementAt(0).Value.Value });
				}
			}

			return labelUriList;
		}

		private dynamic Execute<T>(string name, dynamic model, bool debug, bool showQuery) where T : class
		{
			Logger.Info("url: " + HttpContext.Current.Request.Url);

			var queryLogItem = QueryLogItem.FromRequest(name, HttpContext.Current.Request);

			var timeTracker = new TimeTracker();
			timeTracker.Start("Query");

			var query = _queryRepository.GetByAlias(name);

			if (query == null)
			{
				var notFoundMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
				throw new HttpResponseException(notFoundMessage);
			}

      var endpoint = (model != null && model["endpoint"] != null) ? FindEndpointByName(model["endpoint"]) : FindEndpointForQuery(query);

			if (!IsAuthorized(query))
			{
				var message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
				throw new HttpResponseException(message);
			}

			var sq = new Parameterized‎SparqlQuery(endpoint.Namespaces, query.SparqlQuery);
			sq.InjectParameterValues(model);

      if (showQuery) {
        return sq;
      }
      
      var xmlOut = (T)ExecuteCachedQuery<T>(name, debug, sq, endpoint, timeTracker, queryLogItem);

			timeTracker.End("Query");
			timeTracker.WriteLog(Logger);

			queryLogItem.ExecutionTime = timeTracker.TotalTime;

			_queryLogRepository.Add(queryLogItem);

			return xmlOut;
		}

		private static Type GetOutputType() {
			string accept = HttpContext.Current.Request.Headers["Accept"];
			string format = HttpContext.Current.Request.Params["format"];

			if (format != null && ("json").Contains(format)) {
				return typeof(object);
			}

      if ((accept != null && (accept.Contains("text/html") || accept.Contains("application/xml") || accept.Contains("application/xhtml+xml"))) || (format != null && ("csv|xml").Contains(format))) {
				return typeof(XmlDocument);
			}

			return typeof(object);
		}

		private static bool IsAuthorized(Query query)
		{
			if (!query.AllowAnonymous)
			{
				var apiKey = HttpContext.Current.Request.Params["api_key"];
				if (query.ApiKeys.All(k => k != Guid.Parse(apiKey)))
				{
					return false;
				}
			}
			return true;
		}

		private static SparqlEndpoint FindEndpointForQuery(Query query)
		{
			var endpoint = (query.Endpoint == null)
											 ? ApiConfiguration.Current.SparqlEndpoints.First()
											 : ApiConfiguration.Current.SparqlEndpoints.First(s => s.Name == query.Endpoint);
			return endpoint;
		}

    private static SparqlEndpoint FindEndpointByName(string name) {
      return ApiConfiguration.Current.SparqlEndpoints.First(s => s.Name == name);
    }
    
    private SparqlResponse ExecuteCachedJsonQuery(string name, bool debug, Parameterized‎SparqlQuery sq, SparqlEndpoint endpoint,
																					 TimeTracker timeTracker, QueryLogItem queryLogItem)
		{
			SparqlResponse response;

      string cacheKey = endpoint.Name + "|" + name + "|" + string.Join("|", sq.Params.Select((key, value) => key + ":" + value));

			if (Cache[cacheKey] != null && !debug)
			{
				response = (SparqlResponse)Cache[cacheKey];
				Logger.Info("Cache hit on: " + cacheKey);
				if (queryLogItem != null) queryLogItem.CacheHit = true;
			}
			else
			{
				response = endpoint.Query<SparqlResponse>(sq.Query, debug);

				if (!debug && ApiConfiguration.Current.CacheTime > 0)
				{
					Cache.Add(cacheKey, response, DateTime.Now.AddMinutes(ApiConfiguration.Current.CacheTime));
					//Cache[cacheKey] = response;
          Logger.Debug("Added to cache: " + cacheKey);
				}
			}
			return response;
		}

		private dynamic ExecuteCachedQuery<T>(string name, bool debug, Parameterized‎SparqlQuery sq, SparqlEndpoint endpoint,
																					 TimeTracker timeTracker, QueryLogItem queryLogItem) where T : class
		{
			dynamic xmlOut;

      string cacheKey = endpoint.Name + "|" + name + "|" + typeof(T).Name + "|" + string.Join("|", sq.Params.Select((key, value) => key + ":" + value));

			if (Cache[cacheKey] != null && !debug)
			{
				xmlOut = (dynamic) Cache[cacheKey];
				Logger.Info("Cache hit on: " + cacheKey);
				if (queryLogItem != null) queryLogItem.CacheHit = true;
			}
			else
			{
				xmlOut = endpoint.Query<T>(sq.Query, debug);
				Logger.Debug("SPARQL: " + sq.Query);

				if (!debug && ApiConfiguration.Current.CacheTime > 0)
				{
					Cache.Add(cacheKey, xmlOut, DateTime.Now.AddMinutes(ApiConfiguration.Current.CacheTime));
					//Cache[cacheKey] = xmlOut;
					Logger.Debug("Added to cache: " + cacheKey);
				}
			}
			return xmlOut;
		}
	}
}