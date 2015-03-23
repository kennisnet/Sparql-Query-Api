using System;

namespace MongoBackup
{
	using System.Diagnostics;
	using System.IO;

	using CommandLine;

	using NLog;

	class Program {
		private static Logger Logger = LogManager.GetCurrentClassLogger();

		static void Main(string[] args) {
			Logger.Info("*** Start Mongo Backup");
			ParseCommandLineArgsAndBackup(args);
			Logger.Info("*** Stop Mongo backup");
		}

		private static void ParseCommandLineArgsAndBackup(string[] args)
		{
			var options = new Options();

			Logger.Info("Parsing command line args.");			
			if (Parser.Default.ParseArguments(args, options))
			{
				if (args.Length == 0 || 
					string.IsNullOrEmpty(options.DestinationBackupRootFolder) || 
					string.IsNullOrEmpty(options.DatabaseName) ||
					string.IsNullOrEmpty(options.CollectionName) || 
					string.IsNullOrEmpty(options.RotationDays)) {
					Logger.Info("Cannot parse required command line params.");
					Console.WriteLine(options.GetUsage());
					Console.ReadLine();
				}
				else {
					DeletePreviousBackups(options.DestinationBackupRootFolder, options.RotationDays);
					RunMongoDump(options.DestinationBackupRootFolder, options.DatabaseName, options.CollectionName);
				}
			}
		}


		private static void DeletePreviousBackups(string destinationBackupRootFolder, string rotationDays)
		{
			Logger.Info("Start deleting backups older than {0} days in {1}", rotationDays, destinationBackupRootFolder);
			var rotationDaysCount = int.Parse(rotationDays);

			var dailyBackupFolders = Directory.GetDirectories(destinationBackupRootFolder);
			foreach (var dailyBackupFolder in dailyBackupFolders)
			{
				var directoryInfo = new DirectoryInfo(dailyBackupFolder);
				if (directoryInfo.CreationTime < DateTime.Now.AddDays(-rotationDaysCount)) {
					directoryInfo.Delete(true);
				}
			}
			Logger.Info("Stop deleting old backups.");
		}


		private static void RunMongoDump(string destinationBackupRootFolder, string databaseName, string collectionName) {
			var today = DateTime.Now.ToString("yyyy-MM-dd");
			var backupFolderIncDate = destinationBackupRootFolder + "/MongoBackup#" + today;

			Logger.Info("Start mongodump to output folder {0} of database \"{1}\" collection \"{2}\"", backupFolderIncDate, databaseName, collectionName);
			var mongodumpStartInfo = new ProcessStartInfo() {
				FileName = @"mongodump",
				Arguments = string.Format("-d {0} -c {1} -o \"{2}\"", databaseName, collectionName, backupFolderIncDate),
				UseShellExecute = false,
				WindowStyle = ProcessWindowStyle.Normal
			};

			var mongodump = new Process { StartInfo = mongodumpStartInfo };
			mongodump.Start();
			mongodump.Close();
			Logger.Info("Stop mongodump.");
		}
	}
}
