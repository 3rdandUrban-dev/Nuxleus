<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0" xmlns="http://www.w3.org/2005/Atom" xmlns:atom="http://www.w3.org/2005/Atom" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:saxon="http://saxon.sf.net/" xmlns:clitype="http://saxon.sf.net/clitype" xmlns:at="http://atomictalk.org" xmlns:func="http://atomictalk.org/function" xmlns:http-sgml-to-xml="clitype:Xameleon.Function.HttpSgmlToXml?partialname=Xameleon" xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web" xmlns:atomv03="http://purl.org/atom/ns#" xmlns:proxy="http://xameleon.org/service/proxy" xmlns:html="http://www.w3.org/1999/xhtml" exclude-result-prefixes="#all">

  <xsl:import href="../../../functions/funcset-Util.xslt" />
  <xsl:import href="../../../base.xslt" />
  <xsl:param name="current-context" />

  <xsl:template match="proxy:return-news-by-location">
    <xsl:variable name="location" select="func:resolve-variable(@location)" />
    <xsl:variable name="topic" select="tokenize(func:resolve-variable(@topic), '\|')" />
    <xsl:variable name="uri" select="@uri"/>
    <xsl:variable name="result">
      <xsl:element name="result" namespace="http://nuxleus.com/message/response">
        <xsl:for-each select="$topic">
          <xsl:element name="{.}" namespace="http://nuxleus.com/message/response">
            <xsl:apply-templates select="document(concat('http://news.google.com/news?hl=en&amp;q=', $location, '+', ., '&amp;ie=UTF-8&amp;output=atom'))//atomv03:entry"/>
          </xsl:element>
        </xsl:for-each>
      </xsl:element>
    </xsl:variable>
    <xsl:sequence select="$result"/>
  </xsl:template>

  <xsl:template match="atomv03:entry">
    <atom:entry>
      <xsl:apply-templates />
    </atom:entry>
  </xsl:template>

  <xsl:template match="atomv03:content">
    <xsl:variable name="xhtml" select="http-sgml-to-xml:GetXmlFromHtmlString(http-sgml-to-xml:DecodeHtmlString(.))"/>
    <atom:content type="xhtml">
      <xsl:element name="div" namespace="http://www.w3.org/1999/xhtml">
        <xsl:apply-templates select="saxon:parse($xhtml)" mode="strip"/>
      </xsl:element>
    </atom:content>
  </xsl:template>

  <xsl:template match="*" mode="strip">
    <xsl:variable name="ln" select="local-name()"/>
    <xsl:choose>
      <xsl:when test="$ln = 'a'">
        <xsl:if test="@href">
          <xsl:element name="h3" namespace="http://www.w3.org/1999/xhtml">
            <xsl:element name="{local-name()}" namespace="http://www.w3.org/1999/xhtml">
              <xsl:copy-of select="@href"/>
              <xsl:apply-templates mode="strip"/>
            </xsl:element>
          </xsl:element>
        </xsl:if>
      </xsl:when>
      <xsl:when test="$ln = 'b'">
        <xsl:apply-templates mode="strip"/>
      </xsl:when>
      <xsl:when test="$ln = 'nobr'">
        <xsl:element name="span" namespace="http://www.w3.org/1999/xhtml">
          <xsl:attribute name="class">date</xsl:attribute>
          <xsl:apply-templates mode="strip"/>
        </xsl:element>
      </xsl:when>
      <xsl:when test="$ln = 'font'">
        <xsl:element name="{if(font) then 'div' else (if(parent::font) then 'span' else 'p')}" namespace="http://www.w3.org/1999/xhtml">
          <xsl:if test="parent::font">
            <xsl:attribute name="class">source</xsl:attribute>
          </xsl:if>
          <xsl:apply-templates mode="strip"/>
        </xsl:element>
      </xsl:when>
      <xsl:otherwise>
        <xsl:apply-templates mode="strip"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="text()" mode="strip">
    <xsl:value-of select="."/>
  </xsl:template>

  <xsl:template match="atomv03:title">
    <atom:title>
      <xsl:value-of select="."/>
    </atom:title>
  </xsl:template>

  <xsl:template match="atomv03:link">
    <atom:link>
      <xsl:copy-of select="@rel|@type|@href"/>
    </atom:link>
  </xsl:template>

  <xsl:template match="atomv03:tagline">
    <atom:subtitle>
      <xsl:value-of select="."/>
    </atom:subtitle>
  </xsl:template>

  <xsl:template match="atomv03:modified">
    <atom:updated>
      <xsl:value-of select="."/>
    </atom:updated>
  </xsl:template>

  <xsl:template match="atomv03:issued"/>

  <xsl:template match="atomv03:created">
    <atom:published>
      <xsl:value-of select="."/>
    </atom:published>
  </xsl:template>

  <xsl:template match="atomv03:id">
    <atom:id>
      <xsl:value-of select="."/>
    </atom:id>
  </xsl:template>

  <xsl:template match="atomv03:summary">
    <atom:summary>
      <xsl:value-of select="."/>
    </atom:summary>
  </xsl:template>

  <xsl:template match="atomv03:author">
    <atom:author>
      <xsl:apply-templates select="*"/>
    </atom:author>
  </xsl:template>

  <xsl:template match="atomv03:name">
    <atom:name>
      <xsl:value-of select="."/>
    </atom:name>
  </xsl:template>

  <xsl:template match="atomv03:url">
    <atom:uri>
      <xsl:value-of select="."/>
    </atom:uri>
  </xsl:template>

  <xsl:template match="atomv03:email">
    <atom:email>
      <xsl:value-of select="."/>
    </atom:email>
  </xsl:template>

  <xsl:template match="atomv03:copyright">
    <atom:rights>
      <xsl:value-of select="."/>
    </atom:rights>
  </xsl:template>

  <xsl:template match="atomv03:generator">
    <atom:generator>
      <xsl:copy-of select="@version"/>
      <xsl:if test="@url">
        <xsl:attribute name="uri">
          <xsl:value-of select="@url"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:value-of select="."/>
    </atom:generator>
  </xsl:template>

</xsl:transform>
