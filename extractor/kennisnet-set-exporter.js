// Exports a subset of the OBK rdf data to file or RDF Store 
//
// run with node kennisnet-set-exporter.js
// 
//
var http = require('http');
var fs = require('fs');

var queryList = [
  'KennisnetsetTypesRdf',
  'KennisnetSetInhoudenRdf',
  'LeerniveausRdf'
];

var index = 0;

// exportMode file | rdfStore
var exportMode = 'file'; 

var apiKey = 'api_key=bea588a3-c802-4838-a801-1e168b463b25'

var queryApi = {
    host: 'staging-api-beta.obk.kennisnet.nl',
    port: 80,
    // host: 'localhost',
    // port: 49975,
    path: '',
    method: 'GET',
    headers: {
      accept: 'text/html,application/xhtml+xml,application/xml'
    }
};

var sesameApi = {
    host: 'staging-api-beta.obk.kennisnet.nl',
    port: 80,
    path: '/sparql/kennisnet-set/statements',
    method: 'POST',
    headers: {
      'Content-Type': 'application/rdf+xml',
      'Content-Length': 0
    }
};

// loads the rdf data from the query api, using the array of queries 
function LoadQueryData() {
  queryApi.path = '/0.7/Query/' + queryList[index] + '?' + apiKey + '&format=rdf&debug=true'
	console.log('getting data for: ' + queryList[index] + ' from ' + queryApi.host);
  var request = http.get(queryApi, function(response) {
	  var xml = '';
	  response.on('data', function(chunk) {
	    xml += chunk;
	  });

	  response.on('end', function() {
      console.log('updating rdf for: ' + queryList[index]);
	    //console.log(xml);
      UpdateRdfData(xml, queryList[index], exportMode);
 		  
      index++;
      if (queryList.length > index) {
        LoadQueryData();
      }
	  });

	  // or you can pipe the data to a parser
	  //response.pipe(dest);
	});

	request.on('error', function(err) {
	  // debug error
	});	
}

// adds a set of statements to the RDF Store
function UpdateRdfData(rdf, set, mode) {
  if (mode == 'file') {
      fs.writeFile(set + '.rdf', rdf, function (err) {
      if (err) return console.log(err);
    });
  }
  else {
    sesameApi.headers['Content-Length'] = rdf.length + 200;
    console.log('posting rdf data ' + rdf.length + ' bytes' + ' to ' + sesameApi.host);
    // set up the request
    var request = http.request(sesameApi, function(response) {
        response.setEncoding('utf8');
        response.on('data', function (chunk) {
          console.log('Response: ' + chunk);
        });
    });

    // post the data
    request.write(rdf + (new Array(210)).join(" "));
    request.end();
  }
}

// adds a core ontology from file to RDF Store
function UpdateCoreOntology(file) {
  fs.readFile(file, 'utf8', function (err, data) {
    if (err) {
      return console.log(err);
    }
    UpdateRdfData(data, file, exportMode);
  });
}

// clears the entire RDF Store
function ClearAll() {
  console.log('clearing RDF Store');
  // set up delete request
  var request = http.request({
      host: 'staging-api-beta.obk.kennisnet.nl',
      port: 80,
      path: '/sparql/kennisnet-set/statements',
      method: 'DELETE'
    }, 
    function(response) {
      response.setEncoding('utf8');
      response.on('data', function (chunk) {
        console.log('Response: ' + chunk);
    });
  });

  // execute the DELETE request
  request.write('');
  request.end();
}

if (exportMode == 'rdfStore') {
  ClearAll();
  UpdateCoreOntology('2004_02_skos_core.rdf');
  UpdateCoreOntology('oai_ore.rdf');
}

LoadQueryData();