using System.Collections.Generic;
using Trezorix.Sparql.Api.Core.Queries;
using Trezorix.Sparql.Api.Core.Accounts;

namespace Trezorix.Sparql.Api.Core.Repositories
{
	public interface IAccountRepository
	{
		Account Get(string id);
		Account GetById(string id);
		Account GetByUserName(string userName);
		Account Add(Account account);
		Account Update(Account account);
		void Delete(Account account);
		IEnumerable<Account> All();
	}
}