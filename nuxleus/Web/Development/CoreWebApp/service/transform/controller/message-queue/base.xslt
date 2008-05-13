<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform  xmlns="http://nuxleus.com/message/response" xmlns:lookup="http://nuxleus.com/message/response/lookup" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:saxon="http://saxon.sf.net/" xmlns:clitype="http://saxon.sf.net/clitype" xmlns:at="http://atomictalk.org" xmlns:func="http://atomictalk.org/function" xmlns:queue-manager="clitype:Xameleon.Function.QueueManager?partialname=Xameleon" xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web" xmlns:queue="http://xameleon.org/service/queue" xmlns:guid="clitype:System.Guid?partialname=mscorlib" xmlns:html="http://www.w3.org/1999/xhtml" version="2.0" exclude-result-prefixes="#all">

  <xsl:import href="../../base.xslt" />

  <xsl:param name="current-context"/>
  
  <xsl:output method="xml" indent="no" />
  
  <xsl:variable name="response-output">
    <lookup:response-code-lookup>
      <lookup:message response-code="message sent">
        <lookup:human-readable>Message was sent successfully!</lookup:human-readable>
      </lookup:message>
      <lookup:message response-code="error">
        <lookup:human-readable>There was an error sending your message.</lookup:human-readable>
      </lookup:message>
    </lookup:response-code-lookup>
  </xsl:variable>

  <xsl:template match="queue:push">
    <xsl:variable name="message" select="@message"/>
    <xsl:variable name="message-queue" select="@message-queue"/>
    <xsl:sequence select="func:queue-push($current-context, $message-queue, $message)"/>
  </xsl:template>

  <xsl:function name="func:queue-push">
    <xsl:param name="current-context" />
    <xsl:param name="message-queue" as="xs:string"/>
    <xsl:param name="message" as="xs:string"/>
    <xsl:variable name="guid" select="string(guid:NewGuid())"/>
    <xsl:variable name="response">
      <xsl:sequence select="queue-manager:Push($current-context, $message-queue, concat($guid, ':', $message))"/>
    </xsl:variable>
    <response>
      <xsl:apply-templates select="$response" mode="generate-human-readable-response">
        <xsl:with-param name="message-id" select="$guid"/>
      </xsl:apply-templates>
    </response>
  </xsl:function>

  <xsl:template match="text()" mode="generate-human-readable-response">
    <xsl:param name="message-id"/>
    <xsl:variable name="message" select="$response-output//lookup:message[@response-code = current()]"/>
    <message response-code="{.}" message-id="{$message-id}">
      <xsl:value-of select="$message/lookup:human-readable"/>
    </message>
  </xsl:template>

</xsl:transform>
