app.directive('checkboxSelector', ['$parse',
  function ($parse) {
    return {
      restrict: 'E',
      replace: true,
      scope: {
        selectAll: '&',
        selectNone: '&',
        ngDisabled: '='
      },
      template:
        '<div>' +
          '<button class="btn btn-sm btn-link dropdown-toggle" data-toggle="dropdown" data-ng-disabled="ngDisabled">selecteer <i class="fa fa-caret-down"></i></button>' +
          '<ul class="dropdown-menu dropdown-toggle">' +
            '<li>' +
              '<button class="btn btn-sm btn-link" data-ng-click="onSelectAll()"><i class="fa fa-check-square-o fa-fw"></i> alles</button>' +
            '</li>' +
            '<li>' +
              '<button class="btn btn-sm btn-link" data-ng-click="onSelectNone()"><i class="fa fa-minus-square-o fa-fw"></i> geen</button>' +
            '</li>' +
          '</ul>' +
        '<div>',
      link: function (scope, element, attrs) {

        scope.onSelectAll = function () {
          scope.$parent.$evalAsync(scope.selectAll);
        } 

        scope.onSelectNone = function () {
          scope.$parent.$evalAsync(scope.selectNone);
        } 
      }
    };
  }
]);