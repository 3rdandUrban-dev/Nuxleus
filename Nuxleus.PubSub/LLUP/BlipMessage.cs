//
// Nuxleus.PubSub.LLUP.BlipMessage.cs: 
//
// Author:
//   M. David Peterson (m.david@3rdandUrban.com)
//
// Copyright (C) 2007-2011, 3rd&Urban, LLC (http://www.3rdandUrban.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Nuxleus.Atom;

namespace Nuxleus.PubSub.LLUP
{
    public enum LLUPAction
    {
        Publish,
        Update,
        Delete
    }

    [XmlRootAttribute("message", Namespace = "http://www.llup.org/blip#", IsNullable = false)]
    public class BlipMessage
    {
        private static XmlSerializerNamespaces xmlns = null;
        private static XmlWriterSettings settings = null;

        [XmlAttribute(AttributeName = "id")]
        public string Id;

        [XmlAttribute("action", DataType = "string", Type = typeof(string))]
        public LLUPAction Action;

        [XmlElement(ElementName = "reference", Type = typeof(Link))]
        public Link Reference;

        [XmlElement(ElementName = "relevance")]
        public List<Relevance> Relevance;

        [XmlElement(ElementName = "scope")]
        public Scope Scope;

        [XmlElement(Type = typeof(Entry), ElementName = "entry", Namespace = "http://www.w3.org/2005/Atom")]
        public Entry Entry;

        [XmlElement(ElementName = "point", Namespace = "http://www.georss.org/georss")]
        public string Coordinates;

        public static BlipMessage Parse(string xml)
        {
            XmlReader reader = XmlReader.Create(new StringReader(xml));
            XmlSerializer serializer = new XmlSerializer(typeof(BlipMessage));
            return (BlipMessage)serializer.Deserialize(reader);
        }

        public static BlipMessage Parse(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BlipMessage));
            return (BlipMessage)serializer.Deserialize(stream);
        }

        public static BlipMessage Parse(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            XmlSerializer serializer = new XmlSerializer(typeof(BlipMessage));
            BlipMessage m = (BlipMessage)serializer.Deserialize(stream);
            stream.Close();
            return m;
        }

        public override string ToString()
        {
            if (xmlns == null)
            {
                xmlns = new XmlSerializerNamespaces();
                xmlns.Add("llup", "http://www.llup.org/blip#");
                xmlns.Add("georss", "http://www.georss.org/georss");
                xmlns.Add(String.Empty, "http://www.w3.org/2005/Atom");
            }

            if (settings == null)
            {
                settings = new XmlWriterSettings();
                settings.Indent = false;
                settings.OmitXmlDeclaration = true;
            }

            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb, settings);
            XmlSerializer serializer = new XmlSerializer(typeof(BlipMessage));
            serializer.Serialize(writer, this, xmlns);
            return sb.ToString();
        }

        public static byte[] Serialize(BlipMessage message)
        {
            return Encoding.UTF8.GetBytes(message.ToString());
        }

        public static byte[] Serialize(BlipMessage message, Encoding encoding)
        {
            return encoding.GetBytes(message.ToString());
        }
    }
}