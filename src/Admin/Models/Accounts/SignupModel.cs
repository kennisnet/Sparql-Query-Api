using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Trezorix.Sparql.Api.Admin.Models.Accounts {

  public class SignupModel {
    [Required(ErrorMessage = "Please specify your username.")]
    [DisplayName("FullName")]
    public string FullName { get; set; }

    [DisplayName("Username")]
		public string Username { get; set; }

    [DisplayName("E-mail")]
    public string Email { get; set; }

    [DataType(DataType.Password)]
    [DisplayName("Password")]
    public string Password { get; set; }

    public string HashedPassword { get; set; } // sha512
  }

}
