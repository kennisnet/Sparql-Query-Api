var QueryEditor = angular.module('QueryEditor', ['ui.bootstrap', 'loadingScreen', 'ui.codemirror']);

function QueryController($scope, $location, $http, $timeout) {
    var original = null;
    var query = null;

	  $scope.newGroup = '';


	  $scope.isNewquery = function (value) {
	  	return (value == '' || value.indexOf('000000') > -1);
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

    $scope.delete = function () {
        $scope.returnUrl = app.config.siteRoot + 'Query';

        if (queryId == '') {
            document.location.href = $scope.returnUrl;
        } else
        {
          $http.delete(app.config.apiRoot + '/Delete/' + queryId).then(function(response) {
              document.location.href = $scope.returnUrl;
          });
        }
    };

	$scope.addGroup = function() {
		$scope.query.Groups.push($scope.newGroup);
		$scope.query.Group = $scope.newGroup;
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
		if ($scope.data.groups[value] == null) {
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

	$scope.deleteQuery = function(value) {
		$scope.returnUrl = app.config.siteRoot + 'Query';

		if (window.confirm("Weet u zeker dat u query '" + value + "' wilt verwijderen?")) {
			if (value == '') {
				document.location.href = $scope.returnUrl;
			} else {
				$http.post(app.config.apiRoot + '/Delete/' + value).then(function (response) {
					document.location.href = $scope.returnUrl;
				});
			}

		}

	};
}