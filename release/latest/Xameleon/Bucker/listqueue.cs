//
// listqueue.cs: Client API for the bucker queue system
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
  public class ListQueue : Message {
    private IList<string> queues = new List<string>();

    public ListQueue() : base() {
      this.Op = Operation.ListQueues;
    }

    public ListQueue(IMessage m) : base(m) {}

    /// <summary>
    /// Gets the list of queue identifiers.
    /// </summary>
    public IList<string> Queues {
      get {
	return this.queues;
      }
    }

    /// <summary>
    /// Add a new queue identifier to the list of queues.
    /// </summary>
    /// <param name="qid">Queue identifier.</param>
    public void Add(string qid){
      this.queues.Add(qid);
    }

    /// <summary>
    /// Remove a queue identifier from the list of queues.
    /// </summary>
    /// <param name="qid">Queue identifier.</param>
    public void Remove(string qid){
      this.queues.Remove(qid);
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
	xw.WriteStartElement("list-queues", QS_NS);
	xw.WriteEndElement(); // </list-queues>
	xw.WriteEndElement(); // </op>

	xw.WriteStartElement("queues", QS_NS);
	foreach(string qid in this.queues){
	  xw.WriteElementString("queue", QS_NS, qid);
	}
	xw.WriteEndElement(); // </queues>

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