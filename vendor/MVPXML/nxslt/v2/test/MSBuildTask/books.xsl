<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"  xmlns:f="foo ns">
<xsl:param name="param1" select="'DEFAULT VALUE'"/>
<xsl:param name="f:param2" select="'DEFAULT VALUE2'"/>
	<xsl:template match="/">
	<html>
		<xsl:apply-templates/>
	</html>
	</xsl:template>

	<xsl:template match="bookstore">
	<!-- Prices and books -->
<p><xsl:value-of select="$param1"/></p>
<p><xsl:value-of select="$f:param2"/></p>
		<table>
			<xsl:apply-templates select="book"/>
		</table>
	</xsl:template>

	<xsl:template match="book">
		<tr>
                        <td>
				<xsl:value-of select="@ISBN"/>
			</td>
			<td><xsl:value-of select="price"/></td>
		</tr>
	</xsl:template>

</xsl:stylesheet>
