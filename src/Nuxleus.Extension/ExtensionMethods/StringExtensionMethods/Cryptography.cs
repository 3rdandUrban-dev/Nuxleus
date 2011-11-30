using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Nuxleus
{
    public static partial class StringExtensionMethods
    {
        static byte[] entropy = Encoding.Unicode.GetBytes ("Salt Is Not A Password");

        public static string EncryptString (this SecureString input)
        {
            byte[] encryptedData = ProtectedData.Protect
            (
                Encoding.Unicode.GetBytes (ToClearString (input)),
                entropy,
                DataProtectionScope.CurrentUser
            );

            return Convert.ToBase64String (encryptedData);
        }

        public static SecureString DecryptString (this string encryptedString)
        {
            try {
                byte[] decryptedString = ProtectedData.Unprotect
                (
                    Convert.FromBase64String (encryptedString),
                    entropy,
                    DataProtectionScope.CurrentUser
                );
                return ToSecureString (Encoding.Unicode.GetString (decryptedString));
            } catch {
                return new SecureString ();
            }
        }

        public static SecureString ToSecureString (this string input)
        {
            SecureString secure = new SecureString ();
            foreach (char c in input) {
                secure.AppendChar (c);
            }
            secure.MakeReadOnly ();
            return secure;
        }

        public static string ToClearString (this SecureString input)
        {
            string returnValue = string.Empty;
            IntPtr ptr = Marshal.SecureStringToBSTR (input);
            try {
                returnValue = Marshal.PtrToStringBSTR (ptr);
            } finally {
                Marshal.ZeroFreeBSTR (ptr);
            }
            return returnValue;
        }
    }
}

