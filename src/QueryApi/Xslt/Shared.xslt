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
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
	
	<xsl:variable name="bkPrefix" select="name(//namespace::*[. = 'http://purl.edustandaard.nl/begrippenkader/'])" />
	
	<xsl:template name="StripNamespace">
		<xsl:param name="uri" />
		<!--<xsl:variable name="x" select="document(.)" />
		<xsl:for-each select="$x/@*">
			<xsl:choose>
				<xsl:when test="starts-with($uri, .)">bk:<xsl:value-of select="substring($uri, string-length(.) + 1)" /></xsl:when>
			</xsl:choose>
		</xsl:for-each>-->
		<xsl:choose>
			<xsl:when test="starts-with($uri, 'http://purl.edustandaard.nl/begrippenkader/')">bk:<xsl:value-of select="substring($uri, string-length('http://purl.edustandaard.nl/begrippenkader/') + 1)" /></xsl:when>
			<xsl:when test="starts-with($uri, 'http://www.w3.org/2004/02/skos/core#')">skos:<xsl:value-of select="substring($uri, string-length('http://www.w3.org/2004/02/skos/core#') + 1)" /></xsl:when>
			<xsl:when test="starts-with($uri, 'http://www.w3.org/1999/02/22-rdf-syntax-ns#')">rdf:<xsl:value-of select="substring($uri, string-length('http://www.w3.org/1999/02/22-rdf-syntax-ns#') + 1)" /></xsl:when>
			<xsl:when test="starts-with($uri, 'http://www.w3.org/2000/01/rdf-schema#')">rdfs:<xsl:value-of select="substring($uri, string-length('http://www.w3.org/2000/01/rdf-schema#') + 1)" /></xsl:when>
			<xsl:when test="starts-with($uri, 'http://www.rnaproject.org/data/rnax/')">rnax:<xsl:value-of select="substring($uri, string-length('http://www.rnaproject.org/data/rnax/') + 1)" /></xsl:when>
			<xsl:otherwise><xsl:value-of select="$uri"/></xsl:otherwise>
		</xsl:choose>
	</xsl:template>
		
</xsl:stylesheet>
