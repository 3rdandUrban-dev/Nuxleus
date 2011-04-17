
/* =======================================================================
 * vCard Library for .NET
 * Copyright (c) 2007 David Pinch; http://www.thoughtproject.com.
 * See LICENSE.TXT for licensing information.
 * ======================================================================= */

using System;
using System.Collections.ObjectModel;

namespace Thought.Net.vCards
{

    /// <summary>
    ///     A collection of <see cref="vCardWebSite"/> objects.
    /// </summary>
    /// <seealso cref="vCardWebSite"/>
    /// <seealso cref="vCardWebSiteType"/>
    public class vCardWebSiteCollection : Collection<vCardWebSite>
    {

        public vCardWebSite GetFirstChoice(vCardWebSiteType siteType)
        {

            vCardWebSite alternate = null;

            foreach (vCardWebSite webSite in this)
            {

                if ((webSite.WebSiteType & siteType) == siteType)
                {
                    return webSite;
                }
                else
                {

                    if (
                        (alternate == null) && 
                        (webSite.WebSiteType == vCardWebSiteType.Default))
                    {
                        alternate = webSite;
                    }

                }
            }

            return alternate;

        }

    }
}
