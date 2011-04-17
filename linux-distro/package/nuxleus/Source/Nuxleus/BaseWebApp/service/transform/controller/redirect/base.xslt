<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0" 
               xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
               xmlns:request="http://atomictalk.org/function/aspnet/request"
               xmlns:response="http://atomictalk.org/function/aspnet/response"
               xmlns:redirect="http://xameleon.org/service/redirect"
               xmlns:operation="http://xameleon.org/service/operation"
               xmlns:func="http://atomictalk.org/function"
               xmlns:param="http://xameleon.org/service/session/param"    
               >


  <xsl:param name="current-context"/>
  <xsl:param name="request"/>
  <xsl:param name="response"/>
  
  <xsl:template match="operation:redirct">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="redirect:location">
    <xsl:variable name="status-code" select="redirect:get-value(@status-code, 303)" />
    <xsl:variable name="uri" select="redirect:get-value(@uri, '/')" />
    <xsl:sequence select="concat(response:set-status-code($response, $status-code),
                                 response:set-location($response, $uri))"/>
  </xsl:template>

  <xsl:function name="redirect:get-value">
    <xsl:param name="qs-arg" />
    <xsl:param name="default" />
    <xsl:value-of select="if (func:resolve-variable($qs-arg) != 'not-set') 
                          then func:resolve-variable($qs-arg) 
                          else $default" />
  </xsl:function>
</xsl:transform>
