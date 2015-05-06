namespace Trezorix.Sparql.Api.Admin.Controllers.Api {
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web.Http;

  using AutoMapper;

  using Trezorix.Sparql.Api.Admin.Controllers.Attributes;
  using Trezorix.Sparql.Api.Admin.Models.Accounts;
  using Trezorix.Sparql.Api.Application.Accounts;
  using Trezorix.Sparql.Api.Application.Attributes;
  using Trezorix.Sparql.Api.Core;
  using Trezorix.Sparql.Api.Core.Accounts;
  using Trezorix.Sparql.Api.Core.Repositories;

  [RoutePrefix("Api/Account")]
  [NLogWebApi]
  [AuthenticateUser]
  [Authorize]
  public class AccountController : ApiController
  {
		private readonly IQueryRepository _queryRepository;
		private readonly IAccountRepository _accountRepository;

		public AccountController(IQueryRepository queryRepository, IAccountRepository accountRepository)
		{
			this._queryRepository = queryRepository;
			this._accountRepository = accountRepository;
		}
    
    [HttpGet]
    public dynamic Get() {
			return Mapper.Map<IEnumerable<AccountModel>>(_accountRepository.All());
    }

    [HttpGet]
    [Route("{id}")]
    public dynamic Get(string id)
    {
      var account = (id == "new") ? new Account() : (id.IsObjectId()) ? _accountRepository.GetById(id) : _accountRepository.Get(id);

      var model = new ExtendedAccountModel();
      model.MapFrom(account, _queryRepository.All());

      return model;
    }

    [HttpPost]
    [Route("{id}")]
    public dynamic Post(string id, ExtendedAccountModel model)
    {
      if (string.IsNullOrEmpty(model.Id) || model.Id == Guid.Empty.ToString()) {
        model.ApiKey = Guid.NewGuid().ToString();
        model.Id = model.ApiKey;
      }

      var account = new Account();
      model.MapTo(account, _queryRepository);
      _accountRepository.Save(account);

      return Ok();
    }
        
    [HttpPut]
    [Route("{id}")]
    public dynamic Put(string id, ExtendedAccountModel model)
    {
      var account = (id.IsObjectId()) ? _accountRepository.GetById(id) : _accountRepository.Get(id);

      if (account == null) {
        return NotFound();
      }

      model.MapTo(account, _queryRepository);
			
      _accountRepository.Save(account);

      return Ok();
    }

		[HttpPatch]
		[Route("{id}")]
		public dynamic Patch(string id, [FromBody] string password)
		{
      var operatingAccount = OperatingAccount.Current(_accountRepository);
		  bool hasRights = operatingAccount.Id == id || operatingAccount.Roles.Any(r => r == "administrator");

      if (!hasRights) {
        return this.Unauthorized();
      }

			var account = (id.IsObjectId()) ? _accountRepository.GetById(id) : _accountRepository.Get(id);

			if (account == null)
			{
				return NotFound();
			}

			account.Password = account.ComputeSaltedHash(password);
      if (account.Roles == null) {
        account.Roles = new List<string>();
      }

			if (account.Roles.All(r => r != "editor")) {
				account.Roles = account.Roles.Concat(new List<string> { "editor" });
			}

			_accountRepository.Save(account);

			return Ok();
		}

    [HttpDelete]
    [Route("{id}")]
    public dynamic Delete(string id)
    {
      var account = (id.IsObjectId()) ? _accountRepository.GetById(id) : _accountRepository.Get(id);

      if (account == null) {
        return NotFound();
      }

      _accountRepository.Delete(account);

      return Ok();
    }

  }
}