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
using System.Text;
using Org.Mentalis.SecurityServices.Resources;
using Org.Mentalis.SecurityServices.Win32;

namespace Org.Mentalis.SecurityServices.Smartcard {
    /// <summary>
    /// Represents the reply of a smartcard to an APDU.
    /// </summary>
    public class ApduReply {
        internal ApduReply(byte[] status, byte[] result) {
            m_Status = status;
            m_Result = result;
        }
        /// <summary>
        /// The first byte of the status word.
        /// </summary>
        /// <value>A byte representing the first part of the status word.</value>
        public byte StatusWord1 {
            get { 
                return m_Status[0];
            }
        }
        /// <summary>
        /// The second byte of the status word.
        /// </summary>
        /// <value>A byte representing the second part of the status word.</value>
        public byte StatusWord2 {
            get {
                return m_Status[1];
            }
        }
        /// <summary>
        /// The contents of the reply.
        /// </summary>
        /// <returns>An array of bytes that holds the contents of the reply.</returns>
        public byte[] GetContents() {
            return (byte[])m_Result.Clone();
        }
        /// <summary>
        /// Gets the length of the contents of the APDU reply.
        /// </summary>
        /// <value>An integer that contains the length of the contents of the APDU reply.</value>
        public int Length {
            get {
                return m_Result.Length;
            }
        }
        /// <summary>
        /// Indicates whether the APDU command was successfully executed.
        /// </summary>
        /// <value><b>true</b> if the command was successfully executed, <b>false</b> otherwise.</value>
        public bool Success {
            get {
                return (StatusWord1 == 0x90 && StatusWord2 == 0x00) || StatusWord1 == 0x61;
            }
        }

        private byte[] m_Status;
        private byte[] m_Result;
    }
}