<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0" xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
    xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:saxon="http://saxon.sf.net/"
    xmlns:clitype="http://saxon.sf.net/clitype" xmlns:at="http://atomictalk.org"
    xmlns:func="http://atomictalk.org/function"
    xmlns:http-sgml-to-xml="clitype:Xameleon.Function.HttpSgmlToXml?partialname=Xameleon"
    xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web"
    xmlns:proxy="http://xameleon.org/service/proxy" xmlns:html="http://www.w3.org/1999/xhtml"
    exclude-result-prefixes="xs xsl xsi fn clitype at func http-sgml-to-xml aspnet-context proxy saxon html">

    <xsl:import href="../../functions/funcset-Util.xslt" />
    <xsl:import href="../base.xslt"/>

    <xsl:param name="current-context" />

    <xsl:template match="proxy:return-xml-from-html">
        <xsl:variable name="uri-xpath" select="func:resolve-variable(@uri)" />
        <xsl:variable name="protocol" select="substring-before($uri-xpath, '://')" />
        <xsl:variable name="uri"
            select="concat($protocol, '://', substring-before(substring-after($uri-xpath, '://'), '//'))" />
        <xsl:variable name="xpath" select="substring-after($uri-xpath, concat($uri, '//'))" />
        <xsl:sequence select="func:return-xml-from-html($uri, $xpath)" />
    </xsl:template>

    <xsl:function name="func:return-xml-from-html">
        <xsl:param name="uri" as="xs:string" />
        <xsl:param name="xpath" as="xs:string" />
        <xsl:variable name="html-to-xml"
            select="http-sgml-to-xml:GetDocXml($uri, '/html', false(), $current-context)" />
        <xsl:variable name="html">
            <html:html>
                <xsl:apply-templates select="saxon:parse($html-to-xml)" mode="clean" />
            </html:html>
        </xsl:variable>
        <external-html>
            <xsl:apply-templates select="$html" mode="evaluate">
                <xsl:with-param name="xpath" select="$xpath" />
            </xsl:apply-templates>
        </external-html>
    </xsl:function>

    <xsl:template match="*" mode="clean">
        <xsl:element name="{local-name()}" namespace="http://www.w3.org/1999/xhtml">
            <xsl:copy-of select="@*" />
            <xsl:apply-templates mode="clean" />
        </xsl:element>
    </xsl:template>

    <xsl:template match="text()" mode="clean">
        <xsl:value-of select="." />
    </xsl:template>

    <xsl:template match="*" mode="evaluate">
        <xsl:param name="xpath" />
        <xsl:sequence select="saxon:evaluate($xpath)" />
    </xsl:template>

</xsl:transform>
