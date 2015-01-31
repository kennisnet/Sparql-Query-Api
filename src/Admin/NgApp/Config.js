var configModule = angular.module('app.config', [])
    .constant('config', {
      adminApiUrl: window.configSiteRoot + 'Api/',
      viewsUrl: window.configSiteRoot,
      version: '0.1'
    })
;