namespace Trezorix.Sparql.Api.Admin.App_Start 
{
  using System.Web.Http;
	using System.Web.Mvc;

	using Autofac;
	using Autofac.Integration.WebApi;
	using Autofac.Integration.Mvc;
	
	using Trezorix.Sparql.Api.Admin.Services;
	using Trezorix.Sparql.Api.Application.Autofac;

  public class AutofacConfig 
	{
    public static void SetAsDependencyResolver() 
		{
      var builder = new ContainerBuilder();
      builder.RegisterControllers(typeof (AutofacConfig).Assembly);
      builder.RegisterApiControllers(typeof (AutofacConfig).Assembly);
			builder.RegisterType<FormsAuthenticationService>().As<IFormsAuthenticationService>();
			
			MongoDbRegistration.Register(builder);

			//NLogRegistration.Register(builder);
			//WebRegistration.Register(builder);

      IContainer container = builder.Build();
      DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
      GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
    }
  }

}
