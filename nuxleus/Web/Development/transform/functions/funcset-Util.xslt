<?xml version="1.0"?>
<xsl:transform version="2.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:func="http://atomictalk.org/function"
    xmlns:saxon="http://saxon.sf.net/"
    xmlns:request-collection="clitype:Xameleon.Function.HttpRequestCollection?partialname=Xameleon"
    xmlns:clitype="http://saxon.sf.net/clitype"
    exclude-result-prefixes="xs func clitype saxon request-collection">

  <xsl:param name="request"/>

  <xsl:function name="func:resolve-variable" as="xs:string">
    <xsl:param name="operator"/>
    <xsl:sequence select="if (contains($operator, '{')) then func:evaluate-collection(substring-before(substring-after($operator, '{'), '}')) else $operator"/>
  </xsl:function>

  <xsl:function name="func:evaluate-collection" as="xs:string">
    <xsl:param name="operator"/>
    <xsl:sequence select="if (starts-with($operator, '$')) then $session-params[local-name() = substring-after($operator, '$')] else request-collection:GetValue($request, substring-before($operator, ':'), substring-after($operator, ':'))" />
  </xsl:function>

</xsl:transform>

  
