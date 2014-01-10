using System;
using System.Web.Security;

namespace Trezorix.Sparql.Api.Admin.Services {

  public interface IFormsAuthenticationService {
    void SignIn(string userName, bool createPersistentCookie);

    void SignOut();
  }

  public class FormsAuthenticationService : IFormsAuthenticationService {
    public void SignIn(string userName, bool createPersistentCookie) {
      if (string.IsNullOrEmpty(userName)) {
        throw new ArgumentException("Field can not be empty.", "userName");
      }

      FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
    }

    public void SignOut() {
      FormsAuthentication.SignOut();
    }
  }

}
