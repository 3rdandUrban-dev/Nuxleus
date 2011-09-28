<?xml version="1.0"?>
<xsl:transform version="2.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:func="http://atomictalk.org/function"
    xmlns:saxon="http://saxon.sf.net/"
    xmlns:enc="clitype:System.Text.UTF8Encoding"
    xmlns:string="clitype:System.String"
    xmlns:hmacsha1="clitype:System.Security.Cryptography.HMACSHA1"
    xmlns:convert="clitype:System.Convert"
    xmlns:s3="clitype:Xameleon.Utility.S3.AWSAuthConnection?partialname=Xameleon"
    xmlns:s3response="clitype:Xameleon.Utility.S3.Response?partialname=Xameleon"
    xmlns:awsAuth="clitype:Xameleon.Utility.S3.AWSAuthConnection?partialname=Xameleon"
    xmlns:auth="clitype:Xameleon.Utility.S3.QueryStringAuthGenerator?partialname=Xameleon"
    xmlns:s3object="clitype:Xameleon.Utility.S3.S3Object?partialname=Xameleon"
    xmlns:sortedlist="clitype:System.Collections.SortedList"
    xmlns:clitype="http://saxon.sf.net/clitype"
    exclude-result-prefixes="xs func enc hmacsha1 clitype sortedlist saxon string convert s3response awsAuth s3object">

  <xsl:param name="aws-public-key" />
  <xsl:param name="aws-private-key" />
  <xsl:variable name="issecure" select="false()" as="xs:boolean"/>
  <xsl:variable name="awsauth" select="auth:new($aws-public-key, $aws-private-key, $issecure)"/>
  <xsl:variable name="expires-in" select="func:s3-set-expires-in($awsauth, 15000)"/>
  
  <xsl:function name="func:s3-normalize-key">
    <xsl:param name="folder"/>
    <xsl:param name="key"/>
    <xsl:value-of select="concat(if (not(ends-with($folder, '/'))) then concat($folder, '/') else $folder, $key)"/>
  </xsl:function>

  <xsl:function name="func:s3-list-bucket">
    <xsl:param name="bucket" />
    <xsl:param name="prefix" />
    <xsl:param name="marker" />
    <xsl:param name="maxKeys" as="xs:integer"/>
    <xsl:param name="delimiter" />
    <xsl:variable name="expires-in" select="auth:set_ExpiresIn($awsauth, 15000)"/>
    <xsl:value-of
        select="if (empty($expires-in)) then auth:listBucket($awsauth, $bucket, $prefix, $marker, $maxKeys) else 'not-set'"/>
  </xsl:function>

  <xsl:function name="func:s3-set-expires-in">
    <xsl:param name="awsauth"/>
    <xsl:param name="expires-in"/>
    <xsl:value-of select="auth:set_ExpiresIn($awsauth, $expires-in)"/>
  </xsl:function>

  <xsl:function name="func:s3-get-signature">
    <xsl:param name="bucket" as="xs:string"/>
    <xsl:param name="key" as="xs:string"/>
    <xsl:param name="aws-public-key"/>
    <xsl:param name="aws-private-key"/>
    <xsl:param name="issecure" as="xs:boolean"/>
    <xsl:param name="request" as="xs:string"/>
    <xsl:variable name="awsauth" select="auth:new($aws-public-key, $aws-private-key, $issecure)"/>
    <xsl:value-of select="auth:set_ExpiresIn($awsauth, 15000)"/>
    <xsl:value-of
        select="auth:get($awsauth, $bucket, $key)"/>
  </xsl:function>

  <xsl:function name="func:s3-get-signature">
    <xsl:param name="bucket" as="xs:string"/>
    <xsl:param name="key" as="xs:string"/>
    <xsl:param name="issecure" as="xs:boolean"/>
    <xsl:param name="request" as="xs:string"/>
    <xsl:variable name="awsauth" select="auth:new($aws-public-key, $aws-private-key, $issecure)"/>
    <xsl:value-of select="auth:set_ExpiresIn($awsauth, 15000)"/>
    <xsl:value-of
        select="auth:get($awsauth, $bucket, $key)"/>
  </xsl:function>

  <xsl:function name="func:s3-put-object">
    <xsl:param name="bucket" as="xs:string"/>
    <xsl:param name="key" as="xs:string"/>
    <xsl:param name="object"/>
    <xsl:param name="pubkey"/>
    <xsl:param name="privkey"/>
    <xsl:param name="issecure" as="xs:boolean"/>
    <xsl:variable name="s3Object" select="s3object:new($object)"/>
    <xsl:variable name="awsauth" select="awsAuth:new($pubkey, $privkey, $issecure)"/>
    <xsl:value-of
        select="s3response:getResponseMessage(awsAuth:put($awsauth, $bucket, $key, $s3Object))"/>
  </xsl:function>

  <xsl:function name="func:s3-create-bucket">
    <xsl:param name="publicKey" as="xs:string"/>
    <xsl:param name="privateKey" as="xs:string"/>
    <xsl:param name="issecure" as="xs:boolean"/>
    <xsl:param name="bucket" as="xs:string"/>
    <xsl:value-of
        select="s3response:getResponseMessage(s3:createBucket(s3:new($publicKey, $privateKey, $issecure), $bucket))"/>
  </xsl:function>

</xsl:transform>
