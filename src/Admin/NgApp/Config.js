var configModule = angular.module('app.config', [])
    .constant('config', {
      adminApiUrl: window.configSiteRoot + 'Api/',
      queryUrl: window.configQueryUrl,
      apiKey: window.configApiKey,
      viewsUrl: window.configSiteRoot,
      version: '0.1'
    })
;