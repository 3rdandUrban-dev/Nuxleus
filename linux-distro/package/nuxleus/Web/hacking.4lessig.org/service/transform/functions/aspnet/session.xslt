<?xml version="1.0"?>
<xsl:transform version="2.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:func="http://atomictalk.org/function"
    xmlns:saxon="http://saxon.sf.net/"
    xmlns:session="http://atomictalk.org/function/aspnet/session"
    xmlns:aspnet-session="clitype:System.Web.SessionState.HttpSessionState?partialname=System.Web"
    xmlns:clitype="http://saxon.sf.net/clitype"
    exclude-result-prefixes="xs func clitype saxon aspnet-session">

  <xsl:function name="session:placholder">
    <xsl:param name="request"/>
  </xsl:function>
  
</xsl:transform>
