
/* =======================================================================
 * vCard Library for .NET
 * Copyright (c) 2007 David Pinch; http://www.thoughtproject.com.
 * See LICENSE.TXT for licensing information.
 * ======================================================================= */

using System;

namespace Thought.Net.vCards
{

    /// <summary>
    ///     A web site defined in a vCard.
    /// </summary>
    /// <seealso cref="vCardWebSiteType"/>
    public class vCardWebSite
    {

        private string url;
        private vCardWebSiteType webSiteType;


        /// <summary>
        ///     Creates a vCardWebSite object.
        /// </summary>
        public vCardWebSite()
        {
            this.url = string.Empty;
        }


        /// <summary>
        ///     Creates a new vCardWebSite object with the specified URL.
        /// </summary>
        /// <param name="url">
        ///     The URL of the web site.
        /// </param>
        public vCardWebSite(string url)
        {
            this.url = url == null ? string.Empty : url;
        }


        /// <summary>
        ///     Creates a new vCardWebSite with the
        ///     specified URL and classification.
        /// </summary>
        /// <param name="url">
        ///     The URL of the web site.
        /// </param>
        /// <param name="webSiteType">
        ///     The classification of the web site.
        /// </param>
        public vCardWebSite(string url, vCardWebSiteType webSiteType)
        {
            this.url = url == null ? string.Empty : url;
            this.webSiteType = webSiteType;
        }


        /// <summary>
        ///     Indicates a personal home page.
        /// </summary>
        public bool IsPersonalSite
        {
            get
            {
                return (this.webSiteType & vCardWebSiteType.Personal) ==
                    vCardWebSiteType.Personal;
            }
            set
            {

                if (value)
                {
                    this.webSiteType |= vCardWebSiteType.Personal;
                }
                else
                {
                    this.webSiteType &= ~vCardWebSiteType.Personal;
                }

            }
        }


        /// <summary>
        ///     Indicates a work-related web site.
        /// </summary>
        public bool IsWorkSite
        {
            get
            {
                return (this.webSiteType & vCardWebSiteType.Work) ==
                    vCardWebSiteType.Work;
            }
            set
            {

                if (value)
                {
                    this.webSiteType |= vCardWebSiteType.Work;
                }
                else
                {
                    this.webSiteType &= ~vCardWebSiteType.Work;
                }

            }

        }


        /// <summary>
        ///     The URL of the web site.
        /// </summary>
        /// <remarks>
        ///     The format of the URL is not validated.
        /// </remarks>
        public string Url
        {
            get
            {
                return this.url;
            }
            set
            {
                if (value == null)
                {
                    this.url = string.Empty;
                }
                else
                {
                    this.url = value;
                }
            }
        }


        /// <summary>
        ///     The type of web site (e.g. home page, work, etc).
        /// </summary>
        public vCardWebSiteType WebSiteType
        {
            get
            {
                return this.webSiteType;
            }
            set
            {
                this.webSiteType = value;
            }
        }


        /// <summary>
        ///     Returns the string representation (URL) of the web site.
        /// </summary>
        /// <returns>
        ///     The URL of the web site.
        /// </returns>
        public override string ToString()
        {
            return this.url;
        }
    }
}
