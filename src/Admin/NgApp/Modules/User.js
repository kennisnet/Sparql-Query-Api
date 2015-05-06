angular.module('UserModule', [])

.factory('userService', ['$q', '$localStorage', function ($q, $localStorage) {

  $localStorage.$default({
    queryGroups: {},
    accountGroups: {},
    activeQueryTab: "sparql"
  });

  var ensureQueryGroup = function (value) {
    if (angular.isUndefined($localStorage.queryGroups[value]) || $localStorage.queryGroups[value] === null) {
      $localStorage.queryGroups[value] = { visible: true, edit: false };
    }
    return $localStorage.queryGroups[value];
  };


  var ensureAccountGroup = function (value) {
    if (angular.isUndefined($localStorage.accountGroups[value]) || $localStorage.accountGroups[value] === null) {
      $localStorage.accountGroups[value] = { visible: true, edit: false };
    }
    return $localStorage.accountGroups[value];
  };

  var user = {
    activeQueryTab: function(value) {
      if (angular.isDefined(value)) {
        $localStorage.activeQueryTab = value;
      }
      return $localStorage.activeQueryTab;
    },

    queryGroup: function (name) {
      return ensureQueryGroup(name);
    },

    accountGroup: function(name) {
      return ensureAccountGroup(name);
    }
  }

  return user;
}]);

