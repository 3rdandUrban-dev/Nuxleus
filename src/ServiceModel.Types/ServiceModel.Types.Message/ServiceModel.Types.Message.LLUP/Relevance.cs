/*
// File: LLUP.Relevance.cs:
// Author:
//  M. David Peterson <m.david@3rdandUrban.com>
//
// Copyright � 2007-2011 3rd&Urban, LLC
//
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.
*/

using System;
using System.Xml;
using System.Xml.Serialization;

namespace Nuxleus.ServiceModel.Types.Message.LLUP
{

    [XmlRootAttribute("relevance", Namespace = "http://www.llup.org/blip#", IsNullable = false)]
    public class Relevance
    {
        [XmlAttribute("rel")]
        public string Rel;

        [XmlAttribute("href", DataType = "anyURI", Type = typeof(Uri))]
        public string Href;
    }

}