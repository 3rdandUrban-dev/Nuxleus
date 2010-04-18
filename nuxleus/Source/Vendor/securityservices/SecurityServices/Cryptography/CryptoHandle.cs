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
using System.Security.Cryptography;
using System.Security.Permissions;
using Org.Mentalis.SecurityServices.Resources;
using Org.Mentalis.SecurityServices.Win32;
using Microsoft.Win32;

namespace Org.Mentalis.SecurityServices.Cryptography {
	internal class CryptoHandle {
		internal CryptoHandle() {}
        public static IntPtr Handle {
			get {
				m_Provider.CreateInternalHandle(ref m_Provider.m_Handle, null);
				return m_Provider.m_Handle;
			}
		}
		public static int HandleProviderType {
			get {
				m_Provider.CreateInternalHandle(ref m_Provider.m_Handle, null);
				return m_Provider.m_HandleProviderType;
			}
		}
        public void CreateInternalHandle(ref IntPtr handle, string container) {
			if (handle == IntPtr.Zero) {
				lock(this) {
					if (handle == IntPtr.Zero && !m_Error) {
						int flags, fs = 0, fmk = 0;
						if (!Environment.UserInteractive && Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 5) {
                            fs = NativeMethods.CRYPT_SILENT;
                            fmk = NativeMethods.CRYPT_MACHINE_KEYSET;
						}
						for(int i = 0; i < m_Providers.Length; i++) {
							flags = fs | fmk;
							m_HandleProviderType = m_Providers[i];
                            if (NativeMethods.CryptAcquireContext(ref handle, container, null, m_Providers[i], flags) == 0) {
                                if (Marshal.GetLastWin32Error() == NativeMethods.NTE_BAD_KEYSET) {
                                    NativeMethods.CryptAcquireContext(ref handle, container, null, m_Providers[i], flags | NativeMethods.CRYPT_NEWKEYSET);
								} else if(fmk != 0) {
									flags = fs;
                                    if (NativeMethods.CryptAcquireContext(ref handle, container, null, m_Providers[i], flags) == 0) {
                                        if (Marshal.GetLastWin32Error() == NativeMethods.NTE_BAD_KEYSET) {
                                            NativeMethods.CryptAcquireContext(ref handle, container, null, m_Providers[i], flags | NativeMethods.CRYPT_NEWKEYSET);
										}
									}
								}
							}
                            if (handle != IntPtr.Zero)
								break;
						}
                        if (handle == IntPtr.Zero) {
							m_Error = true;
							m_HandleProviderType = 0;
						}
					}
					if (m_Error)
						throw new CryptographicException(ResourceController.GetString("Error_AcquireCSP"));
				}
			}
		}
        internal static bool PolicyRequiresFips {
            get {
                // we do not require our callers to have a RegistryPermission
                RegistryPermission regPerm = new RegistryPermission(RegistryPermissionAccess.Read, @"System\CurrentControlSet\Control\Lsa\FIPSAlgorithmPolicy");
                regPerm.Assert();

                if (m_RequiresFips == -1) {
                    m_RequiresFips = 0;
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Control\Lsa", false)) {
                        if (key != null) {
                            object val = key.GetValue("FIPSAlgorithmPolicy");
                            if (val != null) {
                                m_RequiresFips = (int)val;
                            }
                        }
                    }
                }
                return m_RequiresFips == 1;
            }
        }
 

        ~CryptoHandle() {
			if (m_Handle != IntPtr.Zero)
                NativeMethods.CryptReleaseContext(m_Handle, 0);
		}
        private IntPtr m_Handle;
		private bool m_Error;
		private int m_HandleProviderType;
        private static int[] m_Providers = new int[] { NativeMethods.PROV_RSA_AES, NativeMethods.PROV_RSA_FULL };
        private static CryptoHandle m_Provider = new CryptoHandle();
        private static int m_RequiresFips = -1;
	}
}