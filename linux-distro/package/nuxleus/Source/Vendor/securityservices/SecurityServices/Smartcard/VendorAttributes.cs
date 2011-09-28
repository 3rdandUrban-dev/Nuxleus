/*
 *   Mentalis.org Security Services for .NET 2.0
 * 
 *     Copyright © 2006, The Mentalis.org Team
 *     All rights reserved.
 *     http://www.mentalis.org/
 *
 *
 *   Redistribution and use in source and binary forms, with or without
 *   modification, are permitted provided that the following conditions
 *   are met:
 *
 *     - Redistributions of source code must retain the above copyright
 *        notice, this list of conditions and the following disclaimer. 
 *
 *     - Neither the name of the Mentalis.org Team, nor the names of its contributors
 *        may be used to endorse or promote products derived from this
 *        software without specific prior written permission. 
 *
 *   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 *   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 *   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 *   FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
 *   THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 *   INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 *   (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 *   SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 *   HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 *   STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 *   ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 *   OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using Org.Mentalis.SecurityServices.Resources;
using Org.Mentalis.SecurityServices.Win32;

namespace Org.Mentalis.SecurityServices.Smartcard {
    /// <summary>
    /// Contains a number of attributes of the vendor of the smartcard reader.
    /// </summary>
    public class VendorAttributes {
        internal VendorAttributes(SmartcardReader sr) {
            int ret, length = NativeMethods.SCARD_AUTOALLOCATE;
            IntPtr result;
            uint resultDWord;
            // get the serial number
            ret = NativeMethods.SCardGetAttrib(sr.Card, NativeMethods.SCARD_ATTR_VENDOR_IFD_SERIAL_NO, out result, ref length);
            if (ret == NativeMethods.SCARD_S_SUCCESS) {
                m_SerialNumber = Marshal.PtrToStringAnsi(result, length);
                NativeMethods.SCardFreeMemory(sr.Context, result);
            }
            // get the device type
            length = NativeMethods.SCARD_AUTOALLOCATE;
            ret = NativeMethods.SCardGetAttrib(sr.Card, NativeMethods.SCARD_ATTR_VENDOR_IFD_TYPE, out result, ref length);
            if (ret == NativeMethods.SCARD_S_SUCCESS) {
                m_DeviceType = Marshal.PtrToStringAnsi(result, length);
                NativeMethods.SCardFreeMemory(sr.Context, result);
            }
            // get the vendor name
            length = NativeMethods.SCARD_AUTOALLOCATE;
            ret = NativeMethods.SCardGetAttrib(sr.Card, NativeMethods.SCARD_ATTR_VENDOR_NAME, out result, ref length);
            if (ret == NativeMethods.SCARD_S_SUCCESS) {
                m_VendorName = Marshal.PtrToStringAnsi(result, length);
                NativeMethods.SCardFreeMemory(sr.Context, result);
            }
            // get the device version
            length = 4;
            ret = NativeMethods.SCardGetAttrib(sr.Card, NativeMethods.SCARD_ATTR_VENDOR_IFD_VERSION, out resultDWord, ref length);
            if (ret == NativeMethods.SCARD_S_SUCCESS) {
                m_DeviceVersion = new Version((int)(resultDWord >> 24), (int)((resultDWord >> 16) & 0xFF), (int)(resultDWord & 0xFFFF));
            }
        }

        /// <summary>
        /// The serial number of the card reader.
        /// </summary>
        /// <value>A string that contains the serial number of the card reader.</value>
        public string SerialNumber {
            get {
                return m_SerialNumber;
            }
        }
        /// <summary>
        /// The type of the card reader.
        /// </summary>
        /// <value>A string containing the type of the card reader.</value>
        public string DeviceType {
            get {
                return m_DeviceType;
            }
        }
        /// <summary>
        /// The name of the vendor of the card reader.
        /// </summary>
        /// <value>A string containing the name of the vendor of the card reader.</value>
        public string VendorName {
            get {
                return m_VendorName;
            }
        }
        /// <summary>
        /// The version of the card reader.
        /// </summary>
        /// <value>A Version instance that contains the version of the card reader.</value>
        public Version DeviceVersion {
            get {
                return m_DeviceVersion;
            }
        }
        /// <summary>
        /// Returns a string representation of the card reader properties.
        /// </summary>
        /// <returns>A string that represents the properties of the card reader.</returns>
        public override string ToString() {
            string ret = "";
            if (m_VendorName != null)
                ret += string.Format(CultureInfo.InvariantCulture, ResourceController.GetString("Info_DeviceVendor"), m_VendorName) + Environment.NewLine;
            if (m_DeviceType != null)
                ret += string.Format(CultureInfo.InvariantCulture, ResourceController.GetString("Info_DeviceType"), m_DeviceType) + Environment.NewLine;
            if (m_DeviceVersion != null)
                ret += string.Format(CultureInfo.InvariantCulture, ResourceController.GetString("Info_DeviceVersion"), m_DeviceVersion.ToString()) + Environment.NewLine;
            if (m_SerialNumber != null)
                ret += string.Format(CultureInfo.InvariantCulture, ResourceController.GetString("Info_DeviceSerial"), m_SerialNumber) + Environment.NewLine;
            return ret;
        }

	    private string m_DeviceType;
        private Version m_DeviceVersion;
        private string m_VendorName;
        private string m_SerialNumber;
	}
}