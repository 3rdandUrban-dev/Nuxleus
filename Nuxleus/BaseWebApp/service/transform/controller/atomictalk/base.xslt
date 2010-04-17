<?xml version="1.0"?>
<!--
  COPYRIGHT: (c) 2007, M. David Peterson (mailto:m.david@xmlhacker.com; http://mdavid.name/)
  LICENSE: The code contained in this file is licensed under The New BSD License. Please see
  http://www.opensource.org/licenses/bsd-license.php for specific detail.
  Contributors to this code base include, 
  Russ Miles (mailto:aohacker@gmail.com; http://www.russmiles.com/)
-->
<xsl:stylesheet xmlns:html="http://www.w3.org/1999/xhtml" xmlns:render="http://atomictalk.org/render" xmlns:debug="http://nuxleus.com/session/debug" xmlns:request-collection="clitype:Xameleon.Function.HttpRequestCollection?partialname=Xameleon" xmlns:request="http://atomictalk.org/function/aspnet/request" xmlns:session="http://atomictalk.org/session" xmlns:geo="http://nuxleus.com/geo" xmlns:my="http://xameleon.org/my" xmlns:page="http://atomictalk.org/page" xmlns:doc="http://atomictalk.org/feed/doc" xmlns:service="http://atomictalk.org/page/service" xmlns:output="http://atomictalk.org/page/output" xmlns:head="http://atomictalk.org/page/output/head" xmlns:body="http://atomictalk.org/page/output/body" xmlns:advice="http://atomictalk.org/page/advice" xmlns:view="http://atomictalk.org/page/view" xmlns:layout="http://atomictalk.org/page/view/layout" xmlns:form="http://atomictalk.org/page/view/form" xmlns:menu="http://atomictalk.org/page/view/menu" xmlns:exsl="http://exslt.org/common" xmlns:resource="http://atomictalk.org/page/resource" xmlns:model="http://atomictalk.org/page/model" xmlns:app="http://purl.org/atom/app#" xmlns:atompub="http://www.w3.org/2007/app" xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web" xmlns:response="http://atomictalk.org/function/aspnet/response" xmlns:atom="http://www.w3.org/2005/Atom" xmlns:func="http://atomictalk.org/function" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="2.0">
	<xsl:import href="../base.xslt"/>
	<xsl:param name="current-context"/>
	<xsl:param name="response" select="aspnet-context:Response($current-context)"/>
	<xsl:param name="request" select="aspnet-context:Request($current-context)"/>
	<xsl:param name="server" select="aspnet-context:Server($current-context)"/>
	<xsl:param name="session" select="aspnet-context:Session($current-context)"/>
	<xsl:param name="timestamp" select="aspnet-context:Timestamp($current-context)"/>
	<xsl:param name="session-params"/>
	<xsl:variable name="application-root" select="request:get-physical-application-path()"/>
	<xsl:variable name="session-info" select="document('/service/session/validate-request/')/message"/>
	<xsl:variable name="session-name" select="$session-info/session/@openid"/>
	<xsl:variable name="geo-ip" select="document('/ipgeolocator/geocode')/location"/>
	<xsl:variable name="lat" select="substring-before($geo-ip/point, ' ')"/>
	<xsl:variable name="long" select="substring-after($geo-ip/point, ' ')"/>
	<xsl:variable name="location" select="$geo-ip/city"/>
	<xsl:param name="closure-token-pre-delimiter" select="'|@@'"/>
	<xsl:param name="closure-token-post-delimiter" select="'@@|'"/>
	<xsl:param name="replace-token-pre-delimiter" select="'@@'"/>
	<xsl:param name="replace-token-post-delimiter" select="'@@'"/>
	<xsl:param name="cond-token-pre-delimiter" select="'|$$'"/>
	<xsl:param name="cond-token-post-delimiter" select="'$$|'"/>
	<xsl:param name="cond-if-token" select="'test:'"/>
	<xsl:param name="cond-then-token" select="'IfTrue:'"/>
	<xsl:param name="cond-else-token" select="'IfFalse:'"/>
	<xsl:param name="system-variable-pre-delimiter" select="'%%'"/>
	<xsl:param name="system-variable-post-delimiter" select="'%%'"/>
	<xsl:param name="parameter-list-pre-delimiter" select="'($'"/>
	<xsl:param name="parameter-list-post-delimiter" select="'$)'"/>
	<xsl:param name="parameter-pre-delimiter" select="':'"/>
	<xsl:param name="parameter-post-delimiter" select="':'"/>
	<xsl:param name="replace-parameter-list-pre-delimiter" select="'[$'"/>
	<xsl:param name="replace-parameter-list-post-delimiter" select="'$]'"/>
	<xsl:param name="replace-parameter-pre-delimiter" select="':'"/>
	<xsl:param name="replace-parameter-post-delimiter" select="':'"/>
	<xsl:param name="parameter-list-delimeter" select="','"/>
	<xsl:param name="parameter-value-assigment-token" select="'='"/>
	<xsl:variable name="vendor" select="system-property('xsl:vendor')"/>
	<xsl:variable name="vendor-uri" select="system-property('xsl:vendor-uri')"/>
	<xsl:variable name="page" select="/my:session/my:page"/>
	<xsl:variable name="config" select="$page/page:config"/>
	<xsl:variable name="page.output.config">
		<xsl:choose>
			<xsl:when test="$page/page:config/@src">
				<xsl:apply-templates select="$page/page:config"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:apply-templates select="$page/page:output/page:body"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<xsl:variable name="page.output.head" select="document(resolve-uri(concat($application-root, $page/page:output/page:head/@src)))/page:head/*|$page/page:output/page:head/*"/>
	<xsl:variable name="browser" select="$config/page:browser[@vendor = $vendor]/@replace"/>
	<xsl:variable name="advice" select="$config/page:advice"/>
	<xsl:variable name="resource" select="document($page/page:resource/@src)/page:config|$page/page:resource"/>
	<xsl:variable name="service" select="document($page/page:service/@src)/page:config|$page/page:service"/>
	<xsl:variable name="view" select="document($page/page:view/@src)/page:config|$page/page:view"/>
	<xsl:variable name="page.output.body">
		<xsl:choose>
			<xsl:when test="$page/page:output/page:body/@src">
				<xsl:apply-templates select="document(resolve-uri(concat($application-root,$page/page:output/page:body/@src)))/page:body"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:apply-templates select="$page/page:output/page:body"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<xsl:variable name="lb">
		<xsl:text/>
	</xsl:variable>
	<xsl:variable name="q">"</xsl:variable>
	<xsl:variable name="sq">'</xsl:variable>
	<xsl:variable name="base-uri">
		<xsl:choose>
			<xsl:when test="$page[@xml:base]">
				<xsl:value-of select="$page/@xml:base"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:call-template name="replace">
					<xsl:with-param name="string" select="$advice/advice:*[local-name() = 'base-uri']/*"/>
				</xsl:call-template>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<xsl:strip-space elements="*"/>
	<xsl:output cdata-section-elements="script" method="xml" indent="no"/>
	<xsl:template match="/my:session">
		<xsl:processing-instruction name="xml-stylesheet">
			<xsl:value-of select="concat('type=', $q, 'text/xsl', $q, ' ', 'href=', $q, '/page/controller/atomictalk/base.xsl', $q)"/>
		</xsl:processing-instruction>
		<xsl:variable name="content-type" select="response:set-content-type($response, 'text/xml')"/>
		<my:session content-type="{if(not(empty($content-type))) then 'text/xml' else 'not-set'}">
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates/>
		</my:session>
	</xsl:template>
	<xsl:template match="debug:info|render:*">
		<xsl:copy-of select="."/>
	</xsl:template>
	<xsl:template match="geo:*">
		<xsl:copy-of select="."/>
	</xsl:template>
	<xsl:template match="my:page">
		<my:page>
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates/>
		</my:page>
	</xsl:template>
	<xsl:template match="page:output">
		<page:output>
			<page:head>
				<xsl:copy-of select="$page.output.head"/>
			</page:head>
			<xsl:copy-of select="$page.output.body"/>
		</page:output>
	</xsl:template>
	<xsl:template match="page:config">
		<page:config>
			<page:advice>
				<xsl:apply-templates select="document(resolve-uri(concat($application-root,$page/page:config/@src)))/page:config/page:advice/*" mode="resolve"/>
				<xsl:apply-templates select="page:advice/*"/>
				<xsl:variable name="search" select="request-collection:GetValue($request, 'query-string', 'search')"/>
				<advice:search.location>
					<xsl:value-of select="if(not(empty($search)) and $search != 'not-set') then $search else 'local'"/>
				</advice:search.location>
			</page:advice>
		</page:config>
	</xsl:template>
	<xsl:template match="page:head">
		<page:head>
			<xsl:copy-of select="*"/>
		</page:head>
	</xsl:template>
	<xsl:template match="page:body">
		<page:body>
			<xsl:apply-templates/>
		</page:body>
	</xsl:template>
	<xsl:template match="body:onload|body:onresize|body:onunload|head:include|head:title|head:link|body:layout">
		<xsl:element name="{name()}">
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template name="resolve-uri">
		<xsl:param name="href"/>
		<xsl:call-template name="replace">
			<xsl:with-param name="string" select="translate($href, ' ', '')"/>
		</xsl:call-template>
	</xsl:template>
	<xsl:template match="advice:*" mode="resolve">
		<xsl:element name="{name()}">
			<xsl:copy-of select="@*"/>
			<xsl:call-template name="replace">
				<xsl:with-param name="string" select="."/>
			</xsl:call-template>
		</xsl:element>
	</xsl:template>
	<xsl:template match="advice:*">
		<xsl:copy-of select="."/>
	</xsl:template>
	<xsl:template match="session:*">
		<xsl:copy-of select="."/>
	</xsl:template>
	<xsl:template match="geo:map">
		<xsl:copy-of select="."/>
	</xsl:template>
	<xsl:template match="geo:location">
		<xsl:copy-of select="."/>
	</xsl:template>
	<xsl:template match="doc:*">
		<xsl:copy-of select="."/>
	</xsl:template>
	<xsl:template match="*" mode="message">
		<xsl:element name="{name()}">
			<xsl:copy-of select="@*"/>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="view:module[@src]|view:item[@src]">
		<xsl:apply-templates select="document(resolve-uri(concat($application-root,@src)))"/>
	</xsl:template>
	<xsl:template match="page:*|view:*|page:heading">
		<xsl:element name="{name()}">
			<xsl:copy-of select="@*"/>
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
	<xsl:template name="replace">
		<xsl:param name="string"/>
		<xsl:variable name="nString">
			<xsl:call-template name="cond">
				<xsl:with-param name="string" select="$string"/>
			</xsl:call-template>
		</xsl:variable>
		<xsl:choose>
			<xsl:when test="contains($nString, $closure-token-pre-delimiter)">
				<xsl:variable name="name" select="substring-before(substring-before(substring-after($nString, $closure-token-pre-delimiter), $closure-token-post-delimiter), $parameter-list-pre-delimiter)"/>
				<xsl:call-template name="replace-vars">
					<xsl:with-param name="value-string" select="substring-before(substring-after($nString, $parameter-list-pre-delimiter), $parameter-list-post-delimiter)"/>
					<xsl:with-param name="replace-var-string" select="substring-before(substring-after($advice/advice:*[local-name() = $name]/text(), $replace-parameter-list-pre-delimiter), $replace-parameter-list-post-delimiter)"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:when test="contains($nString, $replace-token-pre-delimiter)">
				<xsl:variable name="name" select="substring-before(substring-after($nString, $replace-token-pre-delimiter), $replace-token-pre-delimiter)"/>
				<xsl:variable name="replace-with">
					<xsl:apply-templates select="$advice/advice:*[local-name() = $name]"/>
				</xsl:variable>
				<xsl:call-template name="replace">
					<xsl:with-param name="string" select="concat(substring-before($nString, concat($replace-token-pre-delimiter, $name)), $replace-with, substring-after($nString, concat($name, $replace-token-pre-delimiter)))"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$nString"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="cond">
		<xsl:param name="string"/>
		<xsl:choose>
			<xsl:when test="contains($string, $cond-token-pre-delimiter)">
				<xsl:variable name="sString" select="translate($string, ' ', '')"/>
				<xsl:variable name="conditional" select="substring-before(substring-after($sString, $cond-token-pre-delimiter), $cond-token-post-delimiter)"/>
				<xsl:variable name="pre-cond" select="substring-before($sString, $cond-token-pre-delimiter)"/>
				<xsl:variable name="post-cond" select="substring-after($sString, $cond-token-post-delimiter)"/>
				<xsl:variable name="if" select="substring-before(substring-after($conditional, $cond-if-token), $cond-then-token)"/>
				<xsl:variable name="then" select="substring-before(substring-after($conditional, $cond-then-token), $cond-else-token)"/>
				<xsl:variable name="else" select="substring-after($conditional, $cond-else-token)"/>
				<xsl:variable name="nString">
					<xsl:choose>
						<xsl:when test="$advice/advice:*[local-name() = substring-before(substring-after($if, '@@'), '@@')]">
							<xsl:variable name="replace-string">
								<xsl:call-template name="replace">
									<xsl:with-param name="string" select="$then"/>
								</xsl:call-template>
							</xsl:variable>
							<xsl:value-of select="concat($pre-cond, $replace-string, $post-cond)"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:variable name="replace-string">
								<xsl:call-template name="replace">
									<xsl:with-param name="string" select="$else"/>
								</xsl:call-template>
							</xsl:variable>
							<xsl:value-of select="concat($pre-cond, $replace-string, $post-cond)"/>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				<xsl:call-template name="cond">
					<xsl:with-param name="string" select="$nString"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$string"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="replace-vars">
		<xsl:param name="value-string"/>
		<xsl:param name="replace-var-string"/>
		<xsl:variable name="nValue-string">
			<xsl:call-template name="replace">
				<xsl:with-param name="string" select="$value-string"/>
			</xsl:call-template>
		</xsl:variable>
		<xsl:variable name="nReplace-var-string">
			<xsl:call-template name="replace">
				<xsl:with-param name="string" select="$replace-var-string"/>
			</xsl:call-template>
		</xsl:variable>
		<xsl:variable name="name" select="substring-before(substring-after($nValue-string, $parameter-pre-delimiter), $parameter-post-delimiter)"/>
		<xsl:variable name="value" select="substring-before(substring-after($nValue-string, concat($parameter-value-assigment-token, $sq)), $sq)"/>
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
				<xsl:value-of select="$evaluated-value"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>
<!-- <xsl:template match="my:session">
    <xsl:param name="member-directory"/>
    <xsl:processing-instruction name="xml-stylesheet">
      <xsl:value-of select="concat('type=', $q, 'text/xsl', $q, ' ', 'href=', $q, '/page/controller/atomictalk/base.xsl', $q)"/>
    </xsl:processing-instruction>
    <xsl:element name="{name()}">
      <xsl:copy-of select="@*"/>
      <xsl:apply-templates>
        <xsl:with-param name="member-directory" select="$member-directory"/>
      </xsl:apply-templates>
    </xsl:element>
  </xsl:template>

  <xsl:template match="my:*">
    <xsl:param name="member-directory"/>
    <xsl:element name="{concat('my:', local-name())}">
      <xsl:copy-of select="@*"/>
      <xsl:apply-templates>
        <xsl:with-param name="member-directory" select="$member-directory"/>
      </xsl:apply-templates>
    </xsl:element>
  </xsl:template>

  <xsl:template match="page:config|page:output|page:head|page:body|head:include">
    <xsl:param name="member-directory"/>
    <xsl:element name="{concat('page:', local-name())}" namespace="{namespace-uri(.)}">
      <xsl:copy-of select="@*"/>
      <xsl:apply-templates>
        <xsl:with-param name="member-directory" select="$member-directory"/>
      </xsl:apply-templates>
    </xsl:element>
  </xsl:template>

  <xsl:template match="page:advice">
    <xsl:param name="member-directory"/>
    <page:advice>
      <xsl:copy-of select="@*"/>
      <xsl:copy-of select="advice:*"/>
      <advice:current.location>
        <xsl:value-of select="$member-directory"/>
      </advice:current.location>
    </page:advice>
  </xsl:template> -->
