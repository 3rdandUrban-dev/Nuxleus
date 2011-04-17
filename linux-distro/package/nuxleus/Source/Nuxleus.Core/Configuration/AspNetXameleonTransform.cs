using System;
using System.Configuration;

namespace Nuxleus.Configuration
{

    public class AspNetXameleonConfiguration : ConfigurationSection
    {


        public static AspNetXameleonConfiguration GetConfig()
        {
            return (AspNetXameleonConfiguration)ConfigurationManager.GetSection("Xameleon.WebApp/xameleon");
        }

        [ConfigurationProperty("debugMode", IsRequired = true)]
        public string DebugMode
        {
            get
            {
                return this["debugMode"] as string;
            }
        }

        [ConfigurationProperty("useMemcached", DefaultValue = "no", IsRequired = false)]
        public string UseMemcached
        {
            get
            {
                return this["useMemcached"] as string;
            }
        }

        [ConfigurationProperty("objectHashKey", IsRequired = true)]
        public string ObjectHashKey {
            get {
                return this["objectHashKey"] as string;
            }
        }

        [ConfigurationProperty("defaultEngine", DefaultValue = "Saxon", IsRequired = false)]
        public string DefaultEngine
        {
            get
            {
                return this["defaultEngine"] as string;
            }
        }

        [ConfigurationProperty("baseSettings", IsRequired = false)]
        public BaseSettingCollection BaseSettings
        {
            get
            {
                return this["baseSettings"] as BaseSettingCollection;
            }
        }

        [ConfigurationProperty("preCompiledXslt", IsRequired = false)]
        public PreCompiledXsltCollection PreCompiledXslt
        {
            get
            {
                return this["preCompiledXslt"] as PreCompiledXsltCollection;
            }
        }

        [ConfigurationProperty("globalXsltParams", IsRequired = false)]
        public XsltParamCollection GlobalXsltParam
        {
            get
            {
                return this["globalXsltParams"] as XsltParamCollection;
            }
        }

        [ConfigurationProperty("sessionXsltParams", IsRequired = false)]
        public XsltParamCollection SessionXsltParam
        {
            get
            {
                return this["sessionXsltParams"] as XsltParamCollection;
            }
        }

        [ConfigurationProperty("httpContextXsltParams", IsRequired = false)]
        public XsltParamCollection HttpRequestXsltParams
        {
            get
            {
                return this["httpContextXsltParams"] as XsltParamCollection;
            }
        }

        [ConfigurationProperty("requestXsltParams", IsRequired = false)]
        public XsltParamCollection RequestXsltParams
        {
            get
            {
                return this["requestXsltParams"] as XsltParamCollection;
            }
        }
    }
}
