using System;
using System.Xml;
using System.IO;
using Saxon.Api;
using System.Collections.Specialized;
using System.Web;
using Nuxleus.Configuration;
using System.Net;
using Xameleon.Function;
using System.Collections;
using Enyim.Caching;
using System.Text;
using Nuxleus.Cryptography;
using System.Reflection;

namespace Nuxleus.Transform
{

    public struct Context
    {

        String _requestUri;
        String _requestXmlETag;
        FileInfo _requestXmlFileInfo;
        String _eTag;
        String _hashKey;
        Hashtable _xsltParams;
        HashAlgorithm _hashAlgorithm;
        NameValueCollection _httpQueryString;
        NameValueCollection _httpForm;
        HttpCookieCollection _httpCookies;
        NameValueCollection _httpParams;

        public Context(HttpContext context, HashAlgorithm algorithm, String key, FileInfo fileInfo, Hashtable xsltParams, params object[] eTagArray)
        {
            _requestUri = fileInfo.FullName;
            _requestXmlFileInfo = fileInfo;
            _xsltParams = xsltParams;
            _httpQueryString = context.Request.QueryString;
            _httpForm = context.Request.Form;
            _httpCookies = context.Request.Cookies;
            _httpParams = context.Request.Params;
            _hashKey = key;
            _hashAlgorithm = algorithm;
            _requestXmlETag = GenerateETag(key, algorithm, fileInfo.LastWriteTimeUtc, fileInfo.Length, _requestUri);
            _eTag = GenerateETag(key, algorithm, _requestUri, eTagArray);
        }

        public String RequestUri
        {
            get { return _requestUri; }
            set { _requestUri = value; }
        }
        public String RequestXmlETag
        {
            get { return _requestXmlETag; }
            set { _requestXmlETag = value; }
        }
        public FileInfo RequestXmlFileInfo
        {
            get { return _requestXmlFileInfo; }
            set { _requestXmlFileInfo = value; }
        }
        public String ETag
        {
            get { return _eTag; }
            set { _eTag = value; }
        }
        public Hashtable XsltParams
        {
            get { return _xsltParams; }
            set { _xsltParams = value; }
        }
        public NameValueCollection HttpParams
        {
            get { return _httpParams; }
            set { _httpParams = value; }
        }
        public HttpCookieCollection HttpCookies
        {
            get { return _httpCookies; }
            set { _httpCookies = value; }
        }
        public NameValueCollection HttpForm
        {
            get { return _httpForm; }
            set { _httpForm = value; }
        }
        public NameValueCollection HttpQueryString
        {
            get { return _httpQueryString; }
            set { _httpQueryString = value; }
        }
        public static string GenerateETag(string key, HashAlgorithm algorithm, params object[] eTagArray)
        {
            return HashcodeGenerator.GetHMACHashBase64String(key, algorithm, eTagArray);
        }
        public string GetRequestHashcode(bool useQueryString, params object[] objectArray)
        {
            StringBuilder builder = new StringBuilder(_eTag);
            builder.Append(_xsltParams);
            if (useQueryString)
                builder.Append(_httpQueryString);
            builder.Append(_httpForm.ToString());
            return GenerateETag(_hashKey, _hashAlgorithm, builder);
        }
        public void Clear()
        {
            //FOR FUTURE USE
        }
    }
}

