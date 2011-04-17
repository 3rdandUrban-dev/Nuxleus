<?xml version="1.0" encoding="UTF-8"?>
<!--
  COPYRIGHT: (c) 2007, M. David Peterson (mailto:m.david@xmlhacker.com; http://mdavid.name/)
  LICENSE: The code contained in this file is licensed under The New BSD License. Please see
  http://www.opensource.org/licenses/bsd-license.php for specific detail.
  Contributors to this code base include, 
  Russ Miles (mailto:aohacker@gmail.com; http://www.russmiles.com/)
-->
<xsl:stylesheet version="1.0" xmlns:html="http://www.w3.org/1999/xhtml" xmlns:render="http://atomictalk.org/render"
    xmlns:debug="http://nuxleus.com/session/debug" xmlns:request="http://nuxleus.com/session/request"
    xmlns:response="http://nuxleus.com/message/response" xmlns:session="http://atomictalk.org/session" xmlns:geo="http://nuxleus.com/geo"
    xmlns:my="http://xameleon.org/my" xmlns:page="http://atomictalk.org/page" xmlns:doc="http://atomictalk.org/feed/doc"
    xmlns:service="http://atomictalk.org/page/service" xmlns:output="http://atomictalk.org/page/output"
    xmlns:head="http://atomictalk.org/page/output/head" xmlns:body="http://atomictalk.org/page/output/body"
    xmlns:advice="http://atomictalk.org/page/advice" xmlns:view="http://atomictalk.org/page/view"
    xmlns:layout="http://atomictalk.org/page/view/layout" xmlns:form="http://atomictalk.org/page/view/form"
    xmlns:menu="http://atomictalk.org/page/view/menu" xmlns:exsl="http://exslt.org/common"
    xmlns:resource="http://atomictalk.org/page/resource" xmlns:model="http://atomictalk.org/page/model"
    xmlns:app="http://purl.org/atom/app#" xmlns:atompub="http://www.w3.org/2007/app" xmlns:atom="http://www.w3.org/2005/Atom"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    exclude-result-prefixes="html exsl my app response advice atom head page service resource output form body view menu model msxsl doc atompub">


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
    <xsl:param name="parameter-list-delimeter" select="','" />
    <xsl:param name="parameter-value-assigment-token" select="'='" />

    <xsl:variable name="session-info" select="document('/service/session/validate-request/')/response:message" />
    <xsl:variable name="request-total" select="$session-info/response:session/@request-total" />
    <xsl:variable name="request-district" select="$session-info/response:session/@request-district" />
    <xsl:variable name="session-name" select="$session-info/response:session/@request-total" />
    <xsl:variable name="session-id" select="$session-info/response:session/@request-district" />
    <xsl:variable name="request-id" select="$session-info/response:request-guid" />
    <xsl:variable name="request-date" select="$session-info/response:request-date" />
    <xsl:variable name="request-time" select="$session-info/response:request-time" />
    <xsl:variable name="geo-ip" select="$session-info/response:geo" />
    <xsl:variable name="ip" select="$geo-ip/response:ip" />
    <xsl:variable name="city" select="$geo-ip/response:city" />
    <xsl:variable name="country" select="$geo-ip/response:country" />
    <xsl:variable name="lat" select="$geo-ip/response:lat" />
    <xsl:variable name="long" select="$geo-ip/response:long" />

    <xsl:variable name="vendor" select="system-property('xsl:vendor')" />
    <xsl:variable name="vendor-uri" select="system-property('xsl:vendor-uri')" />
    <xsl:variable name="page" select="/my:session/my:page" />
    <xsl:variable name="config" select="$page/page:config" />
    <xsl:variable name="browser" select="$config/page:browser[@vendor = $vendor]/@replace" />
    <xsl:variable name="advice" select="$config/page:advice" />
    <xsl:variable name="resource" select="document($page/page:resource/@src)/page:config|$page/page:resource" />
    <xsl:variable name="service" select="document($page/page:service/@src)/page:config|$page/page:service" />
    <xsl:variable name="view" select="document($page/page:view/@src)/page:config|$page/page:view" />
    <xsl:variable name="navigation" select="$session-info/response:navigation" />

    <xsl:variable name="lb">
        <xsl:text>
</xsl:text>
    </xsl:variable>
    <xsl:variable name="quote">"</xsl:variable>
    <xsl:variable name="squote">'</xsl:variable>

    <xsl:strip-space elements="*" />

    <xsl:output cdata-section-elements="script" doctype-system="-//W3C//DTD HTML 4.01//EN"
        doctype-public="http://www.w3.org/TR/html4/strict.dtd" method="html" indent="no" />

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
            <xsl:apply-templates select="head:meta" />
            <xsl:apply-templates select="head:link" />
            <style type="text/css">
                <xsl:apply-templates select="head:include[@fileType = 'css']" />
            </style>
            <xsl:apply-templates select="head:include[@fileType = 'javascript']" />
        </head>
    </xsl:template>

    <xsl:template match="page:body">
        <body>
            <xsl:apply-templates select="body:onload|body:onresize|body:onunload" />
            <xsl:apply-templates select="body:layout" />
        </body>
    </xsl:template>

    <xsl:template match="doc:nav">
        <xsl:apply-templates select="$navigation//response:path/*" mode="navigation" />
    </xsl:template>

		<xsl:template match="render:xslt">
			<div id="{@id}">
				<script type="text/javascript">
				$('#<xsl:value-of select="@id"/>').getTransform('/page/controller/<xsl:value-of select="@controller"/>.xsl', '<xsl:value-of select="@model"/>.xml', 
					{ params:{showModal:'1'},
        		callback: function(){
	        	}
      		});
				</script>
			</div>
		</xsl:template>

    <xsl:template match="response:*" mode="navigation">
        <li>
            <a href="{.}">
                <xsl:value-of select="local-name()" />
            </a>
        </li>
    </xsl:template>

    <xsl:template match="body:onload|body:onresize|body:onunload">
        <xsl:attribute name="{local-name()}">
            <xsl:call-template name="replace">
                <xsl:with-param name="string" select="@action" />
            </xsl:call-template>
        </xsl:attribute>
    </xsl:template>

    <xsl:template match="head:meta">
        <meta>
            <xsl:apply-templates select="@*" />
        </meta>
    </xsl:template>

    <xsl:template match="head:include[@fileType = 'css']">
        <xsl:variable name="uri">
            <xsl:call-template name="resolve-uri">
                <xsl:with-param name="href" select="@href" />
            </xsl:call-template>
        </xsl:variable>
        <xsl:value-of select="concat('@import ', $quote, $uri, $quote, ';')" />
    </xsl:template>

    <xsl:template match="head:include[@fileType = 'javascript' and not(@src)]|view:script[@type = 'javascript' and not(@src)]">
        <script type="text/javascript">
            <xsl:text>//&lt;![CDATA[</xsl:text>
            <xsl:call-template name="replace">
                <xsl:with-param name="string" select="text()" />
            </xsl:call-template>
            <xsl:text>//]]&gt;</xsl:text>
        </script>
    </xsl:template>

    <xsl:template match="head:include[@fileType = 'javascript' and @src]|view:script[@type = 'javascript' and @src]">
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

    <xsl:template match="head:link">
        <link>
            <xsl:apply-templates select="@*" />
        </link>
    </xsl:template>

    <xsl:template match="body:layout">
        <xsl:apply-templates />
    </xsl:template>

    <xsl:template match="layout:view">
        <ul class="list {@style}" id="{@id}">
            <xsl:apply-templates />
        </ul>
    </xsl:template>

    <xsl:template match="session:name">
        <xsl:variable name="name">
            <xsl:choose>
                <xsl:when test="$session-name != 'not-set'">
                    <xsl:value-of select="$session-name" />
                </xsl:when>
                <xsl:otherwise>
                    <xsl:text>amp.fm visitor</xsl:text>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:variable>
        <xsl:value-of select="$name" />
    </xsl:template>

    <xsl:template match="session:log-in-out">
        <xsl:variable name="status">
            <xsl:choose>
                <xsl:when test="$session-name != 'not-set'">
                    <xsl:text>out:/gatekeeper/logout?uname=</xsl:text>
                    <xsl:value-of select="$session-name" />
                    <xsl:text>&amp;status-code=303</xsl:text>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:text>in:/login</xsl:text>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:variable>
        <a href="{substring-after($status, ':')}">
            <xsl:text>Log</xsl:text>
            <xsl:value-of select="substring-before($status, ':')" />
        </a>
    </xsl:template>

    <xsl:template match="geo:map">
        <script type="text/javascript">
            <xsl:text>//&lt;![CDATA[</xsl:text>
          function load() {
            if (GBrowserIsCompatible()) {
              var map = new GMap2(document.getElementById("map"));
              map.setCenter(new GLatLng(<xsl:value-of
                select="$lat" />, <xsl:value-of select="$long" />), 9);
            }
          }
        <xsl:text>//]]&gt;</xsl:text>
        </script>
        <div id="map" style="width:{@width}; height:{@height};margin:0;padding:0;" />
    </xsl:template>

    <xsl:template match="geo:location">
        <xsl:value-of select="$city" />
    </xsl:template>

    <xsl:template match="geo:country">
        <xsl:value-of select="$country" />
    </xsl:template>

    <xsl:template match="geo:lat">
        <xsl:value-of select="$lat" />
    </xsl:template>

    <xsl:template match="geo:long">
        <xsl:value-of select="$long" />
    </xsl:template>

    <xsl:template match="session:id">
        <xsl:value-of select="$session-id" />
    </xsl:template>

    <xsl:template match="session:request-id">
        <xsl:value-of select="$request-id" />
    </xsl:template>

    <xsl:template match="session:request-date">
        <xsl:value-of select="$request-date" />
    </xsl:template>

    <xsl:template match="session:request-time">
        <xsl:value-of select="$request-time" />
    </xsl:template>

    <xsl:template match="geo:ip">
        <xsl:value-of select="$ip" />
    </xsl:template>

    <xsl:template match="doc:local-news" />

    <xsl:template match="doc:local-flickr-photos" />

    <xsl:template match="doc:local-blog-entries" />

    <xsl:template match="*" mode="blogs">
        <xsl:apply-templates mode="blogs" />
    </xsl:template>

    <xsl:template match="atom:title" mode="blogs">
        <h2>
            <a href="{../atom:link[@rel = 'alternate']/@href}">
                <xsl:value-of select="." />
            </a>
        </h2>
    </xsl:template>

    <xsl:template match="atom:content" mode="blogs">
        <p>
            <xsl:value-of select="." />
        </p>
    </xsl:template>

    <xsl:template match="atom:link|atom:author" mode="blogs" />

    <xsl:template match="*" mode="flickr">
        <xsl:apply-templates />
    </xsl:template>

    <xsl:template match="*" mode="message">
        <xsl:apply-templates />
    </xsl:template>

    <xsl:template match="doc:feed">
        <xsl:variable name="href">
            <xsl:call-template name="replace">
                <xsl:with-param name="string" select="@href" />
            </xsl:call-template>
        </xsl:variable>
        <xsl:apply-templates select="document($href)//atom:entry">
            <xsl:with-param name="articleCount" select="@articleCount" />
            <xsl:with-param name="pos" select="position()" />
            <xsl:with-param name="cCount" select="@characterCount" />
            <xsl:with-param name="displayRank" select="@displayRank" />
        </xsl:apply-templates>
    </xsl:template>

    <xsl:template match="doc:profile">
        <xsl:variable name="doc">
            <xsl:call-template name="replace">
                <xsl:with-param name="string" select="@src" />
            </xsl:call-template>
        </xsl:variable>
        <xsl:apply-templates select="document($doc)/profile/*" mode="profile" />
    </xsl:template>

    <xsl:template match="username|openid" mode="profile">
        <h2>
            <xsl:value-of select="." />
        </h2>
    </xsl:template>

    <xsl:template match="doc:location">
        <xsl:call-template name="replace">
            <xsl:with-param name="string" select="@value" />
        </xsl:call-template>
    </xsl:template>

    <xsl:template match="doc:date">
        <xsl:value-of select="document('/date.xml')/date/current" />
    </xsl:template>

    <xsl:template match="doc:session.openid">
        <xsl:choose>
            <xsl:when test="$session-name = 'not-set'">
                <li class="list menu {@style}" id="{@id}">
                    <a href="/login/" title="Log to your amp.fm profile">Login</a>
                </li>
                <li class="list menu {@style}" id="{@id}">
                    <a href="http://openid.amp.fm/signup/">Create Account</a>
                </li>
            </xsl:when>
            <xsl:otherwise>
                <li class="list menu {@style}" id="{@id}">
                    <a href="/service/session/logout" title="Connected as {$session-name}">Logout</a>
                </li>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>

    <xsl:template match="doc:html[@type = 'myspace-events']">
        <xsl:apply-templates select="document(@href)//html:div[@id = current()/@id]" />
    </xsl:template>

    <xsl:template match="doc:html[@type = 'external-html']">
        <xsl:apply-templates select="document(@href)//body:html" />
    </xsl:template>

    <xsl:template match="page:content[@src]">
        <xsl:apply-templates select="document(@src)" />
    </xsl:template>

    <xsl:template match="page:content">
        <xsl:copy-of select="*" />
    </xsl:template>

    <xsl:template match="page:view">
        <ul>
            <xsl:apply-templates />
        </ul>
    </xsl:template>

    <xsl:template match="view:container">
        <ul class="list {@style}" id="{@id}">
            <xsl:apply-templates />
        </ul>
    </xsl:template>

    <xsl:template match="view:item[@src]">
        <li class="list {@style}" id="{@id}">
            <xsl:apply-templates select="document(@src)/view:*" />
        </li>
    </xsl:template>

    <xsl:template match="view:item[not(@src)]">
        <li class="list menu {@style}" id="{@id}">
            <xsl:apply-templates />
        </li>
    </xsl:template>

    <xsl:template match="view:menu[@src]">
        <ul class="list menu {@style}" id="{@id}">
            <xsl:apply-templates select="document(@src)/view:menu" />
        </ul>
    </xsl:template>

    <xsl:template match="view:menu[not(@src)]">
        <ul class="list menu {@style}" id="{@id}">
            <xsl:apply-templates />
        </ul>
    </xsl:template>

    <xsl:template match="view:module[@src]">
        <li class="list {@style}" id="{@id}">
            <xsl:apply-templates select="document(@src)/view:module/*" />
        </li>
    </xsl:template>

    <xsl:template match="view:module[not(@src)]">
        <li class="list {@style}" id="{@id}">
            <xsl:apply-templates />
        </li>
    </xsl:template>

    <xsl:template match="view:menu">
        <xsl:apply-templates />
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

    <xsl:template match="atom:entry">
        <xsl:param name="articleCount" />
        <xsl:param name="pos" />
        <xsl:param name="cCount" />
        <xsl:param name="displayRank" />
        <xsl:apply-templates select="atom:*">
            <xsl:with-param name="cCount" select="$cCount" />
            <xsl:with-param name="displayRank" select="$displayRank" />
        </xsl:apply-templates>
    </xsl:template>

    <xsl:template match="atom:title">
        <xsl:param name="displayRank" />
        <xsl:if test="$displayRank = 'yes'">
            <div class="rank-box">
                <xsl:value-of select="count(../preceding-sibling::atom:entry) + 1" />
            </div>
        </xsl:if>
        <h4>
            <a href="{../atom:link[@rel = 'alternate']/@href}">
                <xsl:value-of select="." />
            </a>
        </h4>
    </xsl:template>
    <xsl:template match="atom:summary" />
    <xsl:template match="atom:published" />
    <xsl:template match="atom:updated" />
    <xsl:template match="atom:generator" />
    <xsl:template match="atom:id" />
    <xsl:template match="atom:category" />
    <xsl:template match="atom:source" />
    <xsl:template match="atom:author" />
    <xsl:template match="atom:content">
        <xsl:param name="cCount" />
        <p style="font-size:small">
            <xsl:copy-of select="substring(., 1, $cCount)" /> ... [<a href="{../atom:link[@rel = 'alternate']/@href}">more</a>]
    </p>
    </xsl:template>

    <xsl:variable name="page.output.head" select="document($page/page:output/page:head/@src)/page:head|$page/page:output/page:head" />
    <xsl:variable name="page.output.body">
        <xsl:choose>
            <xsl:when test="$page/page:output/page:body/@src">
                <xsl:apply-templates select="document($page/page:output/page:body/@src)/page:body" />
            </xsl:when>
            <xsl:otherwise>
                <xsl:apply-templates select="$page/page:output/page:body" />
            </xsl:otherwise>
        </xsl:choose>
    </xsl:variable>

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
                    <xsl:with-param name="value-string"
                        select="substring-before(substring-after($nString, $parameter-list-pre-delimiter), $parameter-list-post-delimiter)" />
                    <xsl:with-param name="replace-var-string"
                        select="substring-before(substring-after($advice//advice:*[local-name() = $name]/text(), $replace-parameter-list-pre-delimiter), $replace-parameter-list-post-delimiter)"
                     />
                </xsl:call-template>
            </xsl:when>
            <xsl:when test="contains($nString, $replace-token-pre-delimiter)">
                <xsl:variable name="name"
                    select="substring-before(substring-after($nString, $replace-token-pre-delimiter), $replace-token-pre-delimiter)" />
                <xsl:variable name="replace-with">
                    <xsl:apply-templates select="$advice//advice:*[local-name() = $name]" />
                </xsl:variable>
                <xsl:call-template name="replace">
                    <xsl:with-param name="string"
                        select="concat(substring-before($nString, concat($replace-token-pre-delimiter, $name)), $replace-with, substring-after($nString, concat($name, $replace-token-pre-delimiter)))"
                     />
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
                <xsl:variable name="post-cond" select="substring-after($sString, $cond-token-post-delimiter)" />
                <xsl:variable name="if" select="substring-before(substring-after($conditional, $cond-if-token), $cond-then-token)" />
                <xsl:variable name="then" select="substring-before(substring-after($conditional, $cond-then-token), $cond-else-token)" />
                <xsl:variable name="else" select="substring-after($conditional, $cond-else-token)" />
                <xsl:variable name="nString">
                    <xsl:choose>
                        <xsl:when test="$advice//advice:*[local-name() = substring-before(substring-after($if, '@@'), '@@')]">
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
        <xsl:param name="replace-var-string" />
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
        <xsl:variable name="value"
            select="substring-before(substring-after($nValue-string, concat($parameter-value-assigment-token, $squote)), $squote)" />
        <xsl:variable name="evaluated-value"
            select="concat(substring-before($nReplace-var-string, concat($replace-parameter-pre-delimiter, $name)), $value, substring-after($nReplace-var-string, concat($name, $replace-parameter-post-delimiter)))" />
        <xsl:variable name="next" select="substring-after($nValue-string, $parameter-list-delimeter)" />
        <xsl:choose>
            <xsl:when test="$next">
                <xsl:call-template name="replace-vars">
                    <xsl:with-param name="value-string" select="$next" />
                    <xsl:with-param name="replace-var-string" select="$evaluated-value" />
                </xsl:call-template>
            </xsl:when>
            <xsl:otherwise>
                <xsl:value-of select="$evaluated-value" />
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>

</xsl:stylesheet>
