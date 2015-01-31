angular.module('AccountEditor', ['ui.bootstrap', 'ui.codemirror', 'UserModule'])
.controller('AccountController', [
  '$scope', '$routeParams', '$location', '$http', '$timeout', 'config', function($scope, $routeParams, $location, $http, $timeout, config) {
    var original = null;
    var account = null;

    var returnUrl = config.viewsUrl + "#/account";

    var accountId = $routeParams.id;

    $scope.isNewAccount = function(value) {
      return (value === '' || value.indexOf('000000') > -1);
    };

    $http.get(config.adminApiUrl + 'Account/' + (($scope.isNewAccount(accountId)) ? 'new' : accountId)).then(function(response) {
      account = response.data;
      original = account;
      $scope.account = angular.copy(account);
    });

    $scope.isClean = function() {
      return angular.equals(original, $scope.query);
    };

    $scope.save = function() {
      var saveMethod = ($scope.isNewAccount(account.Id)) ? $http.post : $http.put;

      saveMethod(config.adminApiUrl + 'Account/' + accountId, $scope.account)
        .success(function(response) {
          if ($scope.isNewAccount(accountId)) {
            document.location.href = app.config.siteRoot + "Account";
          }
        })
        .error(function() {
          alert("Kan de query niet opslaan. Probeer het nog eens...");
        });

    };

    $scope.cancel = function() {
      document.location.href = returnUrl;
    };

    $scope.delete = function() {
      if (accountId === '') {
        document.location.href = returnUrl;
      } else {
        $http.delete(app.config.apiRoot + "/" + accountId).then(function(response) {
          document.location.href = returnUrl;
        });
      }
    };

  }
])

.controller('AccountIndexController', [
  '$scope', '$routeParams', '$location', '$http', '$window', 'config', 'userService', function($scope, $routeParams, $location, $http, $window, config, userService) {
    $scope.isGroupVisible = function (value) {
      return userService.accountGroup(value).visible;
    };

    $scope.isGroupEdit = function (value) {
      return userService.accountGroup(value).visible;
    };

    $scope.toggleGroup = function (event, value) {
      if (event.target.id.startsWith('group')) {
        userService.accountGroup(value).visible = !userService.accountGroup(value).visible;
      }
      //$scope.panels[value] = !$scope.panels[value];
    };

    $scope.toggleEdit = function (value) {
      userService.accountGroup(value).edit = !userService.accountGroup(value).edit;
    };

    $scope.deleteAccount = function(value) {
      returnUrl = config.viewsUrl + "#/account";

      if (window.confirm("Weet u zeker dat u account '" + value + "' wilt verwijderen?")) {
        if (value === '') {
          document.location.href = returnUrl;
        } else {
          $http.delete(config.adminApiUrl + 'Account/' + value).then(function(response) {
            document.location.href = returnUrl;
            $window.location.reload();
          });
        }

      }

    };
  }
]);