//
// message.cs: Client API for the bucker queue system
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, Sylvain Hellegouarch
// 

using System;
using System.Collections;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Text;

namespace Nuxleus.Bucker
{
  public enum OperationType {
    ListQueues,
    NewQueue,
    DeleteQueue,
    ListMessages,
    PushMessage,
    DeleteMessage,
    GetMessage,
  }

  public class Operation {

    bool peek = false;
    [XmlIgnoreAttribute()]
    public bool Peek { 
      get { return peek; }
      set { peek = value; }
    }

    [XmlAttribute ("peek")]
    public string PeekString {
      get {
	if(peek == true) {
	  return "yes";
	} else {
	  return null;
	}
      }
      set {
	if(value == "yes") {
	  peek = true;
	} else {
	  peek = false;
	}
      }
    }

    bool force = false;
    [XmlIgnoreAttribute()]
    public bool Force { 
      get { return force; }
      set { force = value; }
    }

    [XmlAttribute ("force")]
    public string ForceString {
      get {
	if(force == true) {
	  return "yes";
	} else {
	  return null;
	}
      }
      set {
	if(value == "yes") {
	  force = true;
	} else {
	  force = false;
	}
      }
    }

    OperationType type;
    [XmlIgnoreAttribute()]
    public OperationType Type {
      get { return type; }
      set { type = value; }
    }

    [XmlElement ("new-queue")]
    public string NewQueue {
      get {
	if(type == OperationType.NewQueue) {
	  return string.Empty;
	}
	return null;
      }
      set {
	type = OperationType.NewQueue;
      }
    }

    
    [XmlElement ("list-queues")]
    public string ListQueues {
      get {
	if(type == OperationType.ListQueues) {
	  return string.Empty;
	}
	return null;
      }
      set {
	type = OperationType.ListQueues;
      }
    }	

    
    [XmlElement ("delete-queue")]
    public string DeleteQueue {
      get {
	if(type == OperationType.DeleteQueue) {
	  return string.Empty;
	}
	return null;
      }
      set {
	type = OperationType.DeleteQueue;
      }
    }	

    
    [XmlElement ("list-messages")]
    public string ListMessages {
      get {
	if(type == OperationType.ListMessages) {
	  return string.Empty;
	}
	return null;
      }
      set {
	type = OperationType.ListMessages;
      }
    }	

    
    [XmlElement ("push-message")]
    public string PushMessage {
      get {
	if(type == OperationType.PushMessage) {
	  return string.Empty;
	}
	return null;
      }
      set {
	type = OperationType.PushMessage;
      }
    }	

    
    [XmlElement ("get-message")]
    public string GetMessage {
      get {
	if(type == OperationType.GetMessage) {
	  return string.Empty;
	}
	return null;
      }
      set {
	type = OperationType.GetMessage;
      }
    }	

    
    [XmlElement ("delete-message")]
    public string DeleteMessage {
      get {
	if(type == OperationType.DeleteMessage) {
	  return string.Empty;
	}
	return null;
      }
      set {
	type = OperationType.DeleteMessage;
      }
    }	
  }

  public class Error {    
    [XmlAttribute ("type")]
    public string Type;

    [XmlAttribute ("code", DataType="int")]
    public int Code;
    
    [XmlText]
    public string Message;
  }

  [XmlRootAttribute("qs", Namespace="http://purl.oclc.org/DEFUZE/qs", 
		    IsNullable=false)]
  public class Message
  {
    [XmlAttribute("lang", Form=System.Xml.Schema.XmlSchemaForm.Qualified, 
		  Namespace="http://www.w3.org/XML/1998/namespace")]
    public string Lang;

    [XmlAttribute ("type")]
    public string Type;

    [XmlAttribute ("req")]
    public string RequestId;

    [XmlAttribute ("resp")]
    public string ResponseId;
    
    [XmlElement (ElementName="qid")]
    public string QueueId;

    [XmlElement (ElementName="mid")]
    public string MessageId;
    
    [XmlElement (ElementName="payload")]
    public string Payload;

    [XmlArray("m")]
    [XmlArrayItem (Type=typeof(String), ElementName="mid")]
    public string[] Messages;

    [XmlArray("queues")]
    [XmlArrayItem (Type=typeof(String), ElementName="queue")]
    public string[] Queues;

    [XmlElement (ElementName="op", Type=typeof(Operation))]
    public Operation Op = null;

    [XmlElement (ElementName="error", Type=typeof(Error))]
    public Error Error = null;
    
    [XmlIgnoreAttribute()]
    public bool IsError {
      get { return !(this.Error == null); }
    }

    public static Message Parse(string xml) {
      XmlReader reader = XmlReader.Create(new StringReader(xml));
      XmlSerializer serializer = new XmlSerializer(typeof(Message));
      return (Message)serializer.Deserialize(reader);
    }

    public static Message Parse(Stream stream) {
      XmlSerializer serializer = new XmlSerializer(typeof(Message));
      return (Message)serializer.Deserialize(stream);
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      StringWriter writer = new StringWriter(sb);
      XmlSerializer serializer = new XmlSerializer(typeof(Message));
      serializer.Serialize(writer, this);
      return sb.ToString();
    }
  }
}