namespace Trezorix.Sparql.Api.Admin
{
  using System.Web.Optimization;

  public class BundleConfig
	{
		// For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
		public static void RegisterBundles(BundleCollection bundles) {
      var angular =
        new ScriptBundle("~/bundles/angular", "http://fouteurl.com/ajax/libs/angularjs/1.3.4/angular.min.js").Include(
          "~/Lib/angular/angular.js");
      angular.CdnFallbackExpression = "window.angular";

      bundles.Add(angular);
      bundles.Add(
        new Bundle("~/bundles/angular-components").Include(
          "~/Lib/angular/angular-route.js",
          "~/Lib/angular/angular-resource.js"
      ));
      
      bundles.Add(
		    new Bundle("~/bundles/project").Include(
          "~/Lib/Shortcut.js",
          "~/Lib/ui-codemirror-0.1.1/ui-codemirror.js",
          "~/NgApp/App.js",
          "~/NgApp/Loading.js",
          "~/NgApp/RouteConfig.js",
          "~/NgApp/Config.js",
          "~/NgApp/Modules/Query.js",
          "~/NgApp/Modules/Settings.js",
          "~/NgApp/Modules/User.js",
          "~/NgApp/Modules/Account.js",
          "~/NgApp/Modules/Log.js",
          "~/NgApp/Controllers/NavController.js"
          )
      );
		}

	}
}