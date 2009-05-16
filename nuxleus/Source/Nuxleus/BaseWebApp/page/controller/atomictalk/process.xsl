<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:my="http://xameleon.org/my"
    xmlns:page="http://atomictalk.org/page" xmlns:advice="http://atomictalk.org/page/advice"
    version="1.0">
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
    <xsl:variable name="config"
        select="document($page/page:config/@src)/page:config|$page/page:config"/>
    <xsl:variable name="browser" select="$config/page:browser[@vendor = $vendor]/@replace"/>
    <xsl:variable name="advice" select="$config/page:advice"/>
    <xsl:variable name="resource"
        select="document($page/page:resource/@src)/page:config|$page/page:resource"/>
    <xsl:variable name="service"
        select="document($page/page:service/@src)/page:config|$page/page:service"/>
    <xsl:variable name="view" select="document($page/page:view/@src)/page:config|$page/page:view"/>

    <xsl:variable name="lb">
        <xsl:text>
</xsl:text>
    </xsl:variable>
    <xsl:variable name="quote">"</xsl:variable>
    <xsl:variable name="squote">'</xsl:variable>

    <xsl:variable name="base-uri">
        <xsl:choose>
            <xsl:when test="$page[@xml:base]">
                <xsl:value-of select="$page/@xml:base"/>
            </xsl:when>
            <xsl:otherwise>
                <xsl:call-template name="replace">
                    <xsl:with-param name="string"
                        select="$advice/advice:*[local-name() = 'base-uri']/*"/>
                </xsl:call-template>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:variable>

    <xsl:variable name="page.output.head"
        select="document($page/page:output/page:head/@src)/page:head|$page/page:output/page:head"/>
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
    
    
    <xsl:template name="replace">
        <xsl:param name="string"/>
        <xsl:variable name="nString">
            <xsl:call-template name="cond">
                <xsl:with-param name="string" select="$string"/>
            </xsl:call-template>
        </xsl:variable>
        <xsl:choose>
            <xsl:when test="contains($nString, $closure-token-pre-delimiter)">
                <xsl:variable name="name"
                    select="substring-before(substring-before(substring-after($nString, $closure-token-pre-delimiter), $closure-token-post-delimiter), $parameter-list-pre-delimiter)"/>
                <xsl:call-template name="replace-vars">
                    <xsl:with-param name="value-string"
                        select="substring-before(substring-after($nString, $parameter-list-pre-delimiter), $parameter-list-post-delimiter)"/>
                    <xsl:with-param name="replace-var-string"
                        select="substring-before(substring-after($advice/advice:*[local-name() = $name]/text(), $replace-parameter-list-pre-delimiter), $replace-parameter-list-post-delimiter)"
                    />
                </xsl:call-template>
            </xsl:when>
            <xsl:when test="contains($nString, $replace-token-pre-delimiter)">
                <xsl:variable name="name"
                    select="substring-before(substring-after($nString, $replace-token-pre-delimiter), $replace-token-pre-delimiter)"/>
                <xsl:variable name="replace-with">
                    <xsl:apply-templates select="$advice/advice:*[local-name() = $name]"/>
                </xsl:variable>
                <xsl:call-template name="replace">
                    <xsl:with-param name="string"
                        select="concat(substring-before($nString, concat($replace-token-pre-delimiter, $name)), $replace-with, substring-after($nString, concat($name, $replace-token-pre-delimiter)))"
                    />
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
                <xsl:variable name="conditional"
                    select="substring-before(substring-after($sString, $cond-token-pre-delimiter), $cond-token-post-delimiter)"/>
                <xsl:variable name="pre-cond" select="substring-before($sString, $cond-token-pre-delimiter)"/>
                <xsl:variable name="post-cond"
                    select="substring-after($sString, $cond-token-post-delimiter)"/>
                <xsl:variable name="if"
                    select="substring-before(substring-after($conditional, $cond-if-token), $cond-then-token)"/>
                <xsl:variable name="then"
                    select="substring-before(substring-after($conditional, $cond-then-token), $cond-else-token)"/>
                <xsl:variable name="else" select="substring-after($conditional, $cond-else-token)"/>
                <xsl:variable name="nString">
                    <xsl:choose>
                        <xsl:when
                            test="$advice/advice:*[local-name() = substring-before(substring-after($if, '@@'), '@@')]">
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
        <xsl:variable name="name"
            select="substring-before(substring-after($nValue-string, $parameter-pre-delimiter), $parameter-post-delimiter)"/>
        <xsl:variable name="value"
            select="substring-before(substring-after($nValue-string, concat($parameter-value-assigment-token, $squote)), $squote)"/>
        <xsl:variable name="evaluated-value"
            select="concat(substring-before($nReplace-var-string, concat($replace-parameter-pre-delimiter, $name)), $value, substring-after($nReplace-var-string, concat($name, $replace-parameter-post-delimiter)))"/>
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
