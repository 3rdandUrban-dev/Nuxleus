<?xml version="1.0" encoding="UTF-8"?>
<!--
  COPYRIGHT: (c) 2007, M. David Peterson (mailto:m.david@xmlhacker.com; http://mdavid.name/)
  LICENSE: The code contained in this file is licensed under The New BSD License. Please see
  http://www.opensource.org/licenses/bsd-license.php for specific detail.
  Contributors to this code base include, 
    Russ Miles (mailto:aohacker@gmail.com; http://www.russmiles.com/)
-->
<xsl:stylesheet version="1.0" 
  xmlns:html="http://www.w3.org/1999/xhtml"
  xmlns:my="http://xameleon.org/my" 
  xmlns:page="http://atomictalk.org/page"
  xmlns:doc="http://atomictalk.org/feed/doc" 
  xmlns:service="http://atomictalk.org/page/service"
  xmlns:output="http://atomictalk.org/page/output"
  xmlns:head="http://atomictalk.org/page/output/head"
  xmlns:body="http://atomictalk.org/page/output/body"
  xmlns:advice="http://atomictalk.org/page/advice" 
  xmlns:view="http://atomictalk.org/page/view"
  xmlns:layout="http://atomictalk.org/page/view/layout"
  xmlns:form="http://atomictalk.org/page/view/form"
  xmlns:menu="http://atomictalk.org/page/view/menu" 
  xmlns:exsl="http://exslt.org/common"
  xmlns:resource="http://atomictalk.org/page/resource"
  xmlns:model="http://atomictalk.org/page/model" 
  xmlns:app="http://purl.org/atom/app#"
  xmlns:atompub="http://www.w3.org/2007/app" 
  xmlns:atom="http://www.w3.org/2005/Atom"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  exclude-result-prefixes="html exsl my app advice atom head page service resource output form body view menu model msxsl doc atompub">

  <xsl:include href="../atom/base.xsl"/>
  <xsl:include href="./process.xsl"/>

  <xsl:strip-space elements="html:*"/>

  <xsl:output cdata-section-elements="script" doctype-system="-//W3C//DTD HTML 4.01//EN"
    doctype-public="http://www.w3.org/TR/html4/strict.dtd" method="html" indent="yes"/>

  <xsl:template match="my:session">
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="my:page">
    <xsl:apply-templates select="page:output"/>
  </xsl:template>

  <xsl:template match="page:output">
    <html>
      <xsl:apply-templates select="$page.output.head"/>
      <xsl:copy-of select="$page.output.body"/>
    </html>
  </xsl:template>

  <xsl:template match="page:head">
    <head>
      <xsl:apply-templates select="head:title"/>
      <xsl:apply-templates select="head:link"/>
      <style type="text/css">
        <xsl:apply-templates select="head:include[@fileType = 'css']"/>
      </style>
      <xsl:apply-templates select="head:include[@fileType = 'javascript']"/>
    </head>
  </xsl:template>

  <xsl:template match="page:body">
    <body>
      <xsl:apply-templates/>
    </body>
  </xsl:template>

  <xsl:template match="body:onload|body:onresize">
    <xsl:attribute name="{local-name()}">
      <xsl:call-template name="replace">
        <xsl:with-param name="string" select="@action"/>
      </xsl:call-template>
    </xsl:attribute>
  </xsl:template>

  <xsl:template match="head:include[@fileType = 'css']">
    <xsl:variable name="uri">
      <xsl:call-template name="resolve-uri">
        <xsl:with-param name="href" select="@href"/>
      </xsl:call-template>
    </xsl:variable>
    <xsl:value-of select="concat('@import ', $quote, $uri, $quote, ';')"/>
  </xsl:template>

  <xsl:template match="head:include[@fileType = 'javascript' and not(@src)]">
    <script type="text/javascript">
      <xsl:text>//&lt;![CDATA[</xsl:text>
      <xsl:call-template name="replace">
        <xsl:with-param name="string" select="text()"/>
      </xsl:call-template>
      <xsl:text>//]]&gt;</xsl:text>
    </script>
  </xsl:template>

  <xsl:template match="head:include[@fileType = 'javascript' and @src]">
    <xsl:variable name="uri">
      <xsl:call-template name="resolve-uri">
        <xsl:with-param name="href" select="@src"/>
      </xsl:call-template>
    </xsl:variable>
    <script type="text/javascript" src="{$uri}">
      <xsl:comment>/* hack to ensure browser compatibility */</xsl:comment>
    </script>
  </xsl:template>

  <xsl:template name="resolve-uri">
    <xsl:param name="href"/>
    <xsl:call-template name="replace">
      <xsl:with-param name="string" select="translate($href, ' ', '')"/>
    </xsl:call-template>
  </xsl:template>

  <xsl:template match="advice:*[@compare = 'xsl:vendor']">
    <xsl:value-of select="current()[@compare-with = $vendor]/text()"/>
  </xsl:template>

  <xsl:template match="advice:*">
    <xsl:copy-of select="."/>
  </xsl:template>

  <xsl:template match="head:title">
    <title>
      <xsl:apply-templates/>
    </title>
  </xsl:template>

  <xsl:template match="head:link">
    <link>
      <xsl:copy-of select="@*"/>
    </link>
  </xsl:template>

  <xsl:template match="body:layout">
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="layout:view">
    <ul class="list {@style}" id="{@id}">
      <xsl:apply-templates/>
    </ul>
  </xsl:template>

  <xsl:template match="doc:feed">
    <xsl:apply-templates select="document(@href)/atom:feed/atom:entry">
      <xsl:with-param name="cCount" select="@characterCount"/>
    </xsl:apply-templates>
  </xsl:template>

  <xsl:template match="doc:date">
    <xsl:value-of select="document('/date.xml')/date/current"/>
  </xsl:template>

  <xsl:template match="doc:html[@type = 'myspace-events']">
    <xsl:apply-templates select="document(@href)//html:div[@id = current()/@id]"/>
  </xsl:template>

  <xsl:template match="doc:html[@type = 'external-html']">
    <xsl:apply-templates select="document(@href)//body:html"/>
  </xsl:template>

  <xsl:template match="page:content[@src]">
    <xsl:apply-templates select="document(@src)"/>
  </xsl:template>

  <xsl:template match="page:content">
    <xsl:copy-of select="*"/>
  </xsl:template>

  <xsl:template match="page:view">
    <ul>
      <xsl:apply-templates/>
    </ul>
  </xsl:template>

  <xsl:template match="view:container">
    <ul class="list {@style}" id="{@id}">
      <xsl:apply-templates/>
    </ul>
  </xsl:template>

  <xsl:template match="view:item[@src]">
    <li class="list {@style}" id="{@id}">
      <xsl:apply-templates select="document(@src)/view:*"/>
    </li>
  </xsl:template>

  <xsl:template match="view:item[not(@src)]">
    <li class="list menu {@style}" id="{@id}">
      <xsl:apply-templates/>
    </li>
  </xsl:template>

  <xsl:template match="view:menu[@src]">
    <ul class="list menu {@style}" id="{@id}">
      <xsl:apply-templates select="document(@src)/view:menu"/>
    </ul>
  </xsl:template>

  <xsl:template match="view:menu[not(@src)]">
    <ul class="list menu {@style}" id="{@id}">
      <xsl:apply-templates/>
    </ul>
  </xsl:template>

  <xsl:template match="view:module[@src]">
    <li class="list {@style}" id="{@id}">
      <xsl:apply-templates select="document(@src)/view:module/*"/>
    </li>
  </xsl:template>

  <xsl:template match="view:module[not(@src)]">
    <li class="list {@style}" id="{@id}">
      <xsl:apply-templates/>
    </li>
  </xsl:template>

  <xsl:template match="view:menu">
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="page:heading">
    <xsl:element name="{@size}">
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>

  <xsl:template match="*">
    <xsl:element name="{local-name()}">
      <xsl:apply-templates select="@*"/>
      <xsl:apply-templates/>
    </xsl:element>
  </xsl:template>

  <xsl:template match="@*">
    <xsl:attribute name="{local-name()}">
      <xsl:call-template name="replace">
        <xsl:with-param name="string" select="."/>
      </xsl:call-template>
    </xsl:attribute>
  </xsl:template>

  <xsl:template match="text()">
    <xsl:call-template name="replace">
      <xsl:with-param name="string" select="."/>
    </xsl:call-template>
  </xsl:template>

</xsl:stylesheet>
