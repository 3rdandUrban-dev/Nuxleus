<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0" xmlns="http://www.w3.org/2005/Atom" xmlns:operation="http://xameleon.org/service/operation" xmlns:search="http://xameleon.org/service/search" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:saxon="http://saxon.sf.net/" xmlns:clitype="http://saxon.sf.net/clitype" xmlns:at="http://atomictalk.org" xmlns:func="http://atomictalk.org/function" xmlns:request="http://atomictalk.org/function/aspnet/request" xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web" xmlns:atomv03="http://purl.org/atom/ns#" xmlns:proxy="http://xameleon.org/service/proxy" xmlns:html="http://www.w3.org/1999/xhtml" exclude-result-prefixes="#all">

  <xsl:import href="../../../base.xslt" />
  <xsl:param name="current-context" />
  
  <xsl:template match="operation:search">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="search:return-artist-info-from-music-brainz">
    <xsl:variable name="uri" select="'http://musicbrainz.org/ws/1/artist/?type=xml&amp;name='"/>
    <xsl:variable name="artist" select="func:resolve-variable(@artist)"/>
    <xsl:variable name="rest-query" select="concat($uri, $artist)"/>
    <xsl:sequence select="document($rest-query)" />
  </xsl:template>

</xsl:transform>
