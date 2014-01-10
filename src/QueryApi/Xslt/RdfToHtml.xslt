<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:dc="http://purl.org/dc/elements/1.1/"
	xmlns:rdfs="http://www.w3.org/2000/01/rdf-schema#"
	xmlns:psys="http://proton.semanticweb.org/protonsys#"
	xmlns:owl="http://www.w3.org/2002/07/owl#"
	xmlns:xsd="http://www.w3.org/2001/XMLSchema#"
	xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"
	xmlns:pext="http://proton.semanticweb.org/protonext#"
	xmlns:bk="http://purl.edustandaard.nl/begrippenkader/"
	xmlns:skos="http://www.w3.org/2004/02/skos/core#"
	xmlns:rnax="http://www.rnaproject.org/data/rnax/"
	xmlns:sparql="http://www.w3.org/2005/sparql-results#"
	xmlns:json="http://james.newtonking.com/projects/json"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>

	<xsl:include href="Shared.xslt"/>
	
	<xsl:output method="xml" version="1.0" omit-xml-declaration="yes" doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN" doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd" />
	
	<!--xsl:output method="xml" version="1.0" indent="no" omit-xml-declaration="yes" encoding="windows-1252" doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN" doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd" />
		-->
	
	<xsl:variable name="results" select="sparql:sparql/sparql:results" />
	<xsl:variable name="cols" select="sparql:sparql/sparql:head/sparql:variable" />

	<xsl:template match="/">
		<html>
			<head>
				<style>
					body, td {
					font-family: Verdana;
					font-size: 12px;
					}

					table {
					border-spacing: 0;

					}

					thead tr {
					background-color: #ddd;
					}

					th {
					background-color: #ddd;
					text-align: left;
					padding: 6px 12px 6px 6px;
					border-bottom: solid 2px #999;
					}

					td {
					background-color: #eee;
					padding: 6px 12px 6px 6px;
					border-bottom: solid 1px #ccc;
					}

				</style>
			</head>
			<body>
				<table>
					<thead>
						<tr>
							<xsl:for-each select="$cols">
								<th>
									<xsl:value-of select="@name"/>
								</th>
							</xsl:for-each>
						</tr>
					</thead>
					<tbody>
						<xsl:for-each select="$results/sparql:result">
							<xsl:variable name="thisResult" select="."/>
							<tr>
								<xsl:for-each select="$cols">
									<xsl:variable name="thisCol" select="."/>
									<xsl:variable name="binding" select="$thisResult/sparql:binding[@name = $thisCol/@name]"/>
									<td>
										<xsl:value-of select="$binding/sparql:literal"/>
										<xsl:call-template name="StripNamespace">
											<xsl:with-param name="uri" select="$binding/sparql:uri"/>
										</xsl:call-template>
									</td>
								</xsl:for-each>
							</tr>
						</xsl:for-each>
					</tbody>
				</table>	
			</body>
		</html>
	</xsl:template>
	
</xsl:stylesheet>
