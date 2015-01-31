var AccountEditor = angular.module('AccountEditor', ['ui.bootstrap', 'loadingScreen']);

function AccountController($scope, $location, $http) {
    var original = null;
    var account = null;

    $scope.isNewAccount = function (value) {
    	return (value === '' || value.indexOf('000000') > -1);
    };

    $http.get(app.config.apiRoot + '/' + (($scope.isNewAccount(accountId)) ? 'new' : accountId)).then(function (response) {
    		account = response.data;
    		original = account;
    		$scope.account = angular.copy(account);
    });

    $scope.isClean = function () {
        return angular.equals(original, $scope.query);
    };

    $scope.save = function () {
	    var saveMethod = ($scope.isNewAccount(accountId)) ? $http.post : $http.put;

    	saveMethod(app.config.apiRoot + "/" + accountId, $scope.account)
            .success(function (response) {
            	if ($scope.isNewAccount(accountId)) {
            		document.location.href = app.config.siteRoot + "Account";
            	}
            })
            .error(function () {
                alert("Kan de query niet opslaan. Probeer het nog eens...");
            });

    };

    $scope.cancel = function () {
        $scope.returnUrl = app.config.siteRoot + "Account";
        document.location.href = $scope.returnUrl;
    };

    $scope.delete = function () {
        $scope.returnUrl = app.config.siteRoot + "Account";

        if (accountId === '') {
            document.location.href = $scope.returnUrl;
        } else
        {
        	$http.delete(app.config.apiRoot + "/" + accountId).then(function (response) {
              document.location.href = $scope.returnUrl;
          });
        }
    };

}

function IndexController($scope, $location, $http) {
	$scope.data = {
		isCollapsed: false,
		groups: []
	};

	$scope.ensureGroup = function (value) {
		if ($scope.data.groups[value] === null) {
			$scope.data.groups[value] = { visible: true, edit: false };
		}
		return $scope.data.groups[value];
	};

	$scope.isGroupVisible = function (value) {
		return $scope.ensureGroup(value).visible;
	};

	$scope.isGroupEdit = function (value) {
		return $scope.ensureGroup(value).edit;
	};

	$scope.toggleGroup = function (event, value) {
		if (event.target.id.startsWith('group')) {
			$scope.ensureGroup(value).visible = !$scope.data.groups[value].visible;
		}
		//$scope.panels[value] = !$scope.panels[value];
	};

	$scope.toggleEdit = function (value) {
		$scope.ensureGroup(value).edit = !$scope.data.groups[value].edit;
	};

	$scope.deleteAccount = function (value) {
		$scope.returnUrl = app.config.siteRoot + 'Account';

		if (window.confirm("Weet u zeker dat u account '" + value + "' wilt verwijderen?")) {
			if (value === '') {
				document.location.href = $scope.returnUrl;
			} else {
				$http.post(app.config.apiRoot + '/Delete/' + value).then(function (response) {
					document.location.href = $scope.returnUrl;
				});
			}

		}

	};
}