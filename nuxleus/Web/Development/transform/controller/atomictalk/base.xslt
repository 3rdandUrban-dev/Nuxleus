<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="2.0" xmlns:my="http://xameleon.org/my" xmlns:page="http://atomictalk.org/page" xmlns:advice="http://atomictalk.org/page/advice" exclude-result-prefixes="my page advice">

  <xsl:template match="my:session">
    <xsl:param name="member-directory"/>
    <xsl:processing-instruction name="xml-stylesheet">
      <xsl:value-of select="concat('type=', $q, 'text/xsl', $q, ' ', 'href=', $q, '/page/controller/atomictalk/base.xsl', $q)"/>
    </xsl:processing-instruction>
    <xsl:element name="{name()}">
      <xsl:copy-of select="@*"/>
      <xsl:apply-templates>
        <xsl:with-param name="member-directory" select="$member-directory"/>
      </xsl:apply-templates>
    </xsl:element>
  </xsl:template>

  <xsl:template match="my:*">
    <xsl:param name="member-directory"/>
    <xsl:element name="{concat('my:', local-name())}">
      <xsl:copy-of select="@*"/>
      <xsl:apply-templates>
        <xsl:with-param name="member-directory" select="$member-directory"/>
      </xsl:apply-templates>
    </xsl:element>
  </xsl:template>

  <xsl:template match="page:config|page:output|page:head|page:body">
    <xsl:param name="member-directory"/>
    <xsl:element name="{concat('page:', local-name())}">
      <xsl:copy-of select="@*"/>
      <xsl:apply-templates>
        <xsl:with-param name="member-directory" select="$member-directory"/>
      </xsl:apply-templates>
    </xsl:element>
  </xsl:template>

  <xsl:template match="page:advice">
    <xsl:param name="member-directory"/>
    <page:advice>
      <xsl:copy-of select="@*"/>
      <xsl:copy-of select="advice:*"/>
      <advice:current.location>
        <xsl:value-of select="$member-directory"/>
      </advice:current.location>
    </page:advice>
  </xsl:template>

</xsl:stylesheet>
