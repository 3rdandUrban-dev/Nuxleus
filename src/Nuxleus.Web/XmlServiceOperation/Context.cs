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

namespace Nuxleus.Web {

    public class Context
    {
        String m_requestUri;
        int m_requestXmlETag;
        FileInfo m_requestXmlFileInfo;
        int m_eTag;
        String m_hashKey;
        HashAlgorithm m_hashAlgorithm;
        NameValueCollection m_httpQueryString;
        NameValueCollection m_httpForm;
        HttpCookieCollection m_httpCookies;
        NameValueCollection m_httpParams;

        public Context ( HttpContext context, HashAlgorithm algorithm, String key, FileInfo fileInfo, params object[] eTagArray ) {
            m_requestUri = fileInfo.FullName;
            m_requestXmlFileInfo = fileInfo;
            m_httpQueryString = context.Request.QueryString;
            m_httpForm = context.Request.Form;
            m_httpCookies = context.Request.Cookies;
            m_httpParams = context.Request.Params;
            m_hashKey = key;
            m_hashAlgorithm = algorithm;
            m_requestXmlETag = GenerateETag(key, algorithm, fileInfo.LastWriteTimeUtc, fileInfo.Length, m_requestUri);
            m_eTag = GenerateETag(key, algorithm, m_requestUri, eTagArray);
        }

        public String RequestUri {
            get { return m_requestUri; }
            set { m_requestUri = value; }
        }
        public int RequestXmlETag {
            get { return m_requestXmlETag; }
            set { m_requestXmlETag = value; }
        }
        public FileInfo RequestXmlFileInfo {
            get { return m_requestXmlFileInfo; }
            set { m_requestXmlFileInfo = value; }
        }
        public int ETag {
            get { return m_eTag; }
            set { m_eTag = value; }
        }
        public NameValueCollection HttpParams {
            get { return m_httpParams; }
            set { m_httpParams = value; }
        }
        public HttpCookieCollection HttpCookies {
            get { return m_httpCookies; }
            set { m_httpCookies = value; }
        }
        public NameValueCollection HttpForm {
            get { return m_httpForm; }
            set { m_httpForm = value; }
        }
        public NameValueCollection HttpQueryString {
            get { return m_httpQueryString; }
            set { m_httpQueryString = value; }
        }
        public static int GenerateETag ( string key, HashAlgorithm algorithm, params object[] eTagArray ) {
            return HashcodeGenerator.GetHMACHashBase64String(key, algorithm, eTagArray).GetHashCode();
        }
        public int GetRequestHashcode ( bool useQueryString, params object[] objectArray ) {
            StringBuilder builder = new StringBuilder(m_eTag.ToString());
            if (useQueryString) {
                builder.Append(m_httpQueryString);
            }
            builder.Append(m_httpForm.ToString());
            return GenerateETag(m_hashKey, m_hashAlgorithm, builder, objectArray);
        }
        public void Clear () {
            //FOR FUTURE USE
        }
    }
}

