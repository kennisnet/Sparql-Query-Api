app.config(['$routeProvider', '$sceDelegateProvider', 'config', function ($routeProvider, $sceDelegateProvider, config) {
    $routeProvider
      .when('/home', { templateUrl: config.viewsUrl + 'Home/Overview' })
      .when('/query/:id', { templateUrl: config.viewsUrl + 'Query/Item', controller: 'QueryController' })
      .when('/query', { templateUrl: config.viewsUrl + 'Query/Index', controller: 'QueryIndexController' })
      .when('/account/:id', { templateUrl: config.viewsUrl + 'Account/Item', controller: 'AccountController' })
      .when('/account', { templateUrl: config.viewsUrl + 'Account/Index', controller: 'AccountIndexController' })
      .when('/log', { templateUrl: config.viewsUrl + 'Log/Index', controller: 'LogController' })
      .when('/settings', { templateUrl: config.viewsUrl + 'Settings/Index', controller: 'SettingsController' })
      .when('/test', { templateUrl: config.viewsUrl + 'Test/Index', controller: 'TestIndexController' })
      .otherwise({ redirectTo: '/home' });

    $sceDelegateProvider.resourceUrlWhitelist(['self', '**']);
  }
]);