﻿angular
    .module('loadingScreen', [])
    .config(function ($httpProvider) {
    	var numLoadings = 0;
    	var loadingScreen = $('<div id="loadingScreen" style="position:fixed;top:0;left:0;right:0;bottom:0;z-index:10000;background-color:gray;background-color:rgba(65,70,75,0.2);"><img style="position:absolute;top:40%;left:50%;" alt="" src="data:image/gif;base64,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" /></div>')
					.appendTo($('body')).hide();
	    
    	$httpProvider.interceptors.push(function () {
    		return {
    			request: function(config) {
    				console.log('showing loader screen');
    				//numLoadings++;
    				loadingScreen.show();
    				return config;
    			}
    		};
    	});
	    
    	$httpProvider.responseInterceptors.push(function () {
    		return function (promise) {
    			console.log('showing loader screen');
    			numLoadings++;
    			loadingScreen.show();
    			var hide = function(r) {
    				if (!(--numLoadings)) {
					    loadingScreen.hide();
				    }
    				return r;
    			};
    			return promise.then(hide, hide);
    		};
    	});
	    
    });
