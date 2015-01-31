﻿angular.module('LogViewer', ['ui.bootstrap'])

.controller("LogController", [
  '$scope', '$location', '$http', 'config', function($scope, $location, $http, config) {

    $scope.timespan = 'last-24';
    $scope.downloadsTimespan = 'month';

    $scope.timespans = [
      { id: 'week', label: 'week' },
      { id: 'month', label: 'maand' }
    ];
    $scope.downloadsTimespan = $scope.timespans[1];

    $scope.getTabContent = function(tab) {
      switch (tab) {
      case 'downloads':
        if (!angular.isDefined($scope.downloads)) {
          $scope.updateDownloads(0);
        }
        break;
      default:
      }
    };

    $scope.updateStats = function() {
      $http.get(config.adminApiUrl + 'Log/Statistics' + "/?" + "timespan=" + $scope.timespan).then(function (response) {
        $scope.queryStatistics = angular.copy(response.data);
      });
    };

    $scope.onTimespanChanged = function() {
      console.log($scope.timespan);
      $scope.updateStats();
    };

    $scope.updateStats();

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
