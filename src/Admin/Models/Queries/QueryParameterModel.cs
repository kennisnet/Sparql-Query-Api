namespace Trezorix.Sparql.Api.Admin.Models.Queries
{
	public class QueryParameterModel
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string SampleValue { get; set; }
		public string ValuesQuery { get; set; }
	}
}