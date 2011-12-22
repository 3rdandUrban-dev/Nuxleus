using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using Enyim.Caching;
using Nuxleus.Bucker;
using Nuxleus.Configuration;
using Nuxleus.Cryptography;
using Nuxleus.Extension;
using Nuxleus.Geo;
using Nuxleus.Memcached;
using Nuxleus.Transform;
using Saxon.Api;

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
        XsltTransformationManager m_xsltTransformationManager;
        XPathServiceOperationManager m_xmlServiceOperationManager;
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
        UTF8Encoding m_encoding;
        PledgeCount m_pledgeCount;
        Queue<string> m_pledgeQueue;
        static HashAlgorithm m_hashAlgorithm = HashAlgorithm.MD5;

        protected void Application_Start (object sender, EventArgs e)
        {
            //System.Web.HttpApplication application = (System.Web.HttpApplication)sender;

            m_useMemCached = false;
            m_DEBUG = false;
            m_memcachedClient = null;
            m_appSettings = new AppSettings ();
            m_xameleonConfiguration = AspNetXameleonConfiguration.GetConfig ();
            string hashkey = (string)m_xameleonConfiguration.ObjectHashKey;
            Application ["hashkey"] = hashkey;
            m_awsConfiguration = AspNetAwsConfiguration.GetConfig ();
            m_transform = new Transform.Transform ();
            m_processor = new Processor ();
            m_serializer = new Serializer ();
            m_resolver = new XmlUrlResolver ();
            m_namedXsltHashtable = new Hashtable ();
            m_globalXsltParams = new Hashtable ();
            m_transformContextHashtable = new Hashtable ();
            m_xmlServiceOperationManager = new XPathServiceOperationManager (new Dictionary<int, XPathNavigator> ());
            m_geoIPLookup = new Dictionary<String, IPLocation> ();
            m_requestXsltParams = null;
            m_encoding = new UTF8Encoding ();
            m_pledgeCount = new PledgeCount (0, 0);
            m_pledgeQueue = new Queue<string> ();

            string sdbAccessKey = String.Empty;
            string sdbSecretKey = String.Empty;
            string awsUriEndpoint = "https://sdb.amazonaws.com/";

            Environment.SetEnvironmentVariable ("AWS_URI_ENDPOINT", awsUriEndpoint);
            

            using (XmlReader configReader = XmlReader.Create(HttpContext.Current.Server.MapPath("~/App_Config/aws.config"))) {
                while (configReader.Read()) {
                    if (configReader.IsStartElement ()) {
                        switch (configReader.Name) {
                        case "sdb-access-key":
                            {
                                sdbAccessKey = configReader.ReadString ();
                                Environment.SetEnvironmentVariable ("AWS_PUBLIC_KEY", sdbAccessKey);
                                this.LogInfo ("SDB_ACCESS_KEY: {0}", sdbAccessKey);
                                break;
                            }
                        case "sdb-secret-key":
                            {
                                sdbSecretKey = configReader.ReadString ();
                                Environment.SetEnvironmentVariable ("AWS_PRIVATE_KEY", sdbSecretKey);
                                this.LogInfo ("SDB_PRIVATE_KEY: {0}", sdbSecretKey);
                                break;
                            }
                        default:
                            break;
                        }
                    }
                }
            }

            if (m_xameleonConfiguration.DebugMode == "yes")
                m_DEBUG = true;

            if (m_xameleonConfiguration.UseMemcached == "yes") {
                m_useMemCached = true;
                m_memcachedClient = new Client (new MemcachedClient (), AspNetMemcachedConfiguration.GetConfig ());
            }

            string baseUri = (string)m_xameleonConfiguration.PreCompiledXslt.BaseUri;
            if (baseUri != String.Empty)
                baseUri = (string)m_xameleonConfiguration.PreCompiledXslt.BaseUri;
            else
                baseUri = "~";

            m_xsltTransformationManager = new XsltTransformationManager (m_processor, m_transform, m_resolver, m_serializer);
            m_xsltTransformationManager.HashAlgorithm = m_hashAlgorithm;
            m_resolver.Credentials = CredentialCache.DefaultCredentials;
            m_namedXsltHashtable = m_xsltTransformationManager.NamedXsltHashtable;

            foreach (PreCompiledXslt xslt in m_xameleonConfiguration.PreCompiledXslt) {
                string localBaseUri = (string)m_xameleonConfiguration.PreCompiledXslt.BaseUri;
                if (localBaseUri == String.Empty)
                    localBaseUri = baseUri;
                Uri xsltUri = new Uri (HttpContext.Current.Server.MapPath (localBaseUri + xslt.Uri));
                m_xsltTransformationManager.Compiler.BaseUri = xsltUri;
                m_xsltTransformationManager.AddTransformer (xslt.Name, xsltUri, m_resolver, xslt.InitialMode, xslt.InitialTemplate);
                m_namedXsltHashtable.Add (xslt.Name, xsltUri);
                if (xslt.UseAsBaseXslt == "yes") {
                    m_baseXsltContext = new BaseXsltContext (xsltUri, XsltTransformationManager.GenerateNamedETagKey (xslt.Name, xsltUri), xslt.Name);
                }
            }

            m_xsltTransformationManager.SetBaseXsltContext (m_baseXsltContext);

            foreach (XsltParam xsltParam in m_xameleonConfiguration.GlobalXsltParam) {
                m_globalXsltParams [xsltParam.Name] = (string)xsltParam.Select;
            }

            if (m_memcachedClient != null)
                Application ["as_memcached"] = m_memcachedClient;
            Application ["as_usememcached"] = m_useMemCached;
            Application ["as_xslTransformationManager"] = m_xsltTransformationManager;
            Application ["as_xmlServiceOperationManager"] = m_xmlServiceOperationManager;
            Application ["as_namedXsltHashtable"] = m_namedXsltHashtable;
            Application ["as_globalXsltParams"] = m_globalXsltParams;
            Application ["as_geoIPLookup"] = m_geoIPLookup;
            Application ["as_debug"] = m_DEBUG;
            Application ["as_hashkey"] = hashkey;
            Application ["as_encoding"] = m_encoding;
            Application ["as_pledgeCount"] = m_pledgeCount;
            Application ["as_pledgeQueue"] = m_pledgeQueue;
        }

        protected void Application_BeginRequest (object sender, EventArgs e)
        {
            System.Web.HttpApplication application = (System.Web.HttpApplication)sender;
            if (Application ["as_memcached"] != null)
                Application ["memcached"] = Application ["as_memcached"];
            Application ["usememcached"] = Application ["as_usememcached"];
            Application ["xslTransformationManager"] = Application ["as_xslTransformationManager"];
            Application ["xmlServiceOperationManager"] = Application ["as_xmlServiceOperationManager"];
            Application ["namedXsltHashtable"] = Application ["as_namedXsltHashtable"];
            Application ["globalXsltParams"] = Application ["as_globalXsltParams"];
            Application ["geoIPLookup"] = Application ["as_geoIPLookup"];
            Application ["debug"] = Application ["as_debug"];
            Application ["hashkey"] = Application ["as_hashkey"];
            Application ["encoding"] = Application ["as_encoding"];
            Application ["pledgeCount"] = Application ["as_pledgeCount"];
            Application ["pledgeQueue"] = Application ["as_pledgeQueue"];
            Application ["yahooApiKey"] = "0f24246faa";

            Environment.SetEnvironmentVariable ("APPLICATION_ROOT_PATH", application.Request.PhysicalApplicationPath);
        }

        protected void Application_EndRequest (object sender, EventArgs e)
        {
            System.Web.HttpApplication application = (System.Web.HttpApplication)sender;
            //if ((bool)Application["debug"])
            //    application.Context.Response.Write((string)Application["debugOutput"]);
        }

        protected void Application_AuthenticateRequest (object sender, EventArgs e)
        {

        }

        protected void Application_Error (object sender, EventArgs e)
        {

        }

        protected void Application_End (object sender, EventArgs e)
        {
            QueueClientPool.Shutdown ();
            //SockIOPool.GetInstance ().Shutdown ();
        }
    }

    public struct PledgeCount
    {

        int m_pledgeCountTotal;
        int m_pledgeCountDistrict;

        public PledgeCount (int pledgeCountTotal, int pledgeCountDistrict)
        {
            m_pledgeCountTotal = pledgeCountTotal;
            m_pledgeCountDistrict = pledgeCountDistrict;
        }

        public int PledgeCountTotal { get { return m_pledgeCountTotal; } set { m_pledgeCountTotal = value; } }

        public int PledgeCountDistrict { get { return m_pledgeCountDistrict; } set { m_pledgeCountDistrict = value; } }
    }
}