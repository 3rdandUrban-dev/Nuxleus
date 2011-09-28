#region using

using System;
using System.Xml;
using System.Xml.XPath;

#endregion

namespace Mvp.Xml.Common.XPath
{
	/// <summary>
	/// XPathNavigator over an XmlNode.
	/// Allows to navigate over a subtree of XmlDocument by represening the 
	/// subtree as valid instance of XPath data model.
	/// Allowable types of nodes to navigate over are: 
	/// Element, Text, CDATA, Comment, PI, Whitespace.    
	/// Typical usage - to transform a subtree of an XmlDocument.  
	/// </summary>
	/// <remarks>Author: Oleg Tkachenko, oleg@tkachenko.com
	/// <para>See http://www.tkachenko.com/blog/archives/000117.html</para>
	/// </remarks>
	public class XmlNodeNavigator : XPathNavigator, IHasXmlNode 
	{
		private NavigatorState _state;

		#region Constructors
		/// <summary>
		/// Creates XmlNodeNavigator over given XmlNode.
		/// </summary>
		/// <param name="node">Root node for the navigator.</param>
		public XmlNodeNavigator(XmlNode node) 
		{
			if (node == null)
				throw new NullReferenceException("Node to navigate over cannot be null");                          
			switch (node.NodeType) 
			{
				case XmlNodeType.Element:
				case XmlNodeType.Comment:
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.CDATA:
				case XmlNodeType.Text:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					_state = new NavigatorState(node);
					break;
				default:
					throw new NotSupportedException("Given node to navigate over must be Element, Comment, PI or text node.");
			}		    		    
		}
		
		internal XmlNodeNavigator(NavigatorState state) 
		{
			_state = new NavigatorState(state);
		}
		#endregion
				
		#region Private Members

		private bool IsAtDocumentNode() 
		{
			return ((IHasXmlNode)_state.Navigator).GetNode()==_state.DocumentNode;
		}
        
		internal NavigatorState State 
		{
			get { return _state; }
		}        

		#endregion Private Members

		#region XPathNavigator Overrides

		/// <summary>See <see cref="XPathNavigator.Clone"/></summary>
		public override XPathNavigator Clone() 
		{
			return new XmlNodeNavigator(_state);
		}
		
		/// <summary>See <see cref="XPathNavigator.NodeType"/></summary>
		public override XPathNodeType NodeType 
		{ 
			get { return _state.AtRoot? XPathNodeType.Root: _state.Navigator.NodeType;}
		}
    
		/// <summary>See <see cref="XPathNavigator.LocalName"/></summary>
		public override string LocalName 
		{         
			get { return _state.AtRoot? String.Empty: _state.Navigator.LocalName;}
		}

		/// <summary>See <see cref="XPathNavigator.Name"/></summary>
		public override string Name 
		{ 
			get { return _state.AtRoot? String.Empty: _state.Navigator.Name; }
		}
		
		/// <summary>See <see cref="XPathNavigator.NamespaceURI"/></summary>
		public override string NamespaceURI 
		{
			get {return _state.AtRoot? String.Empty: _state.Navigator.NamespaceURI; }
		}
      
		/// <summary>See <see cref="XPathNavigator.Prefix"/></summary>
		public override string Prefix 
		{
			get { return _state.AtRoot? String.Empty: _state.Navigator.Prefix; }
		}

		/// <summary>See <see cref="XPathNavigator.Value"/></summary>
		public override string Value 
		{
			get { return _state.AtRoot? _state.DocumentNode.Value: _state.Navigator.Value; }
		}

		/// <summary>See <see cref="XPathNavigator.BaseURI"/></summary>
		public override String BaseURI 
		{
			get { return _state.AtRoot? _state.DocumentNode.BaseURI: _state.Navigator.BaseURI; } 
		}

		/// <summary>See <see cref="XPathNavigator.IsEmptyElement"/></summary>
		public override bool IsEmptyElement 
		{
			get { return _state.AtRoot? false: _state.Navigator.IsEmptyElement; }
		}
      
		/// <summary>See <see cref="XPathNavigator.XmlLang"/></summary>
		public override string XmlLang 
		{
			get { return _state.AtRoot? String.Empty: _state.Navigator.XmlLang; }
		}
      
		/// <summary>See <see cref="XPathNavigator.NameTable "/></summary>
		public override XmlNameTable NameTable 
		{
			get { return _state.AtRoot? _state.DocumentNode.OwnerDocument.NameTable: _state.Navigator.NameTable;}
		}		

		/// <summary>See <see cref="XPathNavigator.HasAttributes"/></summary>
		public override bool HasAttributes 
		{
			get { return _state.AtRoot? false: _state.Navigator.HasAttributes; }			
		}

		/// <summary>See <see cref="XPathNavigator.GetAttribute"/></summary>
		public override string GetAttribute(string localName, string namespaceURI) 
		{
			return _state.AtRoot? String.Empty: _state.Navigator.GetAttribute(localName, namespaceURI);
		}

		/// <summary>See <see cref="XPathNavigator.MoveToAttribute"/></summary>
		public override bool MoveToAttribute(string localName, string namespaceURI) 
		{
			return _state.AtRoot? false: _state.Navigator.MoveToAttribute(localName, namespaceURI);
		}

		/// <summary>See <see cref="XPathNavigator.MoveToFirstAttribute"/></summary>
		public override bool MoveToFirstAttribute() 
		{
			return _state.AtRoot? false: _state.Navigator.MoveToFirstAttribute();
		}

		/// <summary>See <see cref="XPathNavigator.MoveToNextAttribute"/></summary>
		public override bool MoveToNextAttribute() 
		{
			return _state.AtRoot? false: _state.Navigator.MoveToNextAttribute();
		}
		
		/// <summary>See <see cref="XPathNavigator.GetNamespace"/></summary>
		public override string GetNamespace(string localname) 
		{
			return _state.AtRoot? String.Empty: _state.Navigator.GetNamespace(localname);
		}
		
		/// <summary>See <see cref="XPathNavigator.MoveToNamespace"/></summary>
		public override bool MoveToNamespace(string @namespace) 
		{
			return _state.AtRoot? false: _state.Navigator.MoveToNamespace(@namespace);
		}

		/// <summary>See <see cref="XPathNavigator.MoveToFirstNamespace"/></summary>
		public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope) 
		{
			return _state.AtRoot? false: _state.Navigator.MoveToFirstNamespace(namespaceScope);
		}

		/// <summary>See <see cref="XPathNavigator.MoveToNextNamespace"/></summary>
		public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope) 
		{
			return _state.AtRoot? false: _state.Navigator.MoveToNextNamespace(namespaceScope);
		}       		
    
		/// <summary>See <see cref="XPathNavigator.MoveToNext"/></summary>
		public override bool MoveToNext() 
		{		    
			return _state.AtRoot || IsAtDocumentNode() ? false: _state.Navigator.MoveToNext();			    
		}
        
		/// <summary>See <see cref="XPathNavigator.MoveToPrevious"/></summary>
		public override bool MoveToPrevious() 
		{		    
			return _state.AtRoot || IsAtDocumentNode()? false: _state.Navigator.MoveToPrevious();
		}

		/// <summary>See <see cref="XPathNavigator.MoveToFirst"/></summary>
		public override bool MoveToFirst() 
		{		    
			return _state.AtRoot || IsAtDocumentNode()? false: _state.Navigator.MoveToFirst();
		}

		/// <summary>See <see cref="XPathNavigator.MoveToFirstChild"/></summary>
		public override bool MoveToFirstChild() 
		{
			if (_state.AtRoot) 
			{
				_state.AtRoot = false;
				_state.Navigator = _state.DocumentNode.CreateNavigator();
				return true;
			} 
			else
				return _state.Navigator.MoveToFirstChild();			
		}
    
		/// <summary>See <see cref="XPathNavigator.MoveToParent"/></summary>
		public override bool MoveToParent() 
		{
			if (_state.AtRoot)
				return false;
			else 
			{			    
				if (IsAtDocumentNode()) 
				{
					_state.AtRoot = true;
					return true;
				} 
				else
					return _state.Navigator.MoveToParent();
			}
		}

		/// <summary>See <see cref="XPathNavigator.MoveToRoot"/></summary>
		public override void MoveToRoot() 
		{		    
			_state.AtRoot = true;
		}
		
		/// <summary>See <see cref="XPathNavigator.MoveTo"/></summary>
		public override bool MoveTo(XPathNavigator other) 
		{
			if (other != null && other is XmlNodeNavigator) 
			{
				XmlNodeNavigator otherNav = (XmlNodeNavigator)other;
				if (otherNav.State.DocumentNode != _state.DocumentNode)
					return false;
				_state = new NavigatorState(otherNav.State);
				return true;
			} 
			else            		
				return false;
		}
		
		/// <summary>See <see cref="XPathNavigator.MoveToId"/></summary>
		public override bool MoveToId(string id) 
		{            
			/*XmlNode node = _state.DocumentNode.OwnerDocument.GetElementById(id);
				  if (node != null) {
					  //Check if the node is in the subtree
					  while (node.ParentNode != null) {
						  if (node == _state.DocumentNode) {
							  //How to move it there?                                                
						  } else
							  node = node.ParentNode;
					  }                
				  }*/
			return false;
		}

		/// <summary>See <see cref="XPathNavigator.IsSamePosition"/></summary>
		public override bool IsSamePosition(XPathNavigator other) 
		{            
			if (other != null && other is XmlNodeNavigator) 
			{
				XmlNodeNavigator otherNav = (XmlNodeNavigator)other;
				return (otherNav.State.DocumentNode == _state.DocumentNode &&
					(otherNav.State.AtRoot == true && _state.AtRoot == true || 
					((IHasXmlNode)otherNav.State.Navigator).GetNode() ==
					((IHasXmlNode)_state.Navigator).GetNode()))? true : false;                
			} 
			else            		
				return false;        
		}
    
		/// <summary>See <see cref="XPathNavigator.HasChildren"/></summary>
		public override bool HasChildren 
		{
			get { return _state.AtRoot? true:_state.Navigator.HasChildren; }
		}

		#endregion XPathNavigator Overrides

		#region IHasXmlNode Overrides

		/// <summary>
		/// See <see cref="IHasXmlNode.GetNode"/>.
		/// </summary>
		public XmlNode GetNode() 
		{
			//TODO: what at root?
			return _state.AtRoot? null : ((IHasXmlNode)_state.Navigator).GetNode();
		}

		#endregion IHasXmlNode Overrides
	}
    
	#region Internal Classes

	/// <summary>
	/// Internal navigator state
	/// </summary>
	internal class NavigatorState 
	{
		public XPathNavigator Navigator;
		public XmlNode DocumentNode;
		public bool AtRoot;
        
		/// <summary>
		/// Initializes state with given node
		/// </summary>        
		public NavigatorState(XmlNode node) 
		{
			Navigator = node.CreateNavigator();
			AtRoot = true;
			DocumentNode = node;
		}
        
		/// <summary>
		/// Copy constructor.
		/// </summary>        
		public NavigatorState(NavigatorState state) 
		{
			Navigator = state.Navigator.Clone();
			AtRoot = state.AtRoot;
			DocumentNode = state.DocumentNode;
		}
	}

	#endregion Internal Classes
}