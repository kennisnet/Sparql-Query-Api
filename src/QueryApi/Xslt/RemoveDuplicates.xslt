<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
								xmlns:sparql="http://www.w3.org/2005/sparql-results#"
								exclude-result-prefixes="msxsl"
>
    <xsl:output method="xml" indent="yes"/>

		<xsl:key name="uri" match="sparql:binding[@name = 'b']" use="sparql:uri"/>
	
    <xsl:template match="@* | node()">
			<xsl:variable name="current" select="." />
			<xsl:if test="not(node()) or 
							not(
								preceding-sibling::node()[
									./sparql:binding[@name = 'b']/sparql:uri = $current/sparql:binding[@name = 'b']/sparql:uri 
									and 
							    ./sparql:binding[@name = 'l']/sparql:literal = $current/sparql:binding[@name = 'l']/sparql:literal 
									and (
										./sparql:binding[@name = 'parent']/sparql:uri = $current/sparql:binding[@name = 'parent']/sparql:uri 
										or not(./sparql:binding[@name = 'parent']/sparql:uri) 
										or not(key('uri', $current/sparql:binding[@name = 'parent']/sparql:uri)) 
									)
								]
							)"> 
				<xsl:copy> 
					<xsl:apply-templates select="@*|node()"/> 
				</xsl:copy> 
			</xsl:if> 
    </xsl:template>
</xsl:stylesheet>
