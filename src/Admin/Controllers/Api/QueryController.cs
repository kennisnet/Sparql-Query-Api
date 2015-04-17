namespace Trezorix.Sparql.Api.Admin.Controllers.Api
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
	using System.Net;
  using System.Web.Http;

	using AutoMapper;
	
	using Trezorix.Sparql.Api.Admin.Controllers.Attributes;
	using Trezorix.Sparql.Api.Admin.Models.Queries;
	using Trezorix.Sparql.Api.Application.Accounts;
  using Trezorix.Sparql.Api.Application.Attributes;
  using Trezorix.Sparql.Api.Core.Authorization;
	using Trezorix.Sparql.Api.Core.Configuration;
	using Trezorix.Sparql.Api.Core.Queries;
	using Trezorix.Sparql.Api.Core.Repositories;

  [RoutePrefix("Api/Query")]
  [NLogWebApi]
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
			var model = new GroupedQueryModel { Groups = new List<QueryGroupModel>() };

			foreach (string group in list.Select(q => q.Group).Distinct())
			{
				string safeId = ((!string.IsNullOrEmpty(group)) ? group.Replace("'", "_") : "");
				string thisGroup = group;
				model.Groups.Add(new QueryGroupModel {
					Id = safeId, 
					Label = ((!string.IsNullOrEmpty(group)) ? group : "Algemeen"), 
					Items = Mapper.Map<IEnumerable<QueryModel>>(list.Where(q => q.Group == thisGroup))
				});
			}

			if (model.Groups.Count == 0)
			{
				model.Groups.Add(new QueryGroupModel {
					Label = "Algemeen", 
					Items = new List<QueryModel>()
				});
			}
			return model;
		}

		[HttpGet]
		[Route("{id}")]
		public dynamic Get(string id)
		{
			var query = (id == "new") ? new Query {
				ApiKeys = new List<string> { OperatingAccount.Current(_accountRepository).ApiKey }
			} : this._queryRepository.GetByAlias(id);

			var model = new ExtendedQueryModel();
			model.MapFrom(query, this._accountRepository.All(), this._queryRepository.All().Select(q => q.Group).Distinct());

			model.Link = ApiConfiguration.Current.QueryApiUrl + model.Link.Replace("$$apikey", OperatingAccount.Current(this._accountRepository).ApiKey.ToString());
			
			return model;
		}

		[HttpPost]
		[Route("{id}")]
		public dynamic Post(ExtendedQueryModel model)
		{
			if (string.IsNullOrEmpty(model.Alias))
			{
				return BadRequest("Query moet een naam hebben.");
			}

		  var account = OperatingAccount.Current(_accountRepository);
      if (!account.IsEditor) {
        return this.Unauthorized();
      }

			var query = new Query();
			model.MapTo(query);

      query.Authorization = new List<AuthorizationSettings> {
        new AuthorizationSettings { AccountId = account.Id, Operation = AuthorizationOperations.Edit }
      };

      this._queryRepository.Save(query);

			this.ClearCacheInQueryApi(query);

			return Ok();
		}

		[HttpPut]
		[Route("{id}")]
		public dynamic Put(string id, ExtendedQueryModel model)
		{
			var query = this._queryRepository.GetById(id);
		
			if (query == null)
			{
				return NotFound();
			}

		  if (!this.CanEdit(query)) {
		    return this.Unauthorized();
		  }

		  if (model.Id != id)
			{
				this._queryRepository.Delete(query);
				this.ClearCacheInQueryApi(query);
			}

			model.MapTo(query);

			this._queryRepository.Save(query);

			this.ClearCacheInQueryApi(query);
		
			return Ok(); 
		}

    [HttpDelete]
    [Route("{id}")]
    public dynamic Delete(string id) {
      var query = this._queryRepository.GetByAlias(id);

      if (!this.CanEdit(query)) {
        return this.Unauthorized();
      }

      this._queryRepository.Delete(query);

      return Ok();
    }

    [HttpPost]
    [Route("{id}/Note")]
    public dynamic PostNote(string id, NoteModel noteModel)
    {
      var query = this._queryRepository.GetById(id);
      if (query == null)
      {
        return NotFound();
      }

      var note = Mapper.Map<Note>(noteModel);
      query.Notes.Insert(0, note);

      this._queryRepository.Save(query);

      return Ok(note);
    }

    [HttpPut]
    [Route("{id}/Note")]
    public dynamic PutNote(string id, NoteModel noteModel)
    {
      var query = this._queryRepository.GetById(id);
      if (query == null)
      {
        return NotFound();
      }

      var note =
        query.Notes.FirstOrDefault(n => n.CreationDate == noteModel.CreationDate && n.AccountId == noteModel.AccountId);

      if (note == null)
      {
        return NotFound();
      }

      Mapper.Map(noteModel, note);

      this._queryRepository.Save(query);

      return Ok();
    }

    [HttpDelete]
    [Route("{id}/Note")]
    public dynamic DeleteNote(string id, string accountId, DateTime creationDate) {
      var query = this._queryRepository.GetById(id);
      if (query == null) {
        return NotFound();
      }

      var note =
        query.Notes.FirstOrDefault(n => n.CreationDate == creationDate.ToUniversalTime() && n.AccountId == accountId);

      if (note == null) {
        return NotFound();
      }

      query.Notes.Remove(note);

      this._queryRepository.Save(query);

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

			var model = new ExtendedQueryModel();
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

			var model = new ExtendedQueryModel();
			model.MapFrom(query, this._accountRepository.All(), this._queryRepository.All().Select(q => q.Group).Distinct());

			model.Link = ApiConfiguration.Current.QueryApiUrl + model.Link.Replace("$$apikey", OperatingAccount.Current(this._accountRepository).ApiKey.ToString());

			var webclient = new WebClient();
			var response = webclient.DownloadString(model.Link + "&debug=true&showQuery=true&format=json");

			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response);
			return data;
		}

    private bool CanEdit(Query query) {
      var account = OperatingAccount.Current(this._accountRepository);
      if (
        (query.Authorization.Any(a => a.AccountId == account.Id && a.Operation == AuthorizationOperations.Edit)
          || account.IsAdministrator)) {
        return true;
      }
      return false;
    }

    private void ClearCacheInQueryApi(Query query)
		{
			var webclient = new WebClient();
			webclient.OpenRead(ApiConfiguration.Current.QueryApiUrl + "/Home/ClearCache?queryName=" + query.Alias);
		}
	
	}
}