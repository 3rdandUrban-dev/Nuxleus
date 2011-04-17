
/* =======================================================================
 * vCard Library for .NET
 * Copyright (c) 2007 David Pinch; http://www.thoughtproject.com.
 * See LICENSE.TXT for licensing information.
 * ======================================================================= */

using System;

namespace Thought.Net.vCards
{

    /// <summary>
    ///     The encoding used to store a vCard property value in text format.
    /// </summary>
    public enum vCardEncoding
    {

        /// <summary>
        ///     Unknown or no encoding.
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     Standard escaped text.
        /// </summary>
        Escaped,

        /// <summary>
        ///   Binary or BASE64 encoding.
        /// </summary>
        Base64,

        /// <summary>
        ///     Quoted-Printable encoding.
        /// </summary>
        QuotedPrintable

    }
}
