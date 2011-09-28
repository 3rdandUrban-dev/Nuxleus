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
    /// Represents an Application Protocol Data Unit (or APDU).
    /// </summary>
    public class ApduCommand {
        /// <summary>
        /// Initializes a new ApduCommand instance.
        /// </summary>
        /// <param name="data">The binary data representing the APDU.</param>
        /// <exception cref="ArgumentNullException"><i>data</i> is a null reference.</exception>
        /// <exception cref="ArgumentException"><i>data</i> represents an invalid APDU.</exception>
        public ApduCommand(byte[] data) {
            if (data == null)
                throw new ArgumentNullException("data", ResourceController.GetString("Error_ParamNull"));
            if (data.Length < 5 || data[4] != data.Length - 5)
                throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"), "data");
            m_Bytes = (byte[])data.Clone();
        }
        /// <summary>
        /// Initializes a new ApduCommand instance.
        /// </summary>
        /// <param name="cls">The class byte of the APDU.</param>
        /// <param name="instr">The instruction byte of the APDU.</param>
        /// <param name="param1">The first parameter byte of the APDU.</param>
        /// <param name="param2">The second parameter byte of the APDU.</param>
        /// <param name="contents">The contents of the APDU.</param>
        /// <exception cref="ArgumentNullException"><i>contents</i> is a null reference.</exception>
        /// <exception cref="ArgumentException"><i>contents</i> is too long.</exception>
        public ApduCommand(byte cls, byte instr, byte param1, byte param2, byte[] contents) {
            if (contents == null)
                throw new ArgumentNullException("data", ResourceController.GetString("Error_ParamNull"));
            if (contents.Length > 255)
                throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"), "data");
            Init(cls, instr, param1, param2, (byte)(contents.Length % 256), contents);
        }
        /// <summary>
        /// Initializes a new ApduCommand instance.
        /// </summary>
        /// <param name="cls">The class byte of the APDU.</param>
        /// <param name="instr">The instruction byte of the APDU.</param>
        /// <param name="param1">The first parameter byte of the APDU.</param>
        /// <param name="param2">The second parameter byte of the APDU.</param>
        /// <param name="length">The length byte of the APDU.</param>
        /// <param name="contents">The contents of the APDU.</param>
        /// <exception cref="ArgumentNullException"><i>contents</i> is a null reference.</exception>
        /// <exception cref="ArgumentException"><i>contents</i> is too long.</exception>
        public ApduCommand(byte cls, byte instr, byte param1, byte param2, byte length, byte[] contents) {
            if (contents == null)
                throw new ArgumentNullException("contents", ResourceController.GetString("Error_ParamNull"));
            if (contents.Length > 255)
                throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"), "contents");
            Init(cls, instr, param1, param2, length, contents);
        }

        private void Init(byte cls, byte instr, byte param1, byte param2, byte length, byte[] contents) {
            m_Bytes = new byte[contents.Length + 5];
            m_Bytes[0] = cls;
            m_Bytes[1] = instr;
            m_Bytes[2] = param1;
            m_Bytes[3] = param2;
            m_Bytes[4] = length;
            Buffer.BlockCopy(contents, 0, m_Bytes, 5, contents.Length);
        }

        /// <summary>
        /// The class byte of the APDU.
        /// </summary>
        /// <value>A byte that holds the class of the apdu.</value>
        public byte Class {
            get { 
                return m_Bytes[0];
            }
            set {
                m_Bytes[0] = value;
            }
        }
        /// <summary>
        /// The instruction byte of the APDU.
        /// </summary>
        /// <value>A byte that holds the instruction of the apdu.</value>
        public byte Instruction {
            get {
                return m_Bytes[1];
            }
            set {
                m_Bytes[1] = value;
            }
        }
        /// <summary>
        /// The first parameter byte of the APDU.
        /// </summary>
        /// <value>A byte that holds the first parameter of the apdu.</value>
        public byte Parameter1 {
            get {
                return m_Bytes[2];
            }
            set {
                m_Bytes[2] = value;
            }
        }
        /// <summary>
        /// The second parameter byte of the APDU.
        /// </summary>
        /// <value>A byte that holds the second parameter of the apdu.</value>
        public byte Parameter2 {
            get {
                return m_Bytes[3];
            }
            set {
                m_Bytes[3] = value;
            }
        }
        /// <summary>
        /// The length byte of the APDU.
        /// </summary>
        /// <value>A byte that holds the length of the apdu.</value>
        public byte Length {
            get {
                return m_Bytes[4];
            }
            set {
                m_Bytes[4] = value;
            }
        }
        /// <summary>
        /// The contents of the APDU.
        /// </summary>
        /// <returns>An array of bytes that holds the contents of the APDU.</returns>
        public byte[] GetContents() {
            byte[] buffer = new byte[Length];
            Buffer.BlockCopy(m_Bytes, 5, buffer, 0, buffer.Length);
            return buffer;
        }

        internal byte[] InternalBytes {
            get {
                return m_Bytes;
            }
        }
        internal int InternalLength {
            get {
                return m_Bytes.Length;
            }
        }

        private byte[] m_Bytes;
    }
}