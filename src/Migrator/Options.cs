
namespace Migrator {
	using CommandLine;
	using CommandLine.Text;

	internal class Options {

		[Option("AccountPath", HelpText = @"D:\Projecten\kennisnet-sparql-query-api\src\Data\API\Account")]
		public string AccountPath { get; set; }

		[Option("QueryPath", HelpText = @"D:\Projecten\kennisnet-sparql-query-api\src\Data\API\Query")]
		public string QueryPath { get; set; }


		[HelpOption]
		public string GetUsage()
		{
			return HelpText.AutoBuild(this,
				(HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
		}
	}
}
