angular.module('UserModule', [])

.factory('userService', ['$q', function ($q) {

  var queryGroups = [];

  var ensureQueryGroup = function (value) {
    if (angular.isUndefined(queryGroups[value]) || queryGroups[value] === null) {
      queryGroups[value] = { visible: true, edit: false };
    }
    return queryGroups[value];
  };

  var accountGroups = [];

  var ensureAccountGroup = function (value) {
    if (angular.isUndefined(accountGroups[value]) || accountGroups[value] === null) {
      accountGroups[value] = { visible: true, edit: false };
    }
    return accountGroups[value];
  };

  var user = {
    queryGroup: function(name) {
      return ensureQueryGroup(name);
    },

    accountGroup: function(name) {
      return ensureAccountGroup(name);
    }
  }

  return user;
}]);

