<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0" xmlns="http://www.w3.org/2005/Atom" xmlns:tapi="http://api.technorati.com/dtd/tapi-002.xml" xmlns:atom="http://www.w3.org/2005/Atom" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:saxon="http://saxon.sf.net/" xmlns:clitype="http://saxon.sf.net/clitype" xmlns:at="http://atomictalk.org" xmlns:func="http://atomictalk.org/function" xmlns:http-sgml-to-xml="clitype:Xameleon.Function.HttpSgmlToXml?partialname=Xameleon" xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web" xmlns:proxy="http://xameleon.org/service/proxy" xmlns:html="http://www.w3.org/1999/xhtml" exclude-result-prefixes="#all">

  <xsl:import href="../../../functions/funcset-Util.xslt" />
  <xsl:param name="current-context" />

  <xsl:template match="proxy:return-blog-entries-by-location">
    <xsl:variable name="location" select="func:resolve-variable(@location)" />
    <xsl:variable name="topic" select="tokenize(func:resolve-variable(@topic), '\|')" />
    <xsl:variable name="result">
      <xsl:element name="result" namespace="http://nuxleus.com/message/response">
        <xsl:for-each select="$topic">
          <xsl:element name="{.}" namespace="http://nuxleus.com/message/response">
            <xsl:apply-templates select="document(concat('http://blogsearch.google.com/blogsearch_feeds?hl=en&amp;q=', $location, '+', ., '&amp;ie=UTF-8&amp;output=atom'))/atom:feed" mode="googleblogs" />
            <!-- <xsl:apply-templates select="document(concat('http://feeds.technorati.com/tag/', $location, '+', ., '?authority=a4&amp;language=en'))" mode="technorati" /> -->
          </xsl:element>
        </xsl:for-each>
      </xsl:element>
    </xsl:variable>
    <xsl:sequence select="$result"/>
  </xsl:template>

  <xsl:template match="atom:feed" mode="googleblogs">
    <xsl:apply-templates select="atom:entry" mode="googleblogs">
      <xsl:sort select="atom:updated" order="descending"/>
    </xsl:apply-templates>
  </xsl:template>

  <xsl:template match="*" mode="technorati">
    <xsl:choose>
      <xsl:when test="local-name(.) = 'item'">
        <atom:entry>
          <xsl:apply-templates mode="technorati" />
        </atom:entry>
      </xsl:when>
      <xsl:when test="local-name(.) = 'title'">
        <xsl:element name="atom:title">
          <xsl:attribute name="type">text</xsl:attribute>
          <xsl:value-of select="text()" />
        </xsl:element>
      </xsl:when>
      <xsl:when test="local-name(.) = 'summary'">
        <xsl:element name="atom:content">
          <xsl:attribute name="type">text</xsl:attribute>
          <xsl:value-of select="text()" />
        </xsl:element>
      </xsl:when>
      <xsl:when test="local-name(.) = 'pubDate'">
        <xsl:element name="atom:published">
          <xsl:value-of select="." />
        </xsl:element>
      </xsl:when>
      <xsl:otherwise>
        <xsl:apply-templates select="*" mode="technorati" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  
  <xsl:template match="tapi:*" mode="technorati"/>

  <xsl:template match="atom:entry" mode="googleblogs">
    <atom:entry>
      <xsl:apply-templates mode="googleblogs" />
    </atom:entry>
  </xsl:template>

  <xsl:template match="atom:summary|atom:author|atom:link" mode="googleblogs">
    <xsl:copy-of select="."/>
  </xsl:template>

  <xsl:template match="atom:summary|atom:author|atom:link" mode="technorati">
    <xsl:copy-of select="."/>
  </xsl:template>

  <xsl:template match="atom:content|atom:title" mode="googleblogs">
    <xsl:element name="{name()}">
      <xsl:attribute name="type">text</xsl:attribute>
      <xsl:value-of select="replace(replace(replace(text(), '&lt;b&gt;', ''), '&lt;/b&gt;', ''), '&lt;br&gt;', '')" />
    </xsl:element>
  </xsl:template>

  <xsl:template match="atom:published|atom:updated|atom:id|atom:category" mode="googleblogs"/>

</xsl:transform>
