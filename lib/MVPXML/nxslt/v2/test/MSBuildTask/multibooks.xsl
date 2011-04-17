<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:param name="param1" select="'DEFAULT VALUE'"/>
	<xsl:template match="/">
	<html>
		<xsl:apply-templates/>
	</html>
	</xsl:template>

	<xsl:template match="bookstore">
	<!-- Prices and books -->
<p><xsl:value-of select="$param1"/></p>
		<table>
			<xsl:apply-templates select="book"/>
		</table>
	</xsl:template>

	<xsl:template match="book">
             <exsl:document href="{title}.xml" xmlns:exsl="http://exslt.org/common">
		<tr>
                        <td>
				<xsl:value-of select="@ISBN"/>
			</td>
			<td><xsl:value-of select="price"/></td>
		</tr>
		</exsl:document>
	</xsl:template>

</xsl:stylesheet>
