using System;
using System.Collections.Generic;
using System.Linq;
using Trezorix.Sparql.Api.Core.Accounts;
using Trezorix.Sparql.Api.Core.Queries;
using Trezorix.Sparql.Api.Core.Repositories;

namespace Trezorix.Sparql.Api.Admin.Models.Accounts
{
	using Trezorix.Sparql.Api.Core.Authorization;

	public class ExtendedAccountModel
	{
		public string Id { get; set; }
		public string FullName { get; set; }
	  public string UserName { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }	
		public string ApiKey { get; set; }
    public bool IsAdministrator { get; set; }
    public bool IsEditor { get; set; }		
		public IEnumerable<QueryAccessModel> QueryAccess { get; set; }

		public void MapFrom(Account account, IEnumerable<Query> queries)
		{
			Id = account.Id;
			FullName = account.FullName;
			UserName = account.UserName;
			Email = account.Email;
			PhoneNumber = account.PhoneNumber;
			ApiKey = account.ApiKey;
			IsAdministrator = account.IsAdministrator;
			IsEditor = account.IsEditor;
			QueryAccess =
				queries.Select(
					q => new QueryAccessModel() {
						Name = q.Alias,
						View = q.ApiKeys.Any(k => k.Equals(account.ApiKey)),
						Edit = q.Authorization.Any(a => a.Operation == AuthorizationOperations.Edit && a.AccountId == account.Id)
					});
		}

	  public void MapTo(Account account, IQueryRepository queryRepository)
		{
			account.FullName = FullName;
			account.UserName = UserName;
			account.Email = Email;
			account.PhoneNumber = PhoneNumber;
			account.ApiKey = ApiKey;
			if (QueryAccess != null)
			{
				foreach (var queryAccessModel in QueryAccess)
				{
					var query = queryRepository.GetByAlias(queryAccessModel.Name);
					if (queryAccessModel.View && query.ApiKeys.All(a => a != account.ApiKey))
					{
						query.ApiKeys.Add(account.ApiKey);
						queryRepository.Save(query);
					}
					else if (!queryAccessModel.View && query.ApiKeys.Any(a => a == account.ApiKey))
					{
						query.ApiKeys.Remove(account.ApiKey);
						queryRepository.Save(query);
					}
					if (queryAccessModel.Edit && !query.Authorization.Any(a => a.Operation == AuthorizationOperations.Edit && a.AccountId == account.Id))
					{
						query.Authorization.Add(new AuthorizationSettings { AccountId = account.Id, Operation = AuthorizationOperations.Edit });
						queryRepository.Save(query);
					}
					else if (!queryAccessModel.Edit && query.Authorization.Any(a => a.Operation == AuthorizationOperations.Edit && a.AccountId == account.Id))
					{
						query.Authorization.Remove(query.Authorization.First(a => a.Operation == AuthorizationOperations.Edit && a.AccountId == account.Id));
						queryRepository.Save(query);
					}

				}				
			}
		}

	}
}