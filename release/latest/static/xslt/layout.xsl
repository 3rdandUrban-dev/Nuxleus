<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:atom="http://www.w3.org/2005/Atom" xmlns:app="http://www.w3.org/2007/app"
    xmlns="http://www.w3.org/1999/xhtml" version="1.0" exclude-result-prefixes="atom app">

    <xsl:param name="atom_feed_uri" />
    <xsl:param name="rss_feed_uri" />

    <xsl:output method="html" indent="yes" omit-xml-declaration="yes"
        doctype-public="-//W3C//DTD XHTML 1.1//EN"
        doctype-system="http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd" />

    <xsl:template match="/app:service">
        <html>
            <head>
                <meta http-equiv="Content-type" content="text/html; charset=utf-8" />
                <meta http-equiv="Content-Script-Type" content="application/javascript" />
                <meta http-equiv="Content-Style-Type" content="text/css" />
                <title>amp.fm : your music - your playlist</title>
                <link rel="stylesheet" href="/blog/css/style.css" type="text/css" media="screen" />
                <script
                    src="http://maps.google.com/maps?file=api&amp;v=2&amp;key=ABQIAAAAEPQGOws5xNa--1d-oQQDjhTjR-GfutZePHsQm3qK0Kl84YKzoBSVoQ0hZFrqptOXUdbZHQgzphUgLg"
                    type="text/javascript" language="javascript">
                    <xsl:text> </xsl:text>
                </script>
                <script
                    src="http://www.google.com/uds/api?file=uds.js&amp;v=1.0&amp;key=ABQIAAAAEPQGOws5xNa--1d-oQQDjhTjR-GfutZePHsQm3qK0Kl84YKzoBSVoQ0hZFrqptOXUdbZHQgzphUgLg"
                    type="text/javascript" language="javascript">
                    <xsl:text> </xsl:text>
                </script>
                <script src="/blog/js/jquery.js" type="application/javascript" language="javascript">
                    <xsl:text> </xsl:text>
                </script>
                <script src="/blog/js/jquery.jsmaps.js" type="application/javascript">
                    <xsl:text> </xsl:text>
                </script>
                <script src="/blog/js/blog.js" type="application/javascript" language="javascript">
                    <xsl:text> </xsl:text>
                </script>
                <script src="/blog/js/geo.js" type="application/javascript" language="javascript">
                    <xsl:text> </xsl:text>
                </script>
                <xsl:choose>
                    <xsl:when test="$atom_feed_uri">
                        <link rel="alternate" type="application/atom+xml" title="Atom 1.0"
                            href="{$atom_feed_uri}" />
                    </xsl:when>
                </xsl:choose>
                <xsl:choose>
                    <xsl:when test="$rss_feed_uri">
                        <link rel="alternate" type="application/xml" title="RSS 2.0"
                            href="{$rss_feed_uri}" />
                    </xsl:when>
                </xsl:choose>
            </head>
            <body>
                <div id="main">
                    <div id="links_container">
                        <div id="logo">
                            <h1>amp.fm blog</h1>
                            <h2>Sonic experience</h2>
                        </div>
                        <div id="links">
                            <a href="/">Home</a> | <a href="http://dev.amp.fm"> amp.fm </a>
                        </div>
                    </div>

                    <div id="content">
                        <div id="column1">
                            <div class="sidebaritem">
                                <h1>Navigation</h1>
                                <xsl:apply-templates select="app:workspace" />
                            </div>
                            <xsl:call-template name="search" />
                        </div>
                        <div id="column2">%s</div>
                    </div>
                    <div id="footer">(c)2007 Sylvain Hellegouarch <br />Powered by <a
                            href="http://trac.defuze.org/wiki/amplee">amplee</a> and <a
                            href="http://www.cherrypy.org">CherryPy</a> | Template by <a
                            href="http://www.dcarter.co.uk">design by dcarter</a>
                        <br />Content under <a rel="license"
                            href="http://creativecommons.org/licenses/by-sa/3.0/">Creative Commons
                            Attribution-Share Alike 3.0 License</a>.</div>
                </div>
            </body>
        </html>
    </xsl:template>

    <xsl:template match="app:workspace">
        <h2>
            <xsl:value-of select="atom:title" />
        </h2>
        <xsl:apply-templates select="app:collection" />
    </xsl:template>

    <xsl:template name="search">
        <div class="sidebaritem">
            <h1>Search</h1>
            <form method="get" action="/search">
                <table>
                    <tr>
                        <td>
                            <input type="text" name="query" value="" />
                        </td>

                        <td>
                            <input type="submit" value="Go" />
                        </td>
                    </tr>
                </table>
            </form>
        </div>
    </xsl:template>

    <xsl:template match="app:collection">
        <p>
            <a href="{/app:service/@xml:base}{substring-after(./@href, '/')}">
                <xsl:value-of select="atom:title" />
            </a>
        </p>
    </xsl:template>
</xsl:stylesheet>
