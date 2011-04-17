<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="html" />
  <xsl:template match="/script">
    <script type="text/javascript">
      <xsl:value-of select="text()"/>
    </script>
  </xsl:template>
</xsl:stylesheet>
