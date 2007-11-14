<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0" 
  xmlns:xs="http://www.w3.org/2001/XMLSchema" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
  xmlns:fn="http://www.w3.org/2005/xpath-functions" 
  xmlns:saxon="http://saxon.sf.net/" 
  xmlns:clitype="http://saxon.sf.net/clitype" 
  xmlns:at="http://atomictalk.org" 
  xmlns:func="http://atomictalk.org/function" 
  xmlns:session="http://xameleon.org/service/session" 
  xmlns:guid="clitype:System.Guid?partialname=mscorlib" 
  xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web" 
  xmlns:proxy="http://xameleon.org/service/proxy" 
  xmlns:html="http://www.w3.org/1999/xhtml" 
  xmlns:operation="http://xameleon.org/service/operation" 
  exclude-result-prefixes="#all">

  <xsl:import href="../../../functions/funcset-Util.xslt" />
  <xsl:param name="current-context" />

  <xsl:template match="session:validate-request">
    <xsl:variable name="session-id" select="func:resolve-variable(@key)" />
    <xsl:variable name="openid" select="func:resolve-variable(@openid)" />
    <xsl:apply-templates>
      <xsl:with-param name="session-id" select="$session-id"/>
      <xsl:with-param name="openid" select="$openid"/>
    </xsl:apply-templates>
  </xsl:template>

  <xsl:template match="operation:return-xml">
  <xsl:param name="session-id"/>
  <xsl:param name="openid"/>
    <session session-id="{$session-id}" openid="{replace($openid, '%2F', '/')}">
      <xsl:apply-templates />
    </session>
  </xsl:template>

  <xsl:template match="session:generate-guid">
    <request-guid>
      <xsl:value-of select="string(guid:NewGuid())"/>
    </request-guid>
  </xsl:template>

</xsl:transform>
