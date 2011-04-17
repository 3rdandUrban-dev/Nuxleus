<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">    
    <xsl:template match="parts">
        <parts>
            <xsl:apply-templates select="item">
                <xsl:sort data-type="number" order="descending" select="@price"/>
            </xsl:apply-templates>
        </parts>
    </xsl:template>
    <xsl:template match="item">
        <xsl:if test="position() &lt;= 3">
            <xsl:copy-of select="."/>
        </xsl:if>
    </xsl:template>
</xsl:stylesheet>
