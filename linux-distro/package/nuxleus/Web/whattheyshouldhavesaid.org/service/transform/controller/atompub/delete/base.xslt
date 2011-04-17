<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0" 
  xmlns:xs="http://www.w3.org/2001/XMLSchema" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
  xmlns:fn="http://www.w3.org/2005/xpath-functions" 
  xmlns:saxon="http://saxon.sf.net/" 
  xmlns:clitype="http://saxon.sf.net/clitype"
  xmlns:delete="http://xameleon.org/service/atompub/delete" 
  xmlns:at="http://atomictalk.org" 
  xmlns:func="http://atomictalk.org/function" 
  xmlns:http-atompub-utils="clitype:Xameleon.Function.HttpAtompubUtils?partialname=Xameleon" 
  xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web" 
  xmlns:aspnet-request="clitype:System.Web.HttpRequest?partialname=System.Web" 
  xmlns:atompub="http://xameleon.org/service/atompub" 
  xmlns:atom="http://www.w3.org/2005/Atom"
  xmlns:html="http://www.w3.org/1999/xhtml" 
  exclude-result-prefixes="xs xsl xsi fn clitype at func http-atompub-utils aspnet-context atompub saxon html">

  <xsl:template match="atompub:delete">
    <xsl:apply-templates>
      <xsl:with-param name="content-type" select="func:resolve-variable(@content-type)"/>
      <xsl:with-param name="content-length" select="func:resolve-variable(@content-length)"/>
    </xsl:apply-templates>
  </xsl:template>
  
  <xsl:template match="delete:entry">
    <xsl:param name="content-type"/>
    <xsl:param name="content-length"/>
    <xsl:variable name="title" select="func:resolve-variable(@title)"/>
    <xsl:variable name="slug" select="func:resolve-variable(@slug)"/>
    <atom:entry>
      <atom:title><xsl:value-of select="$title"/></atom:title>
      <atom:link rel="self" href="{$slug}"/>
      <atom:content type="{$content-type}" length="{$content-length}">
        <xsl:apply-templates />
      </atom:content>
    </atom:entry>
  </xsl:template>

  <xsl:function name="func:delete">
    <xsl:value-of select="concat('entry updated', ' content type=', $content-type)"/>
  </xsl:function>

</xsl:transform>
