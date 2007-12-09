<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform
    version="2.0"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
    xmlns:clitype="http://saxon.sf.net/clitype"
    xmlns:delete="http://xameleon.org/service/atompub/delete"
    xmlns:at="http://atomictalk.org"
    xmlns:func="http://atomictalk.org/function"
    xmlns:http-atompub-utils="clitype:Xameleon.Function.HttpAtompubUtils?partialname=Xameleon"
    xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web"
    xmlns:aspnet-request="clitype:System.Web.HttpRequest?partialname=System.Web"
    xmlns:profile="http://xameleon.org/service/profile"
    xmlns:atom="http://www.w3.org/2005/Atom"
    xmlns:html="http://www.w3.org/1999/xhtml"
    xmlns:request="http://atomictalk.org/function/aspnet/request"
    xmlns:response="http://atomictalk.org/function/aspnet/response"
    xmlns:doc="http://atomictalk.org/feed/doc"
    xmlns:page="http://atomictalk.org/page"
    xmlns:service="http://atomictalk.org/page/service"
    xmlns:output="http://atomictalk.org/page/output"
    xmlns:head="http://atomictalk.org/page/head"
    xmlns:body="http://atomictalk.org/page/body"
    xmlns:advice="http://aspectxml.org/advice"
    xmlns:view="http://atomictalk.org/page/view"
    xmlns:form="http://atomictalk.org/page/view/form"
    xmlns:menu="http://atomictalk.org/page/view/menu"
    xmlns:resource="http://atomictalk.org/page/resource"
    xmlns:property="http://atomictalk.org/page/property"
    xmlns:variable="http://atomictalk.org/page/variable"
    xmlns:model="http://atomictalk.org/page/model"
    xmlns:llup="http://www.x2x2x.org/2005/LLUP"
    xmlns:my="http://xameleon.org/my"
    xmlns:operation="http://xameleon.org/service/operation"
    exclude-result-prefixes="#all">

    <xsl:output
        indent="yes" />

    <xsl:include
        href="../../../functions/funcset-Util.xslt" />
    <xsl:include
        href="../../../functions/aspnet/response-stream.xslt" />

    <xsl:param
        name="current-context" />
    <xsl:param
        name="request" />
    <xsl:param
        name="response" />

    <xsl:template
        match="operation:profile">
        <xsl:apply-templates />
    </xsl:template>

    <xsl:template
        match="operation:profile-add">
        <xsl:apply-templates />
        <xsl:sequence
            select="concat(response:set-status-code($response, 302),
                                 response:set-location($response, '/profile/'))" />
    </xsl:template>

    <xsl:function
        name="profile:is-available">
        <xsl:param
            name="username" />
        <!-- would check our data store of course... -->
        <xsl:value-of
            select="true()" />
    </xsl:function>

    <xsl:function
        name="profile:validate-add">
        <xsl:param
            name="username" />
        <xsl:param
            name="openid" />
        <xsl:variable
            name="errors">
            <profile:errors>
                <xsl:choose>
                    <xsl:when
                        test="not($username) and not(profile:is-available($username))">
                        <profile:error
                            name="username">*</profile:error>
                    </xsl:when>
                    <xsl:when
                        test="$username and not(profile:is-available($username))">
                        <profile:error
                            name="username">* (this name is already used, try again)</profile:error>
                    </xsl:when>
                </xsl:choose>
                <xsl:choose>
                    <xsl:when
                        test="not($openid)">
                        <profile:error
                            name="openid">*</profile:error>
                    </xsl:when>
                </xsl:choose>
            </profile:errors>
        </xsl:variable>

        <xsl:choose>
            <xsl:when
                test="$errors/profile:errors/profile:error">
                <!-- save the errors in the session -->
                <xsl:value-of
                    select="false()" />
            </xsl:when>
            <xsl:otherwise>
                <xsl:value-of
                    select="true()" />
            </xsl:otherwise>
        </xsl:choose>

    </xsl:function>

    <xsl:template
        match="profile:add-user">
        <xsl:variable
            name="username"
            select="func:resolve-variable(@username)" />
        <xsl:variable
            name="openid"
            select="func:resolve-variable(@openid)" />
        <xsl:variable
            name="session"
            select="func:resolve-variable(@session-id)" />

        <xsl:if
            test="profile:validate-add(.)">
            <!-- we write the profile here -->
        </xsl:if>
    </xsl:template>


</xsl:transform>
