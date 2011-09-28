<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:saxon="http://saxon.sf.net/" xmlns:clitype="http://saxon.sf.net/clitype" xmlns:at="http://atomictalk.org" xmlns:func="http://atomictalk.org/function" xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web" xmlns:operation="http://xameleon.org/service/operation" xmlns:echo="http://xameleon.org/service/echo" xmlns:html="http://www.w3.org/1999/xhtml" xmlns:request="http://atomictalk.org/function/aspnet/request" version="2.0" exclude-result-prefixes="#all">

  <xsl:template match="operation:echo">
    <echo>
      <xsl:apply-templates />
    </echo>
  </xsl:template>

  <xsl:template match="echo:return-query-string">
    <query-string>
      <xsl:value-of select="request:get-query-string()"/>
    </query-string>
  </xsl:template>

  <xsl:template match="echo:return-user-agent">
    <user-agent>
      <xsl:value-of select="request:get-user-agent()"/>
    </user-agent>
  </xsl:template>

  <xsl:template match="echo:return-http-method">
    <http-method>
      <xsl:value-of select="request:get-http-method()"/>
    </http-method>
  </xsl:template>

</xsl:transform>
