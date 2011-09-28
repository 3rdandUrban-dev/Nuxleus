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
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using Org.Mentalis.SecurityServices.Resources;
using Org.Mentalis.SecurityServices.Win32;

namespace Org.Mentalis.SecurityServices.Cryptography {
	/// <summary>
	/// Defines the basic operations of a unmanaged Rijndael cryptographic transformation.
	/// </summary>
	internal class RijndaelUnmanagedTransform : ICryptoTransform {
		/// <summary>
		/// Initializes a new instance of the RijndaelUnmanagedTransform class.
		/// </summary>
		/// <param name="algorithm">One of the <see cref="CryptoAlgorithm"/> values.</param>
		/// <param name="method">One of the <see cref="CryptoMethod"/> values.</param>
		/// <param name="key">The key to use.</param>
		/// <param name="iv">The IV to use.</param>
		/// <param name="mode">One of the <see cref="CipherMode"/> values.</param>
		/// <param name="feedback">The feedback size of the cryptographic operation in bits.</param>
		/// <param name="padding">One of the <see cref="PaddingMode"/> values.</param>
        /// <exception cref="CryptographicException">An error occurs when acquiring the cryptographic context.</exception>
        public RijndaelUnmanagedTransform(CryptoAlgorithm algorithm, CryptoMethod method, byte[] key, byte[] iv, CipherMode mode, int feedback, PaddingMode padding) {
			m_Key = new SymmetricKey(algorithm, key);
			m_Key.IV = iv;
			m_Key.Mode = mode;
			if (mode == CipherMode.CFB)
				m_Key.FeedbackSize = feedback;
			m_Key.Padding = padding;
			m_BlockSize = 128;
			m_Method = method;
		}
		/// <summary>
		/// Releases all unmanaged resources.
		/// </summary>
		~RijndaelUnmanagedTransform() {
			Dispose();
		}
		/// <summary>
		/// Releases all unmanaged resources.
		/// </summary>
		public void Dispose() {
			if (m_Key != null) {
				m_Key.Dispose();
				m_Key = null;
			}
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// Gets a value indicating whether the current transform can be reused.
		/// </summary>
		/// <value><b>true</b> if the current transform can be reused; otherwise, <b>false</b>.</value>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public bool CanReuseTransform {
			get {
				if (m_Key == null)
                    throw new ObjectDisposedException(this.GetType().FullName, ResourceController.GetString("Error_Disposed"));
				return true;
			}
		}
		/// <summary>
		/// Gets a value indicating whether multiple blocks can be transformed.
		/// </summary>
		/// <value><b>true</b> if multiple blocks can be transformed; otherwise, <b>false</b>.</value>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public bool CanTransformMultipleBlocks {
			get {
				if (m_Key == null)
                    throw new ObjectDisposedException(this.GetType().FullName, ResourceController.GetString("Error_Disposed"));
				return true;
			}
		}
		/// <summary>
		/// Gets the input block size.
		/// </summary>
		/// <value>The size of the input data blocks in bytes.</value>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public int InputBlockSize {
			get {
				if (m_Key == null)
                    throw new ObjectDisposedException(this.GetType().FullName, ResourceController.GetString("Error_Disposed"));
				return m_BlockSize / 8;
			}
		}
		/// <summary>
		/// Gets the output block size.
		/// </summary>
		/// <value>The size of the output data blocks in bytes.</value>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public int OutputBlockSize {
			get {
				if (m_Key == null)
                    throw new ObjectDisposedException(this.GetType().FullName, ResourceController.GetString("Error_Disposed"));
				return m_BlockSize / 8;
			}
		}
		/// <summary>
		/// Transforms the specified region of the input byte array and copies the resulting transform to the specified region of the output byte array.
		/// </summary>
		/// <param name="inputBuffer">The input for which to compute the transform.</param>
		/// <param name="inputOffset">The offset into the input byte array from which to begin using data.</param>
		/// <param name="inputCount">The number of bytes in the input byte array to use as data.</param>
		/// <param name="outputBuffer">The output to which to write the transform.</param>
		/// <param name="outputOffset">The offset into the output byte array from which to begin writing data.</param>
		/// <returns>The number of bytes written.</returns>
		/// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="inputBuffer"/> or <paramref name="outputBuffer"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="ArgumentOutOfRangeException">One of the specified offsets or lengths is invalid.</exception>
		/// <exception cref="CryptographicException">An error occurs while transforming the specified data.</exception>
		public unsafe int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset) {
			if (m_Key == null)
                throw new ObjectDisposedException(this.GetType().FullName, ResourceController.GetString("Error_Disposed"));
			if (inputBuffer == null)
                throw new ArgumentNullException("inputBuffer", ResourceController.GetString("Error_ParamNull"));
            if (outputBuffer == null)
                throw new ArgumentNullException("outputBuffer", ResourceController.GetString("Error_ParamNull"));
            if (inputCount < 0 || inputOffset < 0 || outputOffset < 0 || inputOffset + inputCount > inputBuffer.Length)
                throw new ArgumentOutOfRangeException(ResourceController.GetString("Error_ParamOutOfRange"));
			int length = inputCount;
			if (m_Method == CryptoMethod.Encrypt) {
				if (NativeMethods.CryptEncrypt(m_Key.Handle, IntPtr.Zero, 0, 0, null, ref length, 0) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_Encrypt"));
				if (outputBuffer.Length - outputOffset < length)
                    throw new ArgumentOutOfRangeException(ResourceController.GetString("Error_ParamOutOfRange"));
				Array.Copy(inputBuffer, inputOffset, outputBuffer, outputOffset, inputCount);
				length = inputCount;
				GCHandle h = GCHandle.Alloc(outputBuffer, GCHandleType.Pinned);
				try {
                    byte* outPointer = (byte*)h.AddrOfPinnedObject().ToPointer() + outputOffset;
                    if (NativeMethods.CryptEncrypt(m_Key.Handle, IntPtr.Zero, 0, 0, new IntPtr(outPointer), ref length, outputBuffer.Length - outputOffset) == 0)
                        throw new CryptographicException(ResourceController.GetString("Error_Encrypt"));
				} finally {
					h.Free();
				}
			} else { // decrypt
				byte[] orgCopy = new byte[inputCount];
				Array.Copy(inputBuffer, inputOffset, orgCopy, 0, inputCount);
                if (NativeMethods.CryptDecrypt(m_Key.Handle, IntPtr.Zero, 0, 0, orgCopy, ref length) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_Decrypt"));
				if (length > outputBuffer.Length - outputOffset)
                    throw new ArgumentOutOfRangeException(ResourceController.GetString("Error_ParamOutOfRange"));
				Array.Copy(orgCopy, 0, outputBuffer, outputOffset, length);
			}
			return length;
		}
		/// <summary>
		/// Transforms the specified region of the specified byte array.
		/// </summary>
		/// <param name="inputBuffer">The input for which to compute the transform.</param>
		/// <param name="inputOffset">The offset into the byte array from which to begin using data.</param>
		/// <param name="inputCount">The number of bytes in the byte array to use as data.</param>
		/// <returns>The computed transform.</returns>
		/// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="inputBuffer"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="ArgumentOutOfRangeException">The combination of offset and length is invalid.</exception>
		/// <exception cref="CryptographicException">An error occurs while transforming the specified data.</exception>
		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount ) {
			if (m_Key == null)
                throw new ObjectDisposedException(this.GetType().FullName, ResourceController.GetString("Error_Disposed"));
			if (inputBuffer == null)
                throw new ArgumentNullException("inputBuffer", ResourceController.GetString("Error_ParamNull"));
			if (inputCount < 0 || inputOffset < 0 ||inputOffset + inputCount > inputBuffer.Length)
                throw new ArgumentOutOfRangeException(ResourceController.GetString("Error_ParamOutOfRange"));
			int length = inputCount;
			byte[] ret;
			if (m_Method == CryptoMethod.Encrypt) {
                if (NativeMethods.CryptEncrypt(m_Key.Handle, IntPtr.Zero, 1, 0, null, ref length, 0) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_Encrypt"));
				ret = new byte[length];
				Array.Copy(inputBuffer, inputOffset, ret, 0, inputCount);
				length = inputCount;
                if (NativeMethods.CryptEncrypt(m_Key.Handle, IntPtr.Zero, 1, 0, ret, ref length, ret.Length) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_Encrypt"));
			} else { // decrypt
				byte[] orgCopy = new byte[inputCount];
				Array.Copy(inputBuffer, inputOffset, orgCopy, 0, inputCount);
                if (NativeMethods.CryptDecrypt(m_Key.Handle, IntPtr.Zero, 1, 0, orgCopy, ref length) == 0)
                    throw new CryptographicException(ResourceController.GetString("Error_Decrypt"));
				ret = new byte[length];
				Array.Copy(orgCopy, 0, ret, 0, length);
			}
			return ret;
		}

		private int m_BlockSize;
		private SymmetricKey m_Key;
		private CryptoMethod m_Method;
	}
}