//make sure console.log exists in ie.
window.console = window.console || {};
window.console.log = window.console.log || function () { };

var app = angular.module('QueryAdmin', [
	'ngRoute',
	'ngResource',
	'app.config',
	'no-cache',
  'bgDirectives',
	'loadingScreen',
  'monospaced.elastic',
	'Navigator',
	'QueryEditor',
	'AccountEditor',
	'SettingsEditor',
	'LogViewer',
	'Test'
]);

app.config(function ($httpProvider) { // Note: this will work if you are using AngularJS service $http or $resource
  $httpProvider.interceptors.push('loadingScreenInterceptor');
});