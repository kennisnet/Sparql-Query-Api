namespace Trezorix.Sparql.Api.Application.Accounts {
  using Trezorix.Sparql.Api.Core;
  using Trezorix.Sparql.Api.Core.Repositories;

  public class AccountIdResolver : IAccountIdResolver {
    private readonly IAccountRepository accountRepository;

    public AccountIdResolver(IAccountRepository accountRepository) {
      this.accountRepository = accountRepository;
    }

    public string GetAccountId() {
      return OperatingAccount.Current(this.accountRepository).Id;
    }
  }
}
