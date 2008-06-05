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
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Org.Mentalis.SecurityServices.Resources;
using Org.Mentalis.SecurityServices.Win32;
using Org.Mentalis.SecurityServices.Permissions;

namespace Org.Mentalis.SecurityServices.Smartcard {
    /// <summary>
    /// Represents a smartcard reader.
    /// </summary>
    public sealed class SmartcardReader : IDisposable {
        /// <summary>
        /// Initializes a new SmartcardReader instance.
        /// </summary>
        /// <param name="readerName">The name of the smartcard reader.</param>
        /// <exception cref="ArgumentNullException"><i>readerName</i> is a null reference.</exception>
        /// <exception cref="SmartcardException">An error occurred while communication with the smartcard reader.</exception>
        public SmartcardReader(string readerName)
            : this(readerName, DatabaseScope.User) {
        }
        /// <summary>
        /// Initializes a new SmartcardReader instance.
        /// </summary>
        /// <param name="readerName">The name of the smartcard reader.</param>
        /// <param name="scope">The scope of the resource manager context.</param>
        /// <exception cref="ArgumentNullException"><i>readerName</i> is a null reference.</exception>
        /// <exception cref="SmartcardException">An error occurred while communication with the smartcard reader.</exception>
        [SmartcardPermission(SecurityAction.Demand, SmartcardConnectOption.AllowedAtrs)]
        public SmartcardReader(string readerName, DatabaseScope scope) {
            if (readerName == null)
                throw new ArgumentNullException("readerName", ResourceController.GetString("Error_ParamNull"));
            int ret = NativeMethods.SCardEstablishContext((int)scope, IntPtr.Zero, IntPtr.Zero, out m_Context);
            if (ret != NativeMethods.SCARD_S_SUCCESS)
                throw new SmartcardException(ResourceController.GetString("Error_SmartcardEstablishContext"), ret);
            m_ReaderName = readerName;
        }
        /// <summary>
        /// Connects to the card in the smartcard reader.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The SmartcardReader has been closed.</exception>
        /// <exception cref="SmartcardException">An error occurred while communication with the smartcard reader.</exception>
        public void Connect() {
            Connect(SmartcardShare.Shared, SmartcardProtocols.T0);
        }
        /// <summary>
        /// Connects to the card in the smartcard reader.
        /// </summary>
        /// <param name="share">A flag that indicates whether other applications may form connections to the card.</param>
        /// <param name="protocol">A bit mask of acceptable protocols for the connection. Possible values may be combined with the <b>OR</b> operation.</param>
        /// <exception cref="ObjectDisposedException">The SmartcardReader has been closed.</exception>
        /// <exception cref="SmartcardException">An error occurred while communication with the smartcard reader.</exception>
        public void Connect(SmartcardShare share, SmartcardProtocols protocol) {
            if (m_Disposed)
                throw new ObjectDisposedException(this.GetType().FullName);
            if (m_Card != IntPtr.Zero)
                throw new SmartcardException(ResourceController.GetString("Error_SmartcardAlreadyConnected"));
            int ret = NativeMethods.SCardConnect(m_Context, m_ReaderName, (int)share, (int)protocol, out m_Card, out m_ActiveProtocol);
            if (ret != NativeMethods.SCARD_S_SUCCESS)
                throw new SmartcardException(ResourceController.GetString("Error_SmartcardConnect"), ret);
            // make sure the user has at least the 'AllowedAtrs' option of the SmartcardPermission
            // for the current ATR
            try {
                SmartcardPermission permission = new SmartcardPermission(new Atr[] { Atr });
                permission.Demand();
            } catch {
                Dispose();
                throw;
            }
        }
        /// <summary>
        /// Reconnects to the card in the smartcard reader.
        /// </summary>
        /// <param name="share">A flag that indicates whether other applications may form connections to the card.</param>
        /// <param name="protocol">A bit mask of acceptable protocols for the connection. Possible values may be combined with the <b>OR</b> operation.</param>
        /// <param name="disposition">Type of initialization that should be performed on the card.</param>
        /// <exception cref="ObjectDisposedException">The SmartcardReader has been closed.</exception>
        /// <exception cref="SmartcardException">An error occurred while communication with the smartcard reader.</exception>
        public void Reconnect(SmartcardShare share, SmartcardProtocols protocol, SmartcardDisposition disposition) {
            if (m_Disposed)
                throw new ObjectDisposedException(this.GetType().FullName);
            if (m_Card == IntPtr.Zero)
                throw new SmartcardException(ResourceController.GetString("Error_SmartcardNotConnected"));
            int ret = NativeMethods.SCardReconnect(m_Card, (int)share, (int)protocol, (int)disposition, out m_ActiveProtocol);
            if (ret != NativeMethods.SCARD_S_SUCCESS)
                throw new SmartcardException(ResourceController.GetString("Error_SmartcardReconnect"), ret);
        }
        /// <summary>
        /// Starts a transaction, waiting for the completion of all other transactions before it begins.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The SmartcardReader has been closed.</exception>
        /// <exception cref="SmartcardException">An error occurred while communication with the smartcard reader.</exception>
        public void Lock() {
            if (m_Disposed)
                throw new ObjectDisposedException(this.GetType().FullName);
            if (m_Card == IntPtr.Zero)
                throw new SmartcardException(ResourceController.GetString("Error_SmartcardNotConnected"));
            int ret = NativeMethods.SCardBeginTransaction(m_Card);
            if (ret != NativeMethods.SCARD_S_SUCCESS)
                throw new SmartcardException(ResourceController.GetString("Error_SmartcardLock"), ret);
        }
        /// <summary>
        /// Sends an APDU to the smartcard.
        /// </summary>
        /// <param name="apdu">The apdu to send to the smartcard.</param>
        /// <returns>An ApduReply instance, representing the reply from the smartcard.</returns>
        /// <exception cref="ObjectDisposedException">The SmartcardReader has been closed.</exception>
        /// <exception cref="ArgumentNullException"><i>apdu</i> is a null reference.</exception>
        /// <exception cref="SmartcardException">An error occurred while communication with the smartcard reader.</exception>
        public ApduReply SendCommand(ApduCommand apdu) {
            if (m_Disposed)
                throw new ObjectDisposedException(this.GetType().FullName);
            if (apdu == null)
                throw new ArgumentNullException("apdu", ResourceController.GetString("Error_ParamNull"));
            if (m_Card == IntPtr.Zero)
                throw new SmartcardException(ResourceController.GetString("Error_SmartcardNotConnected"));
            SCARD_IO_REQUEST sendPci = new SCARD_IO_REQUEST(m_ActiveProtocol, 8);
            byte[] buffer = new byte[1024];
            int recvSize = buffer.Length;
            // send the message to the smart card
            int ret = NativeMethods.SCardTransmit(m_Card, ref sendPci, apdu.InternalBytes, apdu.InternalLength, IntPtr.Zero, buffer, ref recvSize);
            if (ret != NativeMethods.SCARD_S_SUCCESS || recvSize < 2)
                throw new SmartcardException(ResourceController.GetString("Error_SmartcardTransmit"), ret);
            byte[] result = new byte[recvSize - 2];
            Array.Copy(buffer, 0, result, 0, result.Length);
            byte[] status = new byte[2];
            Array.Copy(buffer, recvSize - status.Length, status, 0, status.Length);
            return new ApduReply(status, result);
        }
        /// <summary>
        /// Completes a previously declared transaction, allowing other applications to resume interactions with the card.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The SmartcardReader has been closed.</exception>
        /// <exception cref="SmartcardException">An error occurred while communication with the smartcard reader.</exception>
        public void Unlock() {
            Unlock(SmartcardDisposition.Leave);
        }
        /// <summary>
        /// Completes a previously declared transaction, allowing other applications to resume interactions with the card.
        /// </summary>
        /// <param name="disposition">Action to take on the card in the connected reader on close.</param>
        /// <exception cref="ObjectDisposedException">The SmartcardReader has been closed.</exception>
        /// <exception cref="SmartcardException">An error occurred while communication with the smartcard reader.</exception>
        public void Unlock(SmartcardDisposition disposition) {
            if (m_Disposed)
                throw new ObjectDisposedException(this.GetType().FullName);
            if (m_Card == IntPtr.Zero)
                throw new SmartcardException(ResourceController.GetString("Error_SmartcardNotConnected"));
            int ret = NativeMethods.SCardEndTransaction(m_Card, (int)disposition);
            if (ret != NativeMethods.SCARD_S_SUCCESS)
                throw new SmartcardException(ResourceController.GetString("Error_SmartcardUnlock"), ret);
        }
        /// <summary>
        /// Disconnects the connection with the smartcard.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The SmartcardReader has been closed.</exception>
        public void Disconnect() {
            Disconnect(SmartcardDisposition.Leave);
        }
        /// <summary>
        /// Disconnects the connection with the smartcard.
        /// </summary>
        /// <param name="disposition">Action to take on the card in the connected reader on close.</param>
        /// <exception cref="ObjectDisposedException">The SmartcardReader has been closed.</exception>
        public void Disconnect(SmartcardDisposition disposition) {
            if (m_Disposed)
                throw new ObjectDisposedException(this.GetType().FullName);
            if (m_Card != IntPtr.Zero) {
                NativeMethods.SCardDisconnect(m_Card, (int)disposition);
                m_Card = IntPtr.Zero;
            }
        }
        /// <summary>
        /// Releases all resources used by the SmartcardReader.
        /// </summary>
        public void Dispose() {
            Dispose(SmartcardDisposition.Leave);
        }
        /// <summary>
        /// Releases all resources used by the SmartcardReader.
        /// </summary>
        /// <param name="disposition">Action to take on the card in the connected reader on close.</param>
        public void Dispose(SmartcardDisposition disposition) {
            if (!m_Disposed) {
                Disconnect(disposition);
                NativeMethods.SCardReleaseContext(m_Context);
                m_Disposed = true;
                GC.SuppressFinalize(this);
            }
        }
        /// <summary>
        /// Releases all resources used by the SmartcardReader.
        /// </summary>
        ~SmartcardReader() {
            Dispose();
        }
        /// <summary>
        /// Returns the name of the smartcard reader.
        /// </summary>
        /// <value>A string that holds the name of the smartcard reader.</value>
        /// <exception cref="ObjectDisposedException">The SmartcardReader has been closed.</exception>
        public string ReaderName {
            get {
                if (m_Disposed)
                    throw new ObjectDisposedException(this.GetType().FullName);
                return m_ReaderName;
            }
        }
        /// <summary>
        /// Returns the status of the smartcard reader.
        /// </summary>
        /// <value>One of the SmartcardState values.</value>
        /// <exception cref="ObjectDisposedException">The SmartcardReader has been closed.</exception>
        /// <exception cref="SmartcardException">An error occurred while communication with the smartcard reader.</exception>
        public SmartcardState Status {
            get {
                if (m_Disposed)
                    throw new ObjectDisposedException(this.GetType().FullName);
                if (m_Card == IntPtr.Zero)
                    throw new SmartcardException(ResourceController.GetString("Error_SmartcardNotConnected"));
                RefreshStatus();
                return (SmartcardState)m_State;
            }
        }
        /// <summary>
        /// Returns the ATR of the card in the smartcard reader.
        /// </summary>
        /// <value>An Atr instance.</value>
        /// <exception cref="ObjectDisposedException">The SmartcardReader has been closed.</exception>
        /// <exception cref="SmartcardException">An error occurred while communication with the smartcard reader.</exception>
        public Atr Atr {
            get {
                if (m_Disposed)
                    throw new ObjectDisposedException(this.GetType().FullName);
                if (m_Card == IntPtr.Zero)
                    throw new SmartcardException(ResourceController.GetString("Error_SmartcardNotConnected"));
                RefreshStatus();
                return m_Atr;
            }
        }
        /// <summary>
        /// Returns the active protocol.
        /// </summary>
        /// <value>One of the SmartcardProtocol values.</value>
        /// <exception cref="ObjectDisposedException">The SmartcardReader has been closed.</exception>
        /// <exception cref="SmartcardException">An error occurred while communication with the smartcard reader.</exception>
        public SmartcardProtocols ActiveProtocol {
            get {
                if (m_Disposed)
                    throw new ObjectDisposedException(this.GetType().FullName);
                if (m_Card == IntPtr.Zero)
                    throw new SmartcardException(ResourceController.GetString("Error_SmartcardNotConnected"));
                RefreshStatus();
                return (SmartcardProtocols)m_ActiveProtocol;
            }
        }

        private void RefreshStatus() {
            IntPtr readerName;
            int nameLength = NativeMethods.SCARD_AUTOALLOCATE;
            byte[] atrBuffer = new byte[32];
            int atrLength = atrBuffer.Length;
            int ret = NativeMethods.SCardStatus(m_Card, out readerName, ref nameLength ,out m_State , out m_ActiveProtocol , atrBuffer, ref atrLength);
            try {
                if (ret != NativeMethods.SCARD_S_SUCCESS || atrLength < 0) 
                    throw new SmartcardException(ResourceController.GetString("Error_SmartcardGetStatus"), ret);
            } finally {
                NativeMethods.SCardFreeMemory(m_Context, readerName);
            }
            m_Atr = new Atr(atrBuffer, atrLength);
        }

        /// <summary>
        /// Returns the attributes of the smartcard reader.
        /// </summary>
        /// <value>A VendorAttributes instance.</value>
        /// <exception cref="ObjectDisposedException">The SmartcardReader has been closed.</exception>
        /// <exception cref="SmartcardException">An error occurred while communication with the smartcard reader.</exception>
        public VendorAttributes Attributes {
            get {
                if (m_Disposed)
                    throw new ObjectDisposedException(this.GetType().FullName);
                if (m_Card == IntPtr.Zero)
                    throw new SmartcardException(ResourceController.GetString("Error_SmartcardNotConnected"));
                if (m_Attributes == null)
                    m_Attributes = new VendorAttributes(this);
                return m_Attributes;
            }
        }

        internal IntPtr Card {
            get {
                return m_Card;
            }
        }
        internal IntPtr Context {
            get {
                return m_Context;
            }
        }
        
        /// <summary>
        /// Returns a list of readers that are installed on the system.
        /// </summary>
        /// <param name="context">The SCard context to use.</param>
        /// <returns>A list of readers.</returns>
        internal unsafe static string[] InternalGetReaders(IntPtr context) {
            byte* readers;
            int rcount = NativeMethods.SCARD_AUTOALLOCATE;
            if (NativeMethods.SCardListReaders(context, IntPtr.Zero, out readers, ref rcount) != NativeMethods.SCARD_S_SUCCESS)
                return new string[0];
            ArrayList ret = new ArrayList();
            string s = Marshal.PtrToStringAnsi(new IntPtr(readers));
            int offset = 0;
            while (s.Length > 0) {
                ret.Add(s);
                offset += s.Length + 1;
                s = Marshal.PtrToStringAnsi(new IntPtr(readers + offset));
            }
            NativeMethods.SCardFreeMemory(context, readers);
            // extract names from bytes
            return (string[])ret.ToArray(typeof(string));
        }

        /// <summary>
        /// Returns a list of readers that are installed on the system.
        /// </summary>
        /// <returns>A list of readers.</returns>
        [SmartcardPermission(SecurityAction.Demand, SmartcardConnectOption.AllowedAtrs)]
        public static string[] GetReaders() {
            IntPtr context = IntPtr.Zero;
            try {
                if (NativeMethods.SCardEstablishContext(NativeMethods.SCARD_SCOPE_USER, IntPtr.Zero, IntPtr.Zero, out context) != NativeMethods.SCARD_S_SUCCESS)
                    return new string[0];
                return InternalGetReaders(context);
            } finally {
                if (context != IntPtr.Zero)
                    NativeMethods.SCardReleaseContext(context);
            }
        }
        /// <summary>
        /// Opens a SmartcardReader.
        /// </summary>
        /// <returns>An instance of the SmartcardReader class -or- a null reference if no smartcard reader was found.</returns>
        /// <exception cref="SmartcardException">An error occurred while communication with the smartcard reader.</exception>
        public static SmartcardReader OpenReader() {
            string[] readers = SmartcardReader.GetReaders();
            if (readers.Length == 0)
                return null;
            return new SmartcardReader(readers[0]);
        }
        /// <summary>
        /// Opens a SmartcardReader that has a card inserted with a specified ATR.
        /// </summary>
        /// <param name="atr">The ATR to search for.</param>
        /// <returns>An instance of the SmartcardReader class -or- a null reference if no smartcard reader was found.</returns>
        /// <exception cref="ArgumentNullException"><i>atr</i> is a null reference.</exception>
        /// <exception cref="SmartcardException">An error occurred while communication with the smartcard reader.</exception>
        public static SmartcardReader OpenReader(byte[] atr) {
            if (atr == null)
                throw new ArgumentNullException("atr", ResourceController.GetString("Error_ParamNull"));
            return OpenReader(new Atr(atr));
        }
        /// <summary>
        /// Opens a SmartcardReader that has a card inserted with a specified ATR.
        /// </summary>
        /// <param name="atr">The ATR to search for.</param>
        /// <returns>An instance of the SmartcardReader class -or- a null reference if no smartcard reader was found.</returns>
        /// <exception cref="ArgumentNullException"><i>atr</i> is a null reference.</exception>
        /// <exception cref="SmartcardException">An error occurred while communication with the smartcard reader.</exception>
        public static SmartcardReader OpenReader(Atr atr) {
            if (atr == null)
                throw new ArgumentNullException("atr", ResourceController.GetString("Error_ParamNull"));
            string[] readers = SmartcardReader.GetReaders();
            SmartcardReader sr = null;
            foreach(string reader in readers) {
                sr = new SmartcardReader(reader);
                sr.Connect();
                if (atr.IsMatch(sr.Atr.GetValue()))
                    break;
                sr.Dispose();
                sr = null;
            }
            return sr;
        }

        private bool m_Disposed;
        private IntPtr m_Context;
        private IntPtr m_Card;
        private int m_ActiveProtocol;
        private string m_ReaderName;
        private int m_State;
        private Atr m_Atr;
        private VendorAttributes m_Attributes;
    }
}
