﻿namespace Trezorix.Sparql.Api.Admin.Controllers.Api {
  using System;
  using System.Net;
  using System.Web.Http;

  using Trezorix.Sparql.Api.Admin.Controllers.Attributes;
  using Trezorix.Sparql.Api.Admin.Models.Accounts;
  using Trezorix.Sparql.Api.Core.Accounts;
  using Trezorix.Sparql.Api.Core.Configuration;
  using Trezorix.Sparql.Api.Core.Repositories;

  [RoutePrefix("Api/Account")]
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
    public dynamic Get() 
    {
      return Ok();
    }

    [HttpGet]
    [Route("{id}")]
    public dynamic Get(string id)
    {
      var account = (id == "new") ? new Account() : _accountRepository.Get(id);

      var model = new AccountModel();
      model.MapFrom(account, _queryRepository.All());

      return model;
    }

    [HttpPost]
    [Route("{id}")]
    public dynamic Post(string id, AccountModel model)
    {
      if (string.IsNullOrEmpty(model.Id) || model.Id == Guid.Empty.ToString()) {
        model.ApiKey = Guid.NewGuid().ToString();
        model.Id = model.ApiKey;
      }

      var account = new Account();
      model.MapTo(account, _queryRepository);
      _accountRepository.Save(account.Id, account);

      return Ok();
    }
    
    
    [HttpPut]
    [Route("{id}")]
    public dynamic Put(string id, AccountModel model)
    {
      var account = _accountRepository.Get(id);

      if (account == null) {
        return NotFound();
      }

      model.MapTo(account, _queryRepository);

      _accountRepository.Save(account.Id, account);

      return Ok();
    }

    [HttpDelete]
    [Route("{id}")]
    public dynamic Delete(string id)
    {
      var account = _accountRepository.Get(id);

      if (account == null) {
        return NotFound();
      }

      _accountRepository.Delete(account);

      return Ok();
    }

  }
}