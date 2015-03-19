namespace Migrator {
	using System.Linq;

	using Migrator.RavenDB;

	using Raven.Client;
	using Raven.Client.Document;

	using Trezorix.Sparql.Api.Core.Repositories;

	public class ImportQueryLogTask {
		private readonly string ravenDbUrl;

		private readonly string database;

		private IDocumentStore documentStore;

		public ImportQueryLogTask(string ravenDbUrl, string database) {
			this.ravenDbUrl = ravenDbUrl;
			this.database = database;
		}

		private IDocumentStore DocumentStore {
			get {
				if (documentStore == null) {
					var store = new DocumentStore {
						Url = ravenDbUrl,
						DefaultDatabase = database,
						Conventions = {
							FindTypeTagName = type => type.Name
						}
					};
					store.Initialize();
					documentStore = store;
				}
				return documentStore;
			}
		}
		public void Execute() {
			var queryLogRepository = new MongoQueryLogRepository();
			queryLogRepository.DeleteAll();

			using (var session = DocumentStore.OpenSession()) {
				session.Advanced.MaxNumberOfRequestsPerSession = 10000;
				var query = session.Query<Trezorix.Sparql.Api.Core.Queries.QueryLogItem>().AsProjection<QueryLogItem>();
				int totalItemCount = query.Count();
				for (int i = 0; i < totalItemCount; i += 1024) {
					var items = query.Skip(i).Take(1024);
					foreach (var queryLogItem in items) {
						queryLogItem.Id = null;
						var newItem = new Trezorix.Sparql.Api.Core.Queries.QueryLogItem {
							Name = queryLogItem.Name,
							AccountId = queryLogItem.AccountId,
							CacheHit = queryLogItem.CacheHit,
							DateTime = queryLogItem.DateTime,
							ExecutionTime = queryLogItem.ExecutionTime,
							Referrer = queryLogItem.Referrer,
							RemoteIp = queryLogItem.RemoteIp,
						};
						queryLogRepository.Add(newItem);
					}
				}
			}
		}
	}
}
