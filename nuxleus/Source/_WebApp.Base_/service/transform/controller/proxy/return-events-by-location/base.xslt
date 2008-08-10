<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0"  xmlns:dateTime="clitype:System.DateTime" xmlns:dc="http://purl.org/dc/elements/1.1/" xmlns:xCal="urn:ietf:params:xml:ns:xcal" xmlns:geo="http://www.w3.org/2003/01/geo/wgs84_pos#" xmlns:atom="http://www.w3.org/2005/Atom" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:saxon="http://saxon.sf.net/" xmlns:clitype="http://saxon.sf.net/clitype" xmlns:at="http://atomictalk.org" xmlns:func="http://atomictalk.org/function" xmlns:http-sgml-to-xml="clitype:Xameleon.Function.HttpSgmlToXml?partialname=Xameleon" xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web" xmlns:proxy="http://xameleon.org/service/proxy" xmlns:html="http://www.w3.org/1999/xhtml" exclude-result-prefixes="#all">

  <xsl:import href="../../base.xslt" />
  <xsl:output cdata-section-elements="event"/>

  <xsl:template match="proxy:return-events-by-location">
    <xsl:variable name="location" select="func:resolve-variable(@location)" />
    <xsl:variable name="topic" select="tokenize(func:resolve-variable(@topic), '\|')" />
    <xsl:variable name="result">
      <data>
          <xsl:apply-templates select="document(concat('http://upcoming.yahoo.com/syndicate/v2/search_all/?category_id=1&amp;loc=', $location, '+', ., '+United+States&amp;rt=1'))" />
      </data>
    </xsl:variable>
    <xsl:sequence select="$result"/>
  </xsl:template>

  <xsl:template match="rss|channel">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="item">
    <xsl:variable name="dtstart" select="func:format-dateTime-string(xCal:dtstart)"/>
    <xsl:variable name="dtend" select="func:format-dateTime-string(concat(dateTime:ToString(dateTime:ToUniversalTime(dateTime:AddHours(dateTime:Parse($dtstart), 2)), string('s'), null), 'Z'))"/>
    <xsl:if test="$dtstart != ''">
      <event start="{$dtstart}" isDuration="{if(not($dtend = '')) then 'true' else 'false'}" title="{title}" image="/images/amp.fm_icon.png">
        <xsl:if test="$dtend != ''">
          <xsl:attribute name="end">
            <xsl:value-of select="$dtend"/>
          </xsl:attribute>
          <xsl:value-of select="description"/>
        </xsl:if>
      </event>
    </xsl:if>
  </xsl:template>

  <xsl:template match="text()" />

</xsl:transform>
