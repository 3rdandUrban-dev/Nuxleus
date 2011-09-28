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
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Org.Mentalis.SecurityServices.Resources;

namespace Org.Mentalis.SecurityServices.Cryptography {
    /// <summary>
    /// Defines a number of easy-to-use methods to perform string-based encryption.
    /// </summary>
    public sealed class StringEncryption {
        /// <summary>
        /// Initializes a new StringEncryption instance.
        /// </summary>
        /// <remarks>The default bulk cipher algorithm is Rijndael and the default hash algorithm is RIPEMD-160.</remarks>
        public StringEncryption() {
            Init(Rijndael.Create(), RIPEMD160.Create());
        }
        /// <summary>
        /// Initializes a new StringEncryption instance.
        /// </summary>
        /// <param name="bulkCipher">The name of the bulk cipher algorithm to use.</param>
        /// <param name="hash">The name of the hash algorithm to use.</param>
        public StringEncryption(string bulkCipher, string hash) {
            if (bulkCipher == null || bulkCipher.Length == 0)
                bulkCipher = "Rijndael";
            if (hash == null || hash.Length == 0)
                hash = "RIPEMD160";
            Init(SymmetricAlgorithm.Create(bulkCipher), HashAlgorithm.Create(hash));
        }
        /// <summary>
        /// Initializes a new StringEncryption instance.
        /// </summary>
        /// <param name="bulkCipher">The bulk cipher algorithm to use.</param>
        /// <param name="hash">The hash algorithm to use.</param>
        /// <exception cref="ArgumentNullException">One of the parameters is a null reference.</exception>
        public StringEncryption(SymmetricAlgorithm bulkCipher, HashAlgorithm hash) {
            if (bulkCipher == null)
                throw new ArgumentNullException("bulkCipher", ResourceController.GetString("Error_ParamNull"));
            if (hash == null)
                throw new ArgumentNullException("hash", ResourceController.GetString("Error_ParamNull"));
            Init(bulkCipher, hash);
        }

        private void Init(SymmetricAlgorithm bulkCipher, HashAlgorithm hash) { 
            m_BulkCipher = bulkCipher;
            m_Hash = hash;
            m_Key = m_BulkCipher.Key;
            m_IV = m_BulkCipher.IV;
        }

        /// <summary>
        /// Encrypts a given byte array.
        /// </summary>
        /// <param name="input">The array of bytes to encrypt.</param>
        /// <returns>A string representation of the encrypted data.</returns>
        /// <exception cref="ArgumentNullException"><i>input</i> is a null reference.</exception>
        public string Encrypt(byte[] input) {
            if (input == null)
                throw new ArgumentNullException("input", ResourceController.GetString("Error_ParamNull"));
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, m_BulkCipher.CreateEncryptor(m_Key, m_IV), CryptoStreamMode.Write);
            cs.Write(input, 0, input.Length);
            cs.Write(m_Hash.ComputeHash(input, 0, input.Length), 0, m_Hash.HashSize / 8);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }
        /// <summary>
        /// Encrypts a given string.
        /// </summary>
        /// <param name="input">The string to encrypt.</param>
        /// <returns>A string representation of the encrypted data.</returns>
        /// <remarks>The default encoding to convert the input string to an array of bytes is UTF-8.</remarks>
        /// <exception cref="ArgumentNullException"><i>input</i> is a null reference.</exception>
        public string Encrypt(string input) {
            return Encrypt(input, Encoding.UTF8);
        }
        /// <summary>
        /// Encrypts a given string.
        /// </summary>
        /// <param name="input">The string to encrypt.</param>
        /// <param name="encoding">The encoding to use to convert the string to an array of bytes.</param>
        /// <returns>A string representation of the encrypted data.</returns>
        /// <exception cref="ArgumentNullException"><i>input</i> or <i>encoding</i> is a null reference.</exception>
        public string Encrypt(string input, Encoding encoding) {
            if (encoding == null)
                throw new ArgumentNullException("encoding", ResourceController.GetString("Error_ParamNull"));
            if (input == null)
                throw new ArgumentNullException("input", ResourceController.GetString("Error_ParamNull"));
            return Encrypt(encoding.GetBytes(input));
        }
        /// <summary>
        /// Decrypts a given string.
        /// </summary>
        /// <param name="input">The string to decrypt.</param>
        /// <returns>An array of bytes, containing the unencrypted data.</returns>
        /// <exception cref="ArgumentNullException"><i>input</i> is a null reference.</exception>
        /// <exception cref="FormatException"><i>input</i> is an invalid Base64 string.</exception>
        /// <exception cref="ArgumentException">The length of <i>input</i> is invalid.</exception>
        /// <exception cref="CryptographicException">An error occurs during the decryption or integrity verification.</exception>
        public byte[] Decrypt(string input) {
            if (input == null)
                throw new ArgumentNullException("input", ResourceController.GetString("Error_ParamNull"));
            byte[] buffer = Convert.FromBase64String(input); // throws FormatException
            buffer = m_BulkCipher.CreateDecryptor(m_Key, m_IV).TransformFinalBlock(buffer, 0, buffer.Length); // throws CryptographicException
            if (buffer.Length < m_Hash.HashSize / 8)
                throw new ArgumentException(ResourceController.GetString("Error_ParamInvalid"), "input");
            byte[] hash = m_Hash.ComputeHash(buffer, 0, buffer.Length - m_Hash.HashSize / 8);
            int offset = buffer.Length - m_Hash.HashSize / 8;
            for (int i = 0; i < hash.Length; i++) {
                if (hash[i] != buffer[offset + i])
                    throw new CryptographicException(ResourceController.GetString("Error_InvalidHash"));
            }
            byte[] ret = new byte[buffer.Length - m_Hash.HashSize / 8];
            Buffer.BlockCopy(buffer, 0, ret, 0, ret.Length);
            return ret;
        }
        /// <summary>
        /// Decrypts a given string.
        /// </summary>
        /// <param name="input">The string to decrypt.</param>
        /// <param name="encoding">The encoding to use to convert the string to an array of bytes.</param>
        /// <returns>A string containing the unencrypted data.</returns>
        /// <exception cref="ArgumentNullException"><i>input</i> or <i>encoding</i> is a null reference.</exception>
        /// <exception cref="FormatException"><i>input</i> is an invalid Base64 string.</exception>
        /// <exception cref="ArgumentException">The length of <i>input</i> is invalid.</exception>
        /// <exception cref="CryptographicException">An error occurs during the decryption or integrity verification.</exception>
        public string DecryptString(string input, Encoding encoding) {
            if (encoding == null)
                throw new ArgumentNullException("encoding", ResourceController.GetString("Error_ParamNull"));
            if (input == null)
                throw new ArgumentNullException("input", ResourceController.GetString("Error_ParamNull"));
            return encoding.GetString(Decrypt(input));
        }
        /// <summary>
        /// Decrypts a given string.
        /// </summary>
        /// <param name="input">The string to decrypt.</param>
        /// <returns>A string containing the unencrypted data.</returns>
        /// <remarks>The default encoding to convert the input string to an array of bytes is UTF-8.</remarks>
        /// <exception cref="ArgumentNullException"><i>input</i> is a null reference.</exception>
        /// <exception cref="FormatException"><i>input</i> is an invalid Base64 string.</exception>
        /// <exception cref="ArgumentException">The length of <i>input</i> is invalid.</exception>
        /// <exception cref="CryptographicException">An error occurs during the decryption or integrity verification.</exception>
        public string DecryptString(string input) {
            return DecryptString(input, Encoding.UTF8);
        }
        /// <summary>
        /// Gets or sets the key of the bulk cipher algorithm.
        /// </summary>
        /// <value>An array of bytes that contains the key of the bulk cipher algorithm.</value>
        public byte[] Key {
            get {
                return m_Key;
            }
            set {
                if (value == null)
                    throw new ArgumentNullException("value", ResourceController.GetString("Error_ParamNull"));
                if (!m_BulkCipher.ValidKeySize(value.Length * 8))
                    throw new CryptographicException(ResourceController.GetString("Error_InvalidKeySize"));
                m_Key = (byte[])value.Clone();
            }
        }
        /// <summary>
        /// Gets or sets the initialization vector of the bulk cipher algorithm.
        /// </summary>
        /// <value>An array of bytes that contains the initialization vector of the bulk cipher algorithm.</value>
        public byte[] IV {
            get {
                return m_IV;
            }
            set {
                if (value == null)
                    throw new ArgumentNullException("value", ResourceController.GetString("Error_ParamNull"));
                if (value.Length != m_BulkCipher.BlockSize / 8)
                    throw new CryptographicException(ResourceController.GetString("Error_InvalidIVSize"));
                m_IV = (byte[])value.Clone();
            }
        }

        private SymmetricAlgorithm m_BulkCipher;
        private HashAlgorithm m_Hash;
        private byte[] m_Key;
        private byte[] m_IV;
    }
}