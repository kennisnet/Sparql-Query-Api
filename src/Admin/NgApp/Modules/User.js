angular.module('UserModule', [])

.factory('userService', ['$q', function ($q) {

  var _queryGroups = [];
  var _activeQueryTab = "sparql";

  var ensureQueryGroup = function (value) {
    if (angular.isUndefined(_queryGroups[value]) || _queryGroups[value] === null) {
      _queryGroups[value] = { visible: true, edit: false };
    }
    return _queryGroups[value];
  };

  var accountGroups = [];

  var ensureAccountGroup = function (value) {
    if (angular.isUndefined(accountGroups[value]) || accountGroups[value] === null) {
      accountGroups[value] = { visible: true, edit: false };
    }
    return accountGroups[value];
  };

  var user = {
    activeQueryTab: function(value) {
      if (angular.isDefined(value)) {
        _activeQueryTab = value;
      }
      return _activeQueryTab;
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

