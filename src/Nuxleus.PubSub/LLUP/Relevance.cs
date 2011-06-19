//
// Nuxleus.PubSub.LLUP.Relevance.cs: 
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
using System.Xml;
using System.Xml.Serialization;

namespace Nuxleus.PubSub.LLUP
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