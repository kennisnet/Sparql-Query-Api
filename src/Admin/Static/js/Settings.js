var SettingsEditor = angular.module('SettingsEditor', ['ui.bootstrap', 'loadingScreen']);

function SettingsController($scope, $location, $http) {
    var original = null;
    var settings = null;

    $http.get(app.config.apiRoot + '/Details/Global').then(function(response) {
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

    $scope.save = function () {
    	$http.post(app.config.apiRoot + '/Details/Global', { model: $scope.settings })
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
