<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:fn="http://www.w3.org/2005/xpath-functions"
    xmlns:saxon="http://saxon.sf.net/" xmlns:clitype="http://saxon.sf.net/clitype" xmlns:at="http://atomictalk.org"
    xmlns:func="http://atomictalk.org/function" xmlns:aspnet="http://atomictalk.org/function/aspnet"
    xmlns:response="http://atomictalk.org/function/aspnet/response" xmlns:service="http://xameleon.org/service"
    xmlns:operation="http://xameleon.org/service/operation" xmlns:proxy="http://xameleon.org/service/proxy"
    xmlns:session="http://xameleon.org/service/session" xmlns:param="http://xameleon.org/service/session/param"
    xmlns:aws="http://xameleon.org/function/aws" xmlns:s3="http://xameleon.org/function/aws/s3"
    xmlns:header="http://xameleon.org/service/http/header" xmlns:metadata="http://xameleon.org/service/metadata"
    xmlns:aspnet-timestamp="clitype:System.DateTime" xmlns:sortedlist="clitype:System.Collections.SortedList"
    xmlns:aspnet-session="clitype:System.Web.SessionState.HttpSessionState?partialname=System.Web"
    xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web"
    xmlns:browser="clitype:System.Web.HttpBrowserCapabilities?partialname=System.Web"
    xmlns:aspnet-server="clitype:System.Web.HttpServerUtility?partialname=System.Web"
    xmlns:aspnet-request="clitype:System.Web.HttpRequest?partialname=System.Web"
    xmlns:aspnet-response="clitype:System.Web.HttpResponse?partialname=System.Web"
    xmlns:current-context="clitype:Xameleon.Function.GetHttpContext?partialname=Xameleon" xmlns:html="http://www.w3.org/1999/xhtml"
    exclude-result-prefixes="#all">

    <xsl:import href="./atomicxml/base.xslt" />
    <xsl:import href="./atompub/base.xslt" />
    <xsl:import href="./aws/s3/base.xslt" />
    <xsl:import href="./proxy/base.xslt" />
    <xsl:import href="./atom/base.xslt" />
    <xsl:import href="./session/validate-request/base.xslt" />
    <xsl:import href="./message-queue/base.xslt" />
    <xsl:import href="../model/json-to-xml.xslt" />
    <xsl:import href="./test/base.xslt" />
    <xsl:import href="./profile/base.xslt" />
    <xsl:import href="./redirect/base.xslt" />

    <xsl:param name="current-context" />
    <xsl:param name="response" />
    <xsl:param name="request" />
    <xsl:param name="server" />
    <xsl:param name="session" />
    <xsl:param name="timestamp" />
    <xsl:param name="debug" />
    <xsl:variable name="q">"</xsl:variable>

    <xsl:template match="header:*">
        <xsl:param name="sorted-list" as="clitype:System.Collections.SortedList" />
        <xsl:variable name="key" select="local-name() cast as xs:untypedAtomic" />
        <xsl:variable name="value" select=". cast as xs:untypedAtomic" />
        <xsl:sequence select="sortedlist:Add($sorted-list, $key, $value)" />
    </xsl:template>

    <xsl:function name="func:eval-params" as="element()+">
        <xsl:param name="params" />
        <xsl:apply-templates select="$params" mode="evalparam" />
    </xsl:function>

    <xsl:template match="param:*" mode="evalparam">
        <xsl:element name="{local-name()}">
            <xsl:value-of select="func:resolve-variable(.)" />
        </xsl:element>
    </xsl:template>

    <xsl:template match="service:operation">
        <xsl:param name="key-name" />
        <xsl:variable name="issecure" select="false()" as="xs:boolean" />
        <xsl:variable name="content-type"
            select="if ($debug) then response:set-content-type($response, 'text/plain') else response:set-content-type($response, 'text/xml')" />
        <xsl:if test="@use-clientside-xslt">
            <xsl:processing-instruction name="xml-stylesheet">
                <xsl:value-of select="concat('type=', $q, 'text/xsl', $q, ' ', 'href=', $q, @use-clientside-xslt, $q)" />
            </xsl:processing-instruction>
        </xsl:if>
        <xsl:choose>
            <xsl:when test="./@exclude-message-envelope">
                <!-- We need to make sure that the content type is really set
             later since the content-type variable above it never used -->
                <xsl:apply-templates>
                    <xsl:with-param name="content-type" select="'text/xml'" />
                </xsl:apply-templates>
            </xsl:when>
            <xsl:otherwise>
                <message type="service:result"
                    content-type="{if (empty($content-type)) 
                                                     then response:get-content-type($response) 
                                                     else 'not-set'}">
                    <xsl:apply-templates />
                </message>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>

</xsl:transform>
