<xsl:transform version="2.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:func="http://atomictalk.org/function"
    exclude-result-prefixes="xs func">

  <xsl:function name="func:get-current-dateTime" as="xs:dateTime">
    <xsl:value-of select="current-dateTime()"/>
  </xsl:function>

</xsl:transform>
