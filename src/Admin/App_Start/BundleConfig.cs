namespace Trezorix.Sparql.Api.Admin
{
  using System.Web.Optimization;

  public class BundleConfig
	{
		// For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
		public static void RegisterBundles(BundleCollection bundles) {
      bundles.Add(
        new StyleBundle("~/bundles/bootstrap-site").Include(
          "~/Scripts/bower_components/bootstrap/dist/css/bootstrap.css",
          "~/Content/css/bootstrap.css",
          "~/Content/css/font-awesome.css",
          "~/Content/css/loading-bar.css",
          "~/Content/css/bda-admin.css",
          "~/Lib/bg-splitter/css/style.css",
          "~/Content/css/site.css"));

      bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
        "~/Content/js/jquery.js"));

      bundles.Add(new ScriptBundle("~/bundles/bootstrap-js").Include(
        "~/Content/js/bootstrap.js"));
      
      bundles.Add(
        new Bundle("~/bundles/angular").Include(
          "~/Lib/angular/angular.js",
          "~/Lib/angular/angular-route.js",
          "~/Lib/angular/angular-resource.js"
      ));
      
      bundles.Add(
		    new Bundle("~/bundles/project").Include(
          "~/Lib/Shortcut.js",
          "~/Lib/ui-codemirror-0.1.1/ui-codemirror.js",
          "~/Lib/ui-bootstrap/ui-bootstrap-tpls-0.11.0.js",
          "~/Lib/bg-splitter/js/splitter.js",
          "~/NgApp/App.js",
          "~/NgApp/Loading.js",
          "~/NgApp/RouteConfig.js",
          "~/NgApp/Config.js",
          "~/NgApp/Modules/NoCache.js",
					"~/NgApp/Modules/Navigator.js",
					"~/NgApp/Modules/Query.js",
          "~/NgApp/Modules/Settings.js",
          "~/NgApp/Modules/User.js",
          "~/NgApp/Modules/Account.js",
          "~/NgApp/Modules/Log.js",
          "~/NgApp/Modules/Test.js",
          "~/NgApp/Controllers/NavController.js",
          "~/NgApp/Controllers/UserController.js"
          )
      );
		}

	}
}