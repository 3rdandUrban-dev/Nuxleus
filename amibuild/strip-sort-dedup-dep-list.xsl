<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="2.0">
    
    <xsl:param name="text-file" select="'dep-list'"/>
    
    <xsl:variable name="linebreak">
        <xsl:text>
</xsl:text>
    </xsl:variable>
    
    <xsl:output name="text" method="text"/>
    
    <xsl:template match="/">
        <xsl:variable name="temp-file-list">
            <file-list>
                <xsl:call-template name="to-xml">
                    <xsl:with-param name="data" select="unparsed-text(file/dedup/@dedup-file-name)"/>
                </xsl:call-template>
            </file-list>
        </xsl:variable>
        <xsl:apply-templates select="$temp-file-list/file-list">
            <xsl:with-param name="output-file" select="file/dedup/@output-file-name"/>
            <xsl:with-param name="data" select="unparsed-text(file/dedup/@dedup-file-name)"/>
        </xsl:apply-templates>
    </xsl:template>
    
    <xsl:template match="file-list">
        <xsl:param name="output-file"/>
        <xsl:result-document format="text" href="{$output-file}">
        
	<!--
        HACK
            for some reason the linux /lib/ld* library/symlink are not being copied into the dep list
            As such, a hack, which will place this before the other entries.
        HACK
	--> 
        
            <xsl:text>/lib/ld</xsl:text><xsl:value-of select="$linebreak"/>
            <xsl:for-each-group select="file" group-by="@path-name">
                <xsl:sort select="current-grouping-key()"/>
                <xsl:variable name="normalized-string"
                    select="normalize-space(current-grouping-key())"/>
                <xsl:if test="contains($normalized-string, '/')">
                    <xsl:value-of select="concat($normalized-string, $linebreak)"/>
                </xsl:if>
            </xsl:for-each-group>
        </xsl:result-document>
    </xsl:template>
    
    <xsl:template name="to-xml">
        <xsl:param name="data"/>
        <xsl:if test="contains($data,$linebreak)">
            <file path-name="{substring-before($data,$linebreak)}"/>
            <xsl:call-template name="to-xml">
                <xsl:with-param name="data" select="substring-after($data,$linebreak)"/>
            </xsl:call-template>
        </xsl:if>
    </xsl:template>
    
</xsl:stylesheet>

