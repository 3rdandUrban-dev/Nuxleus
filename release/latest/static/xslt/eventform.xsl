<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:atom="http://www.w3.org/2005/Atom" xmlns:app="http://www.w3.org/2007/app"
    xmlns="http://www.w3.org/1999/xhtml" version="1.0" exclude-result-prefixes="atom app">

    <xsl:output method="xml" omit-xml-declaration="yes" indent="yes" />
    <xsl:template match="/">
        <script src="/blog/js/calendar.js" type="application/javascript" language="javascript">
            <xsl:text> </xsl:text>
        </script>

        <link rel="stylesheet" href="/blog/css/calendar.css" type="text/css" media="screen" />
        <div class="form" id="eventform">
            <h2>Add a new event</h2>
            <form method="post" action="/blog/pub/event">
                <table>
                    <tr>
                        <td>Name:</td>
                        <td>
                            <input type="text" name="name" id="name" />
                        </td>
                    </tr>
                    <tr>
                        <td style="vertical-align:top;">Description:</td>
                        <td>
                            <textarea name="description" cols="50" rows="5" id="description">
                                <xsl:text> </xsl:text>
                            </textarea>
                        </td>
                    </tr>
                    <tr>
                        <td style="vertical-align:top;">Location:</td>
                        <td>
                            <input type="hidden" name="location" id="location" />
                            <div id="gmap" />
                        </td>
                    </tr>
                    <tr>
                        <td>Start date:</td>
                        <td>
                            <input type="text" name="startdate" class="calendarSelectDate"
                                id="startdate" />
                        </td>
                    </tr>
                    <tr>
                        <td>End date:</td>
                        <td>
                            <input type="text" name="enddate" class="calendarSelectDate"
                                id="enddate" />
                        </td>
                    </tr>
                    <tr>
                        <td>Genre:</td>
                        <td>
                            <select name="genre" id="genre">
                                <option value="blues">Blues</option>
                                <option value="jazz">jazz</option>
                                <option value="electronica">Electronica</option>
                                <option value="rock">Rock</option>
                                <option value="funk">Funk</option>
                                <option value="hiphop">Hip-Hop</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td>Tags:</td>
                        <td>
                            <input type="text" name="tags" id="tags" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <input type="submit" value="Go" />
                        </td>
                    </tr>
                </table>
            </form>
        </div>
        <div id="calendarDiv" />
    </xsl:template>
</xsl:stylesheet>
