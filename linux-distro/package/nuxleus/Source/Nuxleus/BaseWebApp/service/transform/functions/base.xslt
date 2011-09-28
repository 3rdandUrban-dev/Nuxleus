<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    exclude-result-prefixes="#all">

  <xsl:import href="./amazonaws/funcset-s3.xslt" />
  <xsl:import href="./funcset-dateTime.xslt" />
  <xsl:import href="./funcset-Util.xslt" />
  <xsl:import href="./aspnet/session.xslt" />
  <xsl:import href="./aspnet/server.xslt" />
  <xsl:import href="./aspnet/request-stream.xslt" />
  <xsl:import href="./aspnet/response-stream.xslt" />
  <xsl:import href="./aspnet/timestamp.xslt" />

</xsl:transform>
