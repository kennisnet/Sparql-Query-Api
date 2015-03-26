namespace Trezorix.Sparql.Api.Application.Autofac 
{
	using global::Autofac;

	using MongoDB.Bson;
	using MongoDB.Bson.Serialization;

	using Trezorix.Sparql.Api.Core.Accounts;
	using Trezorix.Sparql.Api.Core.EventSourcing;
	using Trezorix.Sparql.Api.Core.Repositories;

	public class MongoDbRegistration 
	{

    public static void Register(ContainerBuilder builder) 
		{
      builder.RegisterType<MongoEventStoreRepository>()
              .As<IEventStoreRepository>()
              .InstancePerRequest();

			builder.RegisterType<MongoAccountRepository>()
							.As<IAccountRepository>()
							.InstancePerRequest();

			builder.RegisterType<MongoQueryRepository>()
							.As<IQueryRepository>()
							.InstancePerRequest();

			builder.RegisterType<MongoQueryLogRepository>()
							.As<IQueryLogRepository>()
							.InstancePerRequest();

      BsonClassMap.RegisterClassMap<Account>();

      BsonClassMap.RegisterClassMap<EventStore>(classMap => { 
        classMap.AutoMap();
        classMap.GetMemberMap(es => es.EventName).SetRepresentation(BsonType.String);
      });
		}
  }
}
