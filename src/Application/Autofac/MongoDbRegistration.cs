namespace Trezorix.Sparql.Api.Application.Autofac 
{
	using global::Autofac;

	using MongoDB.Bson;
	using MongoDB.Bson.Serialization;
	using MongoDB.Bson.Serialization.IdGenerators;

	using Trezorix.Sparql.Api.Application.Accounts;
	using Trezorix.Sparql.Api.Application.MongoRepositories;
	using Trezorix.Sparql.Api.Core;
	using Trezorix.Sparql.Api.Core.Accounts;
	using Trezorix.Sparql.Api.Core.Authorization;
	using Trezorix.Sparql.Api.Core.EventSourcing;
	using Trezorix.Sparql.Api.Core.Queries;
	using Trezorix.Sparql.Api.Core.Repositories;

	public class MongoDbRegistration 
	{

    public static void Register(ContainerBuilder builder) 
		{
      builder.RegisterType<MongoEventStoreRepository>()
              .As<IEventStoreRepository>()
              .InstancePerRequest();

      builder.Register(c => new AccountIdResolver(new MongoAccountRepository()))
              .As<IAccountIdResolver>()
              .InstancePerRequest();

      builder.RegisterType<MongoAccountRepository>()
              //.As<IAccountRepository>()
              .Named<IAccountRepository>("account")
              .InstancePerRequest();

      builder.RegisterDecorator<IAccountRepository>(
          (c, inner) => new MongoAccountRepositoryWithEventStore(inner,
            new MongoEventStoreRepository(c.Resolve<IAccountIdResolver>())),
          fromKey: "account")
          .InstancePerRequest();
      
			builder.RegisterType<MongoQueryRepository>()
							//.As<IQueryRepository>()
              .Named<IQueryRepository>("query")
              .InstancePerRequest();

      builder.RegisterDecorator<IQueryRepository>(
          (c, inner) => new MongoQueryRepositoryWithEventStore(inner, 
            new MongoEventStoreRepository(c.Resolve<IAccountIdResolver>())),
          fromKey: "query")
          .InstancePerRequest();

			builder.RegisterType<MongoQueryLogRepository>()
							.As<IQueryLogRepository>()
							.InstancePerRequest();

      BsonClassMap.RegisterClassMap<Account>();

      BsonClassMap.RegisterClassMap<EventStore>(classMap => { 
        classMap.AutoMap();
        classMap.GetMemberMap(es => es.EventName).SetRepresentation(BsonType.String);
      });

      BsonClassMap.RegisterClassMap<AuthorizationSettings>(classMap => { 
        classMap.AutoMap();
        classMap.GetMemberMap(aus => aus.Operation).SetRepresentation(BsonType.String);
        classMap.GetMemberMap(aus => aus.AccountId).SetRepresentation(BsonType.ObjectId);
      });

      BsonClassMap.RegisterClassMap<Note>(classMap =>
      {
        classMap.AutoMap();
        classMap.GetMemberMap(n => n.AccountId).SetRepresentation(BsonType.ObjectId);
      });
    }
  }
}
