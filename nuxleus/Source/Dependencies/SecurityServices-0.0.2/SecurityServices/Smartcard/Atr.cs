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
using Org.Mentalis.SecurityServices.Resources;

namespace Org.Mentalis.SecurityServices.Smartcard {
    /// <summary>
    /// Represents an Answer To Reset (or ATR). An ATR could be thought of as an identification string
    /// that a smartcard sends to the reader upon insertion.
    /// </summary>
    public class Atr : ICloneable {
        /// <summary>
        /// Initializes a new Atr instance.
        /// </summary>
        /// <param name="atr">The value of the ATR.</param>
        /// <exception cref="ArgumentNullException"><i>atr</i> is a null reference.</exception>
        /// <exception cref="ArgumentException"><i>atr</i> has an invalid length.</exception>
        public Atr(byte[] atr) {
            if (atr == null)
                throw new ArgumentNullException("atr", ResourceController.GetString("Error_ParamNull"));
            if (atr.Length == 0)
                throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"), "atr");
            Init((byte[])atr.Clone(), null);
        }
        /// <summary>
        /// Initializes a new Atr instance.
        /// </summary>
        /// <param name="atr">The value of the ATR.</param>
        /// <param name="mask">The mask that corresponds with the ATR.</param>
        /// <exception cref="ArgumentNullException"><i>atr</i> or <i>mask</i> are a null reference.</exception>
        /// <exception cref="ArgumentException"><i>atr</i> or <i>mask</i> have an invalid length.</exception>
        public Atr(byte[] atr, byte[] mask) {
            if (atr == null)
                throw new ArgumentNullException("atr", ResourceController.GetString("Error_ParamNull"));
            if (atr.Length == 0)
                throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"), "atr");
            if (mask == null)
                throw new ArgumentNullException("mask", ResourceController.GetString("Error_ParamNull"));
            if (atr.Length != mask.Length)
                throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"), "mask");
            Init((byte[])atr.Clone(), (byte[])mask.Clone());
        }

        internal Atr(byte[] atr, int count) {
            byte[] atrBuffer = new byte[count];
            Buffer.BlockCopy(atr, 0, atrBuffer, 0, count);
            Init(atrBuffer, null);
        }
        private void Init(byte[] atr, byte[] mask) {
            m_Atr = atr;
            if (mask == null) {
                byte[] m_Mask = new byte[atr.Length];
                for (int i = 0; i < atr.Length; i++) {
                    m_Mask[i] = 0xFF;
                }
            } else {
                m_Mask = mask;
            }
        }

        /// <summary>
        /// The binary value of the ATR.
        /// </summary>
        /// <returns>An array of bytes that contains the value of the ATR.</returns>
        public byte[] GetValue() {
            return (byte[])m_Atr.Clone();
        }
        /// <summary>
        /// Gets the length of the ATR.
        /// </summary>
        /// <value>An integer that holds the length of the ATR.</value>
        public int Length {
            get {
                return m_Atr.Length;
            }
        }
        /// <summary>
        /// The binary value of the mask that corresponds with the ATR.
        /// </summary>
        /// <returns>An array of bytes that contains the mask of the ATR.</returns>
        public byte[] GetMask() {
            return (byte[])m_Mask.Clone();
        }
        /// <summary>
        /// Checks whether a given array of bytes matches with this ATR.
        /// </summary>
        /// <param name="atr">The array of bytes to check.</param>
        /// <returns><b>true</b> if the array of bytes matches with this Atr instance, <b>false</b> otherwise.</returns>
        /// <exception cref="ArgumentNullException"><i>atr</i> is a null reference.</exception>
        public bool IsMatch(byte[] atr) {
            if (atr == null)
                throw new ArgumentNullException("atr", ResourceController.GetString("Error_ParamNull"));
            return InternalMatch(atr);
        }
        /// <summary>
        /// Checks whether a given ATR matches with this ATR.
        /// </summary>
        /// <param name="atr">The ATR to check.</param>
        /// <returns><b>true</b> if the given ATR matches with this Atr instance, <b>false</b> otherwise.</returns>
        /// <exception cref="ArgumentNullException"><i>atr</i> is a null reference.</exception>
        public bool Match(Atr atr) {
            if (atr == null)
                throw new ArgumentNullException("atr", ResourceController.GetString("Error_ParamNull"));
            return InternalMatch(atr.m_Atr);
        }

        private bool InternalMatch(byte[] atr) {
            if (atr.Length != m_Atr.Length)
                return false;
            for (int i = 0; i < atr.Length; i++) {
                if ((m_Atr[i] & m_Mask[i]) != (atr[i] & m_Mask[i])) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Creates a copy of the ATR.
        /// </summary>
        /// <returns>An identical copy of the ATR.</returns>
        public Object Clone() {
            return new Atr(m_Atr, m_Mask);
        }

        /// <summary>
        /// Determines whether two ATR instances are equal.
        /// </summary>
        /// <param name="obj">The Object to compare with the current ATR.</param>
        /// <returns><b>true</b> if the specified Object is equal to the current ATR; otherwise, <b>false</b>. </returns>
        public override bool Equals(object obj) {
            Atr a = obj as Atr;
            if (a == null)
                return false;
            if (a.m_Atr.Length != this.m_Atr.Length)
                return false;
            // first ensure that the masks are equal
            for (int i = 0; i < this.m_Mask.Length; i++) {
                if (a.m_Mask[i] != this.m_Mask[i])
                    return false;
            }
            // then make sure that the ATR, with the masks applied, are equal
            for (int i = 0; i < this.m_Atr.Length; i++) {
                if ((a.m_Atr[i] & a.m_Mask[i]) != (this.m_Atr[i] & this.m_Mask[i]))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Serves as a hash function for a particular type. GetHashCode is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>A hash code for the current ATR.</returns>
        public override int GetHashCode() {
            return m_Atr.GetHashCode() ^ m_Mask.GetHashCode();
        }
        internal bool IsValid() {
            return m_Atr != null && m_Mask != null && m_Atr.Length == m_Mask.Length;
        }

        private byte[] m_Atr;
        private byte[] m_Mask;
    }
}
