<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0" 
  xmlns:xs="http://www.w3.org/2001/XMLSchema" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
  xmlns:fn="http://www.w3.org/2005/xpath-functions" 
  xmlns:saxon="http://saxon.sf.net/" 
  xmlns:clitype="http://saxon.sf.net/clitype"
  xmlns:add="http://xameleon.org/service/atompub/add" 
  xmlns:at="http://atomictalk.org" 
  xmlns:func="http://atomictalk.org/function" 
  xmlns:http-atompub-utils="clitype:Xameleon.Function.HttpAtompubUtils?partialname=Xameleon" 
  xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web" 
  xmlns:aspnet-request="clitype:System.Web.HttpRequest?partialname=System.Web" 
  xmlns:atompub="http://xameleon.org/service/atompub" 
  xmlns:atom="http://www.w3.org/2005/Atom"
  xmlns:html="http://www.w3.org/1999/xhtml" 
  exclude-result-prefixes="xs xsl xsi fn clitype at func http-atompub-utils aspnet-context atompub saxon html">

  <xsl:import href="../../functions/funcset-Util.xslt"/>
  <xsl:import href="./add/base.xslt"/>
  <xsl:import href="./update/base.xslt"/>
  <xsl:import href="./delete/base.xslt"/>
  
</xsl:transform>
