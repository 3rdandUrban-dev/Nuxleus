/*  
	Developed by Matt Perdeck and published at http://www.codeproject.com/KB/aspnet/CombineAndMinify.aspx 
	As specified at the above URI this code has been licensed under The Code Project Open License (CPOL)
	A copy of this license has been provided in the ~/license folder of this project and can be viewed online at http://www.codeproject.com/info/cpol10.aspx 
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Nuxleus.Web.Utils
{
    /// <summary>
    /// Needed to create the array of urls in web.config
    /// </summary>
    public class ConfigUrlElement: ConfigurationElement
    {
        private static ConfigurationProperty _urlProperty;
        private static ConfigurationPropertyCollection _properties;

        static ConfigUrlElement()
        {
            _urlProperty = new ConfigurationProperty(
                "url",
                typeof(string)
            );

            _properties = new ConfigurationPropertyCollection();
            _properties.Add(_urlProperty);
        }

        [ConfigurationProperty("url", IsRequired = false)]
        public string Url
        {
            get { return (string)base[_urlProperty]; }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get { return _properties; }
        }
    }
}
