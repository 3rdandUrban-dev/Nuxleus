<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:p="http://amp.fm/python/xsltemplates/"
                exclude-result-prefixes="p">
  
  <xsl:param name="p:base_uri" />
  <xsl:param name="p:status" />
  <xsl:param name="p:redirect_url" />
  <xsl:param name="p:return_location" />
  <xsl:param name="p:message" />

  <xsl:template match="/">
    <xsl:processing-instruction name="xml-stylesheet">
      type="text/xsl" href="/transform/openid-redirect.xsl"
    </xsl:processing-instruction>
    <auth xml:base="{$p:base_uri}" status="{$p:status}">
      <xsl:if test="$p:redirect_url">
          <url><xsl:value-of select="$p:redirect_url" /></url>
      </xsl:if>
      <xsl:if test="$p:return_location">
        <return-location><xsl:value-of select="$p:return_location" /></return-location>
      </xsl:if>
      <message><xsl:value-of select="$p:message" /></message>
    </auth>
  </xsl:template>

</xsl:stylesheet>                
