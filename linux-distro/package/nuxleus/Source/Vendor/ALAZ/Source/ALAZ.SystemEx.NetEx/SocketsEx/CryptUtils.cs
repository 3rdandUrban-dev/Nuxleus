/* ====================================================================
 * Copyright (c) 2007 Andre Luis Azevedo (az.andrel@yahoo.com.br)
 * All rights reserved.
 *                       
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 *    In addition, the source code must keep original namespace names.
 *
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in
 *    the documentation and/or other materials provided with the
 *    distribution. In addition, the binary form must keep the original 
 *    namespace names and original file name.
 * 
 * 3. The name "ALAZ" or "ALAZ Library" must not be used to endorse or promote 
 *    products derived from this software without prior written permission.
 *
 * 4. Products derived from this software may not be called "ALAZ" or
 *    "ALAZ Library" nor may "ALAZ" or "ALAZ Library" appear in their 
 *    names without prior written permission of the author.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY
 * EXPRESSED OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
 * PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR OR
 * ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE. 
 */

using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Diagnostics;

namespace ALAZ.SystemEx.NetEx.SocketsEx
{

    /// <summary>
    /// Crypt tools.
    /// </summary>
    internal static class CryptUtils
    {

        #region CreateSymmetricAlgoritm

        /// <summary>
        /// Creates an asymmetric algoritm.
        /// </summary>
        /// <param name="encryptType">
        /// Encrypt type.
        /// </param>
        public static SymmetricAlgorithm CreateSymmetricAlgoritm(EncryptType encryptType)
        {

            SymmetricAlgorithm result = null;

            switch (encryptType)
            {

                case EncryptType.etTripleDES:
                    {

                        result = new TripleDESCryptoServiceProvider();

                        result.KeySize = 192;
                        result.BlockSize = 64;

                        break;
                    }

                case EncryptType.etRijndael:
                    {

                        result = new RijndaelManaged();

                        result.KeySize = 256;
                        result.BlockSize = 256;

                        break;
                    }

            }

            if (result != null)
            {
                
                result.Mode = CipherMode.CBC;
                result.Padding = PaddingMode.ISO10126;

            }

            return result;

        }

        #endregion

        #region EncryptDataForAuthenticate

        /// <summary>
        /// Encrypts using default padding.
        /// </summary>
        /// <param name="buffer">
        /// Data to be rncrypted
        /// </param>
        public static byte[] EncryptDataForAuthenticate(SymmetricAlgorithm sa, byte[] buffer, PaddingMode padding)
        {

            byte[] result = null;

            using (MemoryStream ms = new MemoryStream())
            {

                sa.Padding = padding;

                using (CryptoStream cs = new CryptoStream(ms, sa.CreateEncryptor(), CryptoStreamMode.Write))
                {

                    sa.Padding = padding;
                    cs.Write(buffer, 0, buffer.Length);
                    cs.FlushFinalBlock();

                    result = ms.ToArray();

                }

            }

            return result;

        }

        #endregion

        #region DecryptDataForAuthenticate

        /// <summary>
        /// Encrypts using default padding.
        /// </summary>
        /// <param name="buffer">
        /// Data to be rncrypted
        /// </param>
        public static byte[] DecryptDataForAuthenticate(SymmetricAlgorithm sa, byte[] buffer, PaddingMode padding)
        {

            byte[] result = null;

            using (MemoryStream ms = new MemoryStream(buffer))
            {
                
                sa.Padding = padding;

                using (CryptoStream cs = new CryptoStream(ms, sa.CreateDecryptor(), CryptoStreamMode.Read))
                using (BinaryReader b = new BinaryReader(cs))
                {
                    ms.Position = 0;
                    sa.Padding = padding;
                    result = b.ReadBytes(8192);
                }

            }
            
            return result;

        }

        #endregion

        #region EncryptData

        /// <summary>
        /// Encrypts the data.
        /// </summary>
        /// <param name="connection">
        /// Connection information.
        /// </param>
        /// <param name="buffer">
        /// Data to be encrypted.
        /// </param>
        /// <param name="signOnly">
        /// Indicates is encrypt method only uses symmetric algoritm.
        /// </param>
        public static byte[] EncryptData(BaseSocketConnection connection, byte[] buffer)
        {

            byte[] result = null;

            if (
                 (connection.EncryptType == EncryptType.etSSL && connection.CompressionType == CompressionType.ctNone) ||
                 (connection.EncryptType == EncryptType.etNone && connection.CompressionType == CompressionType.ctNone)
                )
            {
                result = buffer;
            }
            else
            {

                using(MemoryStream ms = new MemoryStream())
                {

                    CryptoStream cs = null;
                    GZipStream gs = null;

                    switch (connection.EncryptType)
                    {

                        case EncryptType.etNone:
                        case EncryptType.etSSL:
                            {
                                break;
                            }

                        case EncryptType.etBase64:
                            {
                                cs = new CryptoStream(ms, new ToBase64Transform(), CryptoStreamMode.Write);
                                break;
                            }

                        default:
                            {
                                cs = new CryptoStream(ms, connection.Encryptor, CryptoStreamMode.Write);
                                break;
                            }
                    }

                    switch (connection.CompressionType)
                    {

                        case CompressionType.ctGZIP:
                            {

                                if (cs != null)
                                {
                                    gs = new GZipStream(cs, CompressionMode.Compress, true);
                                }
                                else
                                {
                                    gs = new GZipStream(ms, CompressionMode.Compress, true);
                                }

                                break;
                            }

                    }

                    if (gs != null)
                    {
                        gs.Write(buffer, 0, buffer.Length);
                        gs.Flush();
                        gs.Close();
                    }
                    else
                    {
                        cs.Write(buffer, 0, buffer.Length);
                    }

                    if (cs != null)
                    {
                        cs.FlushFinalBlock();
                        cs.Close();
                    }

                    result = ms.ToArray();

                }

            }

            return result;

        }

        #endregion

        #region DecryptData

        /// <summary>
        /// Decrypts the data.
        /// </summary>
        /// <param name="connection">
        /// Connection information.
        /// </param>
        /// <param name="buffer">
        /// Data to be encrypted.
        /// </param>
        /// <param name="maxBufferSize">
        /// Max buffer size accepted.
        /// </param>
        public static byte[] DecryptData(BaseSocketConnection connection, byte[] buffer, int maxBufferSize)
        {

            byte[] result = null;

            if (
                 (connection.EncryptType == EncryptType.etSSL && connection.CompressionType == CompressionType.ctNone) ||
                 (connection.EncryptType == EncryptType.etNone && connection.CompressionType == CompressionType.ctNone)
                )
            {
                result = buffer;
            }
            else
            {

                MemoryStream ms = new MemoryStream(buffer);
                CryptoStream cs = null;
                GZipStream gs = null;

                switch (connection.EncryptType)
                {

                    case EncryptType.etNone:
                    case EncryptType.etSSL:
                        {
                            break;
                        }

                    case EncryptType.etBase64:
                        {
                            cs = new CryptoStream(ms, new FromBase64Transform(), CryptoStreamMode.Read);
                            break;
                        }

                    default:
                        {
                            cs = new CryptoStream(ms, connection.Decryptor, CryptoStreamMode.Read);
                            break;
                        }
                }

                switch (connection.CompressionType)
                {

                    case CompressionType.ctGZIP:
                        {

                            if (cs != null)
                            {
                                gs = new GZipStream(cs, CompressionMode.Decompress, true);
                            }
                            else
                            {
                                gs = new GZipStream(ms, CompressionMode.Decompress, true);
                            }

                            break;
                        }

                }

                BinaryReader b = null;

                if (gs != null)
                {
                    b = new BinaryReader(gs);
                }
                else
                {
                    b = new BinaryReader(cs);
                }

                result = b.ReadBytes(maxBufferSize);

                b.Close();

            }

            return result;

        }

        #endregion

    }

}
