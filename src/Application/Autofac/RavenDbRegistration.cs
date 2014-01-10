using System.Diagnostics;
using System.Reflection;
using System.Web.Compilation;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Trezorix.Sparql.Api.Core.Configuration;

namespace Trezorix.Sparql.Api.Application.Autofac {

  public class RavenDbRegistration {
    public delegate void RegisterRavenDbIndexes(DocumentStore store);

    public static event RegisterRavenDbIndexes RegisterIndexes;

    public static void Register(ContainerBuilder builder) {
      builder.Register(x => {
        var store = new DocumentStore {
					Url = "http://localhost:8090",
					DefaultDatabase = ApiConfiguration.Current.Database,
          Conventions = {
            FindTypeTagName = type => type.Name
          }
        };
        store.Initialize();

        ProfileRavenDb(store);

        IndexCreation.CreateIndexes(typeof (RavenDbRegistration).Assembly, store);
				IndexCreation.CreateIndexes(BuildManager.GetGlobalAsaxType().BaseType.Assembly, store);
				if (RegisterIndexes != null)
				{
          RegisterIndexes(store);
        }

        //FacetSetupCreation.CreateFacetSetups(typeof (RavenDbRegistration).Assembly, store);

        return store;
      }).As<IDocumentStore>().SingleInstance();

      builder.Register(x => x.Resolve<IDocumentStore>().OpenSession())
             .As<IDocumentSession>()
             .InstancePerHttpRequest()
             .InstancePerApiRequest()
             .OnRelease(x => x.Dispose());
    }

    [Conditional("DEBUG")]
    private static void ProfileRavenDb(DocumentStore store) {
      //RavenProfiler.InitializeFor(store);
    }
  }

}
