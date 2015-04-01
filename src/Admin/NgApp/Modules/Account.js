angular.module('AccountEditor', ['ui.bootstrap', 'ui.codemirror', 'UserModule'])

.factory('AccountService', ['$q', '$http', 'config', function ($q, $http, config){
		var service = {}

		var accounts = null;

		service.getAccounts = function () {
			var deferred = $q.defer();
			if (accounts != null) {
				deferred.resolve(accounts);
				return deferred.promise;
			}
			$http.get(config.adminApiUrl + 'Account/').then(function (response) {
				accounts = response.data;
				deferred.resolve(accounts);
			});
			return deferred.promise;
		}

		service.addAccount = function (account) {
			accounts.push(account);
		}

		service.getAccountById = function (accountId) {
			var result = $.grep(accounts, function (account) { return account.Id == accountId; });

			if (result.length == 0) {
				alert('Account met id ' + accountId + ' bestaat niet.');
				return null;
			}
			
			return result[0];
		}

		return service;

	}])

.controller('AccountController', [
  '$scope', '$routeParams', '$location', '$http', '$timeout', 'config', 'AccountService', function ($scope, $routeParams, $location, $http, $timeout, config, AccountService) {
    var original = null;
    var account = null;

    var returnUrl = config.viewsUrl + "#/account";

    var accountId = $routeParams.id;

    $scope.user = config.user;

    $scope.isNewAccount = function (value) {
      return (value === null || value === '' || value === 'new' || value.indexOf('000000') > -1);
    };

    $http.get(config.adminApiUrl + 'Account/' + (($scope.isNewAccount(accountId)) ? 'new' : accountId)).then(function (response) {
      account = response.data;
      original = account;
      $scope.account = angular.copy(account);
    });

    $scope.isClean = function () {
      return angular.equals(original, $scope.query);
    };

    $scope.save = function () {
      var saveMethod = ($scope.isNewAccount(account.Id)) ? $http.post : $http.put;

      saveMethod(config.adminApiUrl + 'Account/' + accountId, $scope.account)
        .success(function (response) {
          if ($scope.isNewAccount(accountId)) {
          	document.location.href = config.viewsUrl + '#/account';
	          AccountService.addAccount($scope.account);
          }
        })
        .error(function () {
          alert("Kan de query niet opslaan. Probeer het nog eens...");
        });
    };

    $scope.savePassword = function (password) {
      var url = config.adminApiUrl + 'Account/' + accountId;
      $http({ method: 'PATCH', url: url, data: angular.toJson(password) })
        .success(function (response) {
          alert("Wachtwoord gewijzigd.");
        })
        .error(function () {
          alert("Kan het wachtwoord niet opslaan. Probeer het nog eens...");
        });
    };

    $scope.cancel = function () {
      document.location.href = returnUrl;
    };

    $scope.delete = function () {
      if (accountId === '') {
        document.location.href = returnUrl;
      } else {
        $http.delete(config.adminApiUrl + 'Account/' + accountId).then(function (response) {
          document.location.href = returnUrl;
        });
      }
    };

  }
])

.controller('AccountIndexController', [
  '$scope', '$routeParams', '$location', '$http', '$window', 'config', 'userService', function ($scope, $routeParams, $location, $http, $window, config, userService) {
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

    $scope.deleteAccount = function (value) {
      returnUrl = config.viewsUrl + "#/account";

      if (window.confirm("Weet u zeker dat u account '" + value + "' wilt verwijderen?")) {
        if (value === '') {
          document.location.href = returnUrl;
        } else {
          $http.delete(config.adminApiUrl + 'Account/' + value).then(function (response) {
            document.location.href = returnUrl;
            $window.location.reload();
          });
        }

      }

    };
  }
])

.directive('passwordForm', [function () {
  return {
    restrict: 'A',
    require: '?form',
    link: function link(scope, element, iAttrs, formController) {

      if (!formController) {
        return;
      }

      // Remove this form from parent controller
      var parentFormController = element.parent().controller('form');
      parentFormController.$removeControl(formController);

      // Replace form controller with a "null-controller"
      var nullFormCtrl = {
        $addControl: angular.noop,
        $removeControl: angular.noop,
        $setValidity: angular.noop,
        $setDirty: angular.noop,
        $setPristine: angular.noop
      };

      scope.isValidPassword = function (val1, val2) {
        return val1 > 3 && val1 == val2;
      };

      angular.extend(formController, nullFormCtrl);
    }
  };
}]);