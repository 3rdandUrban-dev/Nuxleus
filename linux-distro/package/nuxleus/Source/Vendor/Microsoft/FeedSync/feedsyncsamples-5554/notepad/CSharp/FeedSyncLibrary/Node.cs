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
    public abstract class Node
    {
        protected System.Xml.XmlElement m_XmlElement;

        public System.Xml.XmlElement XmlElement
        {
            get
            {
                return m_XmlElement;
            }
        }
    }
}
