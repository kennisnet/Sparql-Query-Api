namespace Trezorix.Sparql.Api.Admin.Models.Accounts
{
	using System;
	using System.Collections.Generic;

	public class AccountModel 
	{
		public string Id { get; set; }
		
		public string FullName { get; set; }

		public string UserName { get; set; }

		public IEnumerable<string> Roles { get; set; }

		public string Password { get; set; }

		public string ApiKey { get; set; }
	}
}
