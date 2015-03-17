using System;
using CommandLine;

namespace Migrator {
	using Trezorix.Sparql.Api.Core.Repositories;

  class Program {

    static void Main(string[] args) {			
	    ParseCommandLineArgumentsAndImportFileRepositories(args);
    }


		private static void ParseCommandLineArgumentsAndImportFileRepositories(string[] args)
		{			
			var options = new Options();

			if (Parser.Default.ParseArguments(args, options))
			{				
				if (args.Length == 0)
				{
					Console.WriteLine(options.GetUsage());
				}								
			}

			if (!string.IsNullOrEmpty(options.AccountPath)) {
				ImportFileAccountRepository(options.AccountPath);
			}

			if (!string.IsNullOrEmpty(options.QueryPath)) {
				ImportFileQueryRepository(options.QueryPath);
			}
		}

		private static void ImportFileAccountRepository(string path){
			var fileAccountRepository = new FileAccountRepository(path);

			var mongoAccountRepository = new MongoAccountRepository();
			mongoAccountRepository.DeleteAll();

			foreach (var account in fileAccountRepository.All())
			{
				account.Id = null;
				mongoAccountRepository.Add(account);
			}

			Console.WriteLine("Account repository was imported");
		}

		private static void ImportFileQueryRepository(string path) {
			var fileQueryRepository = new FileQueryRepository(path);

			var mongoQueryRepository = new MongoQueryRepository();
			mongoQueryRepository.DeleteAll();

			foreach (var query in fileQueryRepository.All()) {
				query.Alias = query.Id;
				query.Id = null;
				mongoQueryRepository.Add(query);
			}

			Console.WriteLine("Query repository was imported");

			foreach (var query in mongoQueryRepository.All())
			{
				Console.WriteLine(query.Id + " - " + query.Label);
			}

		}
  }
}
