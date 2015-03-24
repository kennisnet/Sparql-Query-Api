angular.module('LogViewer', ['ui.bootstrap'])

.controller("LogStatsController", [
  '$scope', '$location', '$routeParams', '$http', 'config', function($scope, $location, $routeParams, $http, config) {

    $scope.statisticsTimespans = [
			{ id: 'last-hour', label: 'afgelopen uur' },
			{ id: 'last-24', label: 'afgelopen 24 uur' },
			{ id: 'last-week', label: 'afgelopen 7 dagen' },
			{ id: 'last-month', label: 'afgelopen maand' }
    ];
    $scope.timespan = $scope.statisticsTimespans[1];

    $scope.updateStats = function (timespan) {
    	if (!timespan) {
		    timespan = $scope.timespan;
    	}	    

	    var url = config.adminApiUrl + 'Log/Statistics' + "/?" + "timespan=" + timespan.id;
	    if ($routeParams.apiKey) {
		    url += "&accountApiKey=" + $routeParams.apiKey;
	    }

	    if ($location.url().indexOf("/query/") > -1) {
	    	url += "&queryAlias=" + $location.url().replace("/query/", "");
	    }

	    $http.get(url).then(function (response) {
        $scope.queryStatistics = angular.copy(response.data);
      });
    };

    $scope.onTimespanChanged = function (timespan) {
      console.log($scope.timespan);
      $scope.updateStats(timespan);
    };
    
    $scope.updateStats();
  }
])

.controller("LogDownloadController", [
  '$scope', '$location', '$http', 'config', function($scope, $location, $http, config) {

  	$scope.getTabContent = function (tab) {
  		switch (tab) {
  			case 'downloads':
  				if (!angular.isDefined($scope.downloads)) {
  					$scope.updateDownloads(0);
  				}
  				break;
  			default:
  		}
  	};

  	$scope.timespans = [
      { id: 'week', label: 'week' },
      { id: 'month', label: 'maand' }
    ];
    $scope.downloadsTimespan = $scope.timespans[1];

    $scope.previous = function() {
      $scope.updateDownloads($scope.downloads.Start - 10);
    };

    $scope.next = function() {
      if ($scope.downloads.End < $scope.downloads.TotalCount) {
        $scope.updateDownloads($scope.downloads.Start + 10);
      }
    };

    $scope.updateDownloads = function(timespan, start) {
      $http.get(config.adminApiUrl + 'Log/Downloads' + '/?start=' + start + '&timespan=' + timespan.id).then(function (response) {
        $scope.downloads = angular.copy(response.data);
      });
    };

    $scope.onDownloadsTimespanChanged = function (timespan) {
      $scope.updateDownloads(timespan, 0);
    };
  }
]);
