var QueryEditor = angular.module('QueryEditor', ['ui.bootstrap', 'loadingScreen', 'ui.codemirror']);

function QueryController($scope, $location, $http, $timeout) {
  var original = null;
  var query = null;

  $scope.newGroup = '';

  $scope.isNewquery = function (value) {
    return (value === '' || value.indexOf('000000') > -1);
  };

  $http.get(app.config.apiRoot + '/Details/' + (($scope.isNewquery(queryId)) ? 'new' : queryId)).then(function (response) {
    query = response.data;
    original = query;
    $scope.query = angular.copy(query);
    $scope.query.Group = group;
  });

  $scope.isClean = function () {
    return angular.equals(original, $scope.query);
  };
	
  $scope.deleteParameter = function (index) {
    $scope.query.Parameters.splice(index, 1);
    $scope.queryForm.$setDirty();
  };

  $scope.addParameter = function (index) {
    $scope.query.Parameters.splice(index + 1, 0, { Name: 'new', Description: '' });
  };
	
  $scope.addNote = function (index) {
    if ($scope.query.Notes == null) {
      $scope.query.Notes = [];
    }
    $scope.query.Notes.splice(index + 1, 0, { CreationDate: new Date(), ModificationDate: new Date(), Content: "" });
  };
	
  $scope.deleteNote = function (index) {
    if ($scope.query.Notes == null) {
      $scope.query.Notes = [];
    }
    $scope.query.Notes.splice(index, 1);
    $scope.queryForm.$setDirty();
  };

  $scope.save = function () {
    var saveMethod = ($scope.isNewquery(queryId)) ? $http.post : $http.put;

    saveMethod(app.config.apiRoot + '/Details/' + (($scope.isNewquery(queryId)) ? 'new' : queryId), { model: $scope.query })
          .success(function (response) {
            $scope.queryForm.$setPristine();
            if ($scope.isNewquery(queryId)) {
              document.location.href = app.config.siteRoot + "Query";
            }
          })
          .error(function () {
            alert('Kan de query niet opslaan. Probeer het nog eens...');
          });

  };

  RegisterShortcut({ funxion: 'Ctrl', key: 'S' }, $scope.save);

  $scope.cancel = function () {
    $scope.returnUrl = app.config.siteRoot + 'Query';
    document.location.href = $scope.returnUrl;
  };

  $scope.duplicate = function () {
    queryId = '';
    $scope.query.Id = 'copy of ' + $scope.query.Id;
    $scope.query.Label = 'copy of ' + $scope.query.Label;
    $scope.save();
  };

  $scope.showPreview = function () {
    $http.get(app.config.apiRoot + '/Preview/' + $scope.query.Id)
      .success(function (response) {
        console.log(response);
        $scope.previewResults = angular.copy(response);
        console.log($scope.previewResults);
      })
      .error(function () {
        alert('Preview mislukt. Probeer het nog eens...');
      });
  };

  $scope.formatSparqlBinding = function (binding) {
    if (!binding) {
      return '';
    }
    switch (binding.type) {
      case "uri":
        return binding.value.replace('http://purl.edustandaard.nl/begrippenkader/', 'bk:');
      case "literal":
        return binding.value;// + '@' + binding['xml:lang'];
      default:
        return binding.value;
    }
  };

  $scope.showQueryPreview = function () {
    $http.get(app.config.apiRoot + '/PreviewQuery/' + $scope.query.Id)
      .success(function (response) {
        console.log(response);
        $scope.previewQuery = angular.copy(response);
        console.log($scope.previewQuery);
      })
      .error(function () {
        alert('Preview query mislukt. Probeer het nog eens...');
      });
  };

  $scope.delete = function () {
    $scope.returnUrl = app.config.siteRoot + 'Query';

    if (queryId === '') {
      document.location.href = $scope.returnUrl;
    } else
    {
      $http.delete(app.config.apiRoot + '/Delete/' + queryId).then(function(response) {
        document.location.href = $scope.returnUrl;
      });
    }
  };

  $scope.addGroup = function(value) {
    $scope.query.Groups = $scope.query.Groups || [];
    $scope.query.Groups.push(value);
    $scope.query.Group = value;
    $scope.newGroup = '';
  };

  $scope.getEditor = function() {
    console.log($('#sparql-editor'), angular.element('#sparql-editor'), angular.element('#edit_query_sparql'));
    $scope.setSparqlEditorParent();
  };

  $scope.setSparqlEditorParent = function() {
    var element = $('#sparql-editor').detach();
    $('#sparql-editor-parent').append(element);
    $scope.codeMirrorRefresh++;
    $timeout(function () { $scope.codeMirrorRefresh++; }, 100);
  };

  //$timeout($scope.setSparqlEditorParent, 100);
}

function IndexController($scope, $location, $http) {
  $scope.data = {
    isCollapsed: false,
    groups: []
  };
	
  $scope.ensureGroup = function (value) {
    if (angular.isUndefined($scope.data.groups[value]) || $scope.data.groups[value] === null) {
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
      $scope.ensureGroup(value).visible = !$scope.ensureGroup(value).visible;
    }
    //$scope.panels[value] = !$scope.panels[value];
  };

  $scope.toggleEdit = function (value) {
    $scope.ensureGroup(value).edit = !$scope.data.groups[value].edit;
  };

  $scope.deleteQuery = function(value) {
    $scope.returnUrl = app.config.siteRoot + 'Query';

    if (window.confirm("Weet u zeker dat u query '" + value + "' wilt verwijderen?")) {
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