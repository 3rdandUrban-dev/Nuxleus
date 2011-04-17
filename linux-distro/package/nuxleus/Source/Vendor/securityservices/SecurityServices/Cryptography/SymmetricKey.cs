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
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using Org.Mentalis.SecurityServices.Resources;
using Org.Mentalis.SecurityServices.Win32;

namespace Org.Mentalis.SecurityServices.Cryptography {
	internal sealed class SymmetricKey : IDisposable {
		private SymmetricKey() {
			m_Provider = CryptoHandle.Handle;
			m_ExponentOfOne = CreateExponentOfOneKey();
			//m_PaddingMode = PaddingMode.None;
		}
		/*internal SymmetricKey(int provider, int key, bool ownsProvider) : this(ownsProvider) {
			if (key == 0 || provider == 0)
				throw new ArgumentNullException();
			m_Provider = provider;
			m_Handle = key;
			m_PaddingMode = PaddingMode.None;
		}*/
        /*public SymmetricKey(CryptoProvider provider, CryptoAlgorithm algorithm) : this(provider) {
            m_Handle = 0;
            if (NativeMethods.CryptGenKey(m_Provider, new IntPtr((int)algorithm), NativeMethods.CRYPT_EXPORTABLE, ref m_Handle) == 0)
                throw new CryptographicException("Cannot generate session key.");
        }*/
        public SymmetricKey(CryptoAlgorithm algorithm, byte[] buffer) : this() {
			if (buffer == null)
                throw new ArgumentNullException("buffer", ResourceController.GetString("Error_ParamNull"));
			m_Handle = KeyFromBytes(m_Provider, algorithm, buffer);
		}
        private unsafe IntPtr KeyFromBytes(IntPtr provider, CryptoAlgorithm algorithm, byte[] key) {
            int dwFlags = NativeMethods.CRYPT_FIRST, dwSize, dwProvSessionKeySize = 0, dwPublicKeySize = 0, dwSessionBlob, offset = 0, algo = (int)algorithm;
            IntPtr provEnum = IntPtr.Zero, dwPrivKeyAlg = IntPtr.Zero, pbSessionBlob = IntPtr.Zero, hTempKey = IntPtr.Zero, hSessionKey = IntPtr.Zero;
            try {
                // Double check to see if this provider supports this algorithm
                // and key size
                bool found = false;
                provEnum = Marshal.AllocHGlobal(84 + IntPtr.Size);
                do {
                    dwSize = 84 + IntPtr.Size;
                    if (NativeMethods.CryptGetProvParam(provider, NativeMethods.PP_ENUMALGS_EX, provEnum, ref dwSize, dwFlags) == 0)
                        break;
                    dwFlags = 0;
                    if (Marshal.ReadInt32(provEnum) == algo)
                        found = true;
                } while (!found);
                if (!found)
                    throw new CryptographicException(ResourceController.GetString("Error_AlgNotSupp"));
                // We have to get the key size(including padding)
                // from an HCRYPTKEY handle.  PP_ENUMALGS_EX contains
                // the key size without the padding so we can't use it.
                if (NativeMethods.CryptGenKey(provider, algo, 0, ref hTempKey) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_KeygenFailed"));
                dwSize = 4; // sizeof(int)
                if (NativeMethods.CryptGetKeyParam(hTempKey, NativeMethods.KP_KEYLEN, ref dwProvSessionKeySize, ref dwSize, 0) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_GetKeyParams"));
                // Our key is too big, leave
                if ((key.Length * 8) > dwProvSessionKeySize)
                    throw new CryptographicException(ResourceController.GetString("Error_BigKey"));
                // Get private key's algorithm
                dwSize = 4; //sizeof(ALG_ID)
                if (NativeMethods.CryptGetKeyParam(m_ExponentOfOne, NativeMethods.KP_ALGID, ref dwPrivKeyAlg, ref dwSize, 0) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_PrivateKeyAlg"));
                // Get private key's length in bits
                dwSize = 4; // sizeof(DWORD)
                if (NativeMethods.CryptGetKeyParam(m_ExponentOfOne, NativeMethods.KP_KEYLEN, ref dwPublicKeySize, ref dwSize, 0) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_KeyLength"));
                // calculate Simple blob's length
                dwSessionBlob = (dwPublicKeySize / 8) + 4 /*sizeof(ALG_ID)*/ + 4 + IntPtr.Size /*sizeof(BLOBHEADER)*/;
                // allocate simple blob buffer
                pbSessionBlob = Marshal.AllocHGlobal(dwSessionBlob);
                // SIMPLEBLOB Format is documented in SDK
                // Copy header to buffer
                PUBLICKEYSTRUC pks = new PUBLICKEYSTRUC();
                pks.bType = NativeMethods.SIMPLEBLOB;
                pks.bVersion = 2;
                pks.reserved = 0;
                pks.aiKeyAlg = algo;
                Marshal.StructureToPtr(pks, pbSessionBlob, false);
                Marshal.WriteIntPtr(pbSessionBlob, offset = Marshal.SizeOf(pks), dwPrivKeyAlg);
                offset += 4; // sizeof(ALG_ID)
                // Place the key material in reverse order
                for (int i = 0; i < key.Length; i++) {
                    Marshal.WriteByte(pbSessionBlob, offset + key.Length - i - 1, key[i]);
                }
                // 3 is for the first reserved byte after the key material + the 2 reserved bytes at the end.
                dwSize = dwSessionBlob - (4 /*sizeof(ALG_ID)*/ + IntPtr.Size + 4 /*sizeof(BLOBHEADER)*/ + key.Length + 3);
                offset += key.Length + 1;
                // Generate random data for the rest of the buffer
                // (except that last two bytes)
                byte* buffer = (byte*)pbSessionBlob.ToPointer() + offset;
                if (NativeMethods.CryptGenRandom(provider, dwSize, buffer) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_Randomizer"));
                for (int i = 0; i < dwSize; i++) {
                    if (Marshal.ReadByte(pbSessionBlob, offset) == 0)
                        Marshal.WriteByte(pbSessionBlob, offset, 1);
                    offset++;
                }
                Marshal.WriteByte(pbSessionBlob, dwSessionBlob - 2, 2);
                if (NativeMethods.CryptImportKey(provider, pbSessionBlob, dwSessionBlob, m_ExponentOfOne, NativeMethods.CRYPT_EXPORTABLE, ref hSessionKey) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_KeyImport"));
            } finally {
                if (provEnum != IntPtr.Zero)
                    Marshal.FreeHGlobal(provEnum);
                if (hTempKey != IntPtr.Zero)
                    NativeMethods.CryptDestroyKey(hTempKey);
                if (pbSessionBlob != IntPtr.Zero)
                    Marshal.FreeHGlobal(pbSessionBlob);
            }
            return hSessionKey;
        }
		/*public unsafe byte[] ToBytes() {
			if (m_Handle == 0)
				throw new ObjectDisposedException(this.GetType().FullName);
			IntPtr pbSessionBlob = IntPtr.Zero;
			try {
				int dwSessionBlob = 0;
				if (NativeMethods.CryptExportKey(m_Handle, m_ExponentOfOne, NativeMethods.SIMPLEBLOB, 0, IntPtr.Zero, ref dwSessionBlob) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_KeyExport"));
				pbSessionBlob = Marshal.AllocHGlobal(dwSessionBlob);
				if (NativeMethods.CryptExportKey(m_Handle, m_ExponentOfOne, NativeMethods.SIMPLEBLOB, 0, pbSessionBlob, ref dwSessionBlob) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_KeyExport"));

				// Get session key size in bits
				int dwSize = 4; // sizeof(DWORD)
				int dwKeyMaterial = 0;
				if (NativeMethods.CryptGetKeyParam(m_Handle, NativeMethods.KP_KEYLEN, ref dwKeyMaterial, ref dwSize, 0) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_GetKeyParams"));
				// Get the number of bytes and allocate buffer
				dwKeyMaterial /= 8;
				byte[] pbKeyMaterial = new byte[dwKeyMaterial];
				// Skip the header
				int offset = 4 + IntPtr.Size; // sizeof(BLOBHEADER);
				offset += 4; // sizeof(ALG_ID);
                byte* buffer = (byte*)pbSessionBlob.ToPointer() + offset;
                Marshal.Copy(new IntPtr(buffer), pbKeyMaterial, 0, pbKeyMaterial.Length);
				// the key is reversed
				Array.Reverse(pbKeyMaterial);
				return pbKeyMaterial;
			} finally {
				if (pbSessionBlob != IntPtr.Zero)
					Marshal.FreeHGlobal(pbSessionBlob);
			}
		}*/
		/*public override string ToString() {
			if (m_Handle == 0)
				throw new ObjectDisposedException(this.GetType().FullName);
			byte[] bytes = ToBytes();
			StringBuilder sb = new StringBuilder(bytes.Length * 2);
			for(int i = 0; i < bytes.Length; i++) {
				sb.Append(bytes[i].ToString("X2"));
			}
			return sb.ToString();
		}*/
		public void Dispose() {
            if (m_Handle != IntPtr.Zero)
				NativeMethods.CryptDestroyKey(m_Handle);
            if (m_ExponentOfOne != IntPtr.Zero)
				NativeMethods.CryptDestroyKey(m_ExponentOfOne);
            m_Handle = m_ExponentOfOne = m_Provider = IntPtr.Zero;
			GC.SuppressFinalize(this);
		}
		~SymmetricKey() {
			Dispose();
		}
        private IntPtr CreateExponentOfOneKey() {
			try {
				return CreateStaticExponentOfOneKey();
            } catch (CryptographicException) {
				return CreateDynamicExponentOfOneKey();
			}
		}
        private IntPtr CreateStaticExponentOfOneKey() {
            IntPtr hPrivateKey = IntPtr.Zero;
			if (NativeMethods.CryptImportKey(m_Provider, ExponentOfOne, ExponentOfOne.Length, IntPtr.Zero, NativeMethods.CRYPT_EXPORTABLE, ref hPrivateKey) == 0)
                throw new CryptographicException(ResourceController.GetString("Error_KeyImport"));
			return hPrivateKey;
		}
        private IntPtr CreateDynamicExponentOfOneKey() {
            IntPtr hPrivateKey = IntPtr.Zero;
			int dwKeyBlob = 0;
			IntPtr keyblob = IntPtr.Zero;
			int dwBitLen;
			try {
				if (NativeMethods.CryptGenKey(m_Provider, NativeMethods.AT_KEYEXCHANGE, NativeMethods.CRYPT_EXPORTABLE, ref hPrivateKey) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_KeygenFailed"));
				// Export the private key, we'll convert it to a private
				// exponent of one key
                if (NativeMethods.CryptExportKey(hPrivateKey, IntPtr.Zero, NativeMethods.PRIVATEKEYBLOB, 0, IntPtr.Zero, ref dwKeyBlob) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_KeyExport"));
				keyblob = Marshal.AllocHGlobal(dwKeyBlob);
                if (NativeMethods.CryptExportKey(hPrivateKey, IntPtr.Zero, NativeMethods.PRIVATEKEYBLOB, 0, keyblob, ref dwKeyBlob) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_KeyExport"));
				NativeMethods.CryptDestroyKey(hPrivateKey);
                hPrivateKey = IntPtr.Zero;
				// Get the bit length of the key
				dwBitLen = Marshal.ReadInt32(keyblob, 12);
				/* Modify the Exponent in Key BLOB format [Key BLOB format is documented in SDK] */
				// Convert pubexp in rsapubkey to 1
				int offset = 16;
				for (int i = 0; i < 4; i++) {
					if (i == 0)
						Marshal.WriteByte(keyblob, offset, 1);
					else
						Marshal.WriteByte(keyblob, offset + i, 0);
				}
				// Skip pubexp
				offset += 4;
				// Skip modulus, prime1, prime2
				offset += dwBitLen / 8;
				offset += dwBitLen / 16;
				offset += dwBitLen / 16;
				// Convert exponent1 to 1
				for (int i = 0; i < dwBitLen / 16; i++) {
					if (i == 0)
						Marshal.WriteByte(keyblob, offset, 1);
					else
						Marshal.WriteByte(keyblob, offset + i, 0);
				}
				// Skip exponent1
				offset += dwBitLen / 16;
				// Convert exponent2 to 1
				for (int i = 0; i < dwBitLen / 16; i++) {
					if (i == 0)
						Marshal.WriteByte(keyblob, offset, 1);
					else
						Marshal.WriteByte(keyblob, offset + i, 0);
				}
				// Skip exponent2, coefficient
				offset += dwBitLen / 16;
				offset += dwBitLen / 16;
				// Convert privateExponent to 1
				for (int i = 0; i < dwBitLen / 8; i++) {
					if (i == 0)
						Marshal.WriteByte(keyblob, offset, 1);
					else
						Marshal.WriteByte(keyblob, offset + i, 0);
				}
				// Import the exponent-of-one private key.      
                if (NativeMethods.CryptImportKey(m_Provider, keyblob, dwKeyBlob, IntPtr.Zero, NativeMethods.CRYPT_EXPORTABLE, ref hPrivateKey) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_KeyImport"));
			} catch {
                if (hPrivateKey != IntPtr.Zero)
					NativeMethods.CryptDestroyKey(hPrivateKey);
				throw;
			} finally {
				if (keyblob != IntPtr.Zero)
					Marshal.FreeHGlobal(keyblob);
			}
			return hPrivateKey;
		}
		/*public int Provider {
			get {
				if (m_Handle == 0)
					throw new ObjectDisposedException(this.GetType().FullName);
				return m_Provider;
			}
		}*/
        public IntPtr Handle {
			get {
                if (m_Handle == IntPtr.Zero)
                    throw new ObjectDisposedException(this.GetType().FullName, ResourceController.GetString("Error_Disposed"));
				return m_Handle;
			}
		}
		public byte[] IV {
			/*get {
				if (m_Handle == 0)
                    throw new ObjectDisposedException(this.GetType().FullName, ResourceController.GetString("Error_Disposed"));
				int length = 0;
				if (NativeMethods.CryptGetKeyParam(m_Handle, NativeMethods.KP_IV, null, ref length, 0) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_GetKeyParams"));
				byte[] buffer = new byte[length];
				if (NativeMethods.CryptGetKeyParam(m_Handle, NativeMethods.KP_IV, buffer, ref length, 0) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_GetKeyParams"));
				return buffer;
			}*/
			set {
                if (m_Handle == IntPtr.Zero)
                    throw new ObjectDisposedException(this.GetType().FullName, ResourceController.GetString("Error_Disposed"));
				if (value == null)
					throw new ArgumentNullException();
				if (NativeMethods.CryptSetKeyParam(m_Handle, NativeMethods.KP_IV, value, 0) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_SetKeyParams"));
			}
		}
		public CipherMode Mode {
			/*get {
				if (m_Handle == 0)
                    throw new ObjectDisposedException(this.GetType().FullName, ResourceController.GetString("Error_Disposed"));
				int ret = 0, length = 4;
				if (NativeMethods.CryptGetKeyParam(m_Handle, NativeMethods.KP_MODE, ref ret, ref length, 0) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_GetKeyParams"));
				return (CipherMode)ret;
			}*/
			set {
                if (m_Handle == IntPtr.Zero)
					throw new ObjectDisposedException(this.GetType().FullName);
				int mode = (int)value;
				if (NativeMethods.CryptSetKeyParam(m_Handle, NativeMethods.KP_MODE, ref mode, 0) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_SetKeyParams"));
			}
		}
		public int FeedbackSize {
			/*get {
				if (m_Handle == 0)
                    throw new ObjectDisposedException(this.GetType().FullName, ResourceController.GetString("Error_Disposed"));
				int ret = 0, length = 4;
				if (NativeMethods.CryptGetKeyParam(m_Handle, NativeMethods.KP_MODE_BITS, ref ret, ref length, 0) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_GetKeyParams"));
				return ret;
			}*/
			set {
                if (m_Handle == IntPtr.Zero)
					throw new ObjectDisposedException(this.GetType().FullName);
				if (NativeMethods.CryptSetKeyParam(m_Handle, NativeMethods.KP_MODE_BITS, ref value, 0) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_SetKeyParams"));
			}
		}
		public PaddingMode Padding {
			/*get {
				return m_PaddingMode;
			}*/
			set {
                if (m_Handle == IntPtr.Zero)
                    throw new ObjectDisposedException(this.GetType().FullName, ResourceController.GetString("Error_Disposed"));
				int val = GetPaddingMode(value);
				if (NativeMethods.CryptSetKeyParam(m_Handle, NativeMethods.KP_PADDING, ref val, 0) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_SetKeyParams"));
				//m_PaddingMode = value;
			}
		}
		private static int GetPaddingMode(PaddingMode mode) {
			if (mode == PaddingMode.PKCS7)
				return NativeMethods.PKCS5_PADDING;
			else
				return NativeMethods.ZERO_PADDING;
		}
        private IntPtr m_Handle;
        private IntPtr m_Provider;
        private IntPtr m_ExponentOfOne;
		//private PaddingMode m_PaddingMode;
		private static readonly byte[] ExponentOfOne = {
				0x07, 0x02, 0x00, 0x00, 0x00, 0xA4, 0x00, 0x00, 0x52, 0x53, 0x41, 0x32, 0x00, 0x04, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x4B, 0x59, 0x4E, 0x26, 0xD0, 0x5A,
				0x33, 0x0B, 0xBD, 0x5D, 0x44, 0x53, 0x19, 0xA3, 0x74, 0x8A, 0x1C, 0x90, 0x71, 0x75, 0x08, 0x41, 0x19, 0xF4, 0xBC, 0x92, 0x23, 0x04, 0x26, 0x67, 0x8D, 0xBE,
				0xE4, 0x6F, 0x74, 0x71, 0x47, 0x60, 0x60, 0x55, 0x1F, 0x72, 0x20, 0x79, 0xF2, 0x21, 0xAB, 0x91, 0xC4, 0xC9, 0x5C, 0xB4, 0x89, 0x67, 0x52, 0x10, 0x9C, 0x71,
				0x52, 0x7B, 0xD4, 0x42, 0xAE, 0x0E, 0x93, 0xA6, 0xAF, 0x8D, 0x3A, 0x61, 0x70, 0x41, 0x98, 0xC3, 0x58, 0xDC, 0xCF, 0x4C, 0xEF, 0x3E, 0xC6, 0xF3, 0xE0, 0xB4,
				0xCD, 0xFB, 0xEC, 0x81, 0x0B, 0x7A, 0x75, 0x29, 0x7A, 0xBE, 0x40, 0xF6, 0x4A, 0x3F, 0x40, 0xB7, 0x43, 0xF0, 0x45, 0x3F, 0x96, 0xF1, 0x73, 0x2F, 0x71, 0xEE,
				0xA7, 0x70, 0x4D, 0xF9, 0x63, 0xB8, 0x52, 0x4C, 0xF1, 0x18, 0xF3, 0x3C, 0x21, 0x13, 0x6A, 0x9A, 0x85, 0xB7, 0xA1, 0xFD, 0xB6, 0xA4, 0xF1, 0xEB, 0x03, 0xD6,
				0x86, 0x05, 0x6A, 0x63, 0x93, 0xB2, 0xE7, 0xF9, 0x2A, 0x77, 0x09, 0xE4, 0x0C, 0x90, 0x2D, 0x6A, 0xA2, 0xCD, 0x37, 0x0B, 0xC0, 0xB6, 0x1C, 0x96, 0xC3, 0xA7,
				0x57, 0xB1, 0x77, 0xF9, 0x55, 0x11, 0x8F, 0x44, 0x8D, 0x77, 0x31, 0xA7, 0x45, 0xE0, 0x8E, 0x42, 0x0D, 0xE4, 0x07, 0x53, 0xF3, 0x5C, 0x8B, 0xC7, 0xD7, 0xB8,
				0x64, 0x1F, 0xC0, 0xEA, 0x6B, 0xF7, 0x9C, 0x91, 0x19, 0xAD, 0x79, 0xE9, 0xDE, 0xC3, 0x45, 0x66, 0xED, 0x3E, 0x1E, 0x90, 0x40, 0x26, 0x8B, 0x01, 0x7F, 0xCE,
				0x05, 0xDA, 0x97, 0x8B, 0xF8, 0x47, 0x3F, 0x4F, 0x74, 0xF2, 0x6D, 0x1F, 0x16, 0xD3, 0x25, 0x57, 0x2D, 0x30, 0x6F, 0x3C, 0xE2, 0x41, 0x86, 0xC1, 0xC7, 0x33,
				0x01, 0x54, 0x03, 0x05, 0xA4, 0x58, 0xCC, 0x88, 0x9C, 0x8D, 0x65, 0x5E, 0x02, 0x5C, 0x22, 0xC8, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xBA, 0xF6, 0x8F, 0x2A, 0x9A, 0x4C, 0x3D, 0xD2, 0xBA, 0xD8, 0x77, 0x59,
				0x41, 0x8A, 0xED, 0x3D, 0x82, 0x24, 0x06, 0xC1, 0x37, 0x79, 0x81, 0x05, 0xFB, 0x9C, 0x6C, 0x15, 0xBE, 0x44, 0x5C, 0xB5, 0x16, 0x04, 0xC4, 0x4E, 0x9D, 0x89,
				0xEF, 0xF1, 0x15, 0x26, 0x19, 0x16, 0x3E, 0xDD, 0xAC, 0x4F, 0xE1, 0xAA, 0x44, 0x7B, 0xA0, 0xC5, 0xE9, 0x93, 0xC1, 0x34, 0x15, 0x67, 0x69, 0x2D, 0xC3, 0x83,
				0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
	}
}