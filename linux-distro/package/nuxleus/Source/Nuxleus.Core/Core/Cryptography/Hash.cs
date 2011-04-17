//
// hash.cs: 
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 

using System;
using System.Text;
using System.Security.Cryptography;

namespace Nuxleus.Cryptography {
    public struct Hash {
        string _salt;

        public Hash ( string salt ) {
            _salt = salt;
        }

        public static string MD5 ( string salt ) {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(salt);
            byte[] hash = md5.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++) {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}