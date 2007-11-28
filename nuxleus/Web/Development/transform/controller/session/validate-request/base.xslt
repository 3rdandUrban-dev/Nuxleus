<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:fn="http://www.w3.org/2005/xpath-functions"
    xmlns:saxon="http://saxon.sf.net/" xmlns:clitype="http://saxon.sf.net/clitype" xmlns:at="http://atomictalk.org"
    xmlns:func="http://atomictalk.org/function" xmlns:session="http://xameleon.org/service/session"
    xmlns:guid="clitype:System.Guid?partialname=mscorlib" xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web"
    xmlns:proxy="http://xameleon.org/service/proxy" xmlns:html="http://www.w3.org/1999/xhtml"
    xmlns:operation="http://xameleon.org/service/operation" xmlns:request-stream="clitype:System.Web.HttpRequest?partialname=System.Web"
    xmlns:browser="http://xameleon.org/service/http/request/browser"
    xmlns:browser-capabilities="clitype:System.Web.HttpBrowserCapabilities?partialname=System.Web" exclude-result-prefixes="#all">

    <xsl:import href="../../../functions/funcset-Util.xslt" />
    <xsl:param name="current-context" />
    <xsl:param name="request" />
    <xsl:param name="browser" />

    <xsl:template match="session:validate-request">
        <xsl:variable name="session-id" select="func:resolve-variable(@key)" />
        <xsl:variable name="openid" select="func:resolve-variable(@openid)" />
        <xsl:apply-templates>
            <xsl:with-param name="session-id" select="$session-id" />
            <xsl:with-param name="openid" select="$openid" />
        </xsl:apply-templates>
    </xsl:template>

    <xsl:template match="operation:return-xml">
        <xsl:param name="session-id" />
        <xsl:param name="openid" />
        <session session-id="{$session-id}" openid="{replace($openid, '%2F', '/')}">
            <xsl:apply-templates />
        </session>
    </xsl:template>

    <xsl:template match="session:generate-guid">
        <request-guid>
            <xsl:value-of select="string(guid:NewGuid())" />
            <xsl:apply-templates />
        </request-guid>
    </xsl:template>

    <xsl:template match="browser:user-agent">
        <user-agent>
            <xsl:value-of select="request-stream:UserAgent($request)" />
        </user-agent>
    </xsl:template>

    <xsl:template match="session:return-request-browser-capabilities">
        <browser-capabilities>
            <xsl:apply-templates />
        </browser-capabilities>
    </xsl:template>

    <xsl:template match="browser:platform">
        <platform>
            <xsl:value-of select="browser-capabilities:Platform($browser)" />
        </platform>
    </xsl:template>

    <xsl:template match="browser:name">
        <browser>
            <xsl:value-of select="browser-capabilities:Browser($browser)" />
        </browser>
    </xsl:template>

    <xsl:template match="browser:major-version">
        <major-version>
            <xsl:value-of select="string(browser-capabilities:MajorVersion($browser))" />
        </major-version>
    </xsl:template>

    <xsl:template match="browser:minor-version">
        <minor-version>
            <xsl:value-of select="string(browser-capabilities:MinorVersion($browser))" />
        </minor-version>
    </xsl:template>

    <xsl:template match="browser:ecma-script-version">
        <ecma-script-version>
            <xsl:value-of select="string(browser-capabilities:EcmaScriptVersion($browser))" />
        </ecma-script-version>
    </xsl:template>

</xsl:transform>
