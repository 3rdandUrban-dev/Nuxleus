<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:saxon="http://saxon.sf.net/" xmlns:clitype="http://saxon.sf.net/clitype" xmlns:at="http://atomictalk.org" xmlns:func="http://atomictalk.org/function" xmlns:queue-manager="clitype:Xameleon.Function.QueueManager?partialname=Xameleon" xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web" xmlns:queue="http://xameleon.org/service/queue" xmlns:html="http://www.w3.org/1999/xhtml" version="2.0" exclude-result-prefixes="xs xsl xsi fn clitype at func queue-manager queue aspnet-context saxon html">

  <xsl:import href="../../base.xslt" />
  
  <xsl:param name="current-context"/>

  <xsl:template match="queue:push">
    <xsl:variable name="message" select="@message"/>
    <xsl:variable name="message-queue" select="@message-queue"/>
    <xsl:sequence select="func:queue-push($current-context, $message-queue, $message)"/>
  </xsl:template>

  <xsl:function name="func:queue-push">
    <xsl:param name="current-context" />
    <xsl:param name="message-queue" as="xs:string"/>
    <xsl:param name="message" as="xs:string"/>
    <response>
      <xsl:sequence select="queue-manager:Push($current-context, $message-queue, $message)"/>
    </response>
  </xsl:function>

</xsl:transform>
