<?xml version="1.0"?>
<xsl:transform version="2.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:func="http://atomictalk.org/function"
    xmlns:saxon="http://saxon.sf.net/"
    xmlns:request="http://atomictalk.org/function/aspnet/request"
    xmlns:request-stream="clitype:System.Web.HttpRequest?partialname=System.Web"
    xmlns:clitype="http://saxon.sf.net/clitype"
    exclude-result-prefixes="xs func clitype saxon request-stream">

  <xsl:function name="request:get-content-type">
    <xsl:param name="request"/>
    <xsl:value-of select="request-stream:ContentType($request)"/>
  </xsl:function>
  
</xsl:transform>
