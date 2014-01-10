using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Trezorix.Sparql.Api.Application.Autofac;
using Trezorix.Sparql.Api.Core.Configuration;
using Trezorix.Sparql.Api.Core.Repositories;

namespace Trezorix.Sparql.Api.QueryApi.App_Start {

  public class AutofacConfig {
    public static void SetAsDependencyResolver() {
      var builder = new ContainerBuilder();
      builder.RegisterControllers(typeof (AutofacConfig).Assembly);
      builder.RegisterApiControllers(typeof (AutofacConfig).Assembly);

			builder.Register(x => new FileAccountRepository(ApiConfiguration.Current.RepositoryRoot + "Account"))
							.As<IAccountRepository>()
							.InstancePerHttpRequest();
							//.OnRelease(x => x.Dispose()); 

			builder.Register(x => new FileQueryRepository(ApiConfiguration.Current.RepositoryRoot + "Query"))
							.As<IQueryRepository>()
							.InstancePerHttpRequest();

			RavenDbRegistration.Register(builder);
			//NLogRegistration.Register(builder);
			//WebRegistration.Register(builder);

      IContainer container = builder.Build();
      DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
      GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
    }
  }

}
