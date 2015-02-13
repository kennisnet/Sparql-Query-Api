app.controller('NavController', [
  '$scope', '$route', '$location', function($scope, $route, $location) {

    $scope.isRoute = function(value) {
      if (angular.isDefined($route.current)) {
        if (angular.isDefined($route.current.originalPath)) {
          return $route.current.originalPath.indexOf(value) == 0;
        }
      }
      return false;
    }
  }
]);