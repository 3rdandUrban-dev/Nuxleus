
using System;

namespace Thought.Net.vCards
{

    /// <summary>
    ///     A formatted delivery label.
    /// </summary>
    /// <seealso cref="vCardDeliveryAddress"/>
    /// <seealso cref="vCardDeliveryLabelCollection"/>
    public class vCardDeliveryLabel
    {

        private vCardDeliveryAddressType addressType;
        private string text;


        /// <summary>
        ///     Initializes a new <see cref="vCardDeliveryLabel"/>.
        /// </summary>
        public vCardDeliveryLabel()
        {
            this.text = string.Empty;
        }


        /// <summary>
        ///     Initializes a new <see cref="vCardDeliveryLabel"/> to
        ///     the specified text.
        /// </summary>
        /// <param name="text">
        ///     The formatted text of a delivery label.  The label 
        ///     may contain carriage returns, line feeds, and other
        ///     control characters.
        /// </param>
        public vCardDeliveryLabel(string text)
        {
            this.text = text == null ? string.Empty : text;
        }


        /// <summary>
        ///     The type of delivery address for the label.
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
        ///     The formatted delivery text.
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                if (value == null)
                {
                    this.text = string.Empty;
                }
                else
                {
                    this.text = value;
                }
            }
        }

    }
}
