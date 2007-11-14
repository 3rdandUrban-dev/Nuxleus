<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0" 
  xmlns:xs="http://www.w3.org/2001/XMLSchema" 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
  xmlns:clitype="http://saxon.sf.net/clitype"
  xmlns:delete="http://xameleon.org/service/atompub/delete" 
  xmlns:at="http://atomictalk.org" 
  xmlns:func="http://atomictalk.org/function" 
  xmlns:http-atompub-utils="clitype:Xameleon.Function.HttpAtompubUtils?partialname=Xameleon" 
  xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web" 
  xmlns:aspnet-request="clitype:System.Web.HttpRequest?partialname=System.Web" 
  xmlns:profile="http://xameleon.org/service/profile" 
  xmlns:atom="http://www.w3.org/2005/Atom"
  xmlns:html="http://www.w3.org/1999/xhtml" 
  xmlns:request="http://atomictalk.org/function/aspnet/request"
  xmlns:response="http://atomictalk.org/function/aspnet/response"
  xmlns:doc="http://atomictalk.org/feed/doc"
  xmlns:page="http://atomictalk.org/page"
  xmlns:service="http://atomictalk.org/page/service"
  xmlns:output="http://atomictalk.org/page/output"
  xmlns:head="http://atomictalk.org/page/head"
  xmlns:body="http://atomictalk.org/page/body"
  xmlns:advice="http://aspectxml.org/advice"
  xmlns:view="http://atomictalk.org/page/view"
  xmlns:form="http://atomictalk.org/page/view/form"
  xmlns:menu="http://atomictalk.org/page/view/menu"
  xmlns:resource="http://atomictalk.org/page/resource"
  xmlns:property="http://atomictalk.org/page/property"
  xmlns:variable="http://atomictalk.org/page/variable"
  xmlns:model="http://atomictalk.org/page/model"
  xmlns:llup="http://www.x2x2x.org/2005/LLUP" 
  xmlns:my="http://xameleon.org/my"
  xmlns:operation="http://xameleon.org/service/operation" 
  exclude-result-prefixes="#all">

  <xsl:output indent="yes" />

  <xsl:include href="../../functions/funcset-Util.xslt" />

  <xsl:param name="current-context"/>
  <xsl:param name="request"/>
  <xsl:param name="response"/>

  <xsl:function name="profile:check-session">
    <xsl:param name="key" />
    <!--<xsl:param name="guid" />-->
    <xsl:value-of select="func:resolve-variable($key)" />
  </xsl:function>

  <xsl:template match="operation:profile-add">
    <xsl:apply-templates mode="profile-copy" />
  </xsl:template>
  
    
    <xsl:template match="profile:create-new">
    <xsl:variable name="username" select="func:resolve-variable(@username)" />
    <xsl:variable name="openid" select="func:resolve-variable(@openid)" />
    <xsl:variable name="file" select="resolve-uri(concat('file:///nuxleus/Web/Development/userprofile/', $username, '/', 'profile.xml'))"/>
    <file>
    	<xsl:sequence select="$file"/>
    </file>
    <xsl:result-document method="xml " href="{$file}">
    	<profile>
		<username><xsl:value-of select="$username"/></username>
		<openid><xsl:value-of select="$openid"/></openid>
	</profile>
    </xsl:result-document> 
  </xsl:template>

  <xsl:template match="my:session" mode="profile-copy">
    <xsl:variable name="content-type" select="response:set-content-type($response, 'text/xml')" />
    <xsl:copy>
      <xsl:attribute name="content-type" select="$content-type" />
      <xsl:copy-of select="@*" />
      <xsl:apply-templates mode="profile-copy" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="node()" mode="profile-copy">
    <xsl:copy>
      <xsl:copy-of select="@*" />
      <xsl:apply-templates mode="profile-copy" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="html:input[@type='text']" mode="profile-copy">
    <xsl:variable name="input-value" select="concat('{form:', ./@name, '}')" />
    <xsl:copy>
      <xsl:copy-of select="@*" />
    </xsl:copy>

    <!--
        This should be where we check the session value
    -->
    <xsl:if test="profile:check-session($input-value) = 'not-set'">
      <html:span>*</html:span>
    </xsl:if>
  </xsl:template>

</xsl:transform>
