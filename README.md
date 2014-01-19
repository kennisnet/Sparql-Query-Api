SPARQL-Query-API
================

The SPARQL Query API is a web application to administer and publish SPARQL queries. The API hides the complexity of SPARQL queries from the API consumer and simply exposes a list of named queries. Furthermore when (minor) changes in the underlying data model are made, the named queries can hide these changes and still produce the same output. The consuming applications can use the named queries like any other json or xml API without any knowledge of the SPARQL query language. 

===Administration===
Queries can be edited in a simple administration application and you can also define parameters that will be injected in the SPARQL query at execution time. For each query you can add documentation on how to use it and which parameters are available for the query.  
Access to the API can be limited by granting access to specific queries for an account using a API key. 

You can configure multiple SPARQL endpoints, including username and password, timeout and prefixes. Each query will have an associated endpoint. 

===Logging===
The API logs the usage of all queries, so you have clear insight of the usage and performance of your queries.
