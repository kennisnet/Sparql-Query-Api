var LogViewer = angular.module('LogViewer', ['ui.bootstrap', 'loadingScreen']);

function LogController($scope, $location, $http) {

	$scope.timespan = 'last-24';
	$scope.downloadsTimespan = 'month';
	
	$scope.getTabContent = function(tab) {
		switch (tab) {
		case 'downloads':
			if (!angular.isDefined($scope.downloads)) {
				$scope.updateDownloads(0);
			}
		default:
		}
	};
	
	$scope.updateStats = function() {
		$http.get(app.config.apiRoot + '/Statistics' + "/?" + "timespan=" + $scope.timespan).then(function (response) {
			$scope.queryStatistics = angular.copy(response.data);
		});
	};
	
	$scope.onTimespanChanged = function() {
		console.log($scope.timespan);
		$scope.updateStats();
	};

	$scope.updateStats();

	$scope.previous = function () {
		$scope.updateDownloads($scope.downloads.Start - 10);
	};

	$scope.next = function () {
		if ($scope.downloads.End < $scope.downloads.TotalCount) {
			$scope.updateDownloads($scope.downloads.Start + 10);
		}
	};

	$scope.updateDownloads = function (start) {
		$http.get(app.config.apiRoot + '/Downloads' + '/?start=' + start + '&timespan=' + $scope.downloadsTimespan).then(function (response) {
			$scope.downloads = angular.copy(response.data);
		});
	};

	$scope.onDownloadsTimespanChanged = function () {
		console.log($scope.downloadsTimespan);
		$scope.updateDownloads(0);
	};

}
