namespace Trezorix.Sparql.Api.Core.Repositories {
  using System;
  using System.Collections.Generic;
  using System.Diagnostics.CodeAnalysis;
  using System.Linq;
  using System.Linq.Expressions;
  using System.Text.RegularExpressions;

  using MongoDB.Bson;
  using MongoDB.Driver;
  using MongoDB.Driver.Builders;

  using MongoRepository;

  using Trezorix.Sparql.Api.Core.Accounts;

  [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", 
    Justification = "Reviewed. Suppression is OK here.")]
  public class GroupRepository : MongoRepository<Account>, IAccountRepository {
    public Account Get(string id) {
      throw new NotImplementedException();
    }

    public Account GetByUserName(string userName) {
      throw new NotImplementedException();
    }

    public void Save(string name, Account account) {
      throw new NotImplementedException();
    }

    public IEnumerable<Account> All() {
      throw new NotImplementedException();
    }
  }
}