/*****************************************************************************************
   
   Copyright (c) Microsoft Corporation. All rights reserved.

   Use of this code sample is subject to the terms of the Microsoft
   Permissive License, a copy of which should always be distributed with
   this file.  You can also access a copy of this license agreement at:
   http://www.microsoft.com/resources/sharedsource/licensingbasics/permissivelicense.mspx

 ****************************************************************************************/

using System;

namespace Microsoft.Samples.FeedSync
{
    public class MiscHelpers
    {
        static public string XMLEncode(string i_String)
        {
            System.Text.StringBuilder StringBuilder = new System.Text.StringBuilder(i_String);

            StringBuilder = StringBuilder.Replace("&", "&amp;");
            StringBuilder = StringBuilder.Replace(">", "&gt;");
            StringBuilder = StringBuilder.Replace("<", "&lt;");
            StringBuilder = StringBuilder.Replace("\"", "&quot;");
            StringBuilder = StringBuilder.Replace("'", "&#39;");

            return StringBuilder.ToString();
        }
    }
}
