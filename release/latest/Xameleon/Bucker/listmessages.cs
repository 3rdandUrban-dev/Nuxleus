//
// listmessages.cs: Client API for the bucker queue system
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, Sylvain Hellegouarch
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;

namespace Xameleon.Bucker
{
  public class ListMessages : Message {
    private IList<string> mids = new List<string>();

    public ListMessages() : base() {
      this.Op = Operation.ListMessages;
    }

    public ListMessages(IMessage m) : base(m) {}

    /// <summary>
    /// Gets the list of message identifiers.
    /// </summary>
    public IList<string> Messages {
      get {
	return this.mids;
      }
    }

    /// <summary>
    /// Add a new message identifier to the list of messages.
    /// </summary>
    /// <param name="mid">Message identifier.</param>
    public void Add(string mid){
      this.mids.Add(mid);
    }

    /// <summary>
    /// Remove a message identifier from the list of messages.
    /// </summary>
    /// <param name="mid">Message identifier.</param>
    public void Remove(string mid){
      this.mids.Remove(mid);
    }

    public override string Serialize() {
      MemoryStream ms = new MemoryStream();
      XmlWriter xw = XmlWriter.Create(ms, this.settings);
      string xml = null;
	
      try {
	xw.WriteStartElement("qs", QS_NS);
	xw.WriteAttributeString("xmlns", null, QS_NS);

	if(this.Type == MessageType.Request){
	  xw.WriteAttributeString("type", "request");
	} else if (this.Type == MessageType.Response){
	  xw.WriteAttributeString("type", "response");
	}

	if((this.RequestId != null) && (this.RequestId != String.Empty)){
	  xw.WriteAttributeString("req", this.RequestId);
	}

	if((this.ResponseId != null) && (this.ResponseId != String.Empty)){
	  xw.WriteAttributeString("req", this.ResponseId);
	}

	xw.WriteStartElement("op", QS_NS);
	if(this.Force == true){
	  xw.WriteAttributeString("force", "yes");
	}
	xw.WriteStartElement("list-messages", QS_NS);
	xw.WriteEndElement(); // </list-messages>
	xw.WriteEndElement(); // </op>
	if(this.QueueId != null) {
	  xw.WriteElementString("qid", QS_NS, this.QueueId);
	}
	xw.WriteStartElement("m", QS_NS);
	foreach(string mid in this.mids){
	  xw.WriteElementString("mid", QS_NS, mid);
	}
	xw.WriteEndElement(); // </m>
	xw.WriteEndElement(); // </qs>
	xw.Flush();
      } finally {
	if(xw != null) {
	  StreamReader sr = new StreamReader(ms);
	  ms.Seek(0, SeekOrigin.Begin);
	  xml = sr.ReadToEnd();
	  ms.Close();
	  xw.Close();
	}
      }

      return xml;
    }
  }
}