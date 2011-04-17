<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0" xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns="http://www.w3.org/1999/xhtml" xmlns:atom="http://www.w3.org/2005/Atom"
    xmlns:omx="http://x2x2x.org/atomicxml/system" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:search="http://xameleon.org/search" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
    xmlns:fn="http://www.w3.org/2005/xpath-functions"
    xmlns:clitype="http://saxon.sf.net/clitype" exclude-result-prefixes="atom xs omx xsi fn">

  <!-- <xsl:import href="./atomicxml.xslt"/> -->

  <xsl:param name="xml.base" select="/atom:feed/@xml:base" as="xs:string"/>
  <xsl:param name="google.maps.key" as="xs:string"/>
  <xsl:param name="request.ip" as="xs:string"/>

  <xsl:variable name="css-base-class" select="'base'" as="xs:string"/>

  <xsl:variable name="geoip-data"
      select="document(concat('http://codemerge.sonicradar.com:3000/ipgeolocator/geocode?debug=true&amp;ip=', $request.ip))/location"/>
  <xsl:param name="lat" select="substring-before($geoip-data/point, '&#32;')" as="xs:string"/>
  <xsl:param name="long" select="substring-after($geoip-data/point, '&#32;')" as="xs:string"/>
  <xsl:param name="map-depth" select="'8'" as="xs:string"/>
  <xsl:param name="city" select="if ($geoip-data/city) then ($geoip-data/city) else 'unknown'" as="xs:string"/>
  <xsl:param name="country" select="if ($geoip-data/country) then ($geoip-data/country) else 'unknown'" as="xs:string"/>
  <xsl:param name="ip" select="if ($geoip-data/ip) then ($geoip-data/ip) else 'unknown'" as="xs:string"/>
  <xsl:param name="search" />
  <xsl:variable name="rights" select="/atom:feed/atom:rights/*"/>
  <xsl:variable name="author" select="/atom:feed/atom:author/atom:name"/>

  <xsl:strip-space elements="*"/>

  <xsl:output doctype-public="-//W3C//DTD XHTML 1.1//EN"
      doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1.1.dtd" include-content-type="yes"
      indent="yes" media-type="text/html" method="xhtml" />

  <xsl:template match="/">
  <!-- 
  <xsl:value-of
    select="file:SaveExternalFile('http://s3.amazonaws.com/m.david/xslt/eveel_plan/XSLnT.png', '/srv/wwwroot/sonicradar.com/foo.png')" 
    xmlns:file="clitype:Xameleon.Function.HttpFileStream?asm=Xameleon;ver=1.0.0.0;sn=8974f7aaf62d3d0f;from=/srv/wwwroot/webapp/bin/Xameleon.dll"/>
   -->
   <xsl:apply-templates select="atom:feed"/>
  </xsl:template>

  <xsl:template match="atom:feed">
  <xsl:variable name="arg" as="xs:double">5</xsl:variable>
  <xsl:variable name="sqrt" select="math:Sqrt($arg)" xmlns:math="clitype:System.Math"/>
  
  <xsl:variable name="xmldoc" select="xmldoc:new()" xmlns:xmldoc="clitype:System.Xml.XmlDocument?asm=System.Xml;version=2.0.0.0;sn=b77a5c561934e089;from=/usr/lib/mono/2.0/System.Xml.dll"/>
  <xsl:variable name="date" select="date:ToShortDateString(date:Now())" xmlns:date="clitype:System.DateTime"/>
    <xsl:choose>
      <xsl:when test="$search">
        <html>
          <head>
            <title>search results</title>
          </head>
          <body>
            <h3>Search Phrase = <xsl:value-of select="$search"/>
            </h3>
            <h3><xsl:value-of select="$sqrt"/></h3>
          </body>
        </html>
      </xsl:when>
      <xsl:otherwise>
        <html>
          <head>
            <title>
              <xsl:value-of select="atom:title"/>
              <xsl:text> :: </xsl:text>
              <xsl:value-of select="atom:subtitle"/>
            </title>
            <xsl:apply-templates select="atom:link[@type = 'text/javascript']"/>
            <style type="text/css">
              <xsl:apply-templates select="atom:link[@type = 'text/css']"/>
            </style>
          </head>
          <body onload="load()" onunload="GUnload()">
            <ul id="{generate-id()}">
              <xsl:apply-templates select="atom:entry/atom:content"/>
            </ul>
          </body>
        </html>
      </xsl:otherwise>
    </xsl:choose>

  </xsl:template>

  <xsl:template match="atom:link[@type = 'text/css']">
    <xsl:text></xsl:text>
    <xsl:text>@import "</xsl:text>
    <xsl:value-of
        select="if (@rel = 'current') then concat($xml.base, @href) else concat(@rel, @href)"/>
    <xsl:text>";</xsl:text>
  </xsl:template>

  <xsl:template match="atom:link[@type = 'text/javascript']">
    <xsl:text></xsl:text>
    <script src="{if (@rel = 'current') then concat($xml.base, @href) else concat(@rel, @href)}" type="text/javascript">
      <xsl:text>/* */</xsl:text>
    </script>
  </xsl:template>

  <xsl:template match="atom:content">
    <xsl:apply-templates select="document(@src)/atom:entry/atom:content/omx:module"/>
  </xsl:template>

  <xsl:template match="text()">
    <xsl:value-of select="normalize-space(.)"/>
  </xsl:template>

</xsl:transform>
