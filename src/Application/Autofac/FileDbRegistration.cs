namespace Trezorix.Sparql.Api.Application.Autofac 
{
	using global::Autofac;

	using Trezorix.Sparql.Api.Application.FileRepositories;
	using Trezorix.Sparql.Api.Application.MongoRepositories;
	using Trezorix.Sparql.Api.Core.Configuration;
	using Trezorix.Sparql.Api.Core.Repositories;

	public class FileDbRegistration 
	{

    public static void Register(ContainerBuilder builder) 
		{
			builder.Register(x => new FileAccountRepository(ApiConfiguration.Current.RepositoryRoot + "Account"))
							.As<IAccountRepository>()
							.InstancePerRequest();

			builder.Register(x => new FileQueryRepository(ApiConfiguration.Current.RepositoryRoot + "Query"))
							.As<IQueryRepository>()
							.InstancePerRequest();

			builder.Register(x => new MongoQueryLogRepository())
							.As<IQueryLogRepository>()
							.InstancePerRequest();
		}
  }
}
