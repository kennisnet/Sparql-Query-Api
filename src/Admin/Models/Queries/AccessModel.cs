namespace Trezorix.Sparql.Api.Admin.Models.Queries
{
  using Trezorix.Sparql.Api.Admin.Models.Accounts;

  public class AccessModel
	{    
		public string Name { get; set; }		
    public AccountModel Account { get; set; }
		public bool CanReadSelected { get; set; }
    public bool CanEditSelected { get; set; }    
	}
}