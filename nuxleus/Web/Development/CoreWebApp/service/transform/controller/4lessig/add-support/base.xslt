<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:my="http://xameleon.org/my"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
    xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:saxon="http://saxon.sf.net/" xmlns:clitype="http://saxon.sf.net/clitype"
    xmlns:at="http://atomictalk.org" xmlns:func="http://atomictalk.org/function"
    xmlns:guid="clitype:Xameleon.Function.Utils?partialname=Xameleon" xmlns:sguid="clitype:System.Guid?partialname=mscorlib"
    xmlns:operation="http://xameleon.org/service/operation" xmlns:lessig="http://xameleon.org/service/lessig"
    xmlns:atom="http://www.w3.org/2005/Atom" xmlns:html="http://www.w3.org/1999/xhtml"
    xmlns:request="http://atomictalk.org/function/aspnet/request" xmlns:response="http://atomictalk.org/function/aspnet/response"
    exclude-result-prefixes="#all">

    <xsl:import href="../../base.xslt" />

    <xsl:param name="content-type" />
    <xsl:param name="session-params" />

    <xsl:output name="xml" method="xml" />

    <xsl:template match="operation:lessig">
        <xsl:apply-templates />
    </xsl:template>

    <xsl:template match="lessig:add-support">
        <xsl:variable name="name" select="func:resolve-variable(@name)" />
        <xsl:variable name="location" select="func:resolve-variable(@location)" />

        <xsl:variable name="set-status-code" select="response:set-status-code($response, 303)" />
        <xsl:variable name="set-location" select="response:set-location($response, '/thanks')" />
        <redirect>
            <status-code>
                <xsl:sequence select="$set-status-code" />
            </status-code>
            <location>
                <xsl:sequence select="$set-location" />
            </location>
        </redirect>
    </xsl:template>

    <xsl:function name="func:add">
        <xsl:value-of select="concat('entry added', ' content type=', $content-type)" />
    </xsl:function>

</xsl:transform>
