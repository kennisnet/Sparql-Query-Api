var configModule = angular.module('app.config', [])
    .constant('config', {
      userName: window.userName,
      adminApiUrl: window.configSiteRoot + 'Api/',
      queryUrl: window.configQueryUrl,
      apiKey: window.configApiKey,
      viewsUrl: window.configSiteRoot,
      version: '0.1'
    })
;