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
using System.Collections;
using System.Security;
using System.Runtime.InteropServices;
using Org.Mentalis.SecurityServices.Resources;

namespace Org.Mentalis.SecurityServices.Authentication {
    internal unsafe class CharEnumerator : IDisposable  {
        public CharEnumerator(string input) {
            m_Index = -1;
            m_BSTR = IntPtr.Zero;
            m_String = input;
            m_Length = input.Length;
        }
        public CharEnumerator(SecureString input) {
            m_Index = -1;
            m_BSTR = Marshal.SecureStringToBSTR(input);
            m_pBSTR = (char*)m_BSTR.ToPointer(); 
            m_Length = input.Length;
        }
        public char Current {
            get {
                if (m_Index == -1)
                    throw new InvalidOperationException(ResourceController.GetString("Error_EnumNotStarted"));
                if (m_Index >= m_Length)
                    throw new InvalidOperationException(ResourceController.GetString("Error_EnumEnded"));
                return m_Current;
            }
        }
        public bool MoveNext() {
            if (m_Index >= m_Length - 1) {
                return false;
            }
            m_Index++;
            if (m_String != null) {
                m_Current = m_String[m_Index];
            } else {
                m_Current = m_pBSTR[m_Index];
            }
            return true;
        }
        public void Dispose() {
            if (!m_Disposed) {
                m_Disposed = true;
                if (m_BSTR != IntPtr.Zero) {
                    Marshal.ZeroFreeBSTR(m_BSTR);
                    m_BSTR = IntPtr.Zero;
                }
                m_String = null;
                GC.SuppressFinalize(this);
            }
        }
        ~CharEnumerator() {
            Dispose();
        }
        private char m_Current;
        private int m_Index;
        private IntPtr m_BSTR;
        private char* m_pBSTR;
        private string m_String;
        private int m_Length;
        private bool m_Disposed;
    }
}
