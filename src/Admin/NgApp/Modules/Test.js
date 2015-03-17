var TestEditor = angular.module('Test', ['ui.bootstrap', 'ui.codemirror', 'UserModule']);

TestEditor.controller('TestIndexController', [
  '$scope', '$location', '$http', '$window', '$filter', 'config', 'userService', function ($scope, $location, $http, $window, $filter, config, userService) {

  	$scope.endpoint = { source: "OBK", compare: "OBK-Virtuoso" };

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

    $scope.testAll = function () {
    	angular.forEach($scope.groupedQueries.Groups, function (group) {
    		angular.forEach(group.Items, function (query) {
			    $scope.test(query);
		    });
  		});
    };

		var getOrderedPropertyValues = function(data) {
			var rows = [];
			angular.forEach(data.results.bindings, function (binding) {
				angular.forEach(binding, function (value, key) {
					rows.push({ name: key, value: value.value});
				});
			});
			return $filter('orderBy')(rows, 'name');
		};

		var runQueryTest = function(query, url) {
			$http.get(url)
		    .then(
			    function (response) {
			    	query.test.push(getOrderedPropertyValues(response.data).toString());
			    },
			    function (response) {
				    console.log(response);
				    query.test.push({ error: response.status + ": " + response.statusText + " (" + url + ")"});
			    });
		}

    $scope.test = function (query) {
			// concat all parameters for the query
    	var params = "";
			angular.forEach(query.Parameters, function(parameter) {
				params += "&" + parameter.Name + "=" + parameter.SampleValue;
			});
    	// create query url
			var url = config.queryUrl + "/" + query.Id + "?" + "api_key=" + config.apiKey + "&format=json&debug=true" + params;

			// run query test with the two endpoints
			query.test = [];

	    runQueryTest(query, url + "&endpoint=" + $scope.endpoint.source);

	    runQueryTest(query, url + "&endpoint=" + $scope.endpoint.compare);
		  
    };

  }
]);