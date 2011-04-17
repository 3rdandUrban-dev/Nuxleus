<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0" xmlns="http://www.w3.org/2005/Atom" xmlns:dc="http://purl.org/dc/elements/1.1/" xmlns:atom="http://www.w3.org/2005/Atom" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:saxon="http://saxon.sf.net/" xmlns:clitype="http://saxon.sf.net/clitype" xmlns:at="http://atomictalk.org" xmlns:func="http://atomictalk.org/function" xmlns:http-sgml-to-xml="clitype:Xameleon.Function.HttpSgmlToXml?partialname=Xameleon" xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web" xmlns:atomv03="http://purl.org/atom/ns#" xmlns:proxy="http://xameleon.org/service/proxy" xmlns:html="http://www.w3.org/1999/xhtml" exclude-result-prefixes="#all">

  <xsl:import href="../../../functions/funcset-Util.xslt" />
  <xsl:param name="current-context" />

  <xsl:template match="proxy:return-images-by-tag-name">
    <xsl:variable name="location" select="func:resolve-variable(@location)" />
    <xsl:variable name="topic" select="tokenize(func:resolve-variable(@topic), '\|')" />
    <xsl:variable name="uri" select="@uri"/>
    <xsl:variable name="result">
      <xsl:element name="result" namespace="http://nuxleus.com/message/response">
        <xsl:for-each select="$topic">
          <xsl:element name="{.}" namespace="http://nuxleus.com/message/response">
            <xsl:apply-templates select="document(concat($uri, $location, ',', .))/atom:feed" mode="flickr" />
          </xsl:element>
        </xsl:for-each>
      </xsl:element>
    </xsl:variable>
    <xsl:sequence select="$result"/>
  </xsl:template>

  <xsl:template match="atom:feed" mode="flickr">
    <xsl:apply-templates select="atom:entry[(position() mod 4) = 0]" mode="flickr">
      <xsl:sort select="dc:date.Taken" order="descending"/>
    </xsl:apply-templates>
  </xsl:template>

  <xsl:template match="atom:entry" mode="flickr">
    <atom:entry>
      <xsl:apply-templates mode="flickr" />
    </atom:entry>
  </xsl:template>

  <xsl:template match="dc:date.Taken|atom:title|atom:summary|atom:author|atom:link" mode="flickr">
    <xsl:copy-of select="."/>
  </xsl:template>
  
  <xsl:template match="atom:content" mode="flickr">
    <xsl:element name="atom:content">
      <xsl:attribute name="type">text</xsl:attribute>
      <xsl:value-of select="replace(replace(text(), '&lt;b&gt;', ''), '&lt;/b&gt;', '')" />
    </xsl:element>
  </xsl:template>
  
  <xsl:template match="atom:published|atom:updated|atom:id|atom:category" mode="flickr"/>

</xsl:transform>
