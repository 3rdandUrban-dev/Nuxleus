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
using System.Globalization;
using System.Runtime.InteropServices;
using Org.Mentalis.SecurityServices.Win32;
using Org.Mentalis.SecurityServices.Resources;

namespace Org.Mentalis.SecurityServices.Authentication {
    /// <summary>
    /// Represents a binary credential that is associated with the current logged on user.
    /// </summary>
    /// <remarks>The Credential class requires Windows XP or higher.</remarks>
    public sealed class Credential : IDisposable {
        /// <summary>
        /// Initializes a new Credential instance.
        /// </summary>
        /// <param name="targetName">The name of the credential.</param>
        /// <remarks><b>targetName</b> must be at least one character and not longer than 32767 characters.</remarks>
        /// <exception cref="NotSupportedException">This functionality requires Windows XP or higher.</exception>
        /// <exception cref="ArgumentNullException"><b>targetName</b> is a null reference.</exception>
        /// <exception cref="ArgumentException"><b>targetName</b> is invalid.</exception>
        /// <exception cref="CredentialException">An error occurs while trying to retrieve the credential.</exception>
        public Credential(string targetName)
            : this(targetName, null, PersistType.Session) {
        }
        /// <summary>
        /// Initializes a new Credential instance.
        /// </summary>
        /// <param name="targetName">The name of the credential.</param>
        /// <param name="comment">A comment associated with the credential.</param>
        /// <param name="persist">One of the PersistType values.</param>
        /// <exception cref="NotSupportedException">This functionality requires Windows XP or higher.</exception>
        /// <exception cref="ArgumentNullException"><b>targetName</b> is a null reference.</exception>
        /// <exception cref="ArgumentException">One of the input parameters is invalid.</exception>
        /// <exception cref="CredentialException">An error occurs while trying to retrieve the credential.</exception>
        /// <remarks><b>targetName</b> must be at least one character and not longer than 32767 characters.<br/>
        /// <b>comment</b> may be a null reference, but may not be longer than 256 characters.<br/>
        /// <b>persist</b> may not have the value PersistType.None.</remarks>
        public Credential(string targetName, string comment, PersistType persist) {
            Platform.AssertWinXP();
            if (targetName == null)
                throw new ArgumentNullException("targetName", ResourceController.GetString("Error_ParamNull"));
            if (targetName.Length == 0 || targetName.Length > NativeMethods.CRED_MAX_GENERIC_TARGET_NAME_LENGTH)
                throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"), "targetName");
            if (!Enum.IsDefined(typeof(PersistType), persist) || persist == PersistType.None)
                throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"), "persist");
            if (comment != null && comment.Length > NativeMethods.CRED_MAX_STRING_LENGTH)
                throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"), "comment");
            m_Name = targetName.ToLower(CultureInfo.InvariantCulture);
            m_Comment = comment;
            m_Persist = persist;
            m_Contents = new byte[0];
            Refresh();
        }
        internal Credential(CREDENTIAL source) {
            RefreshFromCredential(source);
        }
        /// <summary>
        /// Refreshes the information in the credential.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The Credential instance has been closed.</exception>
        /// <exception cref="CredentialException">An error occurs while trying to refresh the credential.</exception>
        public void Refresh() {
            if (m_IsDisposed)
                throw new ObjectDisposedException(this.GetType().FullName);
            IntPtr buffer;
            if (NativeMethods.CredRead(m_Name, NativeMethods.CRED_TYPE_GENERIC, 0, out buffer) == 0) {
                int err = Marshal.GetLastWin32Error();
                if (err != NativeMethods.ERROR_NOT_FOUND)
                    throw new CredentialException(ResourceController.GetString("Error_CredentialRead"), err);
                m_Exists = false;
                return;
            }
            try {
                CREDENTIAL c = (CREDENTIAL)Marshal.PtrToStructure(buffer, typeof(CREDENTIAL));
                RefreshFromCredential(c);
            } finally {
                NativeMethods.CredFree(buffer);
            }
        }
        private unsafe void RefreshFromCredential(CREDENTIAL source) {
            m_Name = source.TargetName;
            m_Comment = source.Comment;
            m_Persist = (PersistType)source.Persist;
            m_Contents = new byte[source.CredentialBlobSize];
            if (source.CredentialBlobSize > 0) {
                for (int i = 0; i < source.CredentialBlobSize; i++) {
                    m_Contents[i] = source.CredentialBlob[i];
                }
            }
            m_Exists = true;
        }
        /// <summary>
        /// Returns the binary contents of the credential.
        /// </summary>
        /// <returns>A byte array that contains the contents of the credential.</returns>
        /// <exception cref="ObjectDisposedException">The Credential instance has been closed.</exception>
        public byte[] Read() {
            if (m_IsDisposed)
                throw new ObjectDisposedException(this.GetType().FullName);
            return m_Contents;
        }
        /// <summary>
        /// Replaces the binary contents of the credential.
        /// </summary>
        /// <param name="contents">The new contents to write.</param>
        /// <exception cref="ObjectDisposedException">The Credential instance has been closed.</exception>
        /// <exception cref="ArgumentNullException"><b>contents</b> is a null reference.</exception>
        /// <exception cref="ArgumentException"><b>contents</b> is invalid.</exception>
        /// <exception cref="CredentialException">An error occurs while trying to write the credential.</exception>
        /// <remarks><b>contents</b> may not be larger than 512 bytes.</remarks>
        public void Write(byte[] contents) {
            if (m_IsDisposed)
                throw new ObjectDisposedException(this.GetType().FullName);
            if (contents == null)
                throw new ArgumentNullException("contents", ResourceController.GetString("Error_ParamNull"));
            if (contents.Length > NativeMethods.CRED_MAX_CREDENTIAL_BLOB_SIZE)
                throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"), "contents");
            InternalWrite(contents);
        }
        private unsafe void InternalWrite(byte[] contents) {
            fixed (byte* blobPointer = contents) {
                CREDENTIAL cred = new CREDENTIAL();
                cred.Flags = 0;
                cred.Type = NativeMethods.CRED_TYPE_GENERIC;
                cred.TargetName = m_Name;
                cred.Comment = m_Comment;
                cred.LastWritten = 0;
                cred.CredentialBlobSize = contents.Length;
                cred.CredentialBlob = blobPointer;
                cred.Persist = (int)m_Persist;
                cred.AttributeCount = 0;
                cred.Attributes = IntPtr.Zero;
                cred.TargetAlias = null;
                cred.UserName = null;
                if (NativeMethods.CredWrite(ref cred, 0) == 0) {
                    int err = Marshal.GetLastWin32Error();
                    throw new CredentialException(ResourceController.GetString("Error_CredentialWrite"), err);
                }
            }
            m_Contents = (byte[])contents.Clone();
            m_Exists = true;
        }
        /// <summary>
        /// Deletes the credential.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The Credential instance has been closed.</exception>
        /// <exception cref="CredentialException">An error occurs while trying to delete the credential.</exception>
        public void Delete() {
            if (m_IsDisposed)
                throw new ObjectDisposedException(this.GetType().FullName);
            if (NativeMethods.CredDelete(m_Name, NativeMethods.CRED_TYPE_GENERIC, 0) == 0) {
                int err = Marshal.GetLastWin32Error();
                throw new CredentialException(ResourceController.GetString("Error_CredentialDelete"), err);
            }
            m_Exists = false;
            Dispose();
        }
        /// <summary>
        /// Closes the credential.
        /// </summary>
        public void Dispose() {
            m_IsDisposed = true;
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Gets or sets the name of the credential.
        /// </summary>
        /// <value>A string that contains the name of the credential.</value>
        /// <exception cref="ObjectDisposedException">The Credential instance has been closed.</exception>
        /// <exception cref="CredentialException">An error occurs while trying to rename the credential.</exception>
        /// <exception cref="ArgumentNullException"><b>value</b> is a null reference.</exception>
        /// <exception cref="ArgumentException"><b>value</b> is invalid.</exception>
        public string Name {
            get {
                return m_Name;
            }
            set {
                if (m_IsDisposed)
                    throw new ObjectDisposedException(this.GetType().FullName);
                if (value == null)
                    throw new ArgumentNullException("value", ResourceController.GetString("Error_ParamNull"));
                if (value.Length == 0 || value.Length > NativeMethods.CRED_MAX_GENERIC_TARGET_NAME_LENGTH)
                    throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"), "value");
                value = value.ToLower(CultureInfo.InvariantCulture);
                if (m_Exists && value != m_Name) {
                    if (NativeMethods.CredRename(m_Name, value, NativeMethods.CRED_TYPE_GENERIC, 0) == 0) {
                        int err = Marshal.GetLastWin32Error();
                        throw new CredentialException(ResourceController.GetString("Error_CredentialRename"), err);
                    }
                }
                m_Name = value;
            }
        }
        /// <summary>
        /// Gets or sets the comment of the credential.
        /// </summary>
        /// <value>A string that contains the comment of the credential.</value>
        /// <exception cref="ObjectDisposedException">The Credential instance has been closed.</exception>
        /// <exception cref="CredentialException">An error occurs while trying to set the comment of the credential.</exception>
        /// <exception cref="ArgumentException"><b>value</b> is invalid.</exception>
        public string Comment {
            get {
                if (m_IsDisposed)
                    throw new ObjectDisposedException(this.GetType().FullName);
                return m_Comment;
            }
            set {
                if (m_IsDisposed)
                    throw new ObjectDisposedException(this.GetType().FullName);
                if (value != null && value.Length > NativeMethods.CRED_MAX_STRING_LENGTH)
                    throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"), "targetName");
                m_Comment = value;
                InternalWrite(m_Contents);
            }
        }
        /// <summary>
        /// Gets or sets the persistence type of the credential.
        /// </summary>
        /// <value>One of the PersistType values.</value>
        /// <exception cref="ObjectDisposedException">The Credential instance has been closed.</exception>
        /// <exception cref="CredentialException">An error occurs while trying to set the comment of the credential.</exception>
        /// <exception cref="ArgumentException"><b>value</b> is invalid.</exception>
        public PersistType Persist {
            get {
                if (m_IsDisposed)
                    throw new ObjectDisposedException(this.GetType().FullName);
                return m_Persist;
            }
            set {
                if (m_IsDisposed)
                    throw new ObjectDisposedException(this.GetType().FullName);
                if (!Enum.IsDefined(typeof(PersistType), value) || value == PersistType.None)
                    throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"), "value");
                m_Persist = value;
                InternalWrite(m_Contents);
            }
        }
        /// <summary>
        /// Gets or sets the contents of the credential.
        /// </summary>
        /// <value>An array of bytes with the contents of the credential.</value>
        /// <exception cref="ObjectDisposedException">The Credential instance has been closed.</exception>
        /// <exception cref="CredentialException">An error occurs while trying to set the contents of the credential.</exception>
        /// <exception cref="ArgumentNullException"><b>value</b> is a null reference.</exception>
        /// <exception cref="ArgumentException"><b>value</b> is invalid.</exception>
        public byte[] GetContents() {
            if (m_IsDisposed)
                throw new ObjectDisposedException(this.GetType().FullName);
            return (byte[])m_Contents.Clone();
        }
        /// <summary>
        /// Gets a value that indicates whether the credential already exists in the system or not.
        /// </summary>
        /// <value><b>true</b> if the credential has already been saved, <b>false</b> otherwise.</value>
        public bool Exists {
            get {
                return m_Exists;
            }
        }
        /// <summary>
        /// Serves as a hash function for a particular type. GetHashCode is suitable for use in hashing algorithms and data structures like a hash table. 
        /// </summary>
        /// <returns>A hash code for the current Credential.</returns>
        public override int GetHashCode() {
            return m_Name.GetHashCode();
        }
        /// <summary>
        /// Determines whether the specified Object is equal to the current Credential. 
        /// </summary>
        /// <param name="obj">The Object to compare with the current Credential.</param>
        /// <returns><b>true</b> if the specified Object is equal to the current Credential; otherwise, <b>false</b>.</returns>
        public override bool Equals(object obj) {
            Credential c = obj as Credential;
            if (obj == null) {
                string s = obj as string;
                if (obj == null) {
                    return false;
                } else {
                    return string.Equals(m_Name, s, StringComparison.InvariantCultureIgnoreCase);
                }
            } else {
                return string.Equals(m_Name, c.m_Name, StringComparison.InvariantCultureIgnoreCase);
            }
        }
        /// <summary>
        /// Returns a String that represents the current Credential.
        /// </summary>
        /// <returns>A String that represents the current Credential.</returns>
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < m_Contents.Length; i++) {
                sb.Append(m_Contents[i].ToString("X2", CultureInfo.InvariantCulture));
            }
            return string.Format(CultureInfo.InvariantCulture, "Org.Mentalis.SecurityServices.Authentication.Credential\r\nName: {0}\r\nComment: {1}\r\nContents: {2}", m_Name, m_Comment, sb.ToString());
        }
        /// <summary>
        /// Allows an Object to attempt to free resources and perform other cleanup operations before the Object is reclaimed by garbage collection.
        /// </summary>
        ~Credential() {
            Dispose();
        }
        private string m_Name;
        private string m_Comment;
        private PersistType m_Persist;
        private byte[] m_Contents;
        private bool m_Exists;
        private bool m_IsDisposed;

        /// <summary>
        /// Retrieves all credentials from the system that are associated with the current login.
        /// </summary>
        /// <returns>An instance of the CredentialCollection class with all the credentials that are associated with the current login.</returns>
        /// <exception cref="NotSupportedException">This functionality requires Windows XP or higher.</exception>
        /// <exception cref="CredentialException">An error occurs while retrieving the credentials.</exception>
        public static CredentialCollection GetCredentials() {
            return Credential.GetCredentials(null);
        }
        /// <summary>
        /// Retrieves all credentials from the system that are associated with the current login and that match with the specified filter.
        /// </summary>
        /// <param name="filter">The filter that's used to match the names of the credentials.</param>
        /// <returns>An instance of the CredentialCollection class with all the credentials that are associated with the current login and that match with the specified filter.</returns>
        /// <remarks>The filter specifies a name prefix followed by an asterisk. For instance, the filter "FRED*" will return all credentials with a TargetName beginning with the string "FRED".</remarks>
        /// <exception cref="NotSupportedException">This functionality requires Windows XP or higher.</exception>
        /// <exception cref="CredentialException">An error occurs while retrieving the credentials.</exception>
        public static CredentialCollection GetCredentials(string filter) {
            return new CredentialCollection(filter);
        }
        /// <summary>
        /// Returns the maximum persistence type that's supported for the current login.
        /// </summary>
        /// <value>One of the PersistType values.</value>
        /// <exception cref="NotSupportedException">This functionality requires Windows XP or higher.</exception>
        /// <exception cref="CredentialException">An error occurs while retrieving the maximum persistence.</exception>
        public static PersistType MaximumPersistence {
            get {
                Platform.AssertWinXP();
                int[] values = new int[NativeMethods.CRED_TYPE_MAXIMUM];
                if (NativeMethods.CredGetSessionTypes(NativeMethods.CRED_TYPE_MAXIMUM, values) == 0) {
                    int err = Marshal.GetLastWin32Error();
                    throw new CredentialException(ResourceController.GetString("Error_CredentialMaxPersistence"), err);
                }
                return (PersistType)values[NativeMethods.CRED_TYPE_GENERIC];
            }
        }
    }
}