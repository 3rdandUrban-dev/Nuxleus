/*
// File: LLUP.Scope.cs:
// Author:
//  M. David Peterson <m.david@3rdandUrban.com>
//
// Copyright � 2007-2011 3rd&Urban, LLC
//
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.
*/

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Nuxleus.ServiceModel.Types.Message.LLUP
{

    [XmlRootAttribute("scope", Namespace = "http://www.llup.org/blip#", IsNullable = false)]
    public class Scope
    {
        [XmlAttribute(AttributeName = "start", DataType = "dateTime", Type = typeof(DateTime))]
        public DateTime Start;

        [XmlAttribute(AttributeName = "publish", DataType = "dateTime", Type = typeof(DateTime))]
        public DateTime Publish;

        [XmlAttribute(AttributeName = "expire", DataType = "dateTime", Type = typeof(DateTime))]
        public DateTime? Expire;

        [XmlAttribute(AttributeName = "archive", DataType = "dateTime", Type = typeof(DateTime))]
        public DateTime? Archive;

        [XmlElementAttribute(Type = typeof(Relevance), ElementName = "link")]
        public List<Relevance> Relevance;
    }

}