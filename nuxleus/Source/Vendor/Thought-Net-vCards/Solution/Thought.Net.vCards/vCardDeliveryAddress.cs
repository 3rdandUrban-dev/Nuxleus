
/* =======================================================================
 * vCard Library for .NET
 * Copyright (c) 2007 David Pinch; http://www.thoughtproject.com.
 * See LICENSE.TXT for licensing information.
 * ======================================================================= */

using System;

namespace Thought.Net.vCards
{
    /// <summary>
    ///     A postal address.
    /// </summary>
    /// <seealso cref="vCardDeliveryAddressCollection"/>
    [Serializable]
    public class vCardDeliveryAddress
    {

        private vCardDeliveryAddressType addressType;
        private string city;
        private string country;
        private string postalCode;
        private string region;
        private string street;


        /// <summary>
        ///     Creates a new delivery address object.
        /// </summary>
        public vCardDeliveryAddress()
        {
            this.city = string.Empty;
            this.country = string.Empty;
            this.postalCode = string.Empty;
            this.region = string.Empty;
            this.street = string.Empty;
        }


        /// <summary>
        ///     The type of postal address.
        /// </summary>
        public vCardDeliveryAddressType AddressType
        {
            get
            {
                return this.addressType;
            }
            set
            {
                this.addressType = value;
            }
        }


        /// <summary>
        ///     The city or locality of the address.
        /// </summary>
        public string City
        {
            get
            {
                return this.city;
            }
            set
            {

                if (value == null)
                {
                    this.city = string.Empty;
                }
                else
                {
                    this.city = value;
                }
            }
        }


        /// <summary>
        ///     The country name of the address.
        /// </summary>
        public string Country
        {
            get
            {
                return this.country;
            }
            set
            {
                if (value == null)
                {
                    this.country = string.Empty;
                }
                else
                {
                    this.country = value;
                }
            }
        }


        /// <summary>
        ///     Indicates a domestic delivery address.
        /// </summary>
        public bool IsDomestic
        {
            get
            {
                return (this.addressType & vCardDeliveryAddressType.Domestic) ==
                    vCardDeliveryAddressType.Domestic;
            }
            set
            {

                if (value)
                {
                    this.addressType |= vCardDeliveryAddressType.Domestic;
                }
                else
                {
                    this.addressType &= ~vCardDeliveryAddressType.Domestic;
                }

            }
        }


        /// <summary>
        ///     Indicates a home address.
        /// </summary>
        public bool IsHome
        {
            get
            {
                return (this.addressType & vCardDeliveryAddressType.Home) ==
                    vCardDeliveryAddressType.Home;
            }
            set
            {
                if (value)
                {
                    this.addressType |= vCardDeliveryAddressType.Home;
                }
                else
                {
                    this.addressType &= ~vCardDeliveryAddressType.Home;
                }

            }
        }


        /// <summary>
        ///     Indicates an international address.
        /// </summary>
        public bool IsInternational
        {
            get
            {
                return (this.addressType & vCardDeliveryAddressType.International) ==
                    vCardDeliveryAddressType.International;
            }
            set
            {
                if (value)
                {
                    this.addressType |= vCardDeliveryAddressType.International;
                }
                else
                {
                    this.addressType &= ~vCardDeliveryAddressType.International;
                }
            }
        }


        /// <summary>
        ///     Indicates a parcel delivery address.
        /// </summary>
        public bool IsParcel
        {
            get
            {
                return (this.addressType & vCardDeliveryAddressType.Parcel) ==
                    vCardDeliveryAddressType.Parcel;
            }
            set
            {
                if (value)
                {
                    this.addressType |= vCardDeliveryAddressType.Parcel;
                }
                else
                {
                    this.addressType &= ~vCardDeliveryAddressType.Parcel;
                }
            }
        }


        /// <summary>
        ///     Indicates a postal address.
        /// </summary>
        public bool IsPostal
        {
            get
            {
                return (this.addressType & vCardDeliveryAddressType.Postal) ==
                    vCardDeliveryAddressType.Postal;
            }
            set
            {
                if (value)
                {
                    this.addressType |= vCardDeliveryAddressType.Postal;
                }
                else
                {
                    this.addressType &= ~vCardDeliveryAddressType.Postal;
                }
            }
        }


        /// <summary>
        ///     Indicates a work address.
        /// </summary>
        public bool IsWork
        {
            get
            {
                return (this.addressType & vCardDeliveryAddressType.Work) ==
                    vCardDeliveryAddressType.Work;
            }
            set
            {
                if (value)
                {
                    this.addressType |= vCardDeliveryAddressType.Work;
                }
                else
                {
                    this.addressType &= ~vCardDeliveryAddressType.Work;
                }
            }
        }


        /// <summary>
        ///     The postal code (e.g. ZIP code) of the address.
        /// </summary>
        public string PostalCode
        {
            get
            {
                return this.postalCode;
            }
            set
            {
                if (value == null)
                {
                    this.postalCode = string.Empty;
                }
                else
                {
                    this.postalCode = value;
                }
            }
        }


        /// <summary>
        ///     The region (state or province) of the address.
        /// </summary>
        public string Region
        {
            get
            {
                return this.region;
            }
            set
            {
                if (value == null)
                {
                    this.region = string.Empty;
                }
                else
                {
                    this.region = value;
                }
            }
        }


        /// <summary>
        ///     The street of the delivery address.
        /// </summary>
        public string Street
        {
            get
            {
                return this.street;
            }
            set
            {
                if (value == null)
                {
                    this.street = string.Empty;
                }
                else
                {
                    this.street = value;
                }
            }
        }

    }
}
