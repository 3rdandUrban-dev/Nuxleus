<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:param="http://xameleon.org/service/session/param" xmlns:service="http://xameleon.org/service" xmlns:request="http://atomictalk.org/function/aspnet/request" xmlns:my="http://xameleon.org/my" xmlns:response="http://atomictalk.org/function/aspnet/response" xmlns:operation="http://xameleon.org/service/operation" xmlns:profile="http://xameleon.org/service/profile" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:saxon="http://saxon.sf.net/" xmlns:clitype="http://saxon.sf.net/clitype" xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web" xmlns:aspnet-request="clitype:System.Web.HttpRequest?partialname=System.Web" xmlns:worker-request="clitype:System.Web.HttpWorkerRequest?partialname=System.Web" xmlns:headers="clitype:System.Web.HttpRequest.Headers?partialname=System.Web" xmlns:request-collection="clitype:Xameleon.Function.HttpRequestCollection?partialname=Xameleon" xmlns:queue-manager="clitype:Xameleon.Function.QueueManager?partialname=Xameleon" xmlns:name-value-collection="System.Collections.Specialized.NameValueCollection" xmlns:browser="clitype:System.Web.HttpBrowserCapabilities?partialname=System.Web" xmlns:timestamp="clitype:System.DateTime" xmlns:uri="clitype:System.Uri?partialname=System" xmlns:func="http://atomictalk.org/function" xmlns:exsl="http://exslt.org/common" xmlns:ppl="http://personplacething.info/people" exclude-result-prefixes="#all">

  <xsl:import href="../../../base.xslt" />
  <xsl:import href="../../../functions/base.xslt" />

  <xsl:param name="current-context" />

  <xsl:strip-space elements="*" />

  <xsl:template match="operation:profile">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="profile:create-new">
    <xsl:variable name="username" select="func:resolve-variable(@username)" />
    <xsl:variable name="openid" select="func:resolve-variable(@openid)" />
    <xsl:variable name="application-root" select="request:get-physical-application-path()"/>
    <xsl:variable name="member-dir-root" select="'/member/'"/>
    <xsl:variable name="member-directory" select="concat($member-dir-root, $username, '/')"/>
    <xsl:variable name="profile" select="resolve-uri(concat($application-root, $member-directory, 'profile.xml'), $application-root)" />
    <xsl:variable name="index" select="resolve-uri(concat($application-root, $member-directory, 'index.page'))" />
    <xsl:variable name="images" select="resolve-uri(concat($application-root, $member-directory, 'images/index.page'))" />
    <xsl:variable name="blog" select="resolve-uri(concat($application-root, $member-directory, 'blog/index.page'))" />
    <xsl:variable name="inbox" select="resolve-uri(concat($application-root, $member-directory, 'inbox/index.page'))" />
    <xsl:variable name="event" select="resolve-uri(concat($application-root, $member-directory, 'event/index.page'))" />
    <xsl:variable name="template">
      <xsl:apply-templates select="document(resolve-uri(concat($application-root, $member-dir-root, 'index.template')))/my:session">
        <xsl:with-param name="member-directory" select="$member-directory"/>
      </xsl:apply-templates>
    </xsl:variable>
    <xsl:variable name="event-template">
      <xsl:apply-templates select="document(resolve-uri(concat($application-root, $member-dir-root, 'event.template')))/my:session">
        <xsl:with-param name="member-directory" select="$member-directory"/>
      </xsl:apply-templates>
    </xsl:variable>
    <xsl:variable name="set-status-code" select="response:set-status-code($response, 303)"/>
    <xsl:variable name="set-location" select="response:set-location($response, $member-directory)"/>
    <redirect>
      <status-code>
        <xsl:sequence select="$set-status-code" />
      </status-code>
      <location>
        <xsl:sequence select="$set-location"/>
      </location>
    </redirect>
    <xsl:result-document method="xml " href="{$profile}">
      <profile>
        <username>
          <xsl:value-of select="$username" />
        </username>
        <openid>
          <xsl:value-of select="$openid" />
        </openid>
      </profile>
    </xsl:result-document>
    <xsl:result-document method="xml " href="{$index}">
      <xsl:copy-of select="$template"/>
    </xsl:result-document>
    <xsl:result-document method="xml " href="{$images}">
      <xsl:copy-of select="$template"/>
    </xsl:result-document>
    <xsl:result-document method="xml " href="{$blog}">
      <xsl:copy-of select="$template"/>
    </xsl:result-document>
    <xsl:result-document method="xml " href="{$inbox}">
      <xsl:copy-of select="$template"/>
    </xsl:result-document>
    <xsl:result-document method="xml " href="{$event}">
      <xsl:copy-of select="$event-template"/>
    </xsl:result-document>
  </xsl:template>

  <!-- <xsl:template match="operation:profile">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="profile:create-new">
    <xsl:variable name="username" select="func:resolve-variable(@username)" />
    <xsl:variable name="openid" select="func:resolve-variable(@openid)" />
    <xsl:variable name="application-root" select="request:get-physical-application-path()" />
    <xsl:variable name="member-dir-root" select="'/member/'" />
    <xsl:variable name="member-directory" select="concat($member-dir-root, $username, '/')" />
    <xsl:variable name="profile" select="resolve-uri(concat($application-root, $member-directory, 'profile.xml'), $application-root)" />
    <xsl:variable name="index" select="resolve-uri(concat($application-root, $member-directory, 'index.page'))" />
    <xsl:variable name="images" select="resolve-uri(concat($application-root, $member-directory, 'images/index.page'))" />
    <xsl:variable name="blog" select="resolve-uri(concat($application-root, $member-directory, 'blog/index.page'))" />
    <xsl:variable name="inbox" select="resolve-uri(concat($application-root, $member-directory, 'inbox/index.page'))" />
    <xsl:variable name="event" select="resolve-uri(concat($application-root, $member-directory, 'event/index.page'))" />
    <xsl:variable name="template">
      <xsl:apply-templates select="document(resolve-uri(concat($application-root, $member-dir-root, 'index.template')))/my:session">
        <xsl:with-param name="member-directory" select="$member-directory" />
      </xsl:apply-templates>
    </xsl:variable>
    <xsl:variable name="event-template">
      <xsl:apply-templates select="document(resolve-uri(concat($application-root, $member-dir-root, 'event.template')))/my:session">
        <xsl:with-param name="member-directory" select="$member-directory" />
      </xsl:apply-templates>
    </xsl:variable>
    <xsl:variable name="set-status-code" select="response:set-status-code($response, 303)" />
    <xsl:variable name="set-location" select="response:set-location($response, $member-directory)" />
    <redirect>
      <status-code>
        <xsl:sequence select="$set-status-code" />
      </status-code>
      <location>
        <xsl:sequence select="$set-location" />
      </location>
    </redirect>
    <xsl:result-document method="xml " href="{$profile}">
      <profile>
        <username>
          <xsl:value-of select="$username" />
        </username>
        <openid>
          <xsl:value-of select="$openid" />
        </openid>
      </profile>
    </xsl:result-document>
    <xsl:result-document method="xml " href="{$index}">
      <xsl:copy-of select="$template" />
    </xsl:result-document>
    <xsl:result-document method="xml " href="{$images}">
      <xsl:copy-of select="$template" />
    </xsl:result-document>
    <xsl:result-document method="xml " href="{$blog}">
      <xsl:copy-of select="$template" />
    </xsl:result-document>
    <xsl:result-document method="xml " href="{$inbox}">
      <xsl:copy-of select="$template" />
    </xsl:result-document>
    <xsl:result-document method="xml " href="{$event}">
      <xsl:copy-of select="$event-template" />
    </xsl:result-document>
  </xsl:template> -->

</xsl:transform>
