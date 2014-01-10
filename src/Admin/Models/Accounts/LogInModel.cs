using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Trezorix.Sparql.Api.Admin.Models.Accounts {

  public class LoginModel {
    public string Version;

    [Required(ErrorMessage = "Please specify your username.")]
    [DisplayName("Username")]
    public string Username { get; set; }

    [DataType(DataType.Password)]
    [DisplayName("Password")]
    public string Password { get; set; }

    [DisplayName("Remember me on this computer")]
    public bool RememberMe { get; set; }

    public string HashedPassword { get; set; } // sha512

    public string ReturnUrl { get; set; }
  }

}
