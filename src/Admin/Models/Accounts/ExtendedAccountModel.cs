using System;
using System.Collections.Generic;
using System.Linq;
using Trezorix.Sparql.Api.Core.Accounts;
using Trezorix.Sparql.Api.Core.Queries;
using Trezorix.Sparql.Api.Core.Repositories;

namespace Trezorix.Sparql.Api.Admin.Models.Accounts
{
	public class ExtendedAccountModel
	{
		public string Id { get; set; }
		public string FullName { get; set; }
	  public string UserName { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }	
		public string ApiKey { get; set; }
		public IEnumerable<QueryAccessModel> QueryAccess { get; set; }

		public void MapFrom(Account account, IEnumerable<Query> queries)
		{
			Id = account.Id;
			FullName = account.FullName;
			UserName = account.UserName;
			Email = account.Email;
			PhoneNumber = account.PhoneNumber;
			ApiKey = account.ApiKey;
			QueryAccess =
				queries.Select(
					q => new QueryAccessModel() { Name = q.Alias, Selected = q.ApiKeys.Contains(account.ApiKey) });
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
					if (queryAccessModel.Selected && query.ApiKeys.All(a => a != account.ApiKey))
					{
						query.ApiKeys.Add(account.ApiKey);
						queryRepository.Save(query);
					}
					else if (!queryAccessModel.Selected && query.ApiKeys.Any(a => a == account.ApiKey))
					{
						query.ApiKeys.Remove(account.ApiKey);
						queryRepository.Save(query);
					}

				}				
			}
		}

	}
}