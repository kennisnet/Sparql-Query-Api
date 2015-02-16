using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator {
  using System.IO;

  using Trezorix.Sparql.Api.Core.Configuration;
  using Trezorix.Sparql.Api.Core.Repositories;

  class Program {
    static void Main(string[] args) {
      var fileAccountRepository = new FileAccountRepository(@"D:\Projecten\Sparql Query API\src\Data\API\Account");

      var mongoAccountRepository = new MongoAccountRepository();
      mongoAccountRepository.DeleteAll();

      foreach (var account in fileAccountRepository.All()) {
        account.Id = null;
        mongoAccountRepository.Add(account);
      }
      
    }
  }
}
