#region using

using System;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Xml;
using System.Reflection;

#endregion

namespace Mvp.Xml.Exslt
{
	/// <summary>
	/// This class implements the EXSLT functions in the http://exslt.org/common namespace.
	/// </summary>
	public class ExsltCommon
	{
	    private static XPathExpression selectItself = null;
	    private static XPathExpression selectText = null;
	
		
/// <summary>
/// Implements the following function
///  string exsl:object-type(object) 
/// </summary>
/// <param name="o"></param>
/// <returns></returns>                
		public string objectType(object o){

			if(o is System.String){
				return "string";
			}else if(o is System.Boolean){
				return "boolean";
			}else if(o is Double || o is Int16 || o is UInt16 || o is Int32
				|| o is UInt32 || o is Int64 || o is UInt64 || o is Single || o is Decimal){
				return "number"; 
			}else if(o is System.Xml.XPath.XPathNavigator){
				return "RTF"; 
			}else if(o is System.Xml.XPath.XPathNodeIterator){
				return "node-set"; 
			}else{
				return "external"; 
			}

		}/* objecttype(object) */
		
		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>		
		public string objectType_RENAME_ME(object o) {
		    return objectType(o);
		}
		
		/// <summary>
		/// Implements the following function
		/// node-set exsl:node-set(object)
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>        
        public XPathNodeIterator nodeSet(object o) 
        {		
            if (o is XPathNavigator) 
            {
                XPathNavigator nav = (XPathNavigator)o;		        
                if (selectItself == null)
                    selectItself = nav.Compile(".");                                    
                return nav.Select(selectItself.Clone());
            } 
            else if (o is XPathNodeIterator)
                return o as XPathNodeIterator;
            else 
            {
                string s;
                if (o is string) 
                    s = o as string;
                else if (o is bool)
                    s = ((bool)o)? "true" : "false";
                else if (o is Double || o is Int16 || o is UInt16 || o is Int32
                    || o is UInt32 || o is Int64 || o is UInt64 || o is Single || o is Decimal)    
                    s = o.ToString();
                else    
                    return null;	
                //Now convert it to text node
                XmlParserContext context = new XmlParserContext(null, null, null, XmlSpace.None);                				    			
                XPathDocument doc = new XPathDocument(
                    new XmlTextReader("<d>"+s+"</d>", XmlNodeType.Element, context));				
                XPathNavigator nav = doc.CreateNavigator();                                         
                if (selectText == null)
                    selectText = nav.Compile("/d/text()");
                return nav.Select(selectText.Clone());
            }
        }

        /// <summary>
        /// This wrapper method will be renamed during custom build 
        /// to provide conformant EXSLT function name.
        /// </summary>        
        public XPathNodeIterator nodeSet_RENAME_ME(object o) {
            return nodeSet(o);
        }

/// <summary>
/// This method converts an ExsltNodeList to an XPathNodeIterator over the nodes in the list.
/// </summary>
/// <param name="list">The list to convert</param>
/// <returns>An XPathNodeIterator over the nodes in the original list</returns>
/// <remarks>Known Issues: A node list containing multiple instances of an attribute 
/// with the same namespace name and local name will cause an error.</remarks>
		internal static XPathNodeIterator ExsltNodeListToXPathNodeIterator(ExsltNodeList list){
		

			Assembly systemXml = typeof(XPathNodeIterator).Assembly; 
			Type arrayIteratorType = systemXml.GetType("System.Xml.XPath.XPathArrayIterator"); 

			return (XPathNodeIterator) Activator.CreateInstance( arrayIteratorType, 
				BindingFlags.Instance | BindingFlags.Public | 
				BindingFlags.CreateInstance, null, new object[]{ list.innerList}, null ); 

		}


	
		
	}
}
