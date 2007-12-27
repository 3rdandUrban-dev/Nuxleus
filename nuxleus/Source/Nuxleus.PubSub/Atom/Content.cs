//
// content.cs: 
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, Sylvain Hellegouarch
// 

using System;
using System.Xml;
using System.Xml.Serialization;

namespace Nuxleus.Atom
{
    public class Content : TextConstruct
    {      
      [XmlAttribute ("src")]
	public string Src;
    }
}