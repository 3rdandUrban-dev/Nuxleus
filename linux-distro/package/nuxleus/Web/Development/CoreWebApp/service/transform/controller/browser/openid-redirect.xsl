<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/1999/xhtml" xmlns:html="http://www.w3.org/1999/xhtml" version="1.0" exclude-result-prefixes="html">

  <xsl:variable name="return-uri" select="/auth/return-location" />
  <xsl:variable name="openid-message" select="/auth/message" />
  <xsl:variable name="openid" select="substring-after($openid-message, 'Logged in as ')"/>
  <xsl:variable name="base-uri" select="/auth/@xml:base"/>
  <xsl:variable name="session-uri" select="concat($base-uri, 'service/session?return_uri=', /auth/return-location, $openid)"/>

  <xsl:output doctype-system="-//W3C//DTD HTML 4.01//EN"
      doctype-public="http://www.w3.org/TR/html4/strict.dtd" cdata-section-elements="script"
      method="html" omit-xml-declaration="yes" />

  <xsl:template match="/">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="auth">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <title>(( sonic radar )) OpenID redirection...</title>
        <meta http-equiv="content-type" content="text/html; charset=utf-8" />
        <script type="text/javascript">
          <![CDATA[
            var fs = new RegExp("%2F", "g");
            var colon = new RegExp("%3A;", "g");
            var amp = new RegExp("&amp;", "g");
            function doRedirect(URI){
            window.location.replace(URI.replace(fs, "/").replace(colon, ":").replace(amp, "&"));
            };
          ]]>
        </script>
      </head>
      <xsl:choose>
        <xsl:when test="@status = 'complete'">
          <xsl:call-template name="url">
            <xsl:with-param name="uri" select="$session-uri"/>
          </xsl:call-template>
        </xsl:when>
        <xsl:when test="@status = 'failure'">
          <xsl:call-template name="url">
            <xsl:with-param name="uri" select="$base-uri"/>
            <xsl:with-param name="message" select="'Authentication Failed'"/>
          </xsl:call-template>
        </xsl:when>
        <xsl:otherwise>
          <xsl:apply-templates select="url"/>
        </xsl:otherwise>
      </xsl:choose>
    </html>
  </xsl:template>

  <xsl:template name="url" match="url">
    <xsl:param name="uri" select="."/>
    <xsl:param name="message" select="'Redirecting...'"/>
    <body onload="doRedirect('{$uri}'); return true;">
      <h3>
        <xsl:value-of select="$message"/>
      </h3>
    </body>
  </xsl:template>

</xsl:stylesheet>
