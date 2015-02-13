angular.module('ip.alerts', [])
  .directive('messagePanel', [
    '$http', '$filter', '$interval', 'alertManager', function ($http, $filter, $interval, alertManager) {
      return {
        restrict: 'C',
        replace: true,
        template: "<div class=\"message-box-container\" ng-show=\"alerts.length > 0\" ng-class=\"{'message-box-modal': hasModal() == true}\">" +
          "<div class=\"container\">" +
          "<div class=\"message-box alert space-above alert-{{alert.type}}\" ng-repeat=\"alert in alerts\">" +
          "<span class=\"fa fa-info space-after\"></span>" +
          "{{alert.message}}" +
          "<button class=\"spritesheet small-space-above small-space-after pull-right\" ng-click=\"delete($index)\"></button>" +
          "</div>" +
          "</div>" +
          "</div>",
        link: function (scope, elm, attrs) {
          scope.alerts = alertManager.alerts;

          scope.delete = function(index) {
            scope.alerts.splice(index, 1);
          };

          scope.hasModal = function() {
            var found = $filter('filter')(scope.alerts, { showType: 'modal' }, true);
            if (found) {
              return found.length > 0;
            }
            return false;
          };

          var stop = $interval(function () {
            var i = scope.alerts.length;
            while (i--) {
              if (scope.alerts[i].expireDate < new Date()) {
                scope.alerts.splice(i, 1);
              }
            } 
          }, 100);

          scope.stopUpdater = function () {
            if (angular.isDefined(stop)) {
              $interval.cancel(stop);
              stop = undefined;
            }
          };

          scope.$on('$destroy', function () {
            // Make sure that the interval is destroyed too
            scope.stopUpdater();
          });
        }
      };
    }
  ])
  .factory('alertManager', function () {
    var service = {
      alerts: [],

      info: function (message) {
        service.addMessage(message, 'info', '', 3000);
      },

      error: function (message) {
        service.addMessage(message, 'error', 'modal', 10000);
      },

      addMessage: function (message, type, showType, timeout) {
        var expireDate = (timeout) ? new Date(Date.now() + timeout) : Number.MAX_VALUE;
        this.alerts.push({ message: message, type: type, showType: showType, expireDate: expireDate });
      }
    };
    return service;
  });