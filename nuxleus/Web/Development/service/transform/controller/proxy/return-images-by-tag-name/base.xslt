<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0" xmlns="http://www.w3.org/2005/Atom" xmlns:atom="http://www.w3.org/2005/Atom" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:saxon="http://saxon.sf.net/" xmlns:clitype="http://saxon.sf.net/clitype" xmlns:at="http://atomictalk.org" xmlns:func="http://atomictalk.org/function" xmlns:http-sgml-to-xml="clitype:Xameleon.Function.HttpSgmlToXml?partialname=Xameleon" xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web" xmlns:atomv03="http://purl.org/atom/ns#" xmlns:proxy="http://xameleon.org/service/proxy" xmlns:html="http://www.w3.org/1999/xhtml" exclude-result-prefixes="#all">

  <xsl:import href="../../../functions/funcset-Util.xslt" />
  <xsl:param name="current-context" />

  <xsl:template match="proxy:return-images-by-tag-name">
    <xsl:variable name="location" select="func:resolve-variable(@location)" />
    <xsl:variable name="topic" select="tokenize(func:resolve-variable(@topic), '\|')" />
    <xsl:variable name="uri" select="@uri"/>
    <xsl:variable name="result">
      <xsl:for-each select="$topic">
        <xsl:sequence select="document(concat($uri, $location, ',', .))"/>
      </xsl:for-each>
    </xsl:variable>
    <xsl:sequence select="$result"/>
  </xsl:template>

</xsl:transform>
