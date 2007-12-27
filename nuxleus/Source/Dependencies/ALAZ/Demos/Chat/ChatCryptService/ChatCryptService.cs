using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

using ALAZ.SystemEx;
using ALAZ.SystemEx.NetEx.SocketsEx;

namespace ChatCryptService
{

    public class ChatCryptService : BaseCryptoService
    {

        #region Methods

        #region OnSymmetricAuthenticate

        public override void OnSymmetricAuthenticate(ISocketConnection connection, out RSACryptoServiceProvider serverKey, out byte[] signMessage)
        {

            /*
             * A RSACryptoServiceProvider is needed to encrypt and send session key.
             * In server side you need public and private key to decrypt session key.
             * In client side tou need only public key to encrypt session key.
             * 
             * You can create a RSACryptoServiceProvider from a string (file, registry), a CspParameters or a certificate.
             * The following certificate and instructions is in MakeCert folder.
             * 
            */

            //----- Sign Message!
            signMessage = new byte[]
            {
                0x1E, 0xF0, 0x5F, 0x4D, 0x2B, 0x33, 0xCE, 0xB0, 
                0x12, 0x29, 0x31, 0xCA, 0xEF, 0xAB, 0xEC, 0x97, 
                0xB3, 0x73, 0x2E, 0xDD, 0x2D, 0x58, 0xAC, 0xE9, 
                0xE0, 0xCC, 0x14, 0xDC, 0x14, 0xEF, 0x97, 0x64,
                0x38, 0xC6, 0x1C, 0xD8, 0x87, 0xFC, 0x30, 0xD5,
                0x79, 0xE4, 0x10, 0x2C, 0xFE, 0x98, 0x30, 0x2C, 
                0xFF, 0xAE, 0x51, 0xD5, 0x47, 0x1D, 0x4D, 0xC5, 
                0x43, 0x75, 0x6C, 0x5E, 0x32, 0xF2, 0x9C, 0x22,
                0x51, 0xBE, 0xA2, 0xC5, 0x31, 0x19, 0xAE, 0x21, 
                0x3D, 0x9A, 0xF2, 0x78, 0x90, 0x19, 0xCF, 0x97, 
                0xA5, 0x75, 0x99, 0xB3, 0xFD, 0x31, 0xE6, 0xB5, 
                0x7F, 0xFD, 0xD0, 0x37, 0x26, 0xC2, 0x7B, 0x27, 
                0x18, 0x43, 0xED, 0xD9, 0xC8, 0x5A, 0xF5, 0xE0, 
                0xDA, 0x33, 0x41, 0x3A, 0xC8, 0xE7, 0x4A, 0x5C, 
                0x9D, 0x48, 0x95, 0x22, 0x56, 0x2F, 0x62, 0x20, 
                0xD8, 0xEC, 0x46, 0x52, 0x49, 0x76, 0xFB, 0x7B

            };

           
            //----- Using Certificate Store!
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            
            X509Certificate2 certificate = store.Certificates.Find(X509FindType.FindBySubjectName, "ALAZ Library", true)[0];

            serverKey = new RSACryptoServiceProvider();
            serverKey.FromXmlString(certificate.PrivateKey.ToXmlString(true));
            
            store.Close();

        }

        #endregion

        #endregion

    }

}
