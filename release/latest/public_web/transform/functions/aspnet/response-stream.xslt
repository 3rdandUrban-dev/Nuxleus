<?xml version="1.0"?>
<xsl:transform version="2.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:func="http://atomictalk.org/function"
    xmlns:response="http://atomictalk.org/function/aspnet/response"
    xmlns:saxon="http://saxon.sf.net/"
    xmlns:response-stream="clitype:System.Web.HttpResponse?partialname=System.Web"
    xmlns:clitype="http://saxon.sf.net/clitype"
    exclude-result-prefixes="xs func clitype saxon response-stream">

  <xsl:function name="response:get-buffer-output">
    <xsl:param name="response"/>
    <xsl:sequence select="response-stream:BufferOutput($response)"/>
  </xsl:function>

  <xsl:function name="response:set-buffer-output">
    <xsl:param name="response"/>
    <xsl:param name="buffer-output" as="xs:boolean"/>
    <xsl:sequence select="response-stream:set_BufferOutput($response, $buffer-output)"/>
  </xsl:function>

  <xsl:function name="response:get-content-encoding">
    <xsl:param name="response"/>
    <xsl:sequence select="response-stream:ContentEncoding($response)"/>
  </xsl:function>

  <xsl:function name="response:set-content-encoding">
    <xsl:param name="response"/>
    <xsl:param name="content-encoding" as="xs:string"/>
    <xsl:sequence select="response-stream:set_ContentEncoding($response, $content-encoding)"/>
  </xsl:function>

  <xsl:function name="response:get-content-type">
    <xsl:param name="response"/>
    <xsl:sequence select="response-stream:ContentType($response)"/>
  </xsl:function>

  <xsl:function name="response:set-content-type">
    <xsl:param name="response"/>
    <xsl:param name="content-type" as="xs:string"/>
    <xsl:sequence select="response-stream:set_ContentType($response, $content-type)"/>
  </xsl:function>

  <xsl:function name="response:get-charset">
    <xsl:param name="response"/>
    <xsl:sequence select="response-stream:Charset($response)"/>
  </xsl:function>

  <xsl:function name="response:set-charset">
    <xsl:param name="response"/>
    <xsl:param name="charset" as="xs:string"/>
    <xsl:sequence select="response-stream:set_Charset($response, $charset)"/>
  </xsl:function>

  <xsl:function name="response:get-cookies">
    <xsl:param name="response"/>
    <xsl:sequence select="response-stream:Cookies($response)"/>
  </xsl:function>

  <xsl:function name="response:transmit-file">
    <xsl:param name="response"/>
    <xsl:param name="file" as="xs:string"/>
    <xsl:sequence select="response-stream:TransmitFile($response, $file)"/>
  </xsl:function>

</xsl:transform>
