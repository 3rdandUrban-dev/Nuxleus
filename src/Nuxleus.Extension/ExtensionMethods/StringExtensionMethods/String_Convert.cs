// 
// StaticExtensionFunctions.cs
//  
// Author:
//       M. David Peterson <m.david@3rdandUrban.com>
// 
// Copyright (c) 2011 M. David Peterson
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Nuxleus
{
	public static partial class StringExtensionMethods
	{
		static readonly Encoding m_encoding = new UTF8Encoding ();
		static readonly HashAlgorithm m_algorithm = MD5.Create ();

		public static bool IsValidUrlString (this string url)
		{
			return Regex.IsMatch(url, @"((https?|rtmp|magnet):((//)|(\\\\)|(\?))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)");
		}
		
		public static string CleanUrlString (this string url)
		{
			return Regex.Replace(Regex.Replace(url, @"/\s/", String.Empty), @"/(\#|\?).*/", String.Empty);
		}

		public static byte[] GetMD5Hash (this string str)
		{
			return m_algorithm.ComputeHash (m_encoding.GetBytes (str));
		}

		public static byte[] GetBytes (this string intString)
		{
			return m_encoding.GetBytes (intString);
		}

	}
}

