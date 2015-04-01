var QueryEditor = angular.module('QueryEditor', ['ui.bootstrap', 'ui.codemirror', 'UserModule']);

QueryEditor.controller('QueryController', [
  '$scope', '$routeParams', '$location', '$http', '$filter', '$timeout', 'config', 'userService', function ($scope, $routeParams, $location, $http, $filter, $timeout, config, userService) {
    var original = null;
    var query = null;

    $scope.user = config.user;

    $scope.newGroup = '';

    $scope.isNewquery = function(value) {
      return (value === 'new' || value === '' || value.indexOf('000000') > -1);
    };

    var queryId = $routeParams.id;
		
    var emptyNote = function() {
	    return {
		    AccountId: $scope.user.id,
		    CreationDate: new Date(),
		    ModificationDate: new Date(),
		    Content: ""
	    };
    }

    $scope.newNote = emptyNote();

    $http.get(config.adminApiUrl + 'Query/' + (($scope.isNewquery(queryId)) ? 'new' : queryId)).then(function (response) {
    	query = response.data;	    
      original = query;
      $scope.query = angular.copy(query);
    });

    $scope.isClean = function() {
      return angular.equals(original, $scope.query);
    };

    $scope.deleteParameter = function(index) {
      $scope.query.Parameters.splice(index, 1);
      $scope.queryForm.$setDirty();
    };

    $scope.addParameter = function(index) {
      $scope.query.Parameters.splice(index + 1, 0, { Name: 'new', Description: '' });
    };		

    $scope.addNote = function(note) {
      if ($scope.query.Notes == null) {
        $scope.query.Notes = [];
      }
      $scope.newNote = emptyNote();

	    var url = config.adminApiUrl + 'Query/' + query.Id + '/Note';
    	$http.post(url, note)
        .success(function (response) {
          $scope.query.Notes.push(response);
				})
        .error(function () {
        	alert('Kan de notitie niet opslaan. Probeer het nog eens...');
        });

    };

    $scope.updateNote = function(note) {
    	var url = config.adminApiUrl + 'Query/' + query.Id + '/Note';
	    note.ModificationDate = new Date();
    	$http.put(url, note)
        .success(function (response) {
	        note.editMode = false;
	      })
        .error(function () {
        	alert('Kan de notitie niet opslaan. Probeer het nog eens...');
        });

    };

    $scope.deleteNote = function(note) {
      if ($scope.query.Notes == null) {
        $scope.query.Notes = [];
      }
      if (window.confirm("Weet u zeker dat u notitie wilt verwijderen?")) {
        var url = config.adminApiUrl + 'Query/' + query.Id + '/Note';
        $http.delete(url + '?accountId=' + note.AccountId + '&creationDate=' + note.CreationDate)
          .success(function(response) {
            var index = $scope.query.Notes.indexOf(note);
            $scope.query.Notes.splice(index, 1);
          })
          .error(function() {
            alert('Kan de notitie niet opslaan. Probeer het nog eens...');
          });

        $scope.queryForm.$setDirty();
      }
    };

    $scope.save = function () {
      var isNewquery = $scope.isNewquery(queryId);

      var saveMethod = (isNewquery) ? $http.post : $http.put;

      saveMethod(config.adminApiUrl + 'Query/' + ((isNewquery) ? 'new' : query.Id), $scope.query)
        .success(function(response) {
          $scope.queryForm.$setPristine();
          document.location.href = config.viewsUrl + '#/query/' + $scope.query.Alias;
        })
        .error(function() {
          alert('Kan de query niet opslaan. Probeer het nog eens...');
        });

    };

    RegisterShortcut({ funxion: 'Ctrl', key: 'S' }, $scope.save);

    $scope.cancel = function() {
      $scope.returnUrl = config.viewsUrl + '#/query';
      document.location.href = $scope.returnUrl;
    };

    $scope.duplicate = function() {
      queryId = '';
      $scope.query.Id = null;
      $scope.query.Alias = 'copy-of-' + $scope.query.Alias;
      $scope.query.Label = 'Copy of ' + $scope.query.Label;
      $scope.save();
    };

    $scope.showPreview = function () {
	    console.log($scope.query.Alias);
      $http.get(config.adminApiUrl + 'Query/' + $scope.query.Alias + '/Preview')
        .success(function(response) {
          console.log(response);
          $scope.previewResults = angular.copy(response);
          console.log($scope.previewResults);
        })
        .error(function() {
          alert('Preview mislukt. Probeer het nog eens...');
        });
    };

    $scope.formatSparqlBinding = function(binding) {
      if (!binding) {
        return '';
      }
      switch (binding.type) {
      case "uri":
        return binding.value.replace('http://purl.edustandaard.nl/begrippenkader/', 'bk:');
      case "literal":
        return binding.value; // + '@' + binding['xml:lang'];
      default:
        return binding.value;
      }
    };

    $scope.showQueryPreview = function() {
      $http.get(config.adminApiUrl + 'Query/' + $scope.query.Alias + '/PreviewQuery/')
        .success(function(response) {
          console.log(response);
          $scope.previewQuery = angular.copy(response);
          console.log($scope.previewQuery);
        })
        .error(function() {
          alert('Preview query mislukt. Probeer het nog eens...');
        });
    };

    $scope.delete = function() {
      $scope.returnUrl = config.viewsUrl + '#/query';

      if (queryId === '') {
        document.location.href = $scope.returnUrl;
      } else {
        $http.delete(config.adminApiUrl + 'Query/' + queryId).then(function (response) {
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

    $scope.activeTab = [];

    $scope.setTab = function (tab) {
      $scope.activeTab = [];
      $scope.activeTab[tab] = true;

      // store current tab in user's profile
      userService.activeQueryTab(tab);

      // activate the sparql code mirror editor 
      if (tab == 'sparql') {
        $timeout(function () { $scope.setSparqlEditorParent(); }, 100);
      }
    };

    $scope.getEditor = function () {
      console.log($('#sparql-editor'), angular.element('#sparql-editor'), angular.element('#edit_query_sparql'));
      $scope.setSparqlEditorParent();
    };

    $scope.codeMirrorOptions = {
    	lineNumbers: true,
    	lineWrapping: true,
    	mode: 'sparql',
    	readOnly: $scope.user.isEditor ? '' : 'nocursor'
    }

    $scope.setSparqlEditorParent = function() {
    	var element = $('#sparql-editor').detach();
      $('#sparql-editor-parent').append(element);
      $scope.codeMirrorRefresh++;
      $timeout(function() { $scope.codeMirrorRefresh++; }, 100);
    };

    $scope.$on("$destroy", function () {
      // clear the setTab function, otherwise it's called multiple times while destroying the tabcontrol
      $scope.setTab = function () { };
    });

    $scope.setTab(userService.activeQueryTab());

  }
]);

QueryEditor.controller('QueryIndexController', [
  '$scope', '$location', '$http', '$window', 'config', 'userService', function ($scope, $location, $http, $window, config, userService) {

    $scope.isGroupVisible = function(value) {
      return userService.queryGroup(value).visible;
    };

    $scope.isGroupEdit = function(value) {
      return userService.queryGroup(value).visible;
    };

    $scope.toggleGroup = function (event, value) {
      var panelId = "" + event.target.id;
      if (panelId.indexOf('group') == 0) {
        userService.queryGroup(value).visible = !userService.queryGroup(value).visible;
      }
    };

    $scope.toggleEdit = function (value) {
      userService.queryGroup(value).edit = !userService.queryGroup(value).edit;
    };

    $scope.deleteQuery = function(value) {
      $scope.returnUrl = config.viewsUrl + '#/query';

      if (window.confirm("Weet u zeker dat u query '" + value + "' wilt verwijderen?")) {
        if (value === '') {
          document.location.href = $scope.returnUrl;
        } else {
          $http.delete(config.adminApiUrl + 'Query/' + value).then(function (response) {
            document.location.href = $scope.returnUrl;
            $window.location.reload();
          });
        }
      }
    };
  }
]);