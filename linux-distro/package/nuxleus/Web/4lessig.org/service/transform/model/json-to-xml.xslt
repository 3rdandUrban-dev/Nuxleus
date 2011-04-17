<?xml version="1.0"?>
<xsl:transform version="2.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:func="http://atomictalk.org/function"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:web-request="clitype:Xameleon.Function.HttpWebRequestStream?partialname=Xameleon"
    xmlns:f="http://fxsl.sf.net/">

  <xsl:import href="../functions/fxsl-xslt2/f/func-json-document.xsl"/>

  <xsl:function name="func:json-to-xml">
    <xsl:param name="external-uri" as="xs:string"/>
    <xsl:variable name="external-json-document" select="web-request:GetResponse($external-uri)"/>
    <xsl:sequence select="f:json-document($external-json-document)"/>
  </xsl:function>

</xsl:transform>

  
