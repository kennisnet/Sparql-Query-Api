namespace Trezorix.Sparql.Api.Admin.Models.Accounts
{
	using System.Collections.Generic;

	public class AccountModel 
	{
		public string Id { get; set; }
		
		public string FullName { get; set; }

		public string UserName { get; set; }

		public string Email { get; set; }

		public string PhoneNumber { get; set; }	

		public IEnumerable<string> Roles { get; set; }

		public string ApiKey { get; set; }

    public bool IsAdministrator { get; set; }
    
    public bool IsEditor { get; set; }

	}
}
