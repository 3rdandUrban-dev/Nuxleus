<?xml version="1.0"?>
<xsl:transform version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:func="http://atomictalk.org/function" xmlns:saxon="http://saxon.sf.net/" xmlns:request="http://atomictalk.org/function/aspnet/request" xmlns:request-stream="clitype:System.Web.HttpRequest?partialname=System.Web" xmlns:clitype="http://saxon.sf.net/clitype" exclude-result-prefixes="xs func clitype saxon request-stream">

  <xsl:param name="request"/>

  <xsl:function name="request:get-content-type">
    <xsl:param name="request" />
    <xsl:sequence select="request-stream:ContentType($request)" />
  </xsl:function>

  <xsl:function name="request:get-browser">
    <xsl:sequence select="request-stream:Browser($request)" />
  </xsl:function>

  <xsl:function name="request:get-client-certificate">
    <xsl:sequence select="request-stream:ClientCertificate($request)" />
  </xsl:function>

  <xsl:function name="request:get-content-encoding">
    <xsl:sequence select="request-stream:ContentEncoding($request)" />
  </xsl:function>

  <xsl:function name="request:get-content-length">
    <xsl:sequence select="request-stream:ContentLength($request)" />
  </xsl:function>

  <xsl:function name="request:get-cookies">
    <xsl:sequence select="request-stream:Cookies($request)" />
  </xsl:function>

  <xsl:function name="request:get-current-execution-file-path">
    <xsl:sequence select="request-stream:CurrentExecutionFilePath($request)" />
  </xsl:function>

  <xsl:function name="request:get-file-path">
    <xsl:sequence select="request-stream:FilePath($request)" />
  </xsl:function>

  <xsl:function name="request:get-files">
    <xsl:sequence select="request-stream:Files($request)" />
  </xsl:function>

  <xsl:function name="request:get-filter">
    <xsl:sequence select="request-stream:Filter($request)" />
  </xsl:function>

  <xsl:function name="request:get-form">
    <xsl:sequence select="request-stream:Form($request)" />
  </xsl:function>

  <xsl:function name="request:get-headers">
    <xsl:sequence select="request-stream:Headers($request)" />
  </xsl:function>

  <xsl:function name="request:get-http-method">
    <xsl:sequence select="request-stream:HttpMethod($request)" />
  </xsl:function>

  <xsl:function name="request:get-input-stream">
    <xsl:sequence select="request-stream:InputStream($request)" />
  </xsl:function>

  <xsl:function name="request:get-is-authenticated">
    <xsl:sequence select="request-stream:IsAuthenticated($request)" />
  </xsl:function>

  <xsl:function name="request:get-is-local">
    <xsl:sequence select="request-stream:IsLocal($request)" />
  </xsl:function>

  <xsl:function name="request:get-is-secure-connection">
    <xsl:sequence select="request-stream:IsSecureConnection($request)" />
  </xsl:function>

  <xsl:function name="request:get-item">
    <xsl:sequence select="request-stream:Item($request)" />
  </xsl:function>

  <xsl:function name="request:get-params">
    <xsl:sequence select="request-stream:Params($request)" />
  </xsl:function>

  <xsl:function name="request:get-total-bytes">
    <xsl:sequence select="request-stream:TotalBytes($request)" />
  </xsl:function>

  <xsl:function name="request:get-user-agent">
    <xsl:param name="request" />
    <xsl:sequence select="request-stream:UserAgent($request)" />
  </xsl:function>

  <xsl:function name="request:get-user-ip">
    <xsl:param name="request" />
    <xsl:sequence select="request-stream:UserHostAddress($request)" />
  </xsl:function>

  <xsl:function name="request:get-raw-url">
    <xsl:sequence select="request-stream:RawUrl($request)" />
  </xsl:function>

  <xsl:function name="request:get-path">
    <xsl:sequence select="request-stream:Path($request)" />
  </xsl:function>

  <xsl:function name="request:get-application-path">
    <xsl:sequence select="request-stream:ApplicationPath($request)" />
  </xsl:function>

  <xsl:function name="request:get-app-relative-current-execution-file-path">
    <xsl:sequence select="request-stream:AppRelativeCurrentExecutionFilePath($request)" />
  </xsl:function>

  <xsl:function name="request:get-physical-application-path">
    <xsl:sequence select="request-stream:PhysicalApplicationPath($request)" />
  </xsl:function>

  <xsl:function name="request:get-physical-path">
    <xsl:sequence select="request-stream:PhysicalPath($request)" />
  </xsl:function>

  <xsl:function name="request:get-query-string">
    <xsl:sequence select="request-stream:QueryString($request)" />
  </xsl:function>

  <xsl:function name="request:get-accept-types">
    <xsl:sequence select="request-stream:AcceptTypes($request)" />
  </xsl:function>

  <xsl:function name="request:get-anonymous-id">
    <xsl:sequence select="request-stream:AnonymousID($request)" />
  </xsl:function>

  <xsl:function name="request:get-url-referrer">
    <xsl:sequence select="request-stream:UrlReferrer($request)" />
  </xsl:function>

  <xsl:function name="request:get-request-type">
    <xsl:sequence select="request-stream:RequestType($request)" />
  </xsl:function>

  <xsl:function name="request:get-user-languages">
    <xsl:sequence select="request-stream:UserLanguages($request)" />
  </xsl:function>

  <xsl:function name="request:get-user-host-name">
    <xsl:sequence select="request-stream:UserHostName($request)" />
  </xsl:function>

  <xsl:function name="request:get-user-host-address">
    <xsl:sequence select="request-stream:UserHostAddress($request)" />
  </xsl:function>

  <xsl:function name="request:get-user-agent">
    <xsl:sequence select="request-stream:UserAgent($request)" />
  </xsl:function>

  <xsl:function name="request:get-server-variables">
    <xsl:sequence select="request-stream:ServerVariables($request)" />
  </xsl:function>

  <xsl:function name="request:get-map-path">
    <xsl:param name="virtual-path"/>
    <xsl:sequence select="request-stream:MapPath($request, $virtual-path)" />
  </xsl:function>

</xsl:transform>
