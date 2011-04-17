<?xml version="1.0"?>
<xsl:transform version="2.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:func="http://atomictalk.org/function"
    xmlns:saxon="http://saxon.sf.net/"
    xmlns:server="http://atomictalk.org/function/aspnet/server"
    xmlns:aspnet-server="clitype:System.Web.HttpServerUtility?partialname=System.Web"
    xmlns:clitype="http://saxon.sf.net/clitype"
    exclude-result-prefixes="xs func clitype saxon aspnet-server">

  <xsl:function name="server:placeholder">
    <xsl:param name="request"/>
  </xsl:function>
  
</xsl:transform>
