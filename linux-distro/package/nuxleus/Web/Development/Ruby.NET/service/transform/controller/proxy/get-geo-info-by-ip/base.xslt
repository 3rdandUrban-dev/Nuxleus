<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0" xmlns="http://www.w3.org/2005/Atom" xmlns:atom="http://www.w3.org/2005/Atom" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:saxon="http://saxon.sf.net/" xmlns:clitype="http://saxon.sf.net/clitype" xmlns:at="http://atomictalk.org" xmlns:func="http://atomictalk.org/function" xmlns:request="http://atomictalk.org/function/aspnet/request" xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web" xmlns:atomv03="http://purl.org/atom/ns#" xmlns:proxy="http://xameleon.org/service/proxy" xmlns:html="http://www.w3.org/1999/xhtml" exclude-result-prefixes="#all">

  <xsl:import href="../../../functions/base.xslt" />
  <xsl:param name="current-context" />

  <xsl:template match="proxy:get-geo-info-by-ip">
    <xsl:variable name="uri" select="func:resolve-variable(@uri)" />
    <!-- <xsl:variable name="return" select="func:resolve-variable(@return)" /> -->
    <xsl:variable name="ip" select="request:get-user-ip($request)"/>
    <xsl:variable name="request" select="concat($uri, $ip)"/>
    <xsl:copy-of select="document($request)" />
  </xsl:template>

</xsl:transform>
