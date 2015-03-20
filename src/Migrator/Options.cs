namespace Migrator 
{
	using CommandLine;
	using CommandLine.Text;

	internal class Options 
	{

		[Option('a', "AccountPath", Required = true, HelpText = @"Path to file account folder, D:\Projecten\kennisnet-sparql-query-api\src\Data\API\Account")]
		public string AccountPath { get; set; }

		[Option('q', "QueryPath", Required = true, HelpText = @"Path to file query folder, D:\Projecten\kennisnet-sparql-query-api\src\Data\API\Query")]
		public string QueryPath { get; set; }


		[HelpOption]
		public string GetUsage()
		{
			return HelpText.AutoBuild(this,
				(HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
		}
	}
}
