namespace Trezorix.Sparql.Api.Admin.Models.Accounts
{
	public class QueryAccessModel
	{
		public string Alias { get; set; }
		public string Label { get; set; }
		public bool View { get; set; }
		public bool Edit { get; set; }
	}
}