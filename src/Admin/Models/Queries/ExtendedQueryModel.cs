﻿using System.Collections.Generic;
using System.Linq;
using Trezorix.Sparql.Api.Core.Configuration;
using Trezorix.Sparql.Api.Core.Queries;

namespace Trezorix.Sparql.Api.Admin.Models.Queries
{
  using AutoMapper;

  using Trezorix.Sparql.Api.Admin.Models.Accounts;
  using Trezorix.Sparql.Api.Core.Authorization;

  public class ExtendedQueryModel
	{
		public readonly bool EnableAllowAnonymous = ApiConfiguration.Current.AllowAnonymous;

		public string Id { get; set; }
		public string Alias { get; set; }
		public string Label { get; set; }
		public string SparqlQuery { get; set; }
		public string Description { get; set; }
		public string Group { get; set; }
		public string Endpoint { get; set; }
		public string Link { get; set; }
		public IEnumerable<NoteModel> Notes { get; set; }
		public bool AllowAnonymous { get; set; }
		public IEnumerable<string> Groups { get; set; }
		public IEnumerable<AccessModel> Access { get; set; }
		public IEnumerable<QueryParameterModel> Parameters { get; set; }
		public IEnumerable<string> Endpoints { get; set; }

		public void MapFrom(Query query, IEnumerable<Core.Accounts.Account> accounts, IEnumerable<string> groups)
		{
			if (query.Parameters == null)
			{
				query.Parameters = new List<QueryParameter>();
			}

			Id = query.Id;
			Alias = query.Alias;
			Label = (string.IsNullOrEmpty(query.Label)) ? query.Alias : query.Label;
			Description = query.Description;
      Notes = Mapper.Map<IEnumerable<NoteModel>>(query.Notes);
			Group = query.Group;
			Groups = groups;
			SparqlQuery = query.SparqlQuery;
			Link = "/" + query.Alias + "?api_key=$$apikey" + query.Parameters.Aggregate("", (current, queryParameter) => current + ("&" + queryParameter.Name + "=" + (queryParameter.SampleValue ?? "")));
			Parameters =
				query.Parameters.Select(
					p => new QueryParameterModel()
						{
							Name = p.Name, 
							Description = p.Description, 
							SampleValue = p.SampleValue,
							ValuesQuery = p.ValuesQuery
						});
			Access =
				accounts.Select(
					a => new AccessModel
						{              
              Account = Mapper.Map<AccountModel>(a),
							Name = a.FullName, 
							CanReadSelected = (query.ApiKeys != null && query.ApiKeys.Contains(a.ApiKey)),
              CanEditSelected = (query.Authorization != null && query.Authorization.Any(aus => aus.AccountId == a.Id && aus.Operation == AuthorizationOperations.Edit))
						});
			Endpoints = ApiConfiguration.Current.SparqlEndpoints
				.Select(e => e.Name);
			Endpoint = query.Endpoint;
			AllowAnonymous = query.AllowAnonymous;
		}

		public void MapTo(Query query)
		{
			query.Id = Id;
			query.Alias = Alias;
			query.Label = Label;
			query.Description = Description;
      query.Notes = Mapper.Map<IList<Note>>(Notes);
      query.Group = Group;
			query.SparqlQuery = SparqlQuery;
			query.Parameters = (Parameters != null)
					                  ? Parameters.Select(p => new QueryParameter()
						                  {
							                  Name = p.Name,
							                  Description = p.Description,
							                  SampleValue = p.SampleValue,
																ValuesQuery = p.ValuesQuery
						                  }).ToList()
					                  : new List<QueryParameter>();
			query.ApiKeys = Access.Where(a => a.CanReadSelected).Select(a => a.Account.ApiKey).ToList();      

		  query.Authorization = Access.Where(a => a.CanEditSelected).Select(a => new AuthorizationSettings() { 
        AccountId = a.Account.Id,
        Operation = a.Account.IsEditor ? AuthorizationOperations.Edit : AuthorizationOperations.Read
      }).ToList();

			query.Endpoint = Endpoint;
			query.AllowAnonymous = AllowAnonymous;
		}

	}
}