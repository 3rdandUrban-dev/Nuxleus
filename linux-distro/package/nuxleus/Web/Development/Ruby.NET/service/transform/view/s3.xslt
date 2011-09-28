<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
    xmlns:fn="http://www.w3.org/2005/xpath-functions"
    xmlns:at="http://atomictalk.org"
    xmlns:func="http://atomictalk.org/function"
    xmlns:stream="clitype:System.IO.Stream?from=file:///usr/lib/mono/2.0/mscorlib.dll"
    xmlns:http-response-stream="clitype:Xameleon.Function.HttpWebResponseStream?partialname=Xameleon"
    xmlns:s3-object-compare="clitype:Xameleon.Function.S3ObjectCompare?partialname=Xameleon"
    xmlns:http-sgml-to-xml="clitype:Xameleon.Function.HttpSgmlToXml?partialname=Xameleon"
    xmlns:sortedlist="clitype:System.Collections.SortedList?from=file:///usr/lib/mono/2.0/mscorlib.dll"
    xmlns:uri="clitype:System.Uri?from=file:///usr/lib/mono/2.0/System.dll"
    xmlns:aspnet="http://atomictalk.org/function/aspnet"
    xmlns:response="http://atomictalk.org/function/aspnet/response"
    xmlns:aspnet-session="clitype:System.Web.SessionState.HttpSessionState?from=file:///usr/lib/mono/2.0/System.Web.dll"
    xmlns:browser="clitype:System.Web.HttpBrowserCapabilities?from=file:///usr/lib/mono/2.0/System.Web.dll"
    xmlns:aspnet-server="clitype:System.Web.HttpServerUtility?from=file:///usr/lib/mono/2.0/System.Web.dll"
    xmlns:aspnet-request="clitype:System.Web.HttpRequest?from=file:///usr/lib/mono/2.0/System.Web.dll"
    xmlns:aspnet-response="clitype:System.Web.HttpResponse?from=file:///usr/lib/mono/2.0/System.Web.dll"
    xmlns:aspnet-timestamp="clitype:System.DateTime?from=file:///usr/lib/mono/2.0/mscorlib.dll"
    xmlns:request-collection="clitype:Xameleon.Function.HttpRequestCollection?partialname=Xameleon"
    xmlns:response-collection="clitype:Xameleon.Function.HttpResponseCollection?partialname=Xameleon"
    xmlns:web-request="clitype:Xameleon.Function.HttpWebRequestStream?partialname=Xameleon"
    xmlns:web-response="clitype:System.Net.WebResponse?from=file:///usr/lib/mono/2.0/System.dll"
    xmlns:service="http://xameleon.org/service"
    xmlns:operation="http://xameleon.org/service/operation"
    xmlns:session="http://xameleon.org/service/session"
    xmlns:param="http://xameleon.org/service/session/param"
    xmlns:aws="http://xameleon.org/function/aws"
    xmlns:s3="http://xameleon.org/function/aws/s3"
    xmlns:header="http://xameleon.org/service/http/header"
    xmlns:metadata="http://xameleon.org/service/metadata"
    xmlns:saxon="http://saxon.sf.net/"
    xmlns:clitype="http://saxon.sf.net/clitype"
    xmlns:amazonaws="http://s3.amazonaws.com/doc/2006-03-01/"
    xmlns:s3response="clitype:Extf.Net.S3.Response?from=file:///srv/wwwroot/webapp/bin/Extf.Net.dll"
    xmlns:aws-conn="clitype:Extf.Net.S3.AWSAuthConnection?from=file:///srv/wwwroot/webapp/bin/Extf.Net.dll"
    xmlns:aws-gen="clitype:Extf.Net.S3.QueryStringAuthGenerator?from=file:///srv/wwwroot/webapp/bin/Extf.Net.dll"
    xmlns:http-util="clitype:System.Web.HttpUtility?from=file:///usr/lib/mono/2.0/System.Web.dll"
    xmlns:s3object="clitype:Extf.Net.S3.S3Object?from=file:///srv/wwwroot/webapp/bin/Extf.Net.dll"
    xmlns:html="http://www.w3.org/1999/xhtml"
    exclude-result-prefixes="http-sgml-to-xml html s3-object-compare web-response web-request stream http-response-stream browser aws-gen aws-conn http-util s3object s3response uri amazonaws at aspnet aspnet-timestamp aspnet-server aspnet-session aspnet-request aspnet-response saxon metadata header sortedlist param service operation session aws s3 func xs xsi fn clitype response-collection request-collection">

  <xsl:import href="../functions/amazonaws/funcset-s3.xslt"/>
  
  <xsl:param name="aws-public-key" select="'not-set'" as="xs:string"/>
  <xsl:param name="aws-private-key" select="'not-set'" as="xs:string"/>
  <xsl:param name="response" />
  <xsl:param name="request"/>
  <xsl:param name="server"/>
  <xsl:param name="session"/>
  <xsl:param name="timestamp"/>
  <xsl:variable name="debug" select="if (request-collection:GetValue($request, 'query-string', 'debug') = 'true') then true() else false()" as="xs:boolean" />
  <xsl:variable name="not-set" select="'not-set'" as="xs:string"/>
  <xsl:variable name="guid" select="request-collection:GetValue($request, 'cookie', 'guid')" as="xs:string" />
  <xsl:variable name="session-params" select="func:eval-params(/service:operation/param:*)"/>
  <xsl:variable name="s3-bucket-name" select="$session-params[local-name() = 's3-bucket-name']" />
  <xsl:variable name="issecure" select="false()" as="xs:boolean"/>
  <xsl:variable name="aws-gen" select="aws-gen:new($aws-public-key, $aws-private-key, $issecure)"/>
  <xsl:variable name="aws-conn" select="aws-conn:new($aws-public-key, $aws-private-key, $issecure)"/>
  <xsl:variable name="expires-in" select="aws:s3-set-expires-in($aws-gen, 60000)"/>
  <xsl:variable name="request-uri" select="aspnet-request:Url($request)"/>
  <xsl:variable name="browser" select="aspnet-request:Browser($request)"/>

  <xsl:template match="header:*">
    <xsl:param name="sorted-list" as="clitype:System.Collections.SortedList"/>
    <xsl:variable name="key" select="local-name() cast as xs:untypedAtomic"/>
    <xsl:variable name="value" select=". cast as xs:untypedAtomic"/>
    <xsl:sequence select="sortedlist:Add($sorted-list, $key, $value)"/>
  </xsl:template>

  <xsl:function name="func:eval-params" as="element()+">
    <xsl:param name="params"/>
    <xsl:apply-templates select="$params" mode="evalparam"/>
  </xsl:function>

  <xsl:template match="param:*" mode="evalparam">
    <xsl:element name="{local-name()}">
      <xsl:value-of select="func:resolve-variable(.)"/>
    </xsl:element>
  </xsl:template>

  <!-- 
    <xsl:template match="service:operation">
      <xsl:param name="key-name"/>
      <xsl:variable name="issecure" select="false()" as="xs:boolean"/>
      <xsl:variable name="content-type" select="if ($debug) then aspnet:response.set-content-type($response, 'text/plain') else aspnet:response.set-content-type($response, 'text/xml')"/>
      <xsl:processing-instruction name="xml-stylesheet">
        <xsl:text>type="text/xsl" href="/transform/openid-redirect.xsl"</xsl:text>
      </xsl:processing-instruction>
      <message type="service:result"
          content-type="{if (empty($content-type)) then aspnet:response.get-content-type($response) else 'not-set'}">
        <xsl:apply-templates/>
      </message>
    </xsl:template> 
  -->

  <xsl:template match="service:operation">
    <xsl:param name="key-name"/>
    <xsl:variable name="issecure" select="false()" as="xs:boolean"/>
    <xsl:variable name="content-type" select="if ($debug) then response:set-content-type($response, 'text/plain') else response:set-content-type($response, 'text/xml')"/>
    <xsl:if test="not($debug)">
      <xsl:processing-instruction name="xml-stylesheet">
        <xsl:text>type="text/xsl" href="/transform/openid-redirect.xsl"</xsl:text>
      </xsl:processing-instruction>
    </xsl:if>
    <auth status="session">
      <url>
        <xsl:sequence select="request-collection:GetValue($request, 'query-string', 'return_uri')"/>
      </url>
      <message>
        <xsl:sequence select="$content-type"/>
        <xsl:apply-templates />
        <xsl:if test="$debug">
          <Params>
            <xsl:for-each select="$session-params">
              <xsl:element name="{local-name()}">
                <xsl:value-of select="."/>
              </xsl:element>
            </xsl:for-each>
          </Params>
          <HttpRequest>
            <RawUrl>
              <xsl:sequence select="aspnet-request:RawUrl($request)" />
            </RawUrl>
            <UserName>
              <xsl:sequence select="browser:ClrVersion($browser)"/>
            </UserName>
            <Cookies>
              <guid>
                <xsl:sequence select="$guid"/>
              </guid>
            </Cookies>
          </HttpRequest>
          <Operation>
            <!-- 
            <FileName>
              <xsl:sequence select="$key-name"/>
            </FileName>
            <S3PutObject>
              <xsl:sequence
                  select="aws:s3-put-object($s3-bucket-name, $key-name, $guid, $aws-public-key, $aws-private-key, $issecure)"/>
            </S3PutObject> 
            
            <S3GetSignature>
              <xsl:variable name="signature" select="aws:s3-get-signature($s3-bucket-name, $key-name, $issecure)" as="xs:string"/>
              <xsl:sequence
                  select="if (unparsed-text-available($signature)) then $signature else $signature"/>
            </S3GetSignature>-->
            <!--
            <S3GetSignature>
            <xsl:sequence
                select="unparsed-text(aws:s3-get-signature($s3-bucket-name, $key-name, $issecure))"/>
              <xsl:sequence select="aws:s3-get-signature($s3-bucket-name, $key-name, $issecure)"/>
            </S3GetSignature>
            <S3GetObject>
              <xsl:copy-of
                  select="document('http://s3.amazonaws.com/m.david/screen-shots/VS.NET_on_MacOSX.foo')/*"/>
            </S3GetObject>
            <S3ListBucket>
              <xsl:variable name="bucket" select="aws:s3-list-bucket($s3-bucket-name, $key-name, '', 1 cast as xs:integer, '/')"/>
              <xsl:sequence select="encode-for-uri('/')"/>
              <xsl:copy-of
                  select="document($bucket)/*"/>
            </S3ListBucket>
            --></Operation>
          <HttpResponse>
            <ContentType>
              <xsl:sequence select="$content-type"/>
            </ContentType>
            <TimeStamp>
              <xsl:sequence select="func:get-timestamp($timestamp, 'short-time')"/>
            </TimeStamp>
          </HttpResponse>
        </xsl:if>
      </message>
    </auth>
  </xsl:template>

  <xsl:template match="operation:aws">
    <auth status="session">
      <url>
        <xsl:sequence select="request-collection:GetValue($request, 'query-string', 'return_uri')"/>
      </url>
      <message>
        <file>
          <xsl:apply-templates />
        </file>
        <xsl:if test="$debug">
          <HttpRequest>
            <RawUrl>
              <xsl:sequence select="aspnet-request:RawUrl($request)" />
            </RawUrl>
            <Cookies>
              <guid>
                <xsl:sequence select="$guid"/>
              </guid>
            </Cookies>
          </HttpRequest>
        </xsl:if>
      </message>
    </auth>
  </xsl:template>

  <xsl:template match="aws:s3">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="s3:check-for-existing-key">
    <xsl:variable name="folder-name" select="func:resolve-variable(@folder)"/>
    <xsl:variable name="file-name" select="func:resolve-variable(@file)"/>
    <xsl:variable name="key-name" select="aws:s3-normalize-key($folder-name, $file-name)"/>
    <xsl:variable name="key-uri" select="aws:s3-get-signature($s3-bucket-name, $key-name, false())"/>
    <xsl:variable name="compare" select="s3-object-compare:Compare($aws-conn, $s3-bucket-name, $key-name, $guid)"/>
    <xsl:variable name="html-to-xml" select="http-sgml-to-xml:GetDocXml('http://mdavid.name/', '/html/head', false())"/>
    <xsl:variable name="web-request" select="web-request:GetResponse('http://www.law.stanford.edu/assets/ajax/search_publications.php', 'year_start=&amp;year_end=&amp;s=Lawrence%20Lessig&amp;format=')"/>
    <uri>
      <!-- <xsl:value-of select="$web-request"/> -->
    </uri>
    <external-html>
      <xsl:sequence select="saxon:parse($html-to-xml)/head/link"/>
    </external-html>
    <compare>
      <xsl:sequence
          select="aws:s3-put-object($s3-bucket-name, $key-name, $guid, $aws-public-key, $aws-private-key, $issecure)"/>
      <xsl:value-of select="if ($compare) then 'True!' else 'False!'"/>
    </compare>
    <!-- <xsl:variable name="web-request-response" select="web-response:GetResponseStream(web-request:GetResponse($web-request))"/>
    <xsl:variable name="web-response" select="http-response-stream:GetResponseString($web-request-response)"/>
    <xsl:apply-templates select="if ($web-response = $guid) then at:IfTrue else at:IfFalse">
      <xsl:with-param name="key-name" select="$key-name"/>
      <xsl:with-param name="key-uri" select="$key-uri"/>
    </xsl:apply-templates> -->
  </xsl:template>

  <xsl:function name="aws:s3-key-exists" as="xs:boolean">
    <xsl:param name="key-uri"/>
    <xsl:sequence select="unparsed-text-available($key-uri)"/>
  </xsl:function>

  <xsl:template match="at:IfFalse">
    <xsl:param name="key-name"/>
   <xsl:sequence
        select="aws:s3-put-object($s3-bucket-name, $key-name, $guid, $aws-public-key, $aws-private-key, false())"/>
  </xsl:template>

  <xsl:template match="at:IfTrue">
    <xsl:param name="key-uri"/>
    <xsl:variable name="params">
      <xsl:apply-templates select="param:*" mode="eval">
        <xsl:with-param name="key-uri" select="$key-uri"/>
      </xsl:apply-templates>
    </xsl:variable>
    <xsl:apply-templates>
      <xsl:with-param name="key-uri" select="$key-uri"/>
      <xsl:with-param name="params" select="$params"/>
    </xsl:apply-templates>
  </xsl:template>

  <xsl:template match="param:*" mode="eval">
    <xsl:param name="key-uri"/>
    <xsl:element name="{local-name()}">
      <xsl:apply-templates>
        <xsl:with-param name="key-uri" select="$key-uri"/>
      </xsl:apply-templates>
    </xsl:element>
  </xsl:template>

  <xsl:template match="at:compare">
    <xsl:param name="key-uri"/>
    <xsl:param name="params"/>
    <!--     
      <xsl:sequence select="$params/*[local-name() = 'key-value']"/> 
    -->
  </xsl:template>

  <xsl:template match="s3:return-file-content">
    <xsl:param name="key-uri"/>
    <xsl:sequence select="unparsed-text($key-uri)"/>
  </xsl:template>

  <xsl:template match="s3:write-file">
    <xsl:param name="key-name"/>
    <xsl:variable name="file-content" select="func:resolve-variable(@content)"/>
    <xsl:sequence select="aws:s3-put-object($s3-bucket-name, $key-name, $file-content, $aws-public-key, $aws-private-key, false())"/>
  </xsl:template>

  <xsl:template match="param:*"/>

  <xsl:template match="text()">
    <xsl:sequence select="normalize-space(.)"/>
  </xsl:template>

</xsl:transform>
