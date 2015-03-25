var Navigator = angular.module('Navigator', []);

Navigator.controller('QueryNavigator', [
  '$scope', '$location', '$route', '$http', '$filter', 'config', 'userService', function ($scope, $location, $route, $http, $filter, config, userService) {

  	$http.get(config.adminApiUrl + 'Query/').then(function (response) {
  		$scope.groupedQueries = response.data;
  	});

  	$scope.isGroupVisible = function (value) {
  		return userService.queryGroup(value).visible;
  	};

  	$scope.toggleGroup = function (event, value) {
  		var panelId = "" + event.target.id;
  		if (panelId.indexOf('group') == 0) {
  			userService.queryGroup(value).visible = !userService.queryGroup(value).visible;
  		}
  	};

  	function setVisibility() {
  		$scope.isVisible = angular.isDefined($route.current.originalPath) ? $route.current.originalPath.indexOf('/query') == 0 : false;
  	}

  	$scope.$on('$routeChangeSuccess', setVisibility);

  	$scope.isVisible = false;
  }
]);

Navigator.controller('AccountNavigator', [
  '$scope', '$location', '$route', '$http', '$filter', 'config', 'userService', function ($scope, $location, $route, $http, $filter, config, userService) {

  	$http.get(config.adminApiUrl + 'Account/').then(function (response) {
  		$scope.accounts = response.data;
  	});

  	function setVisibility() {
  		$scope.isVisible = angular.isDefined($route.current.originalPath) ? $route.current.originalPath.indexOf('/account') == 0 : false;
  	}

  	$scope.$on('$routeChangeSuccess', setVisibility);

  	$scope.isVisible = false;
  }
]);

Navigator.controller('SettingsNavigator', [
  '$scope', '$location', '$route', function ($scope, $location, $route) {

  	function setVisibility() {
  		$scope.isVisible = angular.isDefined($route.current.originalPath) ? $route.current.originalPath.indexOf('/settings') == 0 : false;
  	}

  	$scope.$on('$routeChangeSuccess', setVisibility);

  	$scope.isVisible = false;
  }
]);

Navigator.controller('LogsNavigator', [
  '$scope', '$location', '$route', function ($scope, $location, $route) {

  	function setVisibility() {
  		$scope.isVisible = angular.isDefined($route.current.originalPath) ? $route.current.originalPath.indexOf('/log') == 0 : false;
  	}

  	$scope.$on('$routeChangeSuccess', setVisibility);

  	$scope.isVisible = false;
  }
]);