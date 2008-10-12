<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:my="http://xameleon.org/my" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:saxon="http://saxon.sf.net/" xmlns:clitype="http://saxon.sf.net/clitype" xmlns:add="http://xameleon.org/service/atompub/add" xmlns:at="http://atomictalk.org" xmlns:func="http://atomictalk.org/function" xmlns:http-atompub-utils="clitype:Xameleon.Function.HttpAtompubUtils?partialname=Xameleon" xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web" xmlns:aspnet-request="clitype:System.Web.HttpRequest?partialname=System.Web" xmlns:guid="clitype:Xameleon.Function.Utils?partialname=Xameleon" xmlns:sguid="clitype:System.Guid?partialname=mscorlib" xmlns:file-stream="clitype:Xameleon.Function.HttpFileStream?partialname=Xameleon" xmlns:atompub="http://xameleon.org/service/atompub" xmlns:atom="http://www.w3.org/2005/Atom" xmlns:html="http://www.w3.org/1999/xhtml" xmlns:request="http://atomictalk.org/function/aspnet/request" xmlns:response="http://atomictalk.org/function/aspnet/response" version="2.0" exclude-result-prefixes="xs xsl xsi fn clitype at func http-atompub-utils aspnet-context aspnet-request add atompub saxon html response">
	<xsl:import href="../../base.xslt"/>
	
	<xsl:output name="xml" method="xml"/>
	<xsl:variable name="content-type" select="aspnet-request:ContentType($request)"/>
	<xsl:variable name="request-uri" select="aspnet-request:Url($request)"/>
	<xsl:variable name="browser" select="aspnet-request:Browser($request)"/>
	<xsl:variable name="user-agent" select="aspnet-request:UserAgent($request)"/>
	
	<xsl:template match="atompub:add">
		<xsl:apply-templates>
			<xsl:with-param name="content-type" select="func:resolve-variable(@content-type)"/>
			<xsl:with-param name="content-length" select="func:resolve-variable(@content-length)"/>
		</xsl:apply-templates>
	</xsl:template>
	
	<xsl:template match="add:entry">
		<xsl:param name="content-type"/>
		<xsl:param name="content-length"/>
		<xsl:variable name="title" select="func:resolve-variable(@slug)"/>
		<xsl:variable name="slug" select="func:resolve-variable(@slug)"/>
		<xsl:variable name="author" select="func:resolve-variable(@author)"/>
		<xsl:variable name="base_uri" select="func:resolve-variable(@uri)"/>
		<xsl:variable name="base_path" select="func:resolve-variable(@path)"/>
		<xsl:variable name="filename" select="string(sguid:NewGuid())"/>
		<xsl:variable name="application-path" select="request:get-physical-application-path()"/>
		<xsl:variable name="member_path" select="concat('/member/', $base_path)"/>
		<xsl:variable name="entry_path" select="concat($member_path, 'comment/', $filename)"/>
		<xsl:variable name="file" select="resolve-uri(concat($application-path, $entry_path, '/entry.atom'))"/>
		<xsl:variable name="uri" select="concat($base_uri, $entry_path)"/>
		<xsl:variable name="entry-uri" select="concat($uri, '.atom')"/>
		<xsl:variable name="application-root" select="request:get-physical-application-path()"/>
		<xsl:variable name="member-dir-root" select="'/member/'"/>
		<xsl:variable name="member-directory" select="concat($member-dir-root, $base_path, '/')"/>
		<xsl:variable name="index" select="resolve-uri(concat($application-path, $entry_path, '/index.page'))"/>
		<xsl:variable name="comment-atom-entry">
			<atom:entry>
				<atom:id>
					<xsl:value-of select="$uri"/>
				</atom:id>
				<atom:title type="text">
					<xsl:value-of select="$title"/>
				</atom:title>
				<atom:published>
					<xsl:value-of select="fn:current-dateTime()"/>
				</atom:published>
				<atom:updated>
					<xsl:value-of select="fn:current-dateTime()"/>
				</atom:updated>
				<atom:link rel="self" href="{$entry-uri}" type="application/atom+xml;type=entry"/>
				<atom:link rel="alternate" href="{concat($base_uri, $base_path)}" type="text/html"/>
				<atom:author>
					<atom:name>
						<xsl:value-of select="$author"/>
					</atom:name>
				</atom:author>
				<atom:content type="xhtml">
					<xsl:apply-templates/>
				</atom:content>
			</atom:entry>
		</xsl:variable>
		
		<message>
			<xsl:copy-of select="$session-params"/>
			<xsl:value-of select="concat('filename: ', $filename)"/>
			<xsl:value-of select="concat('base uri: ', $base_uri)"/>
			<xsl:value-of select="concat('base path: ', $base_uri)"/>
			<xsl:value-of select="concat('entry path: ', $entry_path)"/>
			<xsl:value-of select="concat('physical application path: ', request:get-physical-application-path())"/>
			<xsl:value-of select="concat('file: ', $file)"/>
			<xsl:value-of select="concat('index: ', $index)"/>
		</message>
		
		<xsl:result-document href="{$file}" format="xml">
			<xsl:copy-of select="$comment-atom-entry"/>
		</xsl:result-document>
		<xsl:variable name="set-status-code" select="response:set-status-code($response, 303)"/>
		<xsl:variable name="set-location" select="response:set-location($response, $entry_path)"/>
		<!--<redirect>
			<status-code>
				<xsl:sequence select="$set-status-code"/>
			</status-code>
			<location>
				<xsl:sequence select="$set-location"/>
			</location>
		</redirect>-->
<!--<xsl:result-document href="{resolve-uri('file:///Users/sylvain/dev/nuxleus/Web/Development/ume/comments.atom')}" format="xml">
      <atom:feed>
        <atom:id>
          <xsl:value-of select="$uri"/>
        </atom:id>
        <atom:title type="text">
          <xsl:value-of select="$title"/>
        </atom:title>
        <atom:published>
          <xsl:value-of select="datetime:dateTime()"/>
        </atom:published>
        <atom:updated>
          <xsl:value-of select="datetime:dateTime()"/>
        </atom:updated>
        <atom:link rel="self" href="{concat($uri, '.atom')}"
          type="application/atom+xml;type=entry"/>
        <atom:link rel="alternate" href="{$base_uri}" type="text/html"/>
        <atom:author>
          <atom:name>
            <xsl:value-of select="$author"/>
          </atom:name>
        </atom:author>
        <xsl:copy-of select="$comment-atom-entry"/>
        <xsl:apply-templates select="document('file:///Users/sylvain/dev/nuxleus/Web/Development/ume/comment-collection.xml')//catalog/doc" mode="catalog"/>
      </atom:feed>
    </xsl:result-document>-->
	</xsl:template>
<!--  <xsl:template match="*" mode="catalog">
    <foo>bar</foo>
    <xsl:apply-templates select="document(resolve-uri(@href, 'file:///Users/sylvain/dev/nuxleus/Web/Development/ume/'))" mode="atom-entry"/>
  </xsl:template>
  
  <xsl:template match="atom:entry" mode="atom-entry">
    <bar>foo</bar>
    <xsl:copy-of select="."/>
  </xsl:template>-->
	<xsl:template match="add:request-body">
		<xsl:variable name="content" select="func:resolve-variable(.)"/>
		<xsl:copy-of select="$content"/>
<!--<xsl:sequence
      select="concat(response:set-status-code($response, 302), response:set-location($response, 'http://www.google.com'))"
    />-->
	</xsl:template>
	<xsl:template match="atompub:update">
		<xsl:sequence select="func:update()"/>
	</xsl:template>
	<xsl:template match="atompub:delete">
		<xsl:sequence select="func:delete()"/>
	</xsl:template>
	<xsl:function name="func:add">
		<xsl:value-of select="concat('entry added', ' content type=', $content-type)"/>
	</xsl:function>
	<xsl:function name="func:update">
		<xsl:value-of select="concat('entry updated', ' content type=', $content-type)"/>
	</xsl:function>
	<xsl:function name="func:delete">
		<xsl:value-of select="concat('entry deleted', ' content type=', $content-type)"/>
	</xsl:function>
</xsl:transform>
