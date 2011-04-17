
/* =======================================================================
 * vCard Library for .NET
 * Copyright (c) 2007 David Pinch; http://www.thoughtproject.com.
 * See LICENSE.TXT for licensing information.
 * ======================================================================= */

using System;

namespace Thought.Net.vCards
{

    /// <summary>
    ///     An email address in a <see cref="vCard"/>.
    /// </summary>
    /// <remarks>
    ///     Most vCard email addresses are Internet email addresses.  However,
    ///     the vCard specification allows other email address formats,
    ///     such as CompuServe and X400.  Unless otherwise specified, an
    ///     address is assumed to be an Internet address.
    /// </remarks>
    /// <seealso cref="vCardEmailAddressCollection"/>
    /// <seealso cref="vCardEmailType"/>
    public class vCardEmailAddress
    {

        private string address;
        private vCardEmailAddressType emailType;
        private bool isPreferred;


        /// <summary>
        ///     Creates a new <see cref="vCardEmailAddress"/>.
        /// </summary>
        public vCardEmailAddress()
        {
            this.address = string.Empty;
            this.emailType = vCardEmailAddressType.Internet;
        }


        /// <summary>
        ///     Creates a new Internet <see cref="vCardEmailAddress"/>.
        /// </summary>
        /// <param name="address">
        ///     The Internet email address.
        /// </param>
        public vCardEmailAddress(string address)
        {
            this.address = address == null ? string.Empty : address;
            this.emailType = vCardEmailAddressType.Internet;
        }


        /// <summary>
        ///     Creates a new <see cref="vCardEmailAddress"/> of the specified type.
        /// </summary>
        /// <param name="address">
        ///     The email address.
        /// </param>
        /// <param name="emailType">
        ///     The type of email address.
        /// </param>
        public vCardEmailAddress(string address, vCardEmailAddressType emailType)
        {
            this.address = address;
            this.emailType = emailType;
        }


        /// <summary>
        ///     The email address.
        /// </summary>
        /// <remarks>
        ///     The format of the email address is not validated by the class.
        /// </remarks>
        public string Address
        {
            get
            {
                return this.address;
            }
            set
            {
                if (value == null)
                {
                    this.address = string.Empty;
                }
                else
                {
                    this.address = value;
                }
            }
        }


        /// <summary>
        ///     The email address type.
        /// </summary>
        public vCardEmailAddressType EmailType
        {
            get
            {
                return this.emailType;
            }
            set
            {
                this.emailType = value;
            }
        }


        /// <summary>
        ///     Indicates a preferred (top priority) email address.
        /// </summary>
        public bool IsPreferred
        {
            get
            {
                return this.isPreferred;
            }
            set
            {
                this.isPreferred = value;
            }
        }


        /// <summary>
        ///     Builds a string that represents the email address.
        /// </summary>
        public override string ToString()
        {
            return this.address;
        }
    }
}
