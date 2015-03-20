namespace Migrator.RavenDB
{
	using System;

	public class QueryLogItem
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string AccountId { get; set; }
		public string RemoteIp { get; set; }
		public string Referrer { get; set; }
		public bool CacheHit { get; set; }
		public DateTime DateTime { get; set; }
		public long ExecutionTime { get; set; }
	}
}