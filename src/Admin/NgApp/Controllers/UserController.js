app.controller('UserController', [
  '$scope', '$location', '$http', 'config', function ($scope, $location, $http, config) {
    $scope.user = config.user;
  }
]);