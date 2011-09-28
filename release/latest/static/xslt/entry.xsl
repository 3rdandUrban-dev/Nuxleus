<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:atom="http://www.w3.org/2005/Atom" xmlns:app="http://www.w3.org/2007/app"
    xmlns="http://www.w3.org/1999/xhtml" version="1.0" exclude-result-prefixes="atom app">

    <xsl:output method="xml" omit-xml-declaration="yes" indent="yes"/>

    <xsl:template match="/atom:entry">
        <div>
            <xsl:apply-templates select="atom:title" mode="entry" />
            <div class="infos"><strong>By:</strong> <xsl:apply-templates select="atom:author" mode="entry" /></div>
            <div class="infos"> <strong>Published:</strong> <xsl:value-of
                    select="substring-before(atom:published, 'T')" /> Updated: <xsl:value-of
                    select="substring-before(atom:updated, 'T')" />
            </div>
            <div>
                <p>
                <xsl:copy-of
                    select="atom:content/*[namespace-uri()='http://www.w3.org/1999/xhtml' and local-name()='div'][1]/*[namespace-uri()='http://www.w3.org/1999/xhtml' and local-name()='p']/text()"
                 /></p>
            </div>
            <div class="infos"><strong>Tags:</strong><xsl:apply-templates select="atom:category" /></div>
        </div>
    </xsl:template>

    <xsl:template match="atom:title" mode="entry">
        <h1>
            <xsl:value-of select="." />
        </h1>
    </xsl:template>

    <xsl:template match="atom:author" mode="entry">
        <xsl:value-of select="atom:name" />
    </xsl:template>

    <xsl:template match="atom:category">
        <xsl:value-of select="./@label" />, </xsl:template>


</xsl:stylesheet>
