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
using System.Text;

namespace Xameleon.Bucker
{
  /// <summary>
  /// Possible operations carried by a message.
  /// </summary>
  public enum Operation {
    NotSet,
    ListQueues,
    NewQueue,
    DeleteQueue,
    ListMessages,
    PushMessage,
    DeleteMessage,
    GetMessage,
  }

  /// <summary>
  /// Type of message.
  /// </summary>
  public enum MessageType {
    NotSet,
    Request,
    Response,
    Error,
  }

  /// <summary>
  /// Interface definition supported by all the different message types.
  /// </summary>
  public interface IMessage {
    /// <summary>
    /// Get or sets the identifier for the request message.
    /// </summary>
    /// <value>Identifier string set by the client to track the message.</value>
    /// <remarks>Ensure this value is as unique as possible.</remarks>
    string RequestId {
      get;
      set;
    }

    /// <summary>
    /// Get or sets the identifier for the response message.
    /// </summary>
    /// <value>Identifier string set by the server to track the message response.</value>
    string ResponseId {
      get;
      set;
    }

    /// <summary>
    /// Get or sets the operation carried by this message.
    /// </summary>
    /// <value>One of the Operation enumeration's value.</value>
    Operation Op {
      get;
      set;
    }

    /// <summary>
    /// Get or sets the type of message.
    /// </summary>
    /// <value>One of the MessageType enumeration's value.</value>
    MessageType Type {
      get;
      set;
    }

    /// <summary>
    /// Get or sets the queue identifier.
    /// </summary>
    /// <value>A string denoting the identifier of a queue.</value>
    string QueueId {
      get;
      set;
    }

    /// <summary>
    /// Get or sets the message identifier.
    /// </summary>
    /// <value>A string denoting the identifier of a message.</value>
    string MessageId {
      get;
      set;
    } 

    /// <summary>
    /// Get or sets the force attribute of the operation.
    /// </summary>
    /// <value>Indicates if the operation carried 
    /// by the message should be forced on the server.
    /// </value>
    /// <remarks>Not all operations support this feature.</remarks>
    bool Force {
      get;
      set;
    }

    /// <summary>
    /// Get or sets the error code.
    /// </summary>
    /// <value>An integer denoting the error type.</value>
    /// <remarks>Because one code can be used for many distinct error types,
    /// you should correlate this with the ErrorType value.
    /// </remarks>
    int ErrorCode {
      get;
      set;
    }

    /// <summary>
    /// Get or sets the error type.
    /// </summary>
    /// <value>A string specifying the exact error type.</value>
    string ErrorType {
      get;
      set;
    }

    /// <summary>
    /// Get or sets the error message.
    /// </summary>
    /// <value>A string specifying the explanation associated 
    /// with this error for information purpose.</value>
    string ErrorMessage {
      get;
      set;
    }

    /// <summary>
    /// Serializes a message instance into an XML string.
    /// </summary>
    /// <returns>XML string representing the Message instance.</returns>
    string Serialize();
  }

  /// <summary> 
  /// Base class of all the message types.
  /// </summary>
  public class Message : IMessage
  {
    protected XmlWriterSettings settings = new XmlWriterSettings();
    protected string QS_NS = "http://purl.oclc.org/DEFUZE/qs";
    
    protected Operation op;
    protected MessageType type = MessageType.NotSet;
    protected string qid = null;
    protected string mid = null;
    protected string reqId = null;
    protected string respId = null;
    protected bool force = false;

    // only set when an error was parsed
    protected Int32 errorCode = 0;
    protected string errorType = null;
    protected string errorMsg = null;

    public Message() {
      this.SetDefaultSettings();
    }

    public Message(IMessage m) {
      this.SetDefaultSettings();
      this.Type = m.Type;
      this.Op = m.Op;
      this.RequestId = m.RequestId;
      this.ResponseId = m.ResponseId;
      this.QueueId = m.QueueId;
      this.MessageId = m.MessageId;
      this.Force = m.Force;
      this.ErrorCode = m.ErrorCode;
      this.ErrorMessage = m.ErrorMessage;
    }

    /// <summary>
    /// Get or sets settings to be used during serialization.
    /// </summary>
    /// <value>An instance of the XmlWriterSettings class.</value>
    public XmlWriterSettings Settings {
      get {
	return this.settings;
      }
      set {
	this.settings = value;
      }
    }

    /// <summary>
    /// Sets the default XmlWriterSettings instance used for the serialization.
    /// </summary>
    /// <list type="bullet">
    ///    <item>
    ///        <term>Indent</term>
    ///        <description>Indentation is disabled by default.</description>
    ///    </item>
    ///    <item>
    ///        <term>Encoding</term>
    ///        <description>Default encoding to be used: UTF-8.</description>
    ///    </item>
    ///    <item>
    ///        <term>OmitXmlDeclaration</term>
    ///        <description>The XML declaration is not included by default.</description>
    ///    </item>
    /// </list>
    public void SetDefaultSettings() {
      this.settings.Indent = false;
      this.settings.Encoding = Encoding.UTF8;
      this.settings.OmitXmlDeclaration = true;
    }

    public string RequestId {
      get {
	return this.reqId;
      }
      set {
	this.reqId = value;
      }
    }

    public string ResponseId {
      get {
	return this.respId;
      }
      set {
	this.respId = value;
      }
    }

    public Operation Op {
      get {
	return this.op;
      }
      set {
	this.op = value;
      }
    }

    public MessageType Type {
      get {
	return this.type;
      }
      set {
	this.type = value;
      }
    }

    public string QueueId {
      get {
	return this.qid;
      }
      set {
	this.qid = value;
      }
    }

    public string MessageId {
      get {
	return this.mid;
      }
      set {
	this.mid = value;
      }
    } 

    public bool Force {
      get {
	return this.force;
      }
      set {
	this.force = value;
      }
    } 

    public Int32 ErrorCode {
      get {
	return this.errorCode;
      }
      set {
	this.errorCode = value;
      }
    }

    public string ErrorType {
      get {
	return this.errorType;
      }
      set {
	this.errorType = value;
      }
    }

    public string ErrorMessage {
      get {
	return this.errorMsg;
      }
      set {
	this.errorMsg = value;
      }
    }
    
    /// <summary>
    /// Parses an XML string and maps it into a Message instance.
    /// The returned instance depends on the type of operations carried by the message.
    /// </summary>
    /// <param name="xml">Message representation as an XML string.</param>
    /// <returns>An instance of one of the message classes.</returns>
    static public IMessage Parse(string xml){
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(xml);

      XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
      string QS = "http://purl.oclc.org/DEFUZE/qs";
      nsmgr.AddNamespace("QS", QS);

      string type = doc.DocumentElement.GetAttribute("type");
      Message m = new Message();

      if(type == "request"){
	m.Type = MessageType.Request;
      } else if(type == "response"){
	m.Type = MessageType.Response;
      } else if(type == "error") {
	m.Type = MessageType.Error;
      }

      m.RequestId = doc.DocumentElement.GetAttribute("req");
      m.ResponseId = doc.DocumentElement.GetAttribute("req");

      XmlNode qs = doc.DocumentElement;

      if(m.Type != MessageType.Error) {
	XmlNode qid = qs.SelectSingleNode("./QS:qid", nsmgr);
	m.QueueId = null;
	if(qid != null){
	  m.QueueId = qid.InnerText;
	}

	XmlNode mid = qs.SelectSingleNode("./QS:mid", nsmgr);
	m.MessageId = null;
	if(mid != null){
	  m.MessageId = qid.InnerText;
	}

	XmlNode op = qs.SelectSingleNode("./QS:op", nsmgr);
	m.Op = Operation.NotSet;

	if(op != null){
	  string force = ((XmlElement)op).GetAttribute("force");
	  if((force != null) && (force == "yes")){
	    m.Force = true;
	  }

	  IEnumerator e = op.GetEnumerator();

	  XmlNode current = null;
	  while(e.MoveNext()){
	    current = (XmlNode)e.Current;
	    if(current.NamespaceURI == QS){
	      if(current.Name == "list-queues"){
		m.Op = Operation.ListQueues;
		ListQueue _m = new ListQueue(m);
		XmlNodeList queues = qs.SelectNodes("./QS:queues/QS:queue", nsmgr); 
		foreach (XmlNode queue in queues){
		  _m.Add(queue.InnerText);
		}
		m = _m;
	      } else if(current.Name == "new-queue"){
		m.Op = Operation.NewQueue;
		QueueCreation _m = new QueueCreation(m);
		m = _m;
	      } else if(current.Name == "delete-queue"){
		m.Op = Operation.DeleteQueue;
		QueueDeletion _m = new QueueDeletion(m);
		m = _m;
	      } else if(current.Name == "list-messages"){
		m.Op = Operation.ListMessages;
		ListMessages _m = new ListMessages(m);
		XmlNodeList mids = qs.SelectNodes("./QS:m/QS:mid", nsmgr); 
		foreach (XmlNode id in mids){
		  _m.Add(id.InnerText);
		}
		m = _m;
	      } else if(current.Name == "push-message"){
		m.Op = Operation.PushMessage;
		PushMessage _m = new PushMessage(m);
		XmlNode payload = qs.SelectSingleNode("./QS:payload", nsmgr);
		if(payload != null){
		  _m.Payload = payload.InnerText;
		}
		m = _m;
	      } else if(current.Name == "get-message"){
		m.Op = Operation.GetMessage;
		GetMessage _m = new GetMessage(m);
		XmlNode payload = qs.SelectSingleNode("./QS:payload", nsmgr);
		if(payload != null){
		  _m.Payload = payload.InnerText;
		}
		m = _m;
	      } else if(current.Name == "delete-message"){
		m.Op = Operation.DeleteMessage;
		DeleteMessage _m = new DeleteMessage(m);
		m = _m;
	      }
	      break;
	    }
	  }
	} 
      } else {
	XmlNode error = qs.SelectSingleNode("./QS:error", nsmgr);
	string code = ((XmlElement)error).GetAttribute("code");
	if((code != null) && (code != String.Empty)){
	  m.ErrorCode = Convert.ToInt32(code);
	}
	m.ErrorType = ((XmlElement)error).GetAttribute("type");
	m.ErrorMessage = error.InnerText;
      }

      return m;
    }

    public virtual string Serialize() {
      MemoryStream ms = new MemoryStream();
      XmlWriter xw = XmlWriter.Create(ms, this.settings);
      string xml = null;
	
      try {
	xw.WriteStartElement("qs", QS_NS);
	xw.WriteAttributeString("xmlns", null, QS_NS);

	if((this.RequestId != null) && (this.RequestId != String.Empty)){
	  xw.WriteAttributeString("req", this.RequestId);
	}

	if((this.ResponseId != null) && (this.ResponseId != String.Empty)){
	  xw.WriteAttributeString("req", this.ResponseId);
	}

	if(this.Type == MessageType.Request){
	  xw.WriteAttributeString("type", "request");
	} else if (this.Type == MessageType.Response){
	  xw.WriteAttributeString("type", "response");
	} else if (this.Type == MessageType.Error){
	  xw.WriteAttributeString("type", "error");
	  xw.WriteStartElement("error", QS_NS);
	  if(this.ErrorCode != 0){
	    xw.WriteAttributeString("code", Convert.ToString(this.ErrorCode));
	  }
	  if(this.ErrorType != null){
	    xw.WriteAttributeString("type", this.ErrorType);
	  }
	  if(this.ErrorMessage != null){
	    xw.WriteString(this.ErrorMessage);
	  }
	  xw.WriteEndElement(); // </error>
	}

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