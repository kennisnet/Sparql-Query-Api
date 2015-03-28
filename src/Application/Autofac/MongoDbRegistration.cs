namespace Trezorix.Sparql.Api.Application.Autofac 
{
	using global::Autofac;

	using MongoDB.Bson;
	using MongoDB.Bson.Serialization;

	using Trezorix.Sparql.Api.Application.MongoRepositories;
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
              //.As<IAccountRepository>()
              .Named<IAccountRepository>("account")
              .InstancePerRequest();

      builder.RegisterDecorator<IAccountRepository>(
          (c, inner) => new MongoAccountRepositoryWithEventStore(inner, new MongoEventStoreRepository()),
          fromKey: "account");
      
			builder.RegisterType<MongoQueryRepository>()
							//.As<IQueryRepository>()
              .Named<IQueryRepository>("query")
              .InstancePerRequest();

      builder.RegisterDecorator<IQueryRepository>(
          (c, inner) => new MongoQueryRepositoryWithEventStore(inner, new MongoEventStoreRepository()),
          fromKey: "query");
      

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
