<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:param="http://xameleon.org/service/session/param"
    xmlns:service="http://xameleon.org/service" xmlns:request="http://atomictalk.org/function/aspnet/request"
    xmlns:my="http://xameleon.org/my" xmlns:response="http://atomictalk.org/function/aspnet/response"
    xmlns:operation="http://xameleon.org/service/operation" xmlns:profile="http://xameleon.org/service/profile"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:saxon="http://saxon.sf.net/" xmlns:clitype="http://saxon.sf.net/clitype"
    xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web"
    xmlns:aspnet-request="clitype:System.Web.HttpRequest?partialname=System.Web"
    xmlns:worker-request="clitype:System.Web.HttpWorkerRequest?partialname=System.Web"
    xmlns:headers="clitype:System.Web.HttpRequest.Headers?partialname=System.Web"
    xmlns:request-collection="clitype:Xameleon.Function.HttpRequestCollection?partialname=Xameleon"
    xmlns:queue-manager="clitype:Xameleon.Function.QueueManager?partialname=Xameleon"
    xmlns:name-value-collection="System.Collections.Specialized.NameValueCollection"
    xmlns:browser="clitype:System.Web.HttpBrowserCapabilities?partialname=System.Web" xmlns:timestamp="clitype:System.DateTime"
    xmlns:uri="clitype:System.Uri?partialname=System" xmlns:func="http://atomictalk.org/function" xmlns:exsl="http://exslt.org/common"
    xmlns:ppl="http://personplacething.info/people" exclude-result-prefixes="#all">

    <xsl:import href="../functions/funcset-Util.xslt" />
    <xsl:import href="../functions/funcset-dateTime.xslt" />
    <xsl:import href="../functions/aspnet/request-stream.xslt" />
    <xsl:import href="../functions/aspnet/response-stream.xslt" />

    <xsl:param name="current-context" />
    <xsl:param name="response" select="aspnet-context:Response($current-context)" />
    <xsl:param name="request" select="aspnet-context:Request($current-context)" />
    <xsl:param name="server" select="aspnet-context:Server($current-context)" />
    <xsl:param name="session" select="aspnet-context:Session($current-context)" />
    <xsl:param name="timestamp" select="aspnet-context:Timestamp($current-context)" />
    <xsl:param name="aws-config" select="document('../aws.config')/aws" />
    <xsl:param name="aws-default-key-pair-name" select="'primary'" />
    <xsl:param name="aws-default-key-pair-set" select="$aws-config/key[@friendly-name = $aws-default-key-pair-name]" />
    <xsl:param name="aws-public-key" select="$aws-default-key-pair-set/public" />
    <xsl:param name="aws-private-key" select="$aws-default-key-pair-set/private" />
    <xsl:variable name="debug"
        select="if (request-collection:GetValue($request, 'query-string', 'debug') = 'true') then true() else false()" as="xs:boolean" />
    <xsl:variable name="request-uri" select="aspnet-request:Url($request)" />
    <xsl:variable name="browser" select="aspnet-request:Browser($request)" />
    <xsl:variable name="session-params" select="func:eval-params(/service:operation/param:*)" />
    <xsl:variable name="q">"</xsl:variable>
    <xsl:variable name="sq">'</xsl:variable>


    <xsl:strip-space elements="*" />

    <xsl:character-map name="xml">
        <xsl:output-character character="&lt;" string="&lt;" />
        <xsl:output-character character="&gt;" string="&gt;" />
        <xsl:output-character character="&#xD;" string="&#xD;" />
        <xsl:output-character character="&#47;" string="&#47;" />
    </xsl:character-map>

    <xsl:output name="xhtml" doctype-public="-//W3C//DTD XHTML 1.1//EN" doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1.1.dtd"
        include-content-type="yes" indent="no" media-type="text/html" method="xhtml" />

    <xsl:output name="html" doctype-system="-//W3C//DTD HTML 4.01//EN" doctype-public="http://www.w3.org/TR/html4/strict.dtd" method="html"
        cdata-section-elements="script" indent="no" media-type="html" />

    <xsl:output method="xml" indent="no" encoding="UTF-8" use-character-maps="xml" omit-xml-declaration="no" />

    <xsl:template match="/">
        <xsl:apply-templates />
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
            <xsl:when test="@exclude-message-envelope">
                <xsl:apply-templates>
                    <xsl:with-param name="content-type" select="'text/xml'" />
                </xsl:apply-templates>
            </xsl:when>
            <xsl:otherwise>
                <message type="service:result"
                    content-type="{if (empty($content-type)) then response:get-content-type($response) else 'not-set'}">
                    <xsl:apply-templates />
                </message>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>

</xsl:transform>
