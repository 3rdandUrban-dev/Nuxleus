#region using

using System;
using System.Xml;   
using System.Xml.XPath;
using System.Xml.Schema;
using System.IO;
using System.Collections;

using Mvp.Xml.Common.XPath;

#endregion

namespace Mvp.Xml.XPointer 
{       
    /// <summary>
    /// <c>XPointerReader</c> implements XPointer Framework in
    /// a fast, non-caching, forward-only way. <c>XPointerReader</c> 
    /// supports XPointer Framework, element(), xmlns(), xpath1() and
    /// xpointer() (XPath subset only) XPointer schemes.
    /// </summary>
    /// <remarks>See <a href="http://mvp-xml.sf.net/xpointer">XPointer.NET homepage</a> for more info.</remarks>
    /// <author>Oleg Tkachenko, oleg@tkachenko.com</author>
    public class XPointerReader : XmlReader, IHasXPathNavigator 
    {
        
        #region private members

        //Underlying reader
        private XmlReader _reader;                                
        //Nodes selected by xpointer
        private XPathNodeIterator _pointedNodes;        
        //Document cache
        private static Hashtable _cache;
        

        /// <summary>
        /// Initializes the <c>XPointerReader</c>.
        /// </summary>
        private void Init(XPathNavigator nav, string xpointer) 
        {
            Pointer pointer = XPointerParser.ParseXPointer(xpointer);
            _pointedNodes = pointer.Evaluate(nav);                
            //There is always at least one identified node
            //XPathNodeIterator is already at the first node
            _reader = new XPathNavigatorReader(_pointedNodes.Current);
        }

        private XPathDocument CreateAndCacheDocument(XmlReader r, bool supportSchemaDeterminedIDs) 
        {
            string uri = r.BaseURI;
            XmlValidatingReader vr = null;
            if (supportSchemaDeterminedIDs) 
            {
                vr = new IdAssuredValidatingReader(r);                
                vr.ValidationType = ValidationType.Auto;
            } 
            else 
            {
                vr = new XmlValidatingReader(r);                
                vr.ValidationType = ValidationType.None;
            }
            vr.EntityHandling = EntityHandling.ExpandEntities;
            vr.ValidationEventHandler += new System.Xml.Schema.ValidationEventHandler(ValidationCallback);
            XPathDocument doc = new XPathDocument(vr, XmlSpace.Preserve);     
            vr.Close();
            
            lock(_cache) 
            {
                if (!_cache.ContainsKey(uri))                
                    _cache.Add(uri, new WeakReference(doc));            
            }
            return doc;
        }
	           
        //Dummy validation even handler
        private static void ValidationCallback(object sender, ValidationEventArgs args) 
        {
           //do nothing
        }        

        #endregion        

        #region constructors

        /// <summary>
        /// Creates <c>XPointerReader</c> instnace with given <see cref="IXPathNavigable"/>
        /// and xpointer.
        /// </summary>        
        public XPointerReader(IXPathNavigable doc, string xpointer) 
            : this (doc.CreateNavigator(), xpointer) {}

        /// <summary>
        /// Creates <c>XPointerReader</c> instnace with given <see cref="XPathNavigator"/>
        /// and xpointer.
        /// </summary>        
        public XPointerReader(XPathNavigator nav, string xpointer) 
        {
            Init(nav, xpointer);
        }
        
        /// <summary>
        /// Creates <c>XPointerReader</c> instance with given uri and xpointer.
        /// </summary>	    
        public XPointerReader(string uri, string xpointer) 
            : this (new XmlTextReader(uri), xpointer, false) {}

        /// <summary>
        /// Creates <c>XPointerReader</c> instance with given uri, nametable and xpointer.
        /// </summary>	    
        public XPointerReader(string uri, XmlNameTable nt, string xpointer) 
            : this (new XmlTextReader(uri, nt), xpointer, false) {}

        /// <summary>
        /// Creates <c>XPointerReader</c> instance with given uri, nametable and xpointer.
        /// Additionally sets a flag whether to support schema-determined IDs.
        /// </summary>	    
        public XPointerReader(string uri, XmlNameTable nt, string xpointer, bool supportSchemaDeterminedIDs) 
            : this (new XmlTextReader(uri, nt), xpointer, supportSchemaDeterminedIDs) {}

        /// <summary>
        /// Creates <c>XPointerReader</c> instance with given uri, stream, nametable and xpointer.
        /// </summary>	    
        public XPointerReader(string uri, Stream stream, XmlNameTable nt, string xpointer) 
            : this (uri, stream, nt, xpointer, false) {}

        /// <summary>
        /// Creates <c>XPointerReader</c> instance with given uri, stream, nametable and xpointer.
        /// Additionally sets a flag whether to support schema-determined IDs.
        /// </summary>	    
        public XPointerReader(string uri, Stream stream, XmlNameTable nt, string xpointer, 
            bool supportSchemaDeterminedIDs) 
            : this (new XmlTextReader(uri, stream, nt), xpointer, supportSchemaDeterminedIDs) {}

        /// <summary>
        /// Creates <c>XPointerReader</c> instance with given uri, stream and xpointer.
        /// </summary>	    
        public XPointerReader(string uri, Stream stream, string xpointer) 
        : this (uri, stream, new NameTable(), xpointer) {}

        /// <summary>
        /// Creates <c>XPointerReader</c> instance with given uri, stream and xpointer.
        /// Additionally sets a flag whether to support schema-determined IDs.
        /// </summary>	    
        public XPointerReader(string uri, Stream stream, string xpointer, 
            bool supportSchemaDeterminedIDs) 
            : this (uri, stream, new NameTable(), xpointer, supportSchemaDeterminedIDs) {}

        /// <summary>
        /// Creates <c>XPointerReader</c> instance with given XmlReader and xpointer.
        /// Additionally sets a flag whether to support schema-determined IDs.
        /// </summary>	    
        public XPointerReader(XmlReader reader, string xpointer)
        : this (reader, xpointer, false) {}

        /// <summary>
        /// Creates <c>XPointerReader</c> instance with given XmlReader and xpointer.
        /// Additionally sets a flag whether to support schema-determined IDs.
        /// </summary>	    
        public XPointerReader(XmlReader reader, string xpointer, bool supportSchemaDeterminedIDs) 
        {    
            XPathDocument doc = null;
            if (_cache == null)
                _cache = new Hashtable();
            WeakReference wr = (WeakReference)_cache[reader.BaseURI];
            if (wr != null && wr.IsAlive) 
            {
                doc = (XPathDocument)wr.Target;
                reader.Close();
            }
            else 
            {
                //Not cached or GCollected                
                doc = CreateAndCacheDocument(reader, supportSchemaDeterminedIDs);
            }                            
            Init(doc.CreateNavigator(), xpointer);
        }        

        /// <summary>
        /// Creates <c>XPointerReader</c> instance with given
        /// document's URI and content.
        /// </summary>
        /// <param name="uri">XML document's base URI</param>
        /// <param name="content">XML document's content</param>
        /// <param name="xpointer">XPointer pointer</param>        
        public XPointerReader(string uri, string content, string xpointer) 
            : this(uri, content, xpointer, false) {}

        /// <summary>
        /// Creates <c>XPointerReader</c> instance with given
        /// document's URI and content.
        /// </summary>
        /// <param name="uri">XML document's base URI</param>
        /// <param name="content">XML document's content</param>
        /// <param name="xpointer">XPointer pointer</param>
        /// <param name="supportSchemaDeterminedIDs">A flag whether to 
        /// support schema-determined IDs</param>
        public XPointerReader(string uri, string content, string xpointer, bool supportSchemaDeterminedIDs) 
        {
            XPathDocument doc = null;
            if (_cache == null)
                _cache = new Hashtable();
            WeakReference wr = (WeakReference)_cache[uri];
            if (wr != null && wr.IsAlive)            
                doc = (XPathDocument)wr.Target;
            else 
            {
                //Not cached or GCollected
                XmlTextReader r = new XmlTextReader(uri, new StringReader(content));
                doc = CreateAndCacheDocument(r, supportSchemaDeterminedIDs);
            }                            
            Init(doc.CreateNavigator(), xpointer);
        }		        

        #endregion

        #region XmlReader overrides
					
	    /// <summary>See <see cref="XmlReader.AttributeCount"/>.</summary>
        public override int AttributeCount 
        {
            get { return _reader.AttributeCount; }    
        }
		
        /// <summary>See <see cref="XmlReader.BaseURI"/>.</summary>
        public override string BaseURI 
        {
            get { return _reader.BaseURI; }
        }
		
        /// <summary>See <see cref="XmlReader.HasValue"/>.</summary>
        public override bool HasValue 
        {
            get { return _reader.HasValue; }            
        }               
		
        /// <summary>See <see cref="XmlReader.IsDefault"/>.</summary>
        public override bool IsDefault 
        {
            get { return _reader.IsDefault; }             
        }
		
        /// <summary>See <see cref="XmlReader.Name"/>.</summary>
        public override string Name 
        {
            get { return _reader.Name; }            
        }
	
        /// <summary>See <see cref="XmlReader.LocalName"/>.</summary>
        public override string LocalName 
        {
            get { return _reader.LocalName; }             
        }
		
        /// <summary>See <see cref="XmlReader.NamespaceURI"/>.</summary>
        public override string NamespaceURI 
        {
            get { return _reader.NamespaceURI; }             
        }
		
        /// <summary>See <see cref="XmlReader.NameTable"/>.</summary>
        public override XmlNameTable NameTable 
        {
            get{ return _reader.NameTable; }
        }
		
        /// <summary>See <see cref="XmlReader.NodeType"/>.</summary>
        public override XmlNodeType NodeType 
        {
            get { return _reader.NodeType; }             
        }
		
        /// <summary>See <see cref="XmlReader.Prefix"/>.</summary>
        public override string Prefix 
        {
            get { return _reader.Prefix; }             
        }
		
        /// <summary>See <see cref="XmlReader.QuoteChar"/>.</summary>
        public override char QuoteChar 
        {
            get { return _reader.QuoteChar; }             
        }
		
        /// <summary>See <see cref="XmlReader.Close"/>.</summary>
        public override void Close() 
        {
            if (_reader != null)
                _reader.Close();            
        }
		
        /// <summary>See <see cref="XmlReader.Depth"/>.</summary>
        public override int Depth 
        {
            get { return _reader.Depth; }
        }
		
        /// <summary>See <see cref="XmlReader.EOF"/>.</summary>
        public override bool EOF 
        {
            get { return _reader.EOF; }
        }
		
        /// <summary>See <see cref="XmlReader.GetAttribute"/>.</summary>
        public override string GetAttribute(int i) 
        {
            return _reader.GetAttribute(i);
        } 
		
        /// <summary>See <see cref="XmlReader.GetAttribute"/>.</summary>
        public override string GetAttribute(string name) 
        {            
            return _reader.GetAttribute(name);
        }
                
        /// <summary>See <see cref="XmlReader.GetAttribute"/>.</summary>
        public override string GetAttribute(string name, string namespaceURI) 
        {        
            return _reader.GetAttribute(name, namespaceURI);
        }
		
        /// <summary>See <see cref="XmlReader.IsEmptyElement"/>.</summary>
        public override bool IsEmptyElement 
        {
            get { return _reader.IsEmptyElement; }
        }
		
        /// <summary>See <see cref="XmlReader.LookupNamespace"/>.</summary>
        public override String LookupNamespace(String prefix) 
        {
            return _reader.LookupNamespace(prefix);
        } 
		
        /// <summary>See <see cref="XmlReader.MoveToAttribute"/>.</summary>
        public override void MoveToAttribute(int i) 
        {
            _reader.MoveToAttribute(i);
        }
		
        /// <summary>See <see cref="XmlReader.MoveToAttribute"/>.</summary>
        public override bool MoveToAttribute(string name) 
        {
            return _reader.MoveToAttribute(name);
        }
        
        /// <summary>See <see cref="XmlReader.MoveToAttribute"/>.</summary>
        public override bool MoveToAttribute(string name, string ns) 
        {
            return _reader.MoveToAttribute(name, ns);
        }
		
        /// <summary>See <see cref="XmlReader.MoveToElement"/>.</summary>
        public override bool MoveToElement() 
        {
            return _reader.MoveToElement();
        }
		
        /// <summary>See <see cref="XmlReader.MoveToFirstAttribute"/>.</summary>
        public override bool MoveToFirstAttribute() 
        {
            return _reader.MoveToFirstAttribute();
        }
        
        /// <summary>See <see cref="XmlReader.MoveToNextAttribute"/>.</summary>
        public override bool MoveToNextAttribute() 
        {
            return _reader.MoveToNextAttribute();		                
        }
		
        /// <summary>See <see cref="XmlReader.ReadAttributeValue"/>.</summary>
        public override bool ReadAttributeValue() 
        {
            return _reader.ReadAttributeValue();            
        }    

		/// <summary>See <see cref="XmlReader.ReadState"/>.</summary>            
        public override ReadState ReadState 
        {
            get { return _reader.ReadState; }
        }
		
        /// <summary>See <see cref="XmlReader.this"/>.</summary>
        public override String this [int i] 
        {
            get { return _reader[i]; }
        }              	                  
		
        /// <summary>See <see cref="XmlReader.this"/>.</summary>
        public override string this [string name] 
        {
            get { return _reader[name]; }
        }
		
        /// <summary>See <see cref="XmlReader.this"/>.</summary>
        public override string this [string name, string namespaceURI] 
        {
            get { return _reader[name, namespaceURI]; }
        }

        /// <summary>See <see cref="XmlReader.ResolveEntity"/>.</summary>
        public override void ResolveEntity() 
        {
            _reader.ResolveEntity();
        }
		
        /// <summary>See <see cref="XmlReader.XmlLang"/>.</summary>
        public override string XmlLang 
        {
            get { return _reader.XmlLang; }    
        }
		
        /// <summary>See <see cref="XmlReader.XmlSpace"/>.</summary>
        public override XmlSpace XmlSpace 
        {
            get { return _reader.XmlSpace; }                
        }
		
        /// <summary>See <see cref="XmlReader.Value"/>.</summary>
        public override string Value 
        {           
            get { return _reader.Value; }            
        }
		
        /// <summary>See <see cref="XmlReader.ReadInnerXml"/>.</summary>
        public override string ReadInnerXml() 
        {
            return _reader.ReadInnerXml();
        }

        /// <summary>See <see cref="XmlReader.ReadOuterXml"/>.</summary>
        public override string ReadOuterXml() 
        {
            return _reader.ReadOuterXml();             
        }
    
        /// <summary>See <see cref="XmlReader.ReadString"/>.</summary>
        public override string ReadString() 
        {
            return _reader.ReadString();            
        }
		
        /// <summary>See <see cref="XmlReader.Read"/>.</summary>
        public override bool Read() 
        {            
            bool baseRead = _reader.Read();		    
            if (baseRead)
                return true;
            else if (_pointedNodes != null) 
            {
                if (_pointedNodes.MoveNext()) 
                {
                    _reader = new XPathNavigatorReader(_pointedNodes.Current);
                    return _reader.Read();
                }
            }		    
            return false;
        }

        #endregion

        #region IHasXPathNavigator Members
        
        /// <summary>
        /// Returns the XPathNavigator for the current context or position.
        /// </summary>
        /// <returns></returns>
        public XPathNavigator GetNavigator()
        {            
            return _pointedNodes.Current.Clone();
        }

        #endregion
    }
}
