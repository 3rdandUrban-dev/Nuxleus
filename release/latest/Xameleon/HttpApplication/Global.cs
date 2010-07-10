using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Xameleon.Transform;
using Xameleon.Configuration;
using Xameleon.Cryptography;
using Xameleon.Memcached;
using Memcached.ClientLibrary;
using System.Collections.Generic;
using Saxon.Api;
using System.IO;
using System.Xml;
using System.Net;
using System.Text;

namespace Xameleon.HttpApplication
{

    public class Global : System.Web.HttpApplication
    {
        bool _useMemCached = false;
        bool _DEBUG = false;
        Client _memcachedClient = null;
        AppSettings _appSettings = new AppSettings();
        AspNetXameleonConfiguration _xameleonConfiguration = AspNetXameleonConfiguration.GetConfig();
        AspNetAwsConfiguration _awsConfiguration = AspNetAwsConfiguration.GetConfig();
        AspNetBungeeAppConfiguration _bungeeAppConfguration = AspNetBungeeAppConfiguration.GetConfig();
        XsltTransformationManager _xsltTransformationManager;
        Transform.Transform _transform = new Transform.Transform();
        Processor _processor = new Processor();
        Serializer _serializer = new Serializer();
        XmlUrlResolver _resolver = new XmlUrlResolver();
        Hashtable _namedXsltHashtable = new Hashtable();
        Hashtable _globalXsltParams = new Hashtable();
        Hashtable _transformContextHashtable = new Hashtable();
        Hashtable _requestXsltParams = null;
        BaseXsltContext _baseXsltContext;
        String _baseUri;
        HashAlgorithm _hashAlgorithm = HashAlgorithm.SHA1;


        protected void Application_Start(object sender, EventArgs e)
        {
            if (_xameleonConfiguration.DebugMode == "yes") _DEBUG = true;

            if (_xameleonConfiguration.UseMemcached == "yes")
            {
                _useMemCached = true;
                _memcachedClient = new Client(new MemcachedClient(), AspNetMemcachedConfiguration.GetConfig());
            }

            string baseUri = (string)_xameleonConfiguration.PreCompiledXslt.BaseUri;
            if (baseUri != String.Empty)
                baseUri = (string)_xameleonConfiguration.PreCompiledXslt.BaseUri;
            else
                baseUri = "~";

            _xsltTransformationManager = new XsltTransformationManager(_processor, _transform, _resolver, _serializer);
            _xsltTransformationManager.HashAlgorithm = _hashAlgorithm;
            _resolver.Credentials = CredentialCache.DefaultCredentials;
            _namedXsltHashtable = _xsltTransformationManager.NamedXsltHashtable;

            string hashkey = (string)_xameleonConfiguration.BaseSettings.ObjectHashKey;
            Application["hashkey"] = hashkey;

            foreach (PreCompiledXslt xslt in _xameleonConfiguration.PreCompiledXslt)
            {
                string localBaseUri = (string)_xameleonConfiguration.PreCompiledXslt.BaseUri;
                if (localBaseUri == String.Empty)
                    localBaseUri = baseUri;
                Uri xsltUri = new Uri(HttpContext.Current.Server.MapPath(localBaseUri + xslt.Uri));
                _xsltTransformationManager.Compiler.BaseUri = xsltUri;
                _xsltTransformationManager.AddTransformer(xslt.Name, xsltUri, _resolver, xslt.InitialMode, xslt.InitialTemplate);
                _namedXsltHashtable.Add(xslt.Name, xsltUri);
                if (xslt.UseAsBaseXslt == "yes")
                {
                    _baseXsltContext = new BaseXsltContext(xsltUri, XsltTransformationManager.GenerateNamedETagKey(xslt.Name, xsltUri), xslt.Name);
                }
            }

            _xsltTransformationManager.SetBaseXsltContext(_baseXsltContext);

            foreach (XsltParam xsltParam in _xameleonConfiguration.GlobalXsltParam)
            {
                _globalXsltParams[xsltParam.Name] = (string)xsltParam.Select;
            }

            if (_memcachedClient != null)
                Application["as_memcached"] = _memcachedClient;
            Application["as_usememcached"] = _useMemCached;
            Application["as_xslTransformationManager"] = _xsltTransformationManager;
            Application["as_namedXsltHashtable"] = _namedXsltHashtable;
            Application["as_globalXsltParams"] = _globalXsltParams;
            Application["as_debug"] = _DEBUG;

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Application["as_memcached"] != null)
                Application["memcached"] = Application["as_memcached"];
            Application["usememcached"] = Application["as_usememcached"];
            Application["xslTransformationManager"] = Application["as_xslTransformationManager"];
            Application["namedXsltHashtable"] = Application["as_namedXsltHashtable"];
            Application["globalXsltParams"] = Application["as_globalXsltParams"];
            Application["debug"] = Application["as_debug"];
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            System.Web.HttpApplication application = (System.Web.HttpApplication)sender;
            if ((bool)Application["debug"])
                application.Context.Response.Write((string)Application["debugOutput"]);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            SockIOPool.GetInstance().Shutdown();
        }
    }
}
