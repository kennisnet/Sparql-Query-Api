using System;
using System.Security.Principal;
using System.Threading;
using Trezorix.Sparql.Api.Core.Configuration;
using Trezorix.Sparql.Api.Core.Repositories;

namespace Trezorix.Sparql.Api.Core.Accounts {

  public static class OperatingAccount {
    public static void SetByAccountId(string accountId) {
      var principal = new AccountPrincipal(accountId);
      Thread.CurrentPrincipal = principal;
    }

    public static Account Current() {
      var anon = Thread.CurrentPrincipal as AnonymousPrincipal;
      if (anon != null) {
        throw new NotImplementedException("AnonymousAccount not implemented");
        //return AnonymousAccount.Instance;
      }

			//var principal = Thread.CurrentPrincipal as AccountPrincipal;
			//if (principal == null)
			//{
			//	return null;
			//}

			//string accountId = ((AccountPrincipal)principal).AccountId;

			var principal = Thread.CurrentPrincipal;
			if (principal == null)
			{
				return null;
			}

			string accountId = principal.Identity.Name;

			
			var accountRepository = new FileAccountRepository(ApiConfiguration.Current.RepositoryRoot + "Account");
	    var acc = accountRepository.Get(accountId);

      if (acc == null) {
        throw new ApplicationException("No account found for account id: " + accountId);
      }

      return acc;
    }

    public static void SetAnonymous() {
      var principal = new AnonymousPrincipal();
      Thread.CurrentPrincipal = principal;
    }

    internal class AccountPrincipal : IPrincipal {
      public AccountPrincipal(string accountId) {
        AccountId = accountId;
        Identity = new GenericIdentity(accountId, "Forms");
      }

      public string AccountId { get; private set; }

      public bool IsInRole(string role) {
        // Role based model is bad!
        throw new NotSupportedException();
      }

      public IIdentity Identity { get; private set; }
    }

    internal class AnonymousPrincipal : IPrincipal {
      public AnonymousPrincipal() {
        Identity = new GenericIdentity("anonymous", "declared");
      }

      public bool IsInRole(string role) {
        throw new NotImplementedException();
      }

      public IIdentity Identity { get; private set; }
    }

    //public static void SetByApiKey(Guid apiKey) {
    //  var repo = ServiceLocator.Resolve<IApiAccountRepository>();

    //  var account = repo.GetByKey(apiKey);

    //  // ToDo: Why not let caller know? Fail fast?
    //  if (account == null)
    //    return; // Not letting caller know about Account existance

    //  SetByAccountId(account.Id);
    //}

    //public static User ResolveOperatingUser() {
    //  var account = Current();

    //  User user = null;

    //  // if user account, grab user there
    //  if (account is UserAccount) {
    //    user = ((UserAccount)account).User;
    //  }
    //  else if (account is ApiAccount) {
    //    // an ApiAccount might be configured to operate on behalf of some user. Attempt to resolve a user through that route.
    //    // ToDo: Move .User to base Account class, allowing nulls?
    //    var onBehalfOf = ((ApiAccount)account).OnBehalfOf;
    //    if (onBehalfOf != null && onBehalfOf.User != null) {
    //      user = onBehalfOf.User;
    //    }
    //  }
    //  return user;
    //}
  }

}
