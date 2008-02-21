<?xml version="1.0"?>
<xsl:transform version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:func="http://atomictalk.org/function"
  xmlns:saxon="http://saxon.sf.net/" xmlns:aws="http://xameleon.org/function/aws"
  xmlns:service="http://xameleon.org/service"
  xmlns:operation="http://xameleon.org/service/operation"
  xmlns:param="http://xameleon.org/service/session/param"
  xmlns:sdb="http://xameleon.org/function/aws/sdb"
  xmlns:sdb.function="clitype:Xameleon.Function.AWSSimpleDB?partialname=Nuxleus.Extension.Aws.Sdb"
  xmlns:array-list="clitype:System.Collections.ArrayList?partialname=mscorlib"
  xmlns:clitype="http://saxon.sf.net/clitype" exclude-result-prefixes="#all">

  <xsl:import href="../../../../functions/funcset-Util.xslt"/>

  <xsl:variable name="sdb" select="sdb.function:new()"/>

  <xsl:template match="aws:sdb">
    <sdb>
      <xsl:apply-templates/>
    </sdb>
  </xsl:template>

  <xsl:template match="sdb:create-domain">
    <xsl:variable name="domain" select="func:resolve-variable(@domain)"/>
    <create-domain name="{$domain}">
      <xsl:sequence select="func:sdb-create-domain($domain)"/>
    </create-domain>
  </xsl:template>

  <xsl:template match="sdb:create-attribute">
    <xsl:variable name="domain" select="func:resolve-variable(@domain)"/>
    <xsl:variable name="item" select="func:resolve-variable(@item)"/>
    <xsl:variable name="attNameList"
      select="tokenize(func:resolve-variable(func:uri-decode(@attNameList)), '\|')"/>
    <xsl:variable name="attValueList"
      select="tokenize(func:resolve-variable(func:uri-decode(@attValueList)), '\|')"/>
    <create-attribute domain="{$domain}" item="{$item}">
      <xsl:sequence select="func:sdb-create-attribute($domain, $item, $attNameList, $attValueList)"
      />
    </create-attribute>
  </xsl:template>

  <xsl:template match="sdb:delete-attribute">
    <xsl:variable name="domain" select="func:resolve-variable(@domain)"/>
    <xsl:variable name="item" select="func:resolve-variable(@item)"/>
    <xsl:variable name="attNameList" select="tokenize(func:resolve-variable(@attNameList), '\|')"/>
    <xsl:variable name="attValueList" select="tokenize(func:resolve-variable(@attValueList), '\|')"/>
    <delete-attribute name="{$domain}">
      <xsl:sequence select="func:sdb-delete-attribute($domain, $item, $attNameList, $attValueList)"
      />
    </delete-attribute>
  </xsl:template>

  <xsl:template match="sdb:delete-domain">
    <!-- Not sure how we should do this, but putting this out in the open is obviously a bad idea.
      <xsl:variable name="domain" select="func:resolve-variable(@domain)"/>
      <delete-domain name="{$domain}">
        <xsl:sequence select="func:sdb-delete-domain($domain)"/>
      </delete-domain>
    -->
  </xsl:template>

  <xsl:template match="sdb:get-attribute">
    <xsl:variable name="domain" select="func:resolve-variable(@domain)"/>
    <xsl:variable name="item" select="func:resolve-variable(@item)"/>
    <get-attribute name="{$domain}">
      <xsl:sequence select="func:sdb-get-attribute($domain, $item)"/>
    </get-attribute>
  </xsl:template>

  <xsl:template match="sdb:query-domain">
    <xsl:variable name="domain" select="func:resolve-variable(@domain)"/>
    <xsl:variable name="attName" select="func:resolve-variable(@attName)"/>
    <xsl:variable name="attValue" select="func:resolve-variable(@attValue)"/>
    <query-domain name="{$domain}">
      <xsl:sequence select="func:sdb-query-domain($domain, $attName, $attValue)"/>
    </query-domain>
  </xsl:template>

  <xsl:function name="func:sdb-create-domain">
    <xsl:param name="domain" as="xs:string"/>
    <xsl:sequence select="sdb.function:CreateDomain($sdb, $domain)"/>
  </xsl:function>

  <xsl:function name="func:sdb-create-attribute">
    <xsl:param name="domain" as="xs:string"/>
    <xsl:param name="item" as="xs:string"/>
    <xsl:param name="attNameList"/>
    <xsl:param name="attValueList"/>
    <xsl:variable name="attArrayList" select="sdb.function:GetArrayList($sdb)"/>
    <result>
      <attribute-list>
        <xsl:sequence
          select="func:add-items-to-array-list($attArrayList, $attNameList, $attValueList)"/>
      </attribute-list>
      <xsl:sequence select="sdb.function:PutAttributes($sdb, $domain, $item, $attArrayList)"/>
    </result>
  </xsl:function>

  <xsl:function name="func:sdb-delete-attribute">
    <xsl:param name="domain" as="xs:string"/>
    <xsl:param name="item" as="xs:string"/>
    <xsl:param name="attNameList"/>
    <xsl:param name="attValueList"/>
    <xsl:variable name="attArrayList" select="sdb.function:GetArrayList($sdb)"/>
    <result>
      <attribute-list>
        <xsl:sequence
          select="func:add-items-to-array-list($attArrayList, $attNameList, $attValueList)"/>
      </attribute-list>
      <xsl:sequence select="sdb.function:DeleteAttribute($sdb, $domain, $item, $attArrayList)"/>
    </result>
  </xsl:function>

  <xsl:function name="func:sdb-delete-domain">
    <xsl:param name="domain" as="xs:string"/>
    <!-- Not sure how we should do this, but putting this out in the open is obviously a bad idea.
      <xsl:sequence select="sdb.function:DeleteDomain($sdb, $domain)"/>
    -->
  </xsl:function>

  <xsl:function name="func:sdb-get-attribute">
    <xsl:param name="domain" as="xs:string"/>
    <xsl:param name="item" as="xs:string"/>
    <xsl:sequence select="sdb.function:GetAttribute($sdb, $domain, $item)"/>
  </xsl:function>

  <xsl:function name="func:sdb-query-domain">
    <xsl:param name="domain" as="xs:string"/>
    <xsl:param name="paramName" as="xs:string"/>
    <xsl:param name="paramValue" as="xs:string"/>
    <xsl:sequence select="sdb.function:QueryDomain($sdb, $domain, $paramName, $paramValue)"/>
  </xsl:function>

  <xsl:function name="func:add-items-to-array-list">
    <xsl:param name="attNameValueList"/>
    <xsl:param name="attName"/>
    <xsl:param name="attValue"/>
    <xsl:for-each select="$attName">
      <xsl:variable name="pos" select="position()"/>
      <xsl:variable name="attNameValue" select="$attValue[position() = $pos]"/>
      <xsl:element name="Attribute{$pos}">
        <xsl:element name="Name{$pos}">
          <xsl:value-of select="."/>
        </xsl:element>
        <xsl:element name="Value{$pos}">
          <xsl:value-of select="$attNameValue"/>
        </xsl:element>
        <xsl:sequence select="sdb.function:AddAttribute($sdb, $attNameValueList, ., $attNameValue)"
        />
      </xsl:element>
    </xsl:for-each>
  </xsl:function>

</xsl:transform>
