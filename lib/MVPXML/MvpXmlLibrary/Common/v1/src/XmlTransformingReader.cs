#region using

using System;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Security.Policy;

#endregion

namespace Mvp.Xml.Common
{
  /// <summary>
  /// XmlReader, transforms input XML stream according to
  /// an XSLT stylesheet.
  /// </summary>
  /// <remarks>Author: (c) Oleg Tkachenko, oleg@tkachenko.com
  /// See: http://www.tkachenko.com/blog/archives/000112.html
  /// </remarks>
  public class XmlTransformingReader : XmlReader 
  {
    #region private members
    
    private XmlReader _outReader;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates XmlTransformingReader with given XmlReader, XslTransform, 
    /// XsltArgumentList and Xmlresolver.
    /// </summary>
    /// <param name="parentReader">Source XML as XmlReader</param>
    /// <param name="transform">XslTransform to transform source</param>
    /// <param name="args">Arguments to the transformation</param>
    /// <param name="resolver">XmlResolver to resolve URIs in document() function</param>
    public XmlTransformingReader(XmlReader parentReader, 
      XslTransform transform, XsltArgumentList args, XmlResolver resolver) 
    {			
      XPathDocument doc = new XPathDocument(parentReader);
      _outReader = transform.Transform(doc, args, resolver); 
    }
		
    /// <summary>
    /// Creates XmlTransformingReader with given XmlReader, XslTransform and 
    /// XsltArgumentList.
    /// </summary>
    /// <param name="parentReader">Source XML as XmlReader</param>
    /// <param name="transform">XslTransform to transform source</param>
    /// <param name="args">Arguments to the transformation</param>
    public XmlTransformingReader(XmlReader parentReader, 
      XslTransform transform, XsltArgumentList args) 
    {			
      XPathDocument doc = new XPathDocument(parentReader);
      _outReader = transform.Transform(doc, args, new XmlUrlResolver()); 
    }

    /// <summary>
    /// Creates XmlTransformingReader with given XmlReader and XslTransform.    
    /// </summary>
    /// <param name="parentReader">Source XML as XmlReader</param>
    /// <param name="transform">XslTransform to transform source</param>
    public XmlTransformingReader(XmlReader parentReader, XslTransform transform) 
    {			
      XPathDocument doc = new XPathDocument(parentReader);
      _outReader = transform.Transform(doc, null, new XmlUrlResolver()); 
    }

    /// <summary>
    /// Creates XmlTransformingReader with given XmlReader, stylesheet URI, 
    /// XsltArgumentList and Xmlresolver.
    /// </summary>
    /// <param name="parentReader">Source XML as XmlReader</param>
    /// <param name="transformSource">URI of the stylesheet to transform the source</param>
    /// <param name="args">Arguments to the transformation</param>
    /// <param name="resolver">XmlResolver to resolve URIs in document() function</param>
    public XmlTransformingReader(XmlReader parentReader, 
      string transformSource, XsltArgumentList args, XmlResolver resolver) 
    {			
      XPathDocument doc = new XPathDocument(parentReader);
      XslTransform xslt = new XslTransform();
      xslt.Load(transformSource);
      _outReader = xslt.Transform(doc, args, resolver);
    }
		
    /// <summary>
    /// Creates XmlTransformingReader with given XmlReader and stylesheet URI.    
    /// </summary>
    /// <param name="parentReader">Source XML as XmlReader</param>
    /// <param name="transformSource">URI of the stylesheet to transform the source</param>    
    public XmlTransformingReader(XmlReader parentReader, string transformSource) 
    {			
      XPathDocument doc = new XPathDocument(parentReader);
      XslTransform xslt = new XslTransform();
      xslt.Load(transformSource);
      _outReader = xslt.Transform(doc, null, new XmlUrlResolver());
    }

    /// <summary>
    /// Creates XmlTransformingReader with given source and stylesheet URIs.    
    /// </summary>
    /// <param name="source">URI of the source XML</param>
    /// <param name="transformSource">URI of the stylesheet to transform the source</param>    
    public XmlTransformingReader(string source, string transformSource) 
    {			
      XPathDocument doc = new XPathDocument(source);
      XslTransform xslt = new XslTransform();
      xslt.Load(transformSource);
      _outReader = xslt.Transform(doc, null, new XmlUrlResolver());
    }	

    #endregion			

    #region XmlReader impl methods
		
    /// <summary>See <see cref="XmlReader.AttributeCount"/></summary>
    public override int AttributeCount 
    {
      get { return _outReader.AttributeCount;}    
    }
		
    /// <summary>See <see cref="XmlReader.BaseURI"/></summary>
    public override string BaseURI 
    {
      get { return _outReader.BaseURI; }
    }
		
    /// <summary>See <see cref="XmlReader.HasValue"/></summary>
    public override bool HasValue 
    {
      get { return _outReader.HasValue; }			
    }               
		
    /// <summary>See <see cref="XmlReader.IsDefault"/></summary>
    public override bool IsDefault 
    {
      get { return _outReader.IsDefault; } 			
    }
		
    /// <summary>See <see cref="XmlReader.Name"/></summary>
    public override string Name 
    {
      get { return _outReader.Name; }                
    }
	
    /// <summary>See <see cref="XmlReader.LocalName"/></summary>
    public override string LocalName 
    {
      get { return _outReader.LocalName; } 			
    }
		
    /// <summary>See <see cref="XmlReader.NamespaceURI"/></summary>
    public override string NamespaceURI 
    {
      get { return _outReader.NamespaceURI; } 		
    }
		
    /// <summary>See <see cref="XmlReader.NameTable"/></summary>
    public override XmlNameTable NameTable 
    {
      get{ return _outReader.NameTable; }
    }
		
    /// <summary>See <see cref="XmlReader.NodeType"/></summary>
    public override XmlNodeType NodeType 
    {
      get { return _outReader.NodeType; } 			
    }
		
    /// <summary>See <see cref="XmlReader.Prefix"/></summary>
    public override string Prefix 
    {
      get { return _outReader.Prefix; } 			
    }
		
    /// <summary>See <see cref="XmlReader.QuoteChar"/></summary>
    public override char QuoteChar 
    {
      get { return _outReader.QuoteChar; } 			
    }
		
    /// <summary>See <see cref="XmlReader.Close"/></summary>
    public override void Close() 
    {
      _outReader.Close();			
    }
		
    /// <summary>See <see cref="XmlReader.Depth"/></summary>
    public override int Depth 
    {
      get { return _outReader.Depth; }
    }
		
    /// <summary>See <see cref="XmlReader.EOF"/></summary>
    public override bool EOF 
    {
      get { return _outReader.EOF; }
    }
		
    /// <summary>See <see cref="XmlReader.GetAttribute"/></summary>
    public override string GetAttribute(int i) 
    {
      return _outReader.GetAttribute(i);
    } 
		
    /// <summary>See <see cref="XmlReader.GetAttribute"/></summary>
    public override string GetAttribute(string name) 
    {			
      return _outReader.GetAttribute(name);
    }
                
    /// <summary>See <see cref="XmlReader.GetAttribute"/></summary>
    public override string GetAttribute(string name, string namespaceURI) 
    {        			
      return _outReader.GetAttribute(name, namespaceURI);
    }
		
    /// <summary>See <see cref="XmlReader.IsEmptyElement"/></summary>
    public override bool IsEmptyElement 
    {
      get { return _outReader.IsEmptyElement; }
    }
		
    /// <summary>See <see cref="XmlReader.LookupNamespace"/></summary>
    public override String LookupNamespace(String prefix) 
    {
      return _outReader.LookupNamespace(prefix);
    } 
		
    /// <summary>See <see cref="XmlReader.MoveToAttribute"/></summary>
    public override void MoveToAttribute(int i) 
    {
      _outReader.MoveToAttribute(i);
    }
		
    /// <summary>See <see cref="XmlReader.MoveToAttribute"/></summary>
    public override bool MoveToAttribute(string name) 
    {			
      return _outReader.MoveToAttribute(name);
    }
        
    /// <summary>See <see cref="XmlReader.MoveToAttribute"/></summary>
    public override bool MoveToAttribute(string name, string ns) 
    {			
      return _outReader.MoveToAttribute(name, ns);
    }
		
    /// <summary>See <see cref="XmlReader.MoveToElement"/></summary>
    public override bool MoveToElement() 
    {
      return _outReader.MoveToElement();
    }
		
    /// <summary>See <see cref="XmlReader.MoveToFirstAttribute"/></summary>
    public override bool MoveToFirstAttribute() 
    {			
      return _outReader.MoveToFirstAttribute();
    }
        
    /// <summary>See <see cref="XmlReader.MoveToNextAttribute"/></summary>
    public override bool MoveToNextAttribute() 
    {
      return _outReader.MoveToNextAttribute();
    }
		
    /// <summary>See <see cref="XmlReader.ReadAttributeValue"/></summary>
    public override bool ReadAttributeValue() 
    {			                
      return _outReader.ReadAttributeValue();    			
    }    
		            
    /// <summary>See <see cref="XmlReader.ReadState"/></summary>
    public override ReadState ReadState 
    {
      get { return _outReader.ReadState; }
    }
		
    /// <summary>See <see cref="XmlReader.this"/></summary>
    public override String this [int i] 
    {
      get { return GetAttribute(i); }
    }              	                  
		
    /// <summary>See <see cref="XmlReader.this"/></summary>
    public override string this [string name] 
    {
      get { return GetAttribute(name); }
    }
		
    /// <summary>See <see cref="XmlReader.this"/></summary>
    public override string this [string name, string namespaceURI] 
    {
      get { return GetAttribute(name, namespaceURI); }
    }

    /// <summary>See <see cref="XmlReader.ResolveEntity"/></summary>
    public override void ResolveEntity() 
    {
      _outReader.ResolveEntity();
    }
		
    /// <summary>See <see cref="XmlReader.XmlLang"/></summary>
    public override string XmlLang 
    {
      get { return _outReader.XmlLang; }    
    }
		
    /// <summary>See <see cref="XmlReader.XmlSpace"/></summary>
    public override XmlSpace XmlSpace 
    {
      get { return _outReader.XmlSpace; }                
    }
		
    /// <summary>See <see cref="XmlReader.Value"/></summary>
    public override string Value 
    {           
      get { return _outReader.Value; }			
    }
		
    /// <summary>See <see cref="XmlReader.ReadInnerXml"/></summary>
    public override string ReadInnerXml() 
    {
      return _outReader.ReadInnerXml();			
    }

    /// <summary>See <see cref="XmlReader.ReadOuterXml"/></summary>
    public override string ReadOuterXml() 
    {
      return _outReader.ReadOuterXml(); 			
    }
    
    /// <summary>See <see cref="XmlReader.ReadString"/></summary>
    public override string ReadString() 
    {
      return _outReader.ReadString();			
    }                   
				
    /// <summary>See <see cref="XmlReader.Read"/></summary>
    public override bool Read() 
    {
      return _outReader.Read();			
    }
    #endregion			
  }
}
