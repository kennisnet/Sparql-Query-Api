namespace Trezorix.Sparql.Api.Application.Autofac 
{
	using global::Autofac;

	using Trezorix.Sparql.Api.Core.Repositories;

	public class MongoDbRegistration 
	{

    public static void Register(ContainerBuilder builder) 
		{
			builder.Register(x => new MongoAccountRepository())
							.As<IAccountRepository>()
							.InstancePerRequest();

			builder.Register(x => new MongoQueryRepository())
							.As<IQueryRepository>()
							.InstancePerRequest();

			builder.Register(x => new MongoQueryLogRepository())
							.As<IQueryLogRepository>()
							.InstancePerRequest();
		}
  }
}
