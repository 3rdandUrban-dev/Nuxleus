/*  
	Developed by Matt Perdeck and published at http://www.codeproject.com/KB/aspnet/CombineAndMinify.aspx 
	As specified at the above URI this code has been licensed under The Code Project Open License (CPOL)
	A copy of this license has been provided in the ~/license folder of this project and can be viewed online at http://www.codeproject.com/info/cpol10.aspx 
*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Configuration;
using System.Text;
using System.Web;

// For info on using arrays in a config element, see
// http://www.codeproject.com/kb/dotnet/mysteriesofconfiguration.aspx#elementcolls
// http://viswaug.wordpress.com/2007/09/19/using-custom-names-for-the-configurationelementcollection-entries/

namespace Nuxleus.Web.Utils
{
    public class ConfigSection: ConfigurationSection
    {
        public enum HeadCachingOption { None, PerSite, PerFolder, PerPage, PerUrl }
        public enum CombineOption { None, PerGroup, All }
        public enum ActiveOption { Never, Always, ReleaseModeOnly, DebugModeOnly }

        // ----------------------

        [ConfigurationProperty("active", DefaultValue = ActiveOption.ReleaseModeOnly, IsRequired = false)]
        public ActiveOption Active
        {
            get { return (ActiveOption)base["active"]; }
            set { base["active"] = value; }
        }

        // -----------------------

        [ConfigurationProperty("exceptionOnMissingFile", DefaultValue = ActiveOption.Never, IsRequired = false)]
        public ActiveOption ExceptionOnMissingFile
        {
            get { return (ActiveOption)base["exceptionOnMissingFile"]; }
            set { base["exceptionOnMissingFile"] = value; }
        }

        // -----------------------

        [ConfigurationProperty("headCaching", DefaultValue = HeadCachingOption.None, IsRequired = false)]
        public HeadCachingOption HeadCaching 
        {
            get { return (HeadCachingOption)base["headCaching"]; }
            set { base["headCaching"] = value; }
        }

        // -----------------------

        [ConfigurationProperty("combineCSSFiles", DefaultValue = CombineOption.PerGroup, IsRequired = false)]
        public CombineOption CombineCSSFiles 
        {
            get { return (CombineOption)base["combineCSSFiles"]; }
            set { base["combineCSSFiles"] = value; }
        }

        [ConfigurationProperty("combineJavaScriptFiles", DefaultValue = CombineOption.PerGroup, IsRequired = false)]
        public CombineOption CombineJavaScriptFiles 
        {
            get { return (CombineOption)base["combineJavaScriptFiles"]; }
            set { base["combineJavaScriptFiles"] = value; }
        }

        // ----------------------

        [ConfigurationProperty("minifyCSS", DefaultValue = "true", IsRequired = false)]
        public bool MinifyCSS
        {
            get { return (bool)base["minifyCSS"]; }
            set { base["minifyCSS"] = value; }
        }

        // ----------------------

        [ConfigurationProperty("minifyJavaScript", DefaultValue = "true", IsRequired = false)]
        public bool MinifyJavaScript
        {
            get { return (bool)base["minifyJavaScript"]; }
            set { base["minifyJavaScript"] = value; }
        }

        // -----------------------

        [ConfigurationProperty("enableCookielessDomains", DefaultValue = ActiveOption.Always, IsRequired = false)]
        public ActiveOption EnableCookielessDomains
        {
            get { return (ActiveOption)base["enableCookielessDomains"]; }
            set { base["enableCookielessDomains"] = value; }
        }

        [ConfigurationProperty("cookielessDomains", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(ConfigDomainElementCollection))]
        private ConfigDomainElementCollection CookielessDomainCollection
        {
            get { return (ConfigDomainElementCollection)base["cookielessDomains"]; }
        }

        public List<string> CookielessDomains
        {
            get
            {
                List<string> cookielessDomains = new List<string>();

                ConfigDomainElementCollection cdec = this.CookielessDomainCollection;
                foreach (ConfigDomainElement cde in cdec)
                {
                    string domain = cde.Domain.Trim(new char[] { ' ', '/' });
                    if (!CombinedFile.UrlStartsWithProtocol(domain))
                    {
                        throw new ArgumentException(
                            "Domain " + domain + " in section cookielessDomains in web.config does not start with http:// or https://");
                    }

                    cookielessDomains.Add(domain);
                }

                return cookielessDomains;
            }
        }

        // ----------------------

        [ConfigurationProperty("preloadAllImages", DefaultValue = "false", IsRequired = false)]
        public bool PreloadAllImages
        {
            get { return (bool)base["preloadAllImages"]; }
            set { base["preloadAllImages"] = value; }
        }

        [ConfigurationProperty("prioritizedImages", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(ConfigUrlElementCollection))]
        private ConfigUrlElementCollection PrioritizedImagesCollection
        {
            get { return (ConfigUrlElementCollection)base["prioritizedImages"]; }
        }

        public List<string> PrioritizedImages
        {
            get
            {
                List<string> prioritizedImages = new List<string>();

                ConfigUrlElementCollection cdec = this.PrioritizedImagesCollection;
                foreach (ConfigUrlElement cde in cdec)
                {
                    string url = cde.Url;
                    prioritizedImages.Add(url);
                }

                return prioritizedImages;
            }
        }

        // ----------------------

        [ConfigurationProperty("makeImageUrlsLowercase", DefaultValue = "false", IsRequired = false)]
        public bool MakeImageUrlsLowercase
        {
            get { return (bool)base["makeImageUrlsLowercase"]; }
            set { base["makeImageUrlsLowercase"] = value; }
        }

        // ----------------------

        [ConfigurationProperty("insertVersionIdInImageUrls", DefaultValue = "false", IsRequired = false)]
        public bool InsertVersionIdInImageUrls
        {
            get { return (bool)base["insertVersionIdInImageUrls"]; }
            set { base["insertVersionIdInImageUrls"] = value; }
        }

        // ---------------------

        [ConfigurationProperty("removeWhitespace", DefaultValue = "false", IsRequired = false)]
        public bool RemoveWhitespace
        {
            get { return (bool)base["removeWhitespace"]; }
            set { base["removeWhitespace"] = value; }
        }

        // ------------------------

        public static bool OptionIsActive(ConfigSection.ActiveOption activeOption)
        {
            bool optionIsActive =
                (activeOption == ConfigSection.ActiveOption.Always) ||
                ((activeOption == ConfigSection.ActiveOption.ReleaseModeOnly) &&
                 (!HttpContext.Current.IsDebuggingEnabled)) ||
                ((activeOption == ConfigSection.ActiveOption.DebugModeOnly) &&
                 (HttpContext.Current.IsDebuggingEnabled));

            return optionIsActive;
        }

        // ------------------------

        public static ConfigSection CurrentConfigSection()
        {
            System.Configuration.Configuration config =
                WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
            ConfigSection cs = (ConfigSection)config.GetSection("combineAndMinify");

            // ------
            // Throw exception if there are inconsistent attributes

            if (OptionIsActive(cs.Active) && OptionIsActive(cs.ExceptionOnMissingFile) && (!cs.InsertVersionIdInImageUrls))
            {
                throw new ArgumentException(
                    "Attribute exceptionOnMissingFile is active while attribute insertVersionIdInImageUrls is false. " +
                    "This is inconsistent, because the package will only check all images if insertVersionIdInImageUrls is true. " +
                    "You can solve this by either setting exceptionOnMissingFile to Never, or by setting insertVersionIdInImageUrls to true.");
            }

            return cs;
        }
    }
}
