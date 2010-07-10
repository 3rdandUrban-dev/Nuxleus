#region using

using System;
using System.Collections;
using System.Xml;
using System.Xml.XPath;

#endregion

namespace Mvp.Xml.XPointer 
{   
    /// <summary>
    /// SchemaBased XPointer pointer.
    /// </summary>
    internal class SchemaBasedPointer : Pointer 
    {

        #region private members

        private ArrayList _parts;
	    private string _xpointer;

        #endregion

        #region constructors
        
        /// <summary>
        /// Creates scheme based XPointer given list of pointer parts.
        /// </summary>
        /// <param name="parts">List of pointer parts</param>
        /// <param name="xpointer">String representation of the XPointer 
        /// (for error diagnostics)</param>
        public SchemaBasedPointer(ArrayList parts, string xpointer) 
        {
            _parts = parts;		
	        _xpointer = xpointer;
        }

        #endregion

        #region public members

        /// <summary>
        /// Pointer parts collection.
        /// </summary>
        public ArrayList Parts 
        {
            get { return _parts; }
        }	    	    		    	
		
        #endregion

        #region Pointer overrides

        /// <summary>
        /// Evaluates <see cref="XPointer"/> pointer and returns 
        /// iterator over pointed nodes.
        /// </summary>
        /// <param name="nav">XPathNavigator to evaluate the 
        /// <see cref="XPointer"/> on.</param>
        /// <returns><see cref="XPathNodeIterator"/> over pointed nodes</returns>	    					
        public override XPathNodeIterator Evaluate(XPathNavigator nav) 
        {
            XPathNodeIterator result;            
            XmlNamespaceManager nm = new XmlNamespaceManager(nav.NameTable);
            for (int i=0; i<_parts.Count; i++) 
            {
                PointerPart part = (PointerPart)_parts[i];
                result = part.Evaluate(nav, nm);
                if (result != null && result.MoveNext())
                    return result;
            }
            throw new NoSubresourcesIdentifiedException(SR.GetString("NoSubresourcesIdentifiedException", _xpointer));
        }

        #endregion
    }
}
