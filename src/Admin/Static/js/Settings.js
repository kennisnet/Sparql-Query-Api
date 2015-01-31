var SettingsEditor = angular.module('SettingsEditor', ['ui.bootstrap', 'loadingScreen']);

function SettingsController($scope, $location, $http) {
    var original = null;
    var settings = null;

    $http.get(app.config.apiRoot + '/Global').then(function(response) {
        settings = response.data;
        original = settings;
        $scope.settings = angular.copy(settings);
    });

    $scope.isClean = function () {
        return angular.equals(original, $scope.settings);
    };
	
    $scope.deleteEndpoint = function (index) {
    	$scope.settings.SparqlEndpoints.splice(index, 1);
    	$scope.settingsForm.$setDirty();
    };

    $scope.addEndpoint = function (index) {
    	$scope.settings.SparqlEndpoints.splice(index + 1, 0, { Name: 'new' });
    	$scope.settingsForm.$setDirty();
    };

    $scope.clearCache = function () {
      $http.post(app.config.apiRoot + '/ClearCache')
            .success(function (data, status) {
              if (status == 200) {
                alert('Cache legen is gelukt!');
              } else {
                alert('Fout: Cache legen is mislukt...');
              }
            })
            .error(function () {
              alert('Fout: Cache legen is mislukt...');
            });

    };

    $scope.save = function () {
    	$http.post(app.config.apiRoot + '/Global', $scope.settings)
            .success(function (response) {
	            $scope.settingsForm.$setPristine();
            })
            .error(function () {
                alert('Kan de settings niet opslaan. Probeer het nog eens...');
            });

    };

    $scope.cancel = function () {
        $scope.returnUrl = app.config.siteRoot + 'Settings';
        document.location.href = $scope.returnUrl;
    };

}
