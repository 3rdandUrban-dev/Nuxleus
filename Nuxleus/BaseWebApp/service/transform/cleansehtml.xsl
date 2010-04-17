<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:lookup="http://xameleon.org/lookup" xmlns="http://www.w3.org/1999/xhtml" xmlns:html="http://www.w3.org/1999/xhtml" xmlns:ms="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="html lookup ms">
	<lookup:html>
		<html:p use="p"/>
		<html:em use="em"/>
		<html:strong use="strong"/>
		<html:b use="strong"/>
		<html:i use="em"/>
		<html:blockquote use="blockquote"/>
		<html:cite use="cite"/>
		<html:br use="br"/>
		<html:pre use="pre"/>
		<html:code use="code"/>
	</lookup:html>
	<ms:script language="C#" implements-prefix="str"><![CDATA[
	  string ConvertLinebreaks(String content) {
			StringBuilder builder = new StringBuilder();
	    using (System.IO.StringReader reader = new System.IO.StringReader(content)) {
				while (true) {
              string s = reader.ReadLine();
              if (s == null)
                  break;
              if (s.Length == 0)
                  continue;
              builder.Append(String.Format("{0}<br/>", s));
          }
     		}
				return builder.ToString();
	  }
	  ]]></ms:script>
	<xsl:variable name="lb">
		<xsl:text>
</xsl:text>
	</xsl:variable>
	<xsl:variable name="safe-elements" select="document('')//lookup:html/*"/>
	<xsl:template match="/">
		<div>
			<xsl:apply-templates mode="validate"/>
		</div>
	</xsl:template>
	<xsl:template match="html:div" mode="validate">
		<xsl:apply-templates mode="validate"/>
	</xsl:template>
	<xsl:template match="*" mode="validate">
		<xsl:variable name="local-name" select="local-name()"/>
		<xsl:apply-templates select="$safe-elements[local-name() = $local-name]/@use" mode="safe">
			<xsl:with-param name="node" select="."/>
		</xsl:apply-templates>
	</xsl:template>
	<xsl:template match="text()" mode="validate">
<!-- You could do some extended text matching here to remove any text seen as undesirable -->
		<xsl:variable name="content" select="."/>
		<xsl:value-of select="str:ConvertLinebreaks(.)" disable-output-escaping="yes"/>
	</xsl:template>
	<xsl:template match="@*" mode="safe">
		<xsl:param name="node"/>
		<xsl:element name="{.}">
			<xsl:apply-templates select="$node/html:*|$node/text()" mode="validate"/>
		</xsl:element>
	</xsl:template>
	<xsl:template name="convertlinebreak">
		<xsl:param name="content"/>
		<xsl:value-of select="substring-before($content,$lb)"/>
		<br/>
		<xsl:variable name="remainder" select="substring-after($content,$lb)"/>
		<xsl:if test="contains($remainder,$lb)">
			<xsl:call-template name="convertlinebreak">
				<xsl:with-param name="content" select="$remainder"/>
			</xsl:call-template>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>
