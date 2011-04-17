
/* =======================================================================
 * vCard Library for .NET
 * Copyright (c) 2007 David Pinch; http://www.thoughtproject.com.
 * See LICENSE.TXT for licensing information.
 * ======================================================================= */

using System;

namespace Thought.Net.vCards
{

    /// <summary>
    ///     The type of a delivery address.
    /// </summary>
    [Flags]
    public enum vCardDeliveryAddressType
    {

        /// <summary>
        ///     Default address settings.
        /// </summary>
        Default = 0,

        /// <summary>
        ///     A domestic delivery address.
        /// </summary>
        Domestic,

        /// <summary>
        ///     An international delivery address.
        /// </summary>
        International,

        /// <summary>
        ///     A postal delivery address.
        /// </summary>
        Postal,

        /// <summary>
        ///     A parcel delivery address.
        /// </summary>
        Parcel,

        /// <summary>
        ///     A home delivery address.
        /// </summary>
        Home,

        /// <summary>
        ///     A work delivery address.
        /// </summary>
        Work
    }
}
