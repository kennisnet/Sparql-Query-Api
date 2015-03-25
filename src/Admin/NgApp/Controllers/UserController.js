app.controller('UserController', [
  '$scope', '$route', 'config', function ($scope, $route, config) {

		$scope.user = { userName: config.userName };

		$scope.logout = function() {
			alert('');
		};
	}
]);