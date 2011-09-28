
/* =======================================================================
 * vCard Library for .NET
 * Copyright (c) 2007 David Pinch; http://www.thoughtproject.com.
 * See LICENSE.TXT for licensing information.
 * ======================================================================= */

using System;

namespace Thought.Net.vCards
{

    /// <summary>
    ///     Telephone information for a <see cref="vCard"/>.
    /// </summary>
    /// <seealso cref="vCardPhoneCollection"/>
    /// <seealso cref="vCardPhoneType"/>
    [Serializable]
    public class vCardPhone
    {

        private string fullNumber;
        private vCardPhoneType phoneType;


        /// <summary>
        ///     Creates a new <see cref="vCardPhone"/> object.
        /// </summary>
        public vCardPhone()
        {
            this.fullNumber = string.Empty;
        }


        /// <summary>
        ///     Creates a new <see cref="vCardPhone"/> object with the specified number.
        /// </summary>
        /// <param name="fullNumber">
        ///     The phone number.
        /// </param>
        public vCardPhone(string fullNumber)
        {
            this.fullNumber = fullNumber == null ? string.Empty : fullNumber;
        }


        /// <summary>
        ///     Creates a new <see cref="vCardPhone"/> with the specified number and subtype.
        /// </summary>
        /// <param name="fullNumber">The phone number.</param>
        /// <param name="subtype">The phone subtype.</param>
        public vCardPhone(string fullNumber, vCardPhoneType phoneType)
        {
            this.fullNumber = fullNumber == null ? string.Empty : fullNumber;
            this.phoneType = phoneType;
        }


        /// <summary>
        ///     The full telephone number.
        /// </summary>
        public string FullNumber
        {
            get
            {
                return this.fullNumber;
            }
            set
            {
                if (value == null)
                {
                    this.fullNumber = string.Empty;
                }
                else
                {
                    this.fullNumber = value;
                }
            }
        }


        /// <summary>
        ///     Indicates a BBS number.
        /// </summary>
        /// <seealso cref="IsModem"/>
        /// <seealso cref="vCardPhoneType"/>
        public bool IsBBS
        {
            get
            {
                return (this.phoneType & vCardPhoneType.BBS) == vCardPhoneType.BBS;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneType.BBS;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneType.BBS;
                }
            }
        }


        /// <summary>
        ///     Indicates a car number.
        /// </summary>
        /// <seealso cref="vCardPhoneType"/>
        public bool IsCar
        {
            get
            {
                return (this.phoneType & vCardPhoneType.Car) == vCardPhoneType.Car;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneType.Car;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneType.Car;
                }
            }
        }


        /// <summary>
        ///     Indicates a cellular number.
        /// </summary>
        /// <seealso cref="vCardPhoneType"/>
        public bool IsCellular
        {
            get
            {
                return (this.phoneType & vCardPhoneType.Cellular) == vCardPhoneType.Cellular;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneType.Cellular;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneType.Cellular;
                }
            }
        }


        /// <summary>
        ///     Indicates a fax number.
        /// </summary>
        /// <seealso cref="vCardPhoneType"/>
        public bool IsFax
        {
            get
            {
                return (this.phoneType & vCardPhoneType.Fax) == vCardPhoneType.Fax;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneType.Fax;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneType.Fax;
                }
            }
        }


        /// <summary>
        ///     Indicates a home number.
        /// </summary>
        /// <seealso cref="IsWork"/>
        /// <seealso cref="vCardPhoneType"/>
        public bool IsHome
        {
            get
            {
                return (this.phoneType & vCardPhoneType.Home) == vCardPhoneType.Home;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneType.Home;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneType.Home;
                }
            }
        }


        /// <summary>
        ///     Indicates an ISDN number.
        /// </summary>
        /// <seealso cref="vCardPhoneType"/>
        public bool IsISDN
        {
            get
            {
                return (this.phoneType & vCardPhoneType.ISDN) == vCardPhoneType.ISDN;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneType.ISDN;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneType.ISDN;
                }
            }
        }


        /// <summary>
        ///     Indicates a messaging service number.
        /// </summary>
        /// <seealso cref="vCardPhoneType"/>
        public bool IsMessagingService
        {
            get
            {
                return (this.phoneType & vCardPhoneType.MessagingService) ==
                    vCardPhoneType.MessagingService;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneType.MessagingService;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneType.MessagingService;
                }
            }
        }


        /// <summary>
        ///     Indicates a modem number.
        /// </summary>
        /// <seealso cref="IsBBS"/>
        /// <seealso cref="vCardPhoneType"/>
        public bool IsModem
        {
            get
            {
                return (this.phoneType & vCardPhoneType.Modem) == vCardPhoneType.Modem;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneType.Modem;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneType.Modem;
                }
            }
        }


        /// <summary>
        ///     Indicates a pager number.
        /// </summary>
        /// <seealso cref="vCardPhoneType"/>
        public bool IsPager
        {
            get
            {
                return (this.phoneType & vCardPhoneType.Pager) == vCardPhoneType.Pager;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneType.Pager;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneType.Pager;
                }
            }
        }


        /// <summary>
        ///     Indicates a preferred number.
        /// </summary>
        /// <seealso cref="vCardPhoneType"/>
        public bool IsPreferred
        {
            get
            {
                return (this.phoneType & vCardPhoneType.Preferred) == vCardPhoneType.Preferred;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneType.Preferred;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneType.Preferred;
                }
            }
        }


        /// <summary>
        ///     Indicates a video number.
        /// </summary>
        /// <seealso cref="vCardPhoneType"/>
        public bool IsVideo
        {
            get
            {
                return (this.phoneType & vCardPhoneType.Video) == vCardPhoneType.Video;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneType.Video;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneType.Video;
                }
            }
        }


        /// <summary>
        ///     Indicates a voice number.
        /// </summary>
        /// <seealso cref="vCardPhoneType"/>
        public bool IsVoice
        {
            get
            {
                return (this.phoneType & vCardPhoneType.Voice) == vCardPhoneType.Voice;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneType.Voice;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneType.Voice;
                }
            }
        }


        /// <summary>
        ///     Indicates a work number.
        /// </summary>
        /// <seealso cref="IsHome"/>
        /// <seealso cref="vCardPhoneType"/>
        public bool IsWork
        {
            get
            {
                return (this.phoneType & vCardPhoneType.Work) == vCardPhoneType.Work;
            }
            set
            {
                if (value)
                {
                    this.phoneType = this.phoneType | vCardPhoneType.Work;
                }
                else
                {
                    this.phoneType = this.phoneType & ~vCardPhoneType.Work;
                }
            }
        }


        /// <summary>
        ///     The phone subtype.
        /// </summary>
        /// <seealso cref="IsVideo"/>
        /// <seealso cref="IsVoice"/>
        /// <seealso cref="IsWork"/>
        public vCardPhoneType PhoneType
        {
            get
            {
                return this.phoneType;
            }
            set
            {
                this.phoneType = value;
            }
        }

    }
}
