<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:atom="http://www.w3.org/2005/Atom" xmlns:dc="http://purl.org/dc/elements/1.1/"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:param name="pos"/>
  <xsl:param name="feed"/>
  <xsl:param name="stepCount" select="'5'"/>
  <xsl:variable name="entryCount" select="count(atom:feed/atom:entry)"/>
  <xsl:output encoding="UTF-8" indent="yes" method="html" version="1.0"/>
  <xsl:template match="/">
    <xsl:variable name="posStart">
      <xsl:choose>
        <xsl:when test="$pos &gt; count(atom:feed/atom:entry)">1</xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="$pos"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="startRange">
      <xsl:choose>
        <xsl:when test="floor($posStart div 30) &gt;= 1">
          <xsl:value-of select="floor($posStart div 30) * 30"/>
        </xsl:when>
        <xsl:otherwise>0</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="endRange" select="$startRange + 30"/>
    <ul class="viewer" style="float:left;width:100%;margin:0px 0px 0px 0px;z-index:0;font-variant:small-caps;padding:5px 0px 5px 0px;border-top:2px solid #588099;">
      <li style="width:100%;float:left;clear:both;text-align:center;padding:1px 0px 1px 0px;margin:0px 0px 5px 0px;border-top:1px solid #558099;border-bottom:1px solid #558099;">
        <ul style="padding:0px;font-size:small;text-align:left;color:#588099;margin:0px 1px 0px 1px;border-right:3px solid #EEEDE9;border-left:3px solid #EEEDE9;">
          <xsl:apply-templates mode="calLinearGrid" select="atom:feed/atom:entry[position() &gt; $startRange and position() &lt;= $endRange]">
            <xsl:with-param name="posStart" select="$posStart"/>
            <xsl:with-param name="startRange" select="$startRange"/>
            <xsl:with-param name="span" select="$stepCount"/>
          </xsl:apply-templates>
          <li style="float:left;clear:none;font-size:medium">&#160;of <xsl:value-of select="$entryCount"/>
          </li>
          <li style="float:right;clear:none;width:17%">
            <ul style="float:right">
              <xsl:if test="$pos >= 30 and ($entryCount + $pos) > 30">
                <xsl:variable name="stepCount">
                  <xsl:choose>
                    <xsl:when test="$pos >=30 and $pos &lt; 60">30</xsl:when>
                    <xsl:when test="$pos >=30 and $pos &lt;= 90">60</xsl:when>
                  </xsl:choose>
                </xsl:variable>
                <li onclick="SetParam('pos',{$pos - $stepCount});SetParam('feed','{$feed}');initialiseTransform('dataFeed', loadXMLFeed('{$feed}'), '/quickview.xsl', '{$feed}QF{$pos - $stepCount}');return false;" style="border:1px solid #588099;width:auto;height:18px;text-align:center;color:#588099;font-weight:600;padding:2px;margin:1px;cursor:pointer;float:left;text-align:center;">-<xsl:value-of select="$stepCount"/>
                </li>
              </xsl:if>
              <xsl:if test="$pos >= 60 and ($entryCount + $pos) > 60">
                <xsl:variable name="stepCount">
                  <xsl:choose>
                    <xsl:when test="$pos >=60 and $pos &lt; 90">30</xsl:when>
                    <xsl:when test="$pos >=90">60</xsl:when>
                  </xsl:choose>
                </xsl:variable>
                <li onclick="SetParam('pos',{$pos - $stepCount});SetParam('feed','{$feed}');initialiseTransform('dataFeed', loadXMLFeed('{$feed}'), '/quickview.xsl', '{$feed}QF{$pos - $stepCount}');return false;" style="border:1px solid #588099;width:auto;height:18px;text-align:center;color:#588099;font-weight:600;padding:2px;margin:1px;cursor:pointer;float:left;text-align:center;">-<xsl:value-of select="$stepCount"/>
                </li>
              </xsl:if>
              <xsl:if test="$pos &lt;= 60 and ($entryCount - $pos) > 30">
                <xsl:variable name="stepCount">30</xsl:variable>
                <li onclick="SetParam('pos',{$pos + $stepCount});SetParam('feed','{$feed}');initialiseTransform('dataFeed', loadXMLFeed('{$feed}'), '/quickview.xsl', '{$feed}QF{$pos + $stepCount}');return false;" style="border:1px solid #588099;width:auto;height:18px;text-align:center;color:#588099;font-weight:600;padding:2px;margin:1px;cursor:pointer;float:left;text-align:center;">+<xsl:value-of select="$stepCount"/>
                </li>
              </xsl:if>
              <xsl:if test="$pos &lt;= 30 and ($entryCount - $pos) > 60">
                <xsl:variable name="stepCount">60</xsl:variable>
                <li onclick="SetParam('pos',{$pos + $stepCount});SetParam('feed','{$feed}');initialiseTransform('dataFeed', loadXMLFeed('{$feed}'), '/quickview.xsl', '{$feed}QF{$pos + $stepCount}');return false;" style="border:1px solid #588099;width:auto;height:18px;text-align:center;color:#588099;font-weight:600;padding:2px;margin:1px;cursor:pointer;float:left;text-align:center;">+<xsl:value-of select="$stepCount"/>
                </li>
              </xsl:if>
              <li style="float:left;clear:none;margin-left:2px;margin-right:2px">|</li>
              <xsl:if test="$pos &gt; $stepCount">
                <li onclick="SetParam('pos',{$posStart - $stepCount});SetParam('feed','{$feed}');initialiseTransform('dataFeed', loadXMLFeed('{$feed}'), '/quickview.xsl', '{$feed}QF{$pos - $stepCount}');return false;" style="border:1px solid #588099;width:20px;height:18px;text-align:center;color:#588099;font-weight:600;padding:2px;margin:1px;cursor:pointer;float:left;text-align:center;">-<xsl:value-of select="$stepCount"/>
                </li>
              </xsl:if>
              <li onclick="SetParam('pos',{$posStart + $stepCount});SetParam('feed','{$feed}');initialiseTransform('dataFeed', loadXMLFeed('{$feed}'), '/quickview.xsl', '{$feed}QF{$pos + $stepCount}');return false;" style="width:20px;height:18px;border:1px solid #588099;text-align:center;color:#588099;font-weight:600;padding:2px;margin:1px;cursor:pointer;float:left;text-align:center;">+<xsl:value-of select="$stepCount"/>
              </li>
            </ul>
          </li>
        </ul>
      </li>
      <li class="formats" style="z-index:0;width:100%;float:left;clear:both;margin:0px;padding:0px;">
        <ul class="quickview" style="z-index:0;">
          <xsl:apply-templates select="atom:feed/atom:entry[position() &gt;= $posStart and position() &lt; ($posStart + $stepCount)]"/>
        </ul>
      </li>
    </ul>
  </xsl:template>
  <xsl:template match="atom:entry">
    <xsl:variable name="leftMargin">
      <xsl:choose>
        <xsl:when test="position() = 1">0</xsl:when>
        <xsl:otherwise>5</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <a href="{atom:link/@href}">
      <li class="feedEntry" style="z-index:20;margin-left:{$leftMargin}px;color:#FFF" onmouseover="this.style.color = '#588099';this.style.background = '#FFF';this.style.borderColor = '#588099';return true;" onmouseout="this.style.color = '#FFF';this.style.background = '#588099';this.style.borderColor = '#FFF';return true;">
        <ul>
          <li class="qvTitle">
            <xsl:value-of select="substring(atom:title, 0, 75)"/>
            <xsl:if test="string-length(atom:title) &gt; 75">...</xsl:if>
          </li>
          <li class="qvContent">
            <xsl:copy-of select="atom:summary"/>
          </li>
        </ul>
      </li>
    </a>
  </xsl:template>
  <xsl:template match="atom:entry" mode="calLinearGrid">
    <xsl:param name="posStart"/>
    <xsl:param name="startRange"/>
    <xsl:param name="span"/>
    <xsl:variable name="color">
      <xsl:choose>
        <xsl:when test="(position() + $startRange) &gt;= $posStart and (position() + $startRange) &lt; ($posStart + $span)">588099-FFF</xsl:when>
        <xsl:otherwise>FFF-588099</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <li style="float:left;width:20px;height:18px;border:1px solid #{substring-before($color, '-')};background-color:#{substring-after($color,'-')};text-align:center;padding:2px;margin:1px;color:#{substring-before($color, '-')}">
      <xsl:value-of select="position() + $startRange"/>
    </li>
  </xsl:template>
</xsl:stylesheet>

