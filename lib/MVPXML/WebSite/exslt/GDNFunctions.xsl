<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="function">
        <html>
            <head>
                <title>EXSLT.NET. Additional extenstion functions: <xsl:value-of select="concat(@prefix,':', @name)"/>
                </title>
            </head>
            <body>
                <h2>EXSLT.NET. Additional extenstion functions: <xsl:value-of select="concat(@prefix,':', @name)"/>
                </h2>
                <p>
                    <b>Namespace: </b>
                    <xsl:value-of select="@namespace"/>
                </p>
                <xsl:apply-templates select="syntax"/>
                <xsl:copy-of select="description"/>
                <xsl:apply-templates select="sample"/>
            </body>
        </html>
    </xsl:template>
    <xsl:template match="syntax">
        <h3>Function Syntax</h3>
        <p>
            <i>
                <xsl:value-of select="return"/>
            </i>&#xA0;<xsl:value-of select="../@prefix"/>:<xsl:value-of select="../@name"/>&#xA0;(<xsl:for-each select="params/param">
                <i>
                    <xsl:value-of select="."/>
                    <xsl:if test="@optional='yes'">?</xsl:if>
                    <xsl:if test="position() != last()">,&#xA0;</xsl:if>
                </i>
            </xsl:for-each>)</p>
    </xsl:template>
    <xsl:template match="sample">
        <h3>Example:</h3>
        <xsl:apply-templates/>
    </xsl:template>
    <xsl:template match="source">
        <h4>Source XML document:</h4>
        <pre><xsl:value-of select="."/></pre>
    </xsl:template>
    <xsl:template match="stylesheet">
        <h4>Stylesheet:</h4>
        <pre><xsl:value-of select="."/></pre>
    </xsl:template>
    <xsl:template match="result">
        <h4>Result:</h4>
        <pre><xsl:value-of select="."/></pre>
    </xsl:template>
</xsl:stylesheet>
