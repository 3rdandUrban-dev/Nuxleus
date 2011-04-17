<xsl:transform version="2.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:func="http://atomictalk.org/function"
    exclude-result-prefixes="xs func">

  <xsl:function name="func:get-current-dateTime" as="xs:dateTime">
    <xsl:value-of select="current-dateTime()"/>
  </xsl:function>
  
  <xsl:function name="func:format-dateTime-string">
    <xsl:param name="dateTime"/>
    <xsl:variable name="formattedDateTime" select="if(contains($dateTime, 'T')) then format-dateTime(xs:dateTime(if(string-length(substring-before(substring-after($dateTime, 'T'), 'Z')) lt 8) then concat(substring-before($dateTime, 'Z'), ':00Z') else $dateTime), '[MNn] [Do] [Y] [H01]:[m01]:[s01] [z]') else if(empty($dateTime)) then '' else ''"/>
    <xsl:if test="not(empty($dateTime))">
      <xsl:value-of select="$formattedDateTime"/>
    </xsl:if>
  </xsl:function>

</xsl:transform>
