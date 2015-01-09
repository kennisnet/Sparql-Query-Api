var http = require('http'),
    fs = require('fs');

var queryList = [
  'stagingbkt'
];

var index = 0;

var queryApi = {
        host: '85.214.207.241',
        port: 8080,
        // host: 'localhost',
        // port: 49975,
        path: '',
        method: 'GET',
        headers: {
            accept: 'text/rdf+n3'
        }
    };

var sesameApi = {
        host: 'staging-api-beta.obk.kennisnet.nl',
        port: 80,
        path: '/sparql/stagingbkt/statements',
        method: 'POST',
        headers: {
            'Content-Type': 'application/rdf+xml',
            'Content-Length': 0
        }
    };


function LoadQueryData() {
  queryApi.path = "/openrdf-sesame/repositories/" + queryList[index] + "/statements";
	console.log('getting data for: ' + queryList[index] + ' from ' + queryApi.host);
  var req = http.get(queryApi, function(res) {
	  var xml = '';
    var counter = 0
	  res.on('data', function(chunk) {
	    xml += chunk;
      counter++;
      if (counter % 10 == 0) {
        console.log(Math.round(xml.length / 1024) + " kb");
      }
	  });

	  res.on('end', function() {
      console.log('updating rdf for: ' + queryList[index]);
	    //console.log(xml);
      //UpdateRdfData(xml);
 		  console.log('writing file data for: ' + queryApi.path);
      fs.writeFile(queryList[index] + '.rdf', xml, function (err) {
	  	  if (err) return console.log(err);
		  });
      index++;
      if (queryList.length > index) {
        LoadQueryData();
      }
	  });

	  // or you can pipe the data to a parser
	  //res.pipe(dest);
	});

	req.on('error', function(err) {
	  // debug error
	});	
}


function UpdateRdfData(rdf) {
  sesameApi.headers['Content-Length'] = rdf.length + 200;
  console.log('posting rdf data ' + rdf.length + ' bytes' + ' to ' + sesameApi.host);
  // Set up the request
  var post_req = http.request(sesameApi, function(res) {
      res.setEncoding('utf8');
      res.on('data', function (chunk) {
          console.log('Response: ' + chunk);
      });
  });

  // post the data
  post_req.write(rdf + (new Array(210)).join(" "));
  post_req.end();
}

function UpdateCoreOntology(file) {
  fs.readFile(file, 'utf8', function (err,data) {
    if (err) {
      return console.log(err);
    }
    UpdateRdfData(data);
  });
}

function ClearAll() {
  console.log('clearing RDF Store');
  var del_req = http.request({
        host: 'staging-api-beta.obk.kennisnet.nl',
        port: 80,
        path: '/sparql/kennisnet-set/statements',
        method: 'DELETE'
      }, 
      function(res) {
      res.setEncoding('utf8');
      res.on('data', function (chunk) {
          console.log('Response: ' + chunk);
      });
  });

  // post the data
  del_req.write('');
  del_req.end();
}

//ClearAll();
//UpdateCoreOntology('2004_02_skos_core.rdf');
//UpdateCoreOntology('oai_ore.rdf');

LoadQueryData();