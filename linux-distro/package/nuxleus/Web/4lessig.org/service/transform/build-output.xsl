<?xml version="1.0" encoding="UTF-8"?>
<!--
  COPYRIGHT: (c) 2006, M. David Peterson (mailto:m.david@xmlhacker.com; http://mdavid.name/)
  LICENSE: The code contained in this file is licensed under The MIT License. Please see http://www.opensource.org/licenses/mit-license.php for specific detail.
-->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:my="http://xameleon.org/service/session" xmlns="http://www.w3.org/1999/xhtml"
  xmlns:html="http://www.w3.org/1999/xhtml" xmlns:bVend="http://xameleon.org/service/browservendors"
  xmlns:page="http://xameleon.org/service/page/output" xmlns:app="http://purl.org/atom/app#"
  xmlns:atom="http://www.w3.org/2005/Atom" version="1.0"
  exclude-result-prefixes="my html page bVend">

  <xsl:variable name="vendor" select="system-property('xsl:vendor')"/>

  <xsl:output doctype-system="/resources/dtd/xhtml1-strict.dtd"
    doctype-public="-//W3C//DTD XHTML 1.0 Strict//EN" cdata-section-elements="script" method="xml"
    omit-xml-declaration="yes"/>

  <xsl:template match="my:session">
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="my:page">
    <html>
      <head>
        <xsl:apply-templates select="page:output/page:head/page:title"/>
        <style type="text/css">
          <xsl:apply-templates select="page:output/page:head/page:head.include[@fileType = 'css']"/>
          <xsl:apply-templates select="page:config/bVend:bVendors/bVend:vBrowser[@vendor = $vendor]/page:head.include[@fileType = 'css']"/>
        </style>
        <xsl:apply-templates
          select="page:output/page:head/page:head.include[@fileType = 'javascript']"/>
        <xsl:apply-templates
          select="page:config/bVend:bVendors/bVend:vBrowser[@vendor = $vendor]/page:head.include[@fileType = 'javascript']"
        />
      </head>
      <body onload="javascript: hello(); return true;">
        <xsl:apply-templates select="page:output/page:body"/>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="page:head.include[@fileType = 'css']">
    <xsl:text>@import "</xsl:text>
    <xsl:value-of select="@href"/>
    <xsl:text>";</xsl:text>
    <xsl:text>
</xsl:text>
  </xsl:template>

  <xsl:template match="page:head.include[@fileType = 'javascript']">
    <script type="text/javascript" src="{@href}">
      <xsl:comment>/* hack to ensure browser compatibility */</xsl:comment>
    </script>
  </xsl:template>

  <xsl:template match="page:title">
    <title>
      <xsl:apply-templates/>
    </title>
  </xsl:template>

  <xsl:template match="page:body">
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="page:content[@src]">
    <xsl:apply-templates select="document(@src)"/>
  </xsl:template>

  <xsl:template match="page:content">
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="page:heading">
    <xsl:element name="{@size}">
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>

  <xsl:template match="html:*">
    <xsl:element name="{local-name()}">
      <xsl:copy-of select="@*"/>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>

  <xsl:template match="text()">
    <xsl:value-of select="."/>
  </xsl:template>

  <xsl:template match="app:service">
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="app:workspace">
    <xsl:apply-templates mode="workspace"/>
  </xsl:template>

  <xsl:template match="atom:title" mode="workspace">
    <h1> Summary of Collections in the "<xsl:value-of select="."/>" Workspace </h1>
  </xsl:template>

  <xsl:template match="app:collection" mode="workspace">
    <h3><xsl:value-of select="atom:title"/> Collection </h3>
    <ul>
      <li> Collection Location: <a href="{@href}">
          <xsl:value-of select="@href"/>
        </a>
      </li>
      <li> Accepts: <xsl:value-of select="app:accept"/>
      </li>
      <li>
        <xsl:apply-templates select="document(@href)" mode="collection-summary"/>
      </li>
    </ul>
  </xsl:template>

  <xsl:template match="atom:feed" mode="collection-summary">
    <h4><xsl:value-of select="atom:title"/> Feed Info </h4>
    <ul>
      <xsl:apply-templates select="*[not(atom:title)]" mode="collection"/>
      <xsl:if test="atom:entry">
        <li>
          <h4><xsl:value-of select="atom:title"/> Entries </h4>
          <xsl:apply-templates select="atom:entry" mode="collection"/>
        </li>
      </xsl:if>
    </ul>
  </xsl:template>

  <xsl:template match="atom:entry" mode="collection">
    <ul>
      <xsl:apply-templates mode="collection"/>
    </ul>
  </xsl:template>

  <xsl:template match="atom:title" mode="collection"/>

  <xsl:template match="atom:id" mode="collection">
    <li> ID: <xsl:value-of select="."/>
    </li>
  </xsl:template>

  <xsl:template match="atom:updated" mode="collection">
    <li> Last Update: <xsl:value-of select="."/>
    </li>
  </xsl:template>
  
  <xsl:template match="atom:summary" mode="collection">
    <li> Entry Summary: <xsl:value-of select="."/>
    </li>
  </xsl:template>

  <xsl:template match="atom:updated" mode="collection">
    <li> Last Update: <xsl:value-of select="."/>
    </li>
  </xsl:template>

  <xsl:template match="atom:published" mode="collection">
    <li> Date Published: <xsl:value-of select="."/>
    </li>
  </xsl:template>

  <xsl:template match="app:edited" mode="collection">
    <li> Date Last Edited: <xsl:value-of select="."/>
    </li>
  </xsl:template>

</xsl:stylesheet>
