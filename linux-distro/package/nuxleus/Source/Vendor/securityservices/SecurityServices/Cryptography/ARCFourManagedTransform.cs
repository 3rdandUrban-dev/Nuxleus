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
using Org.Mentalis.SecurityServices;
using Org.Mentalis.SecurityServices.Resources;

namespace Org.Mentalis.SecurityServices.Cryptography {
    /// <summary>
    /// Represents an ARCFour managed ICryptoTransform.
    /// </summary>
    internal unsafe sealed class ARCFourManagedTransform : ICryptoTransform {
        /// <summary>
        /// Initializes a new instance of the ARCFourManagedTransform class.
        /// </summary>
        /// <param name="key">The key used to initialize the ARCFour state.</param>
        public ARCFourManagedTransform(byte[] key) {
            m_Key = (byte[])key.Clone();
            m_KeyLen = key.Length;
            m_Permutation = new byte[256];
            Init();
        }
        /// <summary>
        /// Gets a value indicating whether the current transform can be reused.
        /// </summary>
        /// <value>This property returns <b>true</b>.</value>
        public bool CanReuseTransform {
            get {
                return true;
            }
        }
        /// <summary>
        /// Gets a value indicating whether multiple blocks can be transformed.
        /// </summary>
        /// <value>This property returns <b>true</b>.</value>
        public bool CanTransformMultipleBlocks {
            get {
                return true;
            }
        }
        /// <summary>
        /// Gets the input block size.
        /// </summary>
        /// <value>The size of the input data blocks in bytes.</value>
        public int InputBlockSize {
            get {
                return 1;
            }
        }
        /// <summary>
        /// Gets the output block size.
        /// </summary>
        /// <value>The size of the input data blocks in bytes.</value>
        public int OutputBlockSize {
            get {
                return 1;
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
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="inputOffset"/>, <paramref name="inputCount"/> or <paramref name="outputOffset"/> is invalid.</exception>
        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset) {
            if (m_Disposed)
                throw new ObjectDisposedException(this.GetType().FullName);
            if (inputBuffer == null)
                throw new ArgumentNullException("inputBuffer", ResourceController.GetString("Error_ParamNull"));
            if (outputBuffer == null)
                throw new ArgumentNullException("outputBuffer", ResourceController.GetString("Error_ParamNull"));
            if (inputOffset < 0 || outputOffset < 0 || inputOffset + inputCount > inputBuffer.Length || outputOffset + inputCount > outputBuffer.Length)
                throw new ArgumentOutOfRangeException(ResourceController.GetString("Error_ParamOutOfRange"));
            byte j, temp;
            int length = inputOffset + inputCount;
            fixed (byte* permutation = m_Permutation, output = outputBuffer, input = inputBuffer) {
                for (; inputOffset < length; inputOffset++, outputOffset++) {
                    // update indices
                    m_Index1 = (byte)((m_Index1 + 1) % 256);
                    m_Index2 = (byte)((m_Index2 + permutation[m_Index1]) % 256);
                    // swap m_State.permutation[m_State.index1] and m_State.permutation[m_State.index2]
                    temp = permutation[m_Index1];
                    permutation[m_Index1] = permutation[m_Index2];
                    permutation[m_Index2] = temp;
                    // transform byte
                    j = (byte)((permutation[m_Index1] + permutation[m_Index2]) % 256);
                    output[outputOffset] = (byte)(input[inputOffset] ^ permutation[j]);
                }
            }
            return inputCount;
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
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="inputOffset"/> or <paramref name="inputCount"/> is invalid.</exception>
        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount) {
            if (m_Disposed)
                throw new ObjectDisposedException(this.GetType().FullName);
            byte[] ret = new byte[inputCount];
            TransformBlock(inputBuffer, inputOffset, inputCount, ret, 0);
            Init();
            return ret;
        }
        /// <summary>
        /// This method (re)initializes the cipher.
        /// </summary>
        private void Init() {
            byte temp;
            // init state variable
            for (int i = 0; i < m_Permutation.Length; i++) {
                m_Permutation[i] = (byte)i;
            }
            m_Index1 = 0;
            m_Index2 = 0;
            // randomize, using key
            for (int j = 0, i = 0; i < m_Permutation.Length; i++) {
                j = (j + m_Permutation[i] + m_Key[i % m_KeyLen]) % 256;
                // swap m_State.permutation[i] and m_State.permutation[j]
                temp = m_Permutation[i];
                m_Permutation[i] = m_Permutation[j];
                m_Permutation[j] = temp;
            }
        }
        /// <summary>
        /// Disposes of the cryptographic parameters.
        /// </summary>
        public void Dispose() {
            if (!m_Disposed) {
                Array.Clear(m_Key, 0, m_Key.Length);
                Array.Clear(m_Permutation, 0, m_Permutation.Length);
                m_Index1 = 0;
                m_Index2 = 0;
                m_Disposed = true;
                GC.SuppressFinalize(this);
            }
        }
        /// <summary>
        /// Finalizes the object.
        /// </summary>
        ~ARCFourManagedTransform() {
            Dispose();
        }

        private byte[] m_Key;
        private int m_KeyLen;
        private byte[] m_Permutation;
        private byte m_Index1;
        private byte m_Index2;
        private bool m_Disposed;
    }
}