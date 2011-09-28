<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:exslt="http://exslt.org/documentation" exclude-result-prefixes="exslt">
    <xsl:output indent="yes" encoding="ISO8859-1"/>
<!--    <xsl:template match="modules">
        <modules>
            <xsl:for-each select="module">
                <module namespace="{@namespace}">
                    <xsl:variable name="doc" select="document(@url)/*"/>
                    <xsl:variable name="prefix" select="$doc/@prefix"/>
                    <xsl:attribute name="prefix"><xsl:value-of select="$doc/@prefix"/></xsl:attribute>
                    <xsl:attribute name="name"><xsl:value-of select="$doc/exslt:name"/></xsl:attribute>
                    <xsl:attribute name="homepage"><xsl:value-of select="concat('http://exslt.org/', $prefix, '/index.html')"/></xsl:attribute>
                    <xsl:for-each select="$doc/exslt:functions/exslt:function">
                        <function name="{@name}" descriptionURL="{concat('http://exslt.org/', $prefix, '/functions/', @name, '/index.html')}" supported="yes"/>
                    </xsl:for-each>
                </module>
            </xsl:for-each>
        </modules>
    </xsl:template> -->
    <xsl:template match="/">
        <html>
            <head>
                <title>EXSLT.NET 
                <xsl:value-of select="modules/@version"/> Function List</title>
                <style type="text/css">
                    body { font-family: Verdana, Arial, helvetica;}
                    .module-table {
                        width: 100%;
                    }                    
                </style>
            </head>
            <body>
                <h2>EXSLT.NET <xsl:value-of select="modules/@version"/> Function List</h2>
                <hr/>
                <div>EXSLT Modules:</div>
                <ol>                                
                    <xsl:apply-templates select="modules/module[@is-exslt-module='yes']" mode="toc"/>
                </ol>
                <div>Additional modules:</div>
                <ol>                                
                    <xsl:apply-templates select="modules/module[@is-exslt-module='no']" mode="toc"/>
                </ol>
                <hr/>
                <xsl:apply-templates select="modules/module"/>
            </body>
        </html>                
    </xsl:template>
    <xsl:template match="module" mode="toc">
        <li>
            <a href="#{@name}"><xsl:value-of select="@name"/></a>
        </li>       
    </xsl:template>
    <xsl:template match="module">        
        <p>
        <a name="{@name}">
        <div class="module-name"><b>Module:</b>&#xA0;<xsl:value-of select="@name"/></div></a>        
        <div class="module-ns"><b>Namespace:</b>&#xA0;<xsl:value-of select="@namespace"/></div>
        <xsl:if test="@homepage">
            <div class="module-homepage"><b>Homepage:</b>&#xA0;<a href="{@homepage}"><xsl:value-of select="@homepage"/></a></div>
        </xsl:if>
        <xsl:if test="@is-exslt-module='no'">
            <div class="note"><b>Note:</b> These functions are not part of EXSLT.</div>
        </xsl:if>
        <table border="1" class="module-table">
            <tr>
                <th>Function name</th>
                <th>Alias</th>
                <th>Supported</th>               
            </tr>
            <xsl:for-each select="function">
                <tr>
                    <td>
                    <xsl:choose>
                        <xsl:when test="@descriptionURL">
                            <a href="{@descriptionURL}"><xsl:value-of select="concat(../@prefix, ':', @name)"/></a>
                        </xsl:when>
                        <xsl:otherwise>
                            <xsl:value-of select="concat(../@prefix, ':', @name)"/>
                        </xsl:otherwise>
                    </xsl:choose>
                    </td>
                    <td align="center">
                        <xsl:choose>
                            <xsl:when test="@alias">
                                <xsl:value-of select="@alias"/>
                            </xsl:when>
                            <xsl:otherwise>-</xsl:otherwise>
                        </xsl:choose>
                    </td>
                    <td align="center">
                    <xsl:attribute name="style">
                        <xsl:choose>
                            <xsl:when test="@supported='yes'">color:green;</xsl:when>
                            <xsl:otherwise>color:red;</xsl:otherwise>
                        </xsl:choose>
                    </xsl:attribute>
                    <xsl:value-of select="@supported"/></td>
                </tr>
            </xsl:for-each>
        </table>
        </p>
        <hr/>
    </xsl:template>
</xsl:stylesheet>
