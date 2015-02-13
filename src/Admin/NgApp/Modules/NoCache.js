angular.module('no-cache', [])

.config(
  ['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('noCacheInterceptor');
  }])
  .factory('noCacheInterceptor', function () {
    return {
      request: function (config) {
        if (config.method == 'GET' && config.url.slice(-4) != 'html') {
          var separator = config.url.indexOf('?') === -1 ? '?' : (config.url[config.url.length - 1] == '&') ? '' : '&';
          config.url = config.url + separator + 'noCache=' + new Date().getTime();
        }
        return config;
      }
    };
  });