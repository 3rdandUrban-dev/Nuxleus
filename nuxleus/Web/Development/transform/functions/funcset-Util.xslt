<?xml version="1.0"?>
<xsl:transform version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:func="http://atomictalk.org/function"
  xmlns:saxon="http://saxon.sf.net/" xmlns:string="clitype:System.IO.MemoryStream"
  xmlns:request-collection="clitype:Xameleon.Function.HttpRequestCollection?partialname=Xameleon"
  xmlns:aspnet-request="clitype:System.Web.HttpRequest?partialname=System.Web"
  xmlns:clitype="http://saxon.sf.net/clitype"
  exclude-result-prefixes="xs func clitype saxon request-collection">

  <xsl:param name="request"/>

  <xsl:function name="func:resolve-variable" as="xs:string">
    <xsl:param name="operator"/>
    <xsl:sequence
      select="if (contains($operator, '{')) then func:evaluate-collection(substring-before(substring-after($operator, '{'), '}')) else $operator"
    />
  </xsl:function>

  <xsl:function name="func:evaluate-collection" as="xs:string">
    <xsl:param name="operator"/>
    <xsl:sequence
      select="if (starts-with($operator, '$')) 
                          then $session-params[local-name() = substring-after($operator, '$')] 
                          else if (starts-with($operator, 'request')) 
                          then func:evaluate-request($request, substring-after($operator, ':')) 
                          else request-collection:GetValue($request, substring-before($operator, ':'), substring-after($operator, ':'))"/>

  </xsl:function>


  <xsl:function name="func:evaluate-request">
    <xsl:param name="request"/>
    <xsl:param name="property-name"/>
    <xsl:choose>
      <xsl:when test="$property-name = 'accept-types'">
        <xsl:sequence select="string(aspnet-request:AcceptTypes($request))"/>
      </xsl:when>
      <xsl:when test="$property-name = 'content-encoding'">
        <xsl:sequence select="string(aspnet-request:ContentEncoding($request))"/>
      </xsl:when>
      <xsl:when test="$property-name = 'content-length'">
        <xsl:sequence select="string(aspnet-request:ContentLength($request))"/>
      </xsl:when>
      <xsl:when test="$property-name = 'content-type'">
        <xsl:sequence select="string(aspnet-request:ContentType($request))"/>
      </xsl:when>
      <xsl:when test="$property-name = 'input-stream'">
        <xsl:sequence select="string(aspnet-request:InputStream($request))"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:sequence select="'NOTSET'"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:function>

  <xsl:function name="func:true" as="element()">
    <true/>
  </xsl:function>

  <xsl:function name="func:false" as="element()">
    <false/>
  </xsl:function>

</xsl:transform>
