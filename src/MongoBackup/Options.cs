namespace MongoBackup 
{
	using CommandLine;
	using CommandLine.Text;

	internal class Options 
	{

		[Option('o', "DestinationBackupRootFolder", Required = true, HelpText = @"Path to destination backup root folder [D:\MongoBackup]")]
		public string DestinationBackupRootFolder { get; set; }

		[Option('d', "DatabaseName", Required = true, HelpText = @"Database name to backup [QueryApi]")]
		public string DatabaseName { get; set; }

		[Option('c', "CollectionNames", Required = true, HelpText = @"Collection name(s) to backup [Account]")]
		public string CollectionNames { get; set; }

		[Option('r', "RotationDays", Required = true, HelpText = @"How many daily backups (in days) to keep before oldest backups will be deleted [7]")]
		public string RotationDays { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			return HelpText.AutoBuild(this,
				(HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
		}
	}
}
