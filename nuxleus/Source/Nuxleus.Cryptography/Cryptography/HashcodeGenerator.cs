using System;
using System.Security.Cryptography;
using System.Text;

namespace Nuxleus.Cryptography {

    public enum HashAlgorithm { MD5, SHA1, SHA256 };

    public struct HashcodeGenerator {
        static Encoding encoder = new UTF8Encoding();

        static HashAlgorithm _defaultAlgorithm = HashAlgorithm.MD5;
        static string _defaultFormat = "x2";
        static bool _defaultReturnBase64 = true;
        static string _defaultKey = Guid.NewGuid().ToString();

        string _key;
        HashAlgorithm _algorithm;
        bool _returnBase64;
        string _format;
        object[] _hashArray;

        public HashcodeGenerator (params object[] hashArray)
            : this(_defaultKey, hashArray) {
        }
        public HashcodeGenerator (String key, params object[] hashArray)
            : this(key, _defaultAlgorithm, hashArray) {
        }
        public HashcodeGenerator (String key, HashAlgorithm algorithm, params object[] hashArray)
            : this(key, algorithm, _defaultFormat, hashArray) {
        }
        public HashcodeGenerator (String key, HashAlgorithm algorithm, String format, params object[] hashArray)
            : this(key, algorithm, format, Guid.NewGuid(), true, hashArray) {
        }
        public HashcodeGenerator (String key, HashAlgorithm algorithm, String format, Guid guid, params object[] hashArray)
            : this(key, algorithm, format, guid, _defaultReturnBase64, hashArray) {
        }
        public HashcodeGenerator (String key, HashAlgorithm algorithm, String format, Guid guid, bool returnBase64, params object[] hashArray) {
            _returnBase64 = returnBase64;
            _hashArray = hashArray;
            _algorithm = algorithm;
            _format = format;
            _key = FormatKey(key, _format);
        }

        public bool ReturnBase64 { get { return _returnBase64; } set { this._returnBase64 = value; } }
        public object[] HashArray { get { return _hashArray; } set { this._hashArray = value; } }
        public HashAlgorithm HashAlgorithm { get { return _algorithm; } set { this._algorithm = value; } }
        public String Format { get { return _format; } set { this._format = value; } }
        public String Key { get { return _key; } set { this._key = FormatKey(value, _format); } }

        public static String FormatKey (string key, string format) {
            StringBuilder builder = new StringBuilder();
            byte[] bytes = encoder.GetBytes(key);
            for (int i = 0; i < bytes.Length; i++) {
                builder.Append(bytes[i].ToString(format));
            }
            return builder.ToString();
        }

        public String GetHMACHashString () {
            return getHMACHashcode(_key, _algorithm, _returnBase64, _hashArray);
        }

        public String GetHMACHashString (params object[] hashArray) {
            return getHMACHashcode(_key, _algorithm, _returnBase64, _hashArray, hashArray);
        }

        public String GetHMACHashString (string key, params object[] hashArray) {
            return getHMACHashcode(key, _algorithm, _returnBase64, _hashArray, hashArray);
        }

        public String GetHMACHashString (HashAlgorithm algorithm, params object[] hashArray) {
            return getHMACHashcode(_key, algorithm, _returnBase64, _hashArray, hashArray);
        }

        public static int GetHMACBase64Hashcode(string key, HashAlgorithm algorithm, params object[] hashArray) {
            return getHMACHashcode(key, algorithm, true, hashArray).GetHashCode();
        }

        public static String GetHMACHashBase64String (string key, HashAlgorithm algorithm, params object[] hashArray) {
            return getHMACHashcode(key, algorithm, true, hashArray);
        }

        public static String GetHMACHashString (string key, HashAlgorithm algorithm, bool useBase64, params object[] hashArray) {
            return getHMACHashcode(key, algorithm, useBase64, hashArray);
        }

        public static int GetHMACHashcode (string key, HashAlgorithm algorithm, bool useBase64, params object[] hashArray) {
            return getHMACHashcode(key, algorithm, useBase64, hashArray).GetHashCode();
        }

        private static String getHMACHashcode (string key, HashAlgorithm algorithm, bool useBase64, params object[] hashArray) {
            StringBuilder builder = new StringBuilder();
            HMAC hmacProvider;
            switch (algorithm) {
                case HashAlgorithm.SHA1:
                    hmacProvider = new HMACSHA1(encoder.GetBytes(key));
                    break;
                case HashAlgorithm.SHA256:
                    hmacProvider = new HMACSHA256(encoder.GetBytes(key));
                    break;
                case HashAlgorithm.MD5:
                default:
                    hmacProvider = new HMACMD5(encoder.GetBytes(key));
                    break;
            }
            foreach (object obj in hashArray) {
                builder.Append(obj);
            }

            byte[] computeHash = hmacProvider.ComputeHash(encoder.GetBytes(builder.ToString()));

            if (useBase64) {
                return Convert.ToBase64String(computeHash);
            } else {
                return computeHash.ToString();
            }
        }
    }
}
