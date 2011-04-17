<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform version="2.0"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
    xmlns:fn="http://www.w3.org/2005/xpath-functions"
    xmlns:saxon="http://saxon.sf.net/"
    xmlns:clitype="http://saxon.sf.net/clitype"
    xmlns:at="http://atomictalk.org"
    xmlns:func="http://atomictalk.org/function"
    xmlns:aspnet="http://atomictalk.org/function/aspnet"
    xmlns:response="http://atomictalk.org/function/aspnet/response"
    xmlns:service="http://xameleon.org/service"
    xmlns:operation="http://xameleon.org/service/operation"
    xmlns:proxy="http://xameleon.org/service/proxy"
    xmlns:session="http://xameleon.org/service/session"
    xmlns:param="http://xameleon.org/service/session/param"
    xmlns:aws="http://xameleon.org/function/aws"
    xmlns:s3="http://xameleon.org/function/aws/s3"
    xmlns:header="http://xameleon.org/service/http/header"
    xmlns:metadata="http://xameleon.org/service/metadata"
    xmlns:test="http://xameleon.org/controller/test"
    xmlns:aspnet-timestamp="clitype:System.DateTime"
    xmlns:stream="clitype:System.IO.Stream"
    xmlns:sortedlist="clitype:System.Collections.SortedList"
    xmlns:uri="clitype:System.Uri?partialname=System"
    xmlns:http-util="clitype:System.Web.HttpUtility?partialname=System.Web"
    xmlns:web-response="clitype:System.Net.WebResponse?partialname=System"
    xmlns:aspnet-session="clitype:System.Web.SessionState.HttpSessionState?partialname=System.Web"
    xmlns:aspnet-context="clitype:System.Web.HttpContext?partialname=System.Web"
    xmlns:browser="clitype:System.Web.HttpBrowserCapabilities?partialname=System.Web"
    xmlns:aspnet-server="clitype:System.Web.HttpServerUtility?partialname=System.Web"
    xmlns:aspnet-request="clitype:System.Web.HttpRequest?partialname=System.Web"
    xmlns:aspnet-response="clitype:System.Web.HttpResponse?partialname=System.Web"
    xmlns:current-context="clitype:Xameleon.Function.GetHttpContext?partialname=Xameleon"
    xmlns:request-collection="clitype:Xameleon.Function.HttpRequestCollection?partialname=Xameleon"
    xmlns:response-collection="clitype:Xameleon.Function.HttpResponseCollection?partialname=Xameleon"
    xmlns:web-request="clitype:Xameleon.Function.HttpWebRequestStream?partialname=Xameleon"
    xmlns:http-response-stream="clitype:Xameleon.Function.HttpWebResponseStream?partialname=Xameleon"
    xmlns:s3-object-compare="clitype:Xameleon.Function.S3ObjectCompare?partialname=Xameleon"
    xmlns:http-sgml-to-xml="clitype:Xameleon.Function.HttpSgmlToXml?partialname=Xameleon"
    xmlns:s3response="clitype:Nuxleus.Utility.S3.Response?partialname=Nuxleus.Utility"
    xmlns:aws-conn="clitype:Nuxleus.Utility.S3.AWSAuthConnection?partialname=Nuxleus.Utility"
    xmlns:aws-gen="clitype:Nuxleus.Utility.S3.QueryStringAuthGenerator?partialname=Nuxleus.Utility"
    xmlns:s3object="clitype:Nuxleus.Utility.S3.S3Object?partialname=Nuxleus.Utility"
    xmlns:queue="http://xameleon.org/service/queue" 
    xmlns:amazonaws="http://s3.amazonaws.com/doc/2006-03-01/"
    xmlns:html="http://www.w3.org/1999/xhtml"
    xmlns:profile="http://nuxleus.com/profile" 
    exclude-result-prefixes="#all">

  <xsl:import href="../../../model/json-to-xml.xslt"/>
  <xsl:import href="../../test/base.xslt"/>

  <xsl:param name="current-context" />
  <xsl:param name="response" />
  <xsl:param name="request" />
  <xsl:param name="server" />
  <xsl:param name="session" />
  <xsl:param name="timestamp" />
  <xsl:param name="aws-public-key" />
  <xsl:param name="aws-private-key" />
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
  <xsl:variable name="q">"</xsl:variable>

  <!--   
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
  -->
  
  <xsl:template match="at:test">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="operation:test">
    <xsl:apply-templates />
  </xsl:template>
  
  <xsl:template match="operation:queue">
    <xsl:apply-templates />
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

  <xsl:template match="operation:json-to-xml">
    <external-json>
      <xsl:apply-templates/>
    </external-json>
  </xsl:template>

  <xsl:template match="proxy:get-json">
    <xsl:variable name="uri" select="func:resolve-variable(@uri)"/>
    <xsl:apply-templates>
      <xsl:with-param name="xml" select="func:json-to-xml(concat($uri, '?appid=EricBlogDemo&amp;city=Seattle&amp;state=wa&amp;output=json'))"/>
    </xsl:apply-templates>
  </xsl:template>

  <xsl:template match="operation:return-xml">
    <xsl:param name="xml"/>
    <xsl:sequence select="$xml"/>
  </xsl:template>

  <xsl:template match="aws:s3">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="s3:check-for-existing-key">
    <xsl:variable name="folder-name" select="replace(func:resolve-variable(@folder), '%2F', '/')"/>
    <xsl:variable name="file-name" select="func:resolve-variable(@file)"/>
    <xsl:variable name="key-name" select="aws:s3-normalize-key($folder-name, $file-name)"/>
    <xsl:variable name="key-uri" select="aws:s3-get-signature($folder-name, $file-name, false())"/>
    <xsl:sequence
        select="aws:s3-put-object($s3-bucket-name, $key-name, $guid, $aws-public-key, $aws-private-key, false())"/>
    <!-- <xsl:variable name="compare" select="s3-object-compare:Compare($aws-conn, $s3-bucket-name, $key-name, $guid)"/>
    <xsl:variable name="compare-result" select="if($compare) then at:IfTrue else at:IfFalse"/>
    <xsl:apply-templates select="$compare-result">
      <xsl:with-param name="key-name" select="if($compare) then $folder-name else $key-name"/>
    </xsl:apply-templates> -->
  </xsl:template>

  <xsl:template match="operation:slurp">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="proxy:post-to-uri">
    <xsl:variable name="web-request" select="web-request:GetResponse(func:resolve-variable(@uri), 'year_start=&amp;year_end=&amp;s=Lawrence%20Lessig&amp;format=')"/>
    <xsl:variable name="slurped-data">
      <slurped-data>
        <xsl:sequence select="$web-request"/>
      </slurped-data>
    </xsl:variable>
    <xsl:sequence select="$slurped-data"/>
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
    <xsl:param name="key-name"/>
    <!-- <xsl:variable name="params">
      <xsl:apply-templates select="param:*" mode="eval">
        <xsl:with-param name="key-uri" select="$key-name"/>
      </xsl:apply-templates>
    </xsl:variable> 
    <xsl:apply-templates>
      <xsl:with-param name="key-uri" select="$key-name"/>
      <xsl:with-param name="params" select="$params"/>
    </xsl:apply-templates>-->
    <xsl:apply-templates />
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
  
  <xsl:template match="profile:return-profile">
    <xsl:variable name="name" select="func:resolve-variable(@name)"/>
    <profile>If I were a profile, I would be <xsl:value-of select="replace($name, '%2F', '/')"/>'s profile.</profile>
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

  <xsl:function name="aws:s3-normalize-key">
    <xsl:param name="folder"/>
    <xsl:param name="key"/>
    <xsl:sequence select="concat(if (not(ends-with($folder, '/'))) then concat($folder, '/') else $folder, $key)"/>
  </xsl:function>

  <xsl:function name="aws:s3-list-bucket">
    <xsl:param name="bucket" />
    <xsl:param name="prefix" />
    <xsl:param name="marker" />
    <xsl:param name="maxKeys" as="xs:integer"/>
    <xsl:param name="delimiter" />
    <xsl:sequence
        select="if ($expires-in) then aws-gen:listBucket($aws-gen, $bucket, $prefix, $marker, $maxKeys) else $not-set"/>
  </xsl:function>

  <xsl:function name="aws:s3-set-expires-in">
    <xsl:param name="aws-auth-gen"/>
    <xsl:param name="expires-in"/>
    <xsl:sequence
        select="aws-gen:set_ExpiresIn($aws-gen, $expires-in)"/>
  </xsl:function>

  <xsl:function name="aws:s3-get-signature">
    <xsl:param name="bucket" as="xs:string"/>
    <xsl:param name="key" as="xs:string"/>
    <xsl:param name="aws-public-key"/>
    <xsl:param name="aws-private-key"/>
    <xsl:param name="issecure" />
    <xsl:sequence
        select="if ($expires-in) then aws-gen:get($aws-gen, $bucket, $key) else $not-set"/>
  </xsl:function>

  <xsl:function name="aws:s3-get-signature">
    <xsl:param name="bucket" as="xs:string"/>
    <xsl:param name="key" as="xs:string"/>
    <xsl:param name="issecure"/>
    <xsl:variable name="aws-gen" select="aws-gen:new($aws-public-key, $aws-private-key, $issecure)"/>
    <xsl:value-of
        select="aws-gen:get($aws-gen, $bucket, $key)"/>
  </xsl:function>

  <xsl:function name="aws:s3-put-object">
    <xsl:param name="bucket" as="xs:string"/>
    <xsl:param name="key" as="xs:string"/>
    <xsl:param name="object" as="xs:string"/>
    <xsl:param name="pubkey" as="xs:string"/>
    <xsl:param name="privkey" as="xs:string"/>
    <xsl:param name="issecure" as="xs:boolean" />
    <xsl:variable name="s3Object" select="s3object:new($object)"/>
    <xsl:sequence
        select="s3response:getResponseMessage(aws-conn:put($aws-conn, $bucket, $key, $s3Object))"/>
  </xsl:function>

  <xsl:function name="aws:s3-get-object">
    <xsl:param name="bucket" as="xs:string"/>
    <xsl:param name="key" as="xs:string"/>
    <xsl:sequence
        select="s3response:getResponseMessage(aws-conn:get($aws-conn, $bucket, $key))"/>
  </xsl:function>

  <xsl:function name="aws:s3-create-bucket">
    <xsl:param name="publicKey" as="xs:string"/>
    <xsl:param name="privateKey" as="xs:string"/>
    <xsl:param name="issecure" as="xs:boolean"/>
    <xsl:param name="bucket" as="xs:string"/>
    <xsl:sequence
        select="s3response:getResponseMessage(aws-conn:createBucket($aws-conn, $bucket))"/>
  </xsl:function>

</xsl:transform>
