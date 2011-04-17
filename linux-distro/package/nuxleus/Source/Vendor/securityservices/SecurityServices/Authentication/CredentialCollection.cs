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
using System.Collections.ObjectModel;
using System.Text;
using System.Runtime.InteropServices;
using Org.Mentalis.SecurityServices.Win32;
using Org.Mentalis.SecurityServices.Resources;

namespace Org.Mentalis.SecurityServices.Authentication {
    /// <summary>
    /// Represents a collection of Credential objects. This class cannot be inherited. 
    /// </summary>
    /// <remarks>The CredentialCollection class requires Windows XP or higher.</remarks>
    public sealed class CredentialCollection : Collection<Credential> {
        /// <summary>
        /// Initializes an new CredentialCollection instance.
        /// </summary>
        /// <param name="targetFilter">The filter to use when looking for credentials -or- a null reference if all credentials should be returned.</param>
        /// <exception cref="NotSupportedException">This functionality requires Windows XP or higher.</exception>
        /// <exception cref="CredentialException">An error occurs while retrieving the credentials.</exception>
        public CredentialCollection(string targetFilter) {
            Platform.AssertWinXP();
            // enumerate the credentials
            int count;
            IntPtr creds;
            if (NativeMethods.CredEnumerate(targetFilter, 0, out count, out creds) == 0) {
                int err = Marshal.GetLastWin32Error();
                if (err != NativeMethods.ERROR_NOT_FOUND)
                    throw new CredentialException(ResourceController.GetString("Error_CredentialEnumeration"), err);
                return;
            }
            try {
                for(int i = 0; i < count; i++) {
                    IntPtr buffer = Marshal.ReadIntPtr(creds, IntPtr.Size * i);
                    CREDENTIAL c = (CREDENTIAL)Marshal.PtrToStructure(buffer, typeof(CREDENTIAL));
                    this.Add(new Credential(c));
                }
            } finally {
                NativeMethods.CredFree(creds);
            }
        }
    }
}