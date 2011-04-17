<?xml version="1.0" encoding="UTF-8"?>
<!--
  COPYRIGHT: (c) 2007, M. David Peterson (mailto:m.david@xmlhacker.com; http://mdavid.name/)
  LICENSE: The code contained in this file is licensed under The New BSD License. Please see
  http://www.opensource.org/licenses/bsd-license.php for specific detail.
-->
<xsl:stylesheet version="1.0" 
  xmlns:html="http://www.w3.org/1999/xhtml" 
  xmlns:doc="http://atomictalk.org/feed/doc" 
  xmlns:app="http://purl.org/atom/app#" 
  xmlns:atompub="http://www.w3.org/2007/app" 
  xmlns:atom="http://www.w3.org/2005/Atom" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
  exclude-result-prefixes="html app atom doc atompub">

  <xsl:template match="atom:feed">
    <xsl:apply-templates select="atom:entry"/>
  </xsl:template>

  <xsl:template match="atom:entry">
    <xsl:param name="cCount"/>
    <xsl:apply-templates select="atom:*">
      <xsl:with-param name="cCount" select="$cCount"/>
    </xsl:apply-templates>
  </xsl:template>

  <xsl:template match="atom:title">
    <h4>
      <a href="{../atom:link[@rel = 'self']/@href}">
        <xsl:value-of select="."/>
      </a>
    </h4>
  </xsl:template>
  <xsl:template match="atom:summary"/>
  <xsl:template match="atom:published"/>
  <xsl:template match="atom:updated"/>
  <xsl:template match="atom:generator"/>
  <xsl:template match="atom:id"/>
  <xsl:template match="atom:category"/>
  <xsl:template match="atom:source"/>
  <xsl:template match="atom:author"/>
  <xsl:template match="atom:content">
    <xsl:param name="cCount"/>
    <p style="font-size:small">
      <xsl:copy-of select="substring(., 1, $cCount)"/> ... [<a href="{../atom:link[@rel = 'self']/@href}">more</a>]
    </p>
  </xsl:template>

</xsl:stylesheet>
