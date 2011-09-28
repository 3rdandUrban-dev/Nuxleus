<%--<%@ Application Inherits="Xameleon.HttpApplication.Global" Language="C#" %>--%>
<%@ Application Language="C#" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Configuration" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Web" %>
<%@ Import Namespace="System.Web.Security" %>
<%@ Import Namespace="System.Web.SessionState" %>
<%@ Import Namespace="Xameleon.Transform" %>
<%@ Import Namespace="Xameleon.Cryptography" %>
<%@ Import Namespace="Xameleon.Configuration" %>
<%@ Import Namespace="Memcached.ClientLibrary" %>
<%@ Import Namespace="Saxon.Api" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.Xml" %>

<script RunAt="server">
    bool _useMemCached = false;
    bool _DEBUG = false;
    MemcachedClient _memcachedClient = null;
    SockIOPool _pool = null;
    AppSettings _appSettings = new AppSettings();
    AspNetXameleonConfiguration _xameleonConfiguration = AspNetXameleonConfiguration.GetConfig();
    AspNetAwsConfiguration _awsConfiguration = AspNetAwsConfiguration.GetConfig();
    AspNetBungeeAppConfiguration _bungeeAppConfguration = AspNetBungeeAppConfiguration.GetConfig();
    AspNetMemcachedConfiguration _memcachedConfiguration = AspNetMemcachedConfiguration.GetConfig();
    XsltTransformationManager _xsltTransformationManager;
    Transform _transform = new Transform();
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
            _memcachedClient = new Xameleon.Memcached.Client(new MemcachedClient(), AspNetMemcachedConfiguration.GetConfig());
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
            Application["memcached"] = _memcachedClient;
        Application["usememcached"] = _useMemCached;
        Application["xslTransformationManager"] = _xsltTransformationManager;
        Application["namedXsltHashtable"] = _namedXsltHashtable;
        Application["globalXsltParams"] = _globalXsltParams;
        Application["debug"] = _DEBUG;

    }

    protected void Application_BeginRequest(object sender, EventArgs e)
    {

    }

    protected void Application_EndRequest(object sender, EventArgs e)
    {

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

</script>


