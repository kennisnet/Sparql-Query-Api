using System;
using CommandLine;

namespace Migrator
{
	using Trezorix.Sparql.Api.Core.Repositories;

  class Program 
	{

		static void Main(string[] args) 
		{			
			ParseCommandLineArgumentsAndImportFileRepositories(args);
		}


		private static void ParseCommandLineArgumentsAndImportFileRepositories(string[] args)
		{
			var options = new Options();

			if (Parser.Default.ParseArguments(args, options))
			{
				if (args.Length == 0 || string.IsNullOrEmpty(options.AccountPath) || string.IsNullOrEmpty(options.QueryPath))
				{
					Console.WriteLine(options.GetUsage());
					Console.ReadLine();
				}
			}

			if (!string.IsNullOrEmpty(options.AccountPath)) 
			{
				ImportFileAccountRepository(options.AccountPath);
			}

			if (!string.IsNullOrEmpty(options.QueryPath)) 
			{
				ImportFileQueryRepository(options.QueryPath);
			}

			ImportQueryLogRepository();
		}

		private static void ImportFileAccountRepository(string path)
		{
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

		private static void ImportFileQueryRepository(string path) 
		{
			var fileQueryRepository = new FileQueryRepository(path);

			var mongoQueryRepository = new MongoQueryRepository();
			mongoQueryRepository.DeleteAll();

			foreach (var query in fileQueryRepository.All()) 
			{
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

		private static void ImportQueryLogRepository() 
		{
			var importQueryLogTask = new ImportQueryLogTask("RavenDB"); 
			
			importQueryLogTask.Execute();
			
			Console.WriteLine("Query Log repository was imported");
		}
	}
}
