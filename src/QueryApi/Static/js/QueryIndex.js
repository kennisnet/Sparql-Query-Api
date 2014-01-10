var QueryIndex = angular.module('QueryIndex', ['ui.bootstrap']);

function QueryParameterController($scope, $location, $http) {
	var original = null;
	var query = null;

	$scope.fetchParameterValues = function (query) {
		console.log('hi');
		$http.get('http://localhost:49975/Leverancier/ParameterValues/leverancier?api_key=aadc5fb8-f0d7-4d39-a0f2-3da4d49ad09a').then(function(response) {
			$scope.parameters = angular.copy(response.data);
		});

	};

	$scope.goForLink = function(link) {
		console.log(link);
		document.location.href = link + "&leverancier=" + $scope.parameterValue;


	};

}