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
using System.Runtime.InteropServices;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Org.Mentalis.SecurityServices.Win32;

namespace Org.Mentalis.SecurityServices.Authentication {
    /// <summary>
    /// The exception that is thrown when a credential error occurs.
    /// </summary>
    [Serializable]
    public class CredentialException : SystemException {
        /// <summary>
        /// Initializes a new instance of the CredentialException class.
        /// </summary>
        public CredentialException() : base() { }
        /// <summary>
        /// Initializes a new instance of the CredentialException class with its message string set to message and its inner exception set to a null reference (<b>Nothing</b> in Visual Basic).
        /// </summary>
        /// <param name="message">A String that describes the error. The content of message is intended to be understood by humans.</param>
        public CredentialException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the CredentialException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">A String that describes the error. The content of message is intended to be understood by humans.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference (<b>Nothing</b> in Visual Basic), the current exception is raised in a <b>catch</b> block that handles the inner exception.</param>
        public CredentialException(string message, Exception innerException) : base(message, innerException) { }
        /// <summary>
        /// Initializes a new instance of the Exception class with serialized data.
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">The <i>info</i> parameter is a null reference.</exception>
        /// <exception cref="SerializationException">The class name is a null reference or HResult is zero (0).</exception>
        protected CredentialException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        /// <summary>
        /// Initializes a new instance of the CredentialException class with its message string set to message and its inner exception set to a null reference (<b>Nothing</b> in Visual Basic).
        /// </summary>
        /// <param name="message">A String that describes the error. The content of message is intended to be understood by humans.</param>
        /// <param name="error">The error code of the exception.</param>
        public CredentialException(string message, int error)
            : this(message, (uint)error) {
        }
        /// <summary>
        /// Initializes a new instance of the CredentialException class with its message string set to message and its inner exception set to a null reference (<b>Nothing</b> in Visual Basic).
        /// </summary>
        /// <param name="message">A String that describes the error. The content of message is intended to be understood by humans.</param>
        /// <param name="error">The error code of the exception.</param>
        internal CredentialException(string message, uint error)
            : base(string.Format(CultureInfo.InvariantCulture, message, "0x" + error.ToString("X2", CultureInfo.InvariantCulture), NativeMethods.FormatMessage(error))) {
        }
        /// <summary>
        /// When overridden in a derived class, sets the SerializationInfo with information about the exception.
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">The <i>info</i> parameter is a null reference.</exception>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
            base.GetObjectData(info, context);
        }
    }
}