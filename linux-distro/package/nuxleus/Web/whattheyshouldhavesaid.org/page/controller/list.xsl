<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:atom="http://www.w3.org/2005/Atom"
    xmlns:html="http://www.w3.org/1999/xhtml" xmlns="http://www.w3.org/1999/xhtml" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
    xmlns:xsd="http://www.w3.org/2001/XMLSchema" exclude-result-prefixes="atom html xsi xsd" version="1.0">
    <xsl:template match="/">
        <xsl:apply-templates select="//atom:entry/atom:content" />
    </xsl:template>
    <xsl:template match="atom:content">
        <xsl:copy-of select="*" />
    </xsl:template>
    <xsl:template match="text()" />
</xsl:stylesheet>
