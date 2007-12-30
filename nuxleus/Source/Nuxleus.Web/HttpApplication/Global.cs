using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Nuxleus.Transform;
using Nuxleus.Configuration;
using Nuxleus.Cryptography;
using Nuxleus.Memcached;
using Memcached.ClientLibrary;
using System.Collections.Generic;
using Saxon.Api;
using System.IO;
using System.Xml;
using System.Net;
using System.Text;
using Nuxleus.Bucker;
using System.Web.Hosting;
using Nuxleus.Geo;
using Nuxleus.Geo.MaxMind;

namespace Nuxleus.Web.HttpApplication
{
    public class Global : System.Web.HttpApplication
    {
        bool m_useMemCached;
        bool m_DEBUG;
        Client m_memcachedClient;
        AppSettings m_appSettings;
        AspNetXameleonConfiguration m_xameleonConfiguration;
        AspNetAwsConfiguration m_awsConfiguration;
        AspNetBungeeAppConfiguration m_bungeeAppConfguration;
        AspNetQueueServerConfiguration m_queueServerConfiguration;
        XsltTransformationManager m_xsltTransformationManager;
        XmlServiceOperationManager m_xmlServiceOperationManager;
        Transform.Transform m_transform;
        Processor m_processor;
        Serializer m_serializer;
        XmlUrlResolver m_resolver;
        Hashtable m_namedXsltHashtable;
        Hashtable m_globalXsltParams;
        Hashtable m_transformContextHashtable;
        Dictionary<String, IPLocation> m_geoIPLookup;
        Hashtable m_requestXsltParams;
        BaseXsltContext m_baseXsltContext;
        String m_baseUri;
        static HashAlgorithm m_hashAlgorithm = HashAlgorithm.MD5;

        protected void Application_Start(object sender, EventArgs e)
        {

            m_useMemCached = false;
            m_DEBUG = false;
            m_memcachedClient = null;
            m_appSettings = new AppSettings();
            m_xameleonConfiguration = AspNetXameleonConfiguration.GetConfig();
            m_awsConfiguration = AspNetAwsConfiguration.GetConfig();
            m_bungeeAppConfguration = AspNetBungeeAppConfiguration.GetConfig();
            m_queueServerConfiguration = AspNetQueueServerConfiguration.GetConfig();
            m_transform = new Transform.Transform();
            m_processor = new Processor();
            m_serializer = new Serializer();
            m_resolver = new XmlUrlResolver();
            m_namedXsltHashtable = new Hashtable();
            m_globalXsltParams = new Hashtable();
            m_transformContextHashtable = new Hashtable();
            m_xmlServiceOperationManager = new XmlServiceOperationManager(new Dictionary<int, XmlReader>());
            m_geoIPLookup = new Dictionary<String, IPLocation>();
            m_requestXsltParams = null;

            using(XmlReader configReader = XmlReader.Create(HttpContext.Current.Server.MapPath("~/App_Data/aws.config")))
            {
             while (configReader.Read())
                {
                    if (configReader.IsStartElement())
                    {
                        switch (configReader.Name)
                        {
                            case "sdb-access-key":
                                {
                                    Environment.SetEnvironmentVariable("SDB_ACCESS_KEY", configReader.ReadString());
                                    break;
                                }
                            case "sdb-secret-key":
                                {
                                    Environment.SetEnvironmentVariable("SDB_SECRET_KEY", configReader.ReadString());
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }
            }

            if (m_xameleonConfiguration.DebugMode == "yes") m_DEBUG = true;

            if (m_xameleonConfiguration.UseMemcached == "yes")
            {
                m_useMemCached = true;
                m_memcachedClient = new Client(new MemcachedClient(), AspNetMemcachedConfiguration.GetConfig());
            }

            QueueClientPool.Init(m_queueServerConfiguration.PoolSize,
                 m_queueServerConfiguration.IP,
                 m_queueServerConfiguration.Port,
                 m_queueServerConfiguration.Threshold);

            string baseUri = (string)m_xameleonConfiguration.PreCompiledXslt.BaseUri;
            if (baseUri != String.Empty)
                baseUri = (string)m_xameleonConfiguration.PreCompiledXslt.BaseUri;
            else
                baseUri = "~";

            m_xsltTransformationManager = new XsltTransformationManager(m_processor, m_transform, m_resolver, m_serializer);
            m_xsltTransformationManager.HashAlgorithm = m_hashAlgorithm;
            m_resolver.Credentials = CredentialCache.DefaultCredentials;
            m_namedXsltHashtable = m_xsltTransformationManager.NamedXsltHashtable;

            string hashkey = (string)m_xameleonConfiguration.ObjectHashKey;
            Application["hashkey"] = hashkey;

            foreach (PreCompiledXslt xslt in m_xameleonConfiguration.PreCompiledXslt)
            {
                string localBaseUri = (string)m_xameleonConfiguration.PreCompiledXslt.BaseUri;
                if (localBaseUri == String.Empty)
                    localBaseUri = baseUri;
                Uri xsltUri = new Uri(HttpContext.Current.Server.MapPath(localBaseUri + xslt.Uri));
                m_xsltTransformationManager.Compiler.BaseUri = xsltUri;
                m_xsltTransformationManager.AddTransformer(xslt.Name, xsltUri, m_resolver, xslt.InitialMode, xslt.InitialTemplate);
                m_namedXsltHashtable.Add(xslt.Name, xsltUri);
                if (xslt.UseAsBaseXslt == "yes")
                {
                    m_baseXsltContext = new BaseXsltContext(xsltUri, XsltTransformationManager.GenerateNamedETagKey(xslt.Name, xsltUri), xslt.Name);
                }
            }

            m_xsltTransformationManager.SetBaseXsltContext(m_baseXsltContext);

            foreach (XsltParam xsltParam in m_xameleonConfiguration.GlobalXsltParam)
            {
                m_globalXsltParams[xsltParam.Name] = (string)xsltParam.Select;
            }

            if (m_memcachedClient != null)
                Application["as_memcached"] = m_memcachedClient;
            Application["as_usememcached"] = m_useMemCached;
            Application["as_xslTransformationManager"] = m_xsltTransformationManager;
            Application["as_xmlServiceOperationManager"] = m_xmlServiceOperationManager;
            Application["as_namedXsltHashtable"] = m_namedXsltHashtable;
            Application["as_globalXsltParams"] = m_globalXsltParams;
            Application["as_geoIPLookup"] = m_geoIPLookup;
            Application["as_debug"] = m_DEBUG;
            Application["as_hashkey"] = hashkey;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Application["as_memcached"] != null)
                Application["memcached"] = Application["as_memcached"];
            Application["usememcached"] = Application["as_usememcached"];
            Application["xslTransformationManager"] = Application["as_xslTransformationManager"];
            Application["xmlServiceOperationManager"] = Application["as_xmlServiceOperationManager"];
            Application["namedXsltHashtable"] = Application["as_namedXsltHashtable"];
            Application["globalXsltParams"] = Application["as_globalXsltParams"];
            Application["geoIPLookup"] = Application["as_geoIPLookup"];
            Application["debug"] = Application["as_debug"];
            Application["hashkey"] = Application["as_hashkey"];
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
            QueueClientPool.Shutdown();
            SockIOPool.GetInstance().Shutdown();
        }
    }
}
