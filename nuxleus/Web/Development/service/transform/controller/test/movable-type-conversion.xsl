<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet 
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
  xmlns:func="http://atomictalk.org/function" 
  xmlns:test="http://xameleon.org/controller/test" 
  exclude-result-prefixes="func"
  version="2.0">

  <xsl:param name="old-base-uri" select="'http://www.lessig.org'"/>
  <xsl:param name="new-base-uri" select="'http://lessig.org'"/>
  <xsl:param name="old" select="document('old.xml')/entries" />
  <xsl:param name="new" select="document('new.xml')/entries" />

  <xsl:variable name="output">
    <output>
      <file href=".htaccess" type="apache.htaccess" />
      <file href="compare.xml" type="http-redirect-title-test" />
    </output>
  </xsl:variable>

  <xsl:variable name="lb">
    <xsl:text>
</xsl:text>
  </xsl:variable>

  <xsl:output name="text" method="text" encoding="UTF-8" />
  <xsl:output name="xml" method="xml" indent="yes" encoding="UTF-8"/>

  <xsl:template match="/">
    <xsl:apply-templates select="$output/output/file" />
  </xsl:template>

  <xsl:function name="func:mt-redirect-test">
    <xsl:param name="old" />
    <xsl:param name="new" />
    <xsl:apply-templates select="$output/output/file[@type = 'http-redirect-title-test']">
      <xsl:with-param name="old" select="$old/entries"/>
      <xsl:with-param name="new" select="$new/entries"/>
    </xsl:apply-templates>
  </xsl:function>

  <xsl:template match="file[@type = 'apache.htaccess']">
    <xsl:param name="old" select="$old" />
    <xsl:param name="new" select="$new" />
    <xsl:result-document href="{resolve-uri(@href)}" method="text">
      <xsl:apply-templates select="$new/entry" mode="text">
        <xsl:with-param name="old" select="$old/entries"/>
        <xsl:with-param name="new" select="$new/entries"/>
      </xsl:apply-templates>
    </xsl:result-document>
  </xsl:template>

  <xsl:template match="file[@type = 'http-redirect-title-test']">
    <xsl:param name="old" />
    <xsl:param name="new" />
    <xsl:variable name="entries">
      <test:entries>
        <xsl:apply-templates select="$new/entry" mode="xml">
          <xsl:with-param name="old" select="$old"/>
        </xsl:apply-templates>
      </test:entries>
    </xsl:variable>
    <xsl:sequence select="$entries"/>
  </xsl:template>

  <xsl:template match="entry" mode="xml">
    <xsl:param name="old" select="$old" />
    <xsl:variable name="old-uri"
        select="substring-after($old/entry[text() = current()][1]/@href, $old-base-uri)" />
    <test:entry uri="{concat($new-base-uri, $old-uri)}">
      <xsl:value-of select="$old/entry[text() = current()]/text()" />
    </test:entry>
  </xsl:template>

  <xsl:template match="entry" mode="text">
    <xsl:param name="old" select="$old" />
    <xsl:param name="new" select="$new" />
    <xsl:variable name="old-uri"
        select="substring-after($old/entry[text() = current()][1]/@href, $old-base-uri)" />
    <xsl:variable name="new-uri" select="@href" />
    <xsl:variable name="output" select="concat('RedirectPermanent ', $old-uri, ' ', $new-uri, $lb)"/>
    <xsl:value-of select="if ($old-uri) then $output else concat('#', $output)" />
  </xsl:template>

</xsl:stylesheet>
