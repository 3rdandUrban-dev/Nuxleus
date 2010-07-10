<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
    xmlns:fn="http://www.w3.org/2005/xpath-functions"
    xmlns:saxon="http://saxon.sf.net/"
    xmlns:clitype="http://saxon.sf.net/clitype"
    xmlns:at="http://atomictalk.org"
    xmlns:func="http://atomictalk.org/function"
    xmlns:test="http://xameleon.org/controller/test"
    xmlns:http-sgml-to-xml="clitype:Xameleon.Function.HttpSgmlToXml?partialname=Xameleon"
    xmlns:aspnet-response="clitype:System.Web.HttpResponse?partialname=System.Web"
    xmlns:html="http://www.w3.org/1999/xhtml"
    exclude-result-prefixes="http-sgml-to-xml aspnet-response html xs xsi fn clitype">

  <xsl:import href="./movable-type-conversion.xsl"/>
  
  <xsl:template match="test:compare-title">
    <xsl:param name="old" select="func:resolve-variable(@old-mt-archive)" />
    <xsl:param name="new" select="func:resolve-variable(@new-mt-archive)" />
    <xsl:apply-templates select="func:mt-redirect-test(document($old), document($new))"/>
  </xsl:template>

  <xsl:template match="test:entries">
    <result>
      <xsl:apply-templates select="test:entry"/>
    </result>
  </xsl:template>

  <xsl:template match="test:entry">
    <xsl:variable name="title" select="substring-before(saxon:parse(http-sgml-to-xml:GetDocXml(@uri, '/html/head/title', false()))/title/text(), ' (Lessig Blog)')"/>
    <entry href="{@uri}">
      <expected-title>
        <xsl:value-of select="."/>
      </expected-title>
      <actual-title>
        <xsl:sequence select="$title"/>
      </actual-title>
      <pass>
        <xsl:value-of select="if ($title = .) then 'True' else 'False'"/>
        <xsl:sequence select="aspnet-response:Flush($response)"/>
      </pass>
    </entry>
  </xsl:template>

</xsl:transform>
