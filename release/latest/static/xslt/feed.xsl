<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:atom="http://www.w3.org/2005/Atom" xmlns:app="http://www.w3.org/2007/app"
    xmlns="http://www.w3.org/1999/xhtml" version="1.0" exclude-result-prefixes="atom app">

    <xsl:output method="xml" omit-xml-declaration="yes" indent="yes" />

    <xsl:param name="tag" />
    <xsl:template match="/atom:feed">
        <xsl:choose>
            <xsl:when test="$tag">
                <div class="entry">
                <div id="gmap" />
                <script type="text/javascript"> $('document').ready(function(){ 
                        
                        geo.show_points('<xsl:value-of select="/atom:feed/@xml:base" />tag/<xsl:value-of select="$tag" />/feed'); 
                   }); 
                </script>
                </div>
            </xsl:when>
        </xsl:choose>
        <xsl:apply-templates select="atom:entry" />

    </xsl:template>

    <xsl:template match="atom:entry">
        <xsl:variable name="entry_id" select="generate-id(atom:id)" />
        <div id="{$entry_id}" class="entry">
            <xsl:apply-templates select="atom:title" mode="entry" />
            <div class="infos">
                <strong>By:</strong>
                <xsl:apply-templates select="atom:author" mode="entry" />
            </div>
            <div class="infos">
                <span class="published">
                    <strong>Published:</strong>
                    <xsl:value-of select="substring-before(atom:published/text(), 'T')" />
                </span> - <span class="updated">
                    <strong>Updated:</strong>
                    <xsl:value-of select="substring-before(atom:updated/text(), 'T')" />
                </span>
            </div>
            <div id="{$entry_id}-content">
                <p>
                    <xsl:copy-of
                        select="atom:content/*[namespace-uri()='http://www.w3.org/1999/xhtml' and local-name()='div'][1]/*[namespace-uri()='http://www.w3.org/1999/xhtml' and local-name()='p']/node()"
                     />
                </p>
            </div>
            <div class="infos">
                <strong>Tags:</strong>
                <xsl:apply-templates select="atom:category" />
            </div>
            <div class="infos">
                <a href="{/atom:feed/@xml:base}{./atom:link[@rel='alternate']/@href}">Permalink</a>
            </div>
        </div>
        <br />
    </xsl:template>

    <xsl:template match="atom:title" mode="entry">
        <h1>
            <xsl:value-of select="text()" />
        </h1>
    </xsl:template>

    <xsl:template match="atom:author" mode="entry">
        <xsl:value-of select="atom:name/text()" />
    </xsl:template>

    <xsl:template match="atom:category">
        <a href="{/atom:feed/@xml:base}tag/{./@term}">
            <xsl:value-of select="./@label" />
        </a>, </xsl:template>

</xsl:stylesheet>
