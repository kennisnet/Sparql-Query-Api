namespace Trezorix.Sparql.Api.Admin.Controllers.Api
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Web.Http;

	using Trezorix.Sparql.Api.Admin.Controllers.Attributes;
	using Trezorix.Sparql.Api.Admin.Models.Queries;
	using Trezorix.Sparql.Api.Application.Accounts;
	using Trezorix.Sparql.Api.Core.Accounts;
	using Trezorix.Sparql.Api.Core.Configuration;
	using Trezorix.Sparql.Api.Core.Queries;
	using Trezorix.Sparql.Api.Core.Repositories;

	[RoutePrefix("Api/Query")]
	[AuthenticateUser]
	[Authorize]
	public class QueryController : ApiController
	{
		private readonly IQueryRepository _queryRepository;
		private readonly IAccountRepository _accountRepository;

		public QueryController(IQueryRepository queryRepository, IAccountRepository accountRepository)
		{
			this._queryRepository = queryRepository;
			this._accountRepository = accountRepository;
		}

		[HttpGet]
		public dynamic Get() 
		{
			var list = this._queryRepository.All().ToList();
			var model = new GroupedQueryModel
				{
					Groups = new List<QueryGroup>()
				};
			foreach (string group in list.Select(q => q.Group).Distinct())
			{
				string safeId = ((!string.IsNullOrEmpty(group)) ? group.Replace("'", "_") : "");
				string thisGroup = group;
				model.Groups.Add(new QueryGroup { Id = safeId, Label = ((!string.IsNullOrEmpty(group)) ? group : "Algemeen"), Items = list.Where(q => q.Group == thisGroup)});
			}

			if (model.Groups.Count == 0)
			{
				model.Groups.Add(new QueryGroup { Label = "Algemeen", Items = new List<Query>()});
			}
			return model;
		}

		[HttpGet]
		[Route("{id}")]
		public dynamic Get(string id)
		{
			var query = (id == "new") ? new Query {
				ApiKeys = new List<Guid> { OperatingAccount.Current(_accountRepository).ApiKey }
			} : this._queryRepository.GetByAlias(id);

			var model = new QueryModel();
			model.MapFrom(query, this._accountRepository.All(), this._queryRepository.All().Select(q => q.Group).Distinct());

			model.Link = ApiConfiguration.Current.QueryApiUrl + model.Link.Replace("$$apikey", OperatingAccount.Current(this._accountRepository).ApiKey.ToString());
			
			return model;
		}

		[HttpPost]
		[Route("{id}")]
		public dynamic Post(QueryModel model)
		{
			if (string.IsNullOrEmpty(model.Id))
			{
				return BadRequest("Query moet een naam hebben.");
			}
			var query = new Query();
			model.MapTo(query);
			this._queryRepository.Add(query);

			this.ClearCacheInQueryApi(query);

			return Ok();
		}

		[HttpPut]
		[Route("{id}")]
		public dynamic Put(string id, QueryModel model)
		{
			var query = this._queryRepository.GetByAlias(id);
		
			if (query == null)
			{
				return NotFound();
			}

			if (model.Id != id)
			{
				this._queryRepository.Delete(query);
				this.ClearCacheInQueryApi(query);
			}

			model.MapTo(query);

			this._queryRepository.Update(query);

			this.ClearCacheInQueryApi(query);
		
			return Ok(); 
		}

		[HttpDelete]
		[Route("{id}")]
		public dynamic Delete(string id)
		{
			var query = this._queryRepository.GetByAlias(id);

			this._queryRepository.Delete(query);

			return Ok();
		}


		[HttpGet]
		[Route("{id}/Preview")]
		public dynamic Preview(string id) 
		{
			var query = this._queryRepository.GetByAlias(id);

			if (query == null) {
				return NotFound();
			}

			var model = new QueryModel();
			model.MapFrom(query, this._accountRepository.All(), this._queryRepository.All().Select(q => q.Group).Distinct());

			model.Link = ApiConfiguration.Current.QueryApiUrl + model.Link.Replace("$$apikey", OperatingAccount.Current(this._accountRepository).ApiKey.ToString());

			var webclient = new WebClient();
			var response = webclient.DownloadString(model.Link + "&debug=true");

			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response);
			return data;
		}

		[HttpGet]
		[Route("{id}/PreviewQuery")]
		public dynamic PreviewQuery(string id) 
		{
			var query = this._queryRepository.GetByAlias(id);

			if (query == null) {
				return NotFound();
			}

			var model = new QueryModel();
			model.MapFrom(query, this._accountRepository.All(), this._queryRepository.All().Select(q => q.Group).Distinct());

			model.Link = ApiConfiguration.Current.QueryApiUrl + model.Link.Replace("$$apikey", OperatingAccount.Current(this._accountRepository).ApiKey.ToString());

			var webclient = new WebClient();
			var response = webclient.DownloadString(model.Link + "&debug=true&showQuery=true&format=json");

			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response);
			return data;
		}
    
		private void ClearCacheInQueryApi(Query query)
		{
			var webclient = new WebClient();
			webclient.OpenRead(ApiConfiguration.Current.QueryApiUrl + "/Home/ClearCache?queryName=" + query.Alias);
		}
	
	}
}