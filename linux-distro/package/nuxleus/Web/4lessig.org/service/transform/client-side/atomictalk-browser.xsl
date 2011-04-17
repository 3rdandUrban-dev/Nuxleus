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
    xmlns:head="http://atomictalk.org/page/head"
    xmlns:body="http://atomictalk.org/page/body"
    xmlns:advice="http://aspectxml.org/advice"
    xmlns:view="http://atomictalk.org/page/view"
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

  <xsl:include href="/transform/client-side/atom.xsl"/>
    
  <xsl:variable name="vendor" select="system-property('xsl:vendor')" />
  <xsl:variable name="vendor-uri" select="system-property('xsl:vendor-uri')" />
  <xsl:variable name="page" select="/my:session/my:page" />
  <xsl:variable name="config" select="document($page/page:config/@src)/page:config|$page/page:config" />
  <xsl:variable name="browser" select="$config/page:browser[@vendor = $vendor]/@replace" />
  <xsl:variable name="advice" select="$config/page:advice" />
  <xsl:variable name="resource" select="document($page/page:resource/@src)/page:config|$page/page:resource" />
  <xsl:variable name="service" select="document($page/page:service/@src)/page:config|$page/page:service" />
  <xsl:variable name="view" select="document($page/page:view/@src)/page:config|$page/page:view" />
  
  <xsl:param name="closure-token-pre-delimiter" select="'|@@'" />
  <xsl:param name="closure-token-post-delimiter" select="'@@|'" />
  <xsl:param name="replace-token-pre-delimiter" select="'@@'" />
  <xsl:param name="replace-token-post-delimiter" select="'@@'" />
  <xsl:param name="cond-token-pre-delimiter" select="'|$$'" />
  <xsl:param name="cond-token-post-delimiter" select="'$$|'" />
  <xsl:param name="cond-if-token" select="'test:'" />
  <xsl:param name="cond-then-token" select="'IfTrue:'" />
  <xsl:param name="cond-else-token" select="'IfFalse:'" />
  <xsl:param name="system-variable-pre-delimiter" select="'%%'" />
  <xsl:param name="system-variable-post-delimiter" select="'%%'" />
  <xsl:param name="parameter-list-pre-delimiter" select="'($'" />
  <xsl:param name="parameter-list-post-delimiter" select="'$)'" />
  <xsl:param name="parameter-pre-delimiter" select="':'" />
  <xsl:param name="parameter-post-delimiter" select="':'" />
  <xsl:param name="replace-parameter-list-pre-delimiter" select="'[$'" />
  <xsl:param name="replace-parameter-list-post-delimiter" select="'$]'" />
  <xsl:param name="replace-parameter-pre-delimiter" select="':'" />
  <xsl:param name="replace-parameter-post-delimiter" select="':'" />
  <xsl:param name="parameter-list-delimeter" select="','"/>
  <xsl:param name="parameter-value-assigment-token" select="'='"/>
  
  <xsl:variable name="lb">
    <xsl:text>
</xsl:text>
  </xsl:variable>
  <xsl:variable name="quote">"</xsl:variable>
  <xsl:variable name="squote">'</xsl:variable>

  <xsl:variable name="base-uri">
    <xsl:choose>
      <xsl:when test="$page[@xml:base]">
        <xsl:value-of select="$page/@xml:base" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:call-template name="replace">
          <xsl:with-param name="string"
              select="$advice/advice:*[local-name() = 'base-uri']/*" />
        </xsl:call-template>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:variable>
  
  
  <xsl:variable name="page.output.head" select="document($page/page:output/page:head/@src)/page:head|$page/page:output/page:head" />
  <xsl:variable name="page.output.body">
    <xsl:choose>
      <xsl:when test="$page/page:output/page:body/@src">
        <xsl:apply-templates select="document($page/page:output/page:body/@src)/page:body"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:apply-templates select="$page/page:output/page:body"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:variable>

  <xsl:output
      cdata-section-elements="script"
      doctype-system="-//W3C//DTD HTML 4.01//EN"
      doctype-public="http://www.w3.org/TR/html4/strict.dtd"
      method="html"
      indent="no" />

  <xsl:template match="my:session">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="my:page">
    <xsl:apply-templates select="page:output" />
  </xsl:template>

  <xsl:template match="page:output">
    <html>
      <xsl:apply-templates select="$page.output.head" />
      <xsl:copy-of select="$page.output.body" />
    </html>
  </xsl:template>

  <xsl:template match="page:head">
    <head>
      <xsl:apply-templates select="head:title" />
      <link rel="alternate" type="application/atom+xml" title="Amp.fm ON THE AIR"
          href="http://dev.amp.fm/ontheair.atom" />
      <link rel="alternate" type="application/xml" title="Amp.fm ON THE AIR"
          href="http://dev.amp.fm/ontheair.atom" />
      <style type="text/css">
        <xsl:apply-templates select="head:include[@fileType = 'css']" />
      </style>
      <xsl:apply-templates select="head:include[@fileType = 'javascript']" />
    </head>
  </xsl:template>

  <xsl:template match="page:body">
    <body>
      <xsl:apply-templates select="body:onload|body:onresize" />
      <xsl:apply-templates select="body:html" />
    </body>
  </xsl:template>

  <xsl:template match="body:onload|body:onresize">
    <xsl:attribute name="{local-name()}">
      <xsl:call-template name="replace">
        <xsl:with-param name="string" select="@action" />
      </xsl:call-template>
    </xsl:attribute>
  </xsl:template>

  <xsl:template match="head:include[@fileType = 'css']">
    <xsl:variable name="uri">
      <xsl:call-template name="resolve-uri">
        <xsl:with-param name="href" select="@href" />
      </xsl:call-template>
    </xsl:variable>
    <xsl:value-of select="concat('@import ', $quote, $uri, $quote, ';')" />
  </xsl:template>

  <xsl:template match="head:include[@fileType = 'javascript' and not(@src)]">
    <script type="text/javascript">
      <xsl:text>//&lt;![CDATA[</xsl:text>
      <xsl:call-template name="replace">
        <xsl:with-param name="string" select="text()" />
      </xsl:call-template>
      <xsl:text>//]]&gt;</xsl:text>
    </script>
  </xsl:template>

  <xsl:template match="head:include[@fileType = 'javascript' and @src]">
    <xsl:variable name="uri">
      <xsl:call-template name="resolve-uri">
        <xsl:with-param name="href" select="@src" />
      </xsl:call-template>
    </xsl:variable>
    <script type="text/javascript" src="{$uri}">
      <xsl:comment>/* hack to ensure browser compatibility */</xsl:comment>
    </script>
  </xsl:template>

  <xsl:template name="resolve-uri">
    <xsl:param name="href" />
    <xsl:call-template name="replace">
      <xsl:with-param name="string" select="translate($href, ' ', '')" />
    </xsl:call-template>
  </xsl:template>

  <xsl:template match="advice:*[@compare = 'xsl:vendor']">
    <xsl:value-of select="current()[@compare-with = $vendor]/text()" />
  </xsl:template>

  <xsl:template match="advice:*">
    <xsl:copy-of select="." />
  </xsl:template>

  <xsl:template match="head:title">
    <title>
      <xsl:apply-templates />
    </title>
  </xsl:template>

  <xsl:template match="body:html">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="doc:session.openid">
    <xsl:text>Hi, my name is </xsl:text><xsl:value-of select="document(@href)/message/session/@openid"/><xsl:text>.</xsl:text>
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
    <xsl:apply-templates select="document(@src)" />
  </xsl:template>

  <xsl:template match="page:content">
    <xsl:copy-of select="*" />
  </xsl:template>

  <xsl:template match="page:heading">
    <xsl:element name="{@size}">
      <xsl:apply-templates />
    </xsl:element>
  </xsl:template>

  <xsl:template match="*">
    <xsl:element name="{local-name()}">
      <xsl:apply-templates select="@*" />
      <xsl:apply-templates />
    </xsl:element>
  </xsl:template>

  <xsl:template match="@*">
    <xsl:attribute name="{local-name()}">
      <xsl:call-template name="replace">
        <xsl:with-param name="string" select="." />
      </xsl:call-template>
    </xsl:attribute>
  </xsl:template>

  <xsl:template match="text()">
    <xsl:call-template name="replace">
      <xsl:with-param name="string" select="." />
    </xsl:call-template>
  </xsl:template>

  <xsl:template name="replace">
    <xsl:param name="string" />
    <xsl:variable name="nString">
      <xsl:call-template name="cond">
        <xsl:with-param name="string" select="$string" />
      </xsl:call-template>
    </xsl:variable>
    <xsl:choose>
      <xsl:when test="contains($nString, $closure-token-pre-delimiter)">
        <xsl:variable name="name"
            select="substring-before(substring-before(substring-after($nString, $closure-token-pre-delimiter), $closure-token-post-delimiter), $parameter-list-pre-delimiter)" />
        <xsl:call-template name="replace-vars">
          <xsl:with-param name="value-string" select="substring-before(substring-after($nString, $parameter-list-pre-delimiter), $parameter-list-post-delimiter)" />
          <xsl:with-param name="replace-var-string" select="substring-before(substring-after($advice/advice:*[local-name() = $name]/text(), $replace-parameter-list-pre-delimiter), $replace-parameter-list-post-delimiter)"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:when test="contains($nString, $replace-token-pre-delimiter)">
        <xsl:variable name="name"
            select="substring-before(substring-after($nString, $replace-token-pre-delimiter), $replace-token-pre-delimiter)" />
        <xsl:variable name="replace-with">
          <xsl:apply-templates select="$advice/advice:*[local-name() = $name]" />
        </xsl:variable>
        <xsl:call-template name="replace">
          <xsl:with-param name="string"
              select="concat(substring-before($nString, concat($replace-token-pre-delimiter, $name)), $replace-with, substring-after($nString, concat($name, $replace-token-pre-delimiter)))" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$nString" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="cond">
    <xsl:param name="string" />
    <xsl:choose>
      <xsl:when test="contains($string, $cond-token-pre-delimiter)">
        <xsl:variable name="sString" select="translate($string, ' ', '')" />
        <xsl:variable name="conditional"
            select="substring-before(substring-after($sString, $cond-token-pre-delimiter), $cond-token-post-delimiter)" />
        <xsl:variable name="pre-cond" select="substring-before($sString, $cond-token-pre-delimiter)" />
        <xsl:variable name="post-cond"
            select="substring-after($sString, $cond-token-post-delimiter)" />
        <xsl:variable name="if"
            select="substring-before(substring-after($conditional, $cond-if-token), $cond-then-token)" />
        <xsl:variable name="then"
            select="substring-before(substring-after($conditional, $cond-then-token), $cond-else-token)" />
        <xsl:variable name="else" select="substring-after($conditional, $cond-else-token)" />
        <xsl:variable name="nString">
          <xsl:choose>
            <xsl:when test="$advice/advice:*[local-name() = substring-before(substring-after($if, '@@'), '@@')]">
              <xsl:variable name="replace-string">
                <xsl:call-template name="replace">
                  <xsl:with-param name="string" select="$then" />
                </xsl:call-template>
              </xsl:variable>
              <xsl:value-of select="concat($pre-cond, $replace-string, $post-cond)" />
            </xsl:when>
            <xsl:otherwise>
              <xsl:variable name="replace-string">
                <xsl:call-template name="replace">
                  <xsl:with-param name="string" select="$else" />
                </xsl:call-template>
              </xsl:variable>
              <xsl:value-of select="concat($pre-cond, $replace-string, $post-cond)" />
            </xsl:otherwise>
          </xsl:choose>
        </xsl:variable>
        <xsl:call-template name="cond">
          <xsl:with-param name="string" select="$nString" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$string" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="replace-vars">
    <xsl:param name="value-string" />
    <xsl:param name="replace-var-string"/>
    <xsl:variable name="nValue-string">
      <xsl:call-template name="replace">
        <xsl:with-param name="string" select="$value-string" />
      </xsl:call-template>
    </xsl:variable>
    <xsl:variable name="nReplace-var-string">
      <xsl:call-template name="replace">
        <xsl:with-param name="string" select="$replace-var-string" />
      </xsl:call-template>
    </xsl:variable>
    <xsl:variable name="name"
        select="substring-before(substring-after($nValue-string, $parameter-pre-delimiter), $parameter-post-delimiter)" />
    <xsl:variable name="value" select="substring-before(substring-after($nValue-string, concat($parameter-value-assigment-token, $squote)), $squote)"/>
    <xsl:variable name="evaluated-value" select="concat(substring-before($nReplace-var-string, concat($replace-parameter-pre-delimiter, $name)), $value, substring-after($nReplace-var-string, concat($name, $replace-parameter-post-delimiter)))"/>
    <xsl:variable name="next" select="substring-after($nValue-string, $parameter-list-delimeter)"/>
    <xsl:choose>
      <xsl:when test="$next">
        <xsl:call-template name="replace-vars">
          <xsl:with-param name="value-string" select="$next"/>
          <xsl:with-param name="replace-var-string" select="$evaluated-value"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$evaluated-value" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

</xsl:stylesheet>
