#region using

using System;
using System.Xml.Xsl;
using System.Xml;
using System.Xml.XPath;
using System.IO; 
using System.Text;

#endregion

namespace Mvp.Xml.Exslt
{

	/// <summary>
	/// Enumeration used to indicate an EXSLT function namespace. 
	/// </summary>
	[Flags]
	public enum ExsltFunctionNamespace{ 
		None  = 0,
		Common     = 1, 
		DatesAndTimes  = 2, 
		Math = 4, 
		RegularExpressions = 8, 
		Sets = 16, 
		Strings = 32,
		GDNDatesAndTimes = 64,
		GDNSets = 128,
		GDNMath = 256,
		GDNRegularExpressions = 512,
		GDNStrings = 1024, 
		Random = 2048,
        GDNDynamic = 4096,
        AllExslt = Common | DatesAndTimes | Math | Random | RegularExpressions | Sets | Strings,            
		All = Common | DatesAndTimes | Math | Random | RegularExpressions | Sets | Strings | 
		    GDNDatesAndTimes | GDNSets | GDNMath | GDNRegularExpressions | GDNStrings | GDNDynamic
	}

	/// <summary>
	/// Transforms XML data using an XSLT stylesheet. Supports a number of EXSLT as 
	/// defined at http://www.exslt.org
	/// </summary>
	/// <remarks>
	/// XslTransform supports the XSLT 1.0 syntax. The XSLT stylesheet must use the 
	/// namespace http://www.w3.org/1999/XSL/Transform. Additional arguments can also be 
	/// added to the stylesheet using the XsltArgumentList class. 
	/// This class contains input parameters for the stylesheet and extension objects which can be called from the stylesheet.
	/// This class also recognizes functions from the following namespaces 
	/// * http://exslt.org/common
	/// * http://exslt.org/dates-and-times
	/// * http://exslt.org/math
	/// * http://exslt.org/random
	/// * http://exslt.org/regular-expressions
	/// * http://exslt.org/sets
	/// * http://exslt.org/strings
	/// * http://gotdotnet.com/exslt/dates-and-times
	/// * http://gotdotnet.com/exslt/math
	/// * http://gotdotnet.com/exslt/regular-expressions
	/// * http://gotdotnet.com/exslt/sets
	/// * http://gotdotnet.com/exslt/strings
	/// * http://gotdotnet.com/exslt/dynamic
	/// </remarks>
	public class ExsltTransform
	{

		#region Private Fields and Properties
    
    /// <summary>
    /// Sync object.
    /// </summary>
    private object sync = new object();

	/// <summary>
	/// The XslTransform object wrapped by this class. 
	/// </summary>
		private XslTransform xslTransform; 
		
		/// <summary>
		/// Bitwise enumeration used to specify which EXSLT functions should be accessible to 
		/// the ExsltTransform object. The default value is ExsltFunctionNamespace.All 
		/// </summary>
		private ExsltFunctionNamespace _supportedFunctions = ExsltFunctionNamespace.All; 

		/// <summary>
		/// Extension object which implements the functions in the http://exslt.org/common namespace
		/// </summary>
		private ExsltCommon exsltCommon = new ExsltCommon(); 

		/// <summary>
		/// Extension object which implements the functions in the http://exslt.org/math namespace
		/// </summary>
		private ExsltMath exsltMath = new ExsltMath(); 

		/// <summary>
		/// Extension object which implements the functions in the http://exslt.org/random namespace
		/// </summary>
		private ExsltRandom exsltRandom = new ExsltRandom(); 

		/// <summary>
		/// Extension object which implements the functions in the http://exslt.org/dates-and-times namespace
		/// </summary>
		private ExsltDatesAndTimes exsltDatesAndTimes = new ExsltDatesAndTimes(); 

		/// <summary>
		/// Extension object which implements the functions in the http://exslt.org/regular-expressions namespace
		/// </summary>
		private ExsltRegularExpressions exsltRegularExpressions = new ExsltRegularExpressions(); 

		/// <summary>
		/// Extension object which implements the functions in the http://exslt.org/strings namespace
		/// </summary>
		private ExsltStrings exsltStrings = new ExsltStrings(); 

		/// <summary>
		/// Extension object which implements the functions in the http://exslt.org/sets namespace
		/// </summary>
		private ExsltSets exsltSets = new ExsltSets(); 

        /// <summary>
        /// Extension object which implements the functions in the http://gotdotnet.com/exslt/dates-and-times namespace
        /// </summary>
        private GDNDatesAndTimes gdnDatesAndTimes = new GDNDatesAndTimes(); 
        
        /// <summary>
        /// Extension object which implements the functions in the http://gotdotnet.com/exslt/regular-expressions namespace
        /// </summary>
        private GDNRegularExpressions gdnRegularExpressions = new GDNRegularExpressions();
        
        /// <summary>
        /// Extension object which implements the functions in the http://gotdotnet.com/exslt/math namespace
        /// </summary>
        private GDNMath gdnMath = new GDNMath(); 
        
        /// <summary>
        /// Extension object which implements the functions in the http://gotdotnet.com/exslt/sets namespace
        /// </summary>
        private GDNSets gdnSets = new GDNSets();
                
        /// <summary>
        /// Extension object which implements the functions in the http://gotdotnet.com/exslt/strings namespace
        /// </summary>
        private GDNStrings gdnStrings = new GDNStrings();

        /// <summary>
        /// Extension object which implements the functions in the http://gotdotnet.com/exslt/dynamic namespace
        /// </summary>
        private GDNDynamic gdnDynamic = new GDNDynamic();
        
        /// <summary>
        /// Boolean flag used to specify whether multiple output is supported.
        /// </summary>
        private bool _multiOutput = false;

		#endregion

		#region Public Fields and Properties 
		
/// <summary>
/// Sets the XmlResolver used to resolve external resources when the 
/// Transform method is called.
/// </summary>
		public XmlResolver XmlResolver { set { this.xslTransform.XmlResolver = value; } }

/// <summary>
/// Bitwise enumeration used to specify which EXSLT functions should be accessible to 
/// the ExsltTransform object. The default value is ExsltFunctionNamespace.All 
/// </summary>
		public ExsltFunctionNamespace SupportedFunctions{		
			set { if (Enum.IsDefined(typeof(ExsltFunctionNamespace), value)) 
					  this._supportedFunctions = value; 
				} 
			get { return this._supportedFunctions; }
		}
		
        /// <summary>
        /// Boolean flag used to specify whether multiple output (via exsl:document) is 
        /// supported.
        /// Note: This property is ignored (hence multiple output is not supported) when
        /// transformation is done to XmlReader or XmlWriter (use overloaded method, 
        /// which transforms to MultiXmlTextWriter instead).
        /// Note: Because of some restrictions and slight overhead this feature is
        /// disabled by default. If you need multiple output support, set this property to
        /// true before the Transform() call.
        /// </summary>
		public bool MultiOutput {
		    get { return _multiOutput; }
		    set { _multiOutput = value; }
		}

		#endregion

		#region Constructors
/// <summary>
/// Constructor initializes class. 
/// </summary>
		public ExsltTransform(){
			this.xslTransform = new XslTransform(); 			
		}

		#endregion 

		#region Load() method Overloads  
		/// <summary> Loads the XSLT stylesheet contained in the IXPathNavigable</summary>
		public void Load(IXPathNavigable ixn){ this.xslTransform.Load(ixn); }


		/// <summary> Loads the XSLT stylesheet specified by a URL</summary>
		public void Load(string s){ this.xslTransform.Load(s); }

		/// <summary> Loads the XSLT stylesheet contained in the XmlReader</summary>
		public void Load(XmlReader reader){ this.xslTransform.Load(reader); }

		/// <summary> Loads the XSLT stylesheet contained in the XPathNavigator</summary>
		public void Load(XPathNavigator navigator){ this.xslTransform.Load(navigator); }

		/// <summary> Loads the XSLT stylesheet contained in the IXPathNavigable</summary>
		public void Load(IXPathNavigable ixn, XmlResolver resolver){ this.xslTransform.Load(ixn, resolver); }

		/// <summary> Loads the XSLT stylesheet specified by a URL</summary>
		public void Load(string s, XmlResolver resolver){ this.xslTransform.Load(s, resolver); }

		/// <summary> Loads the XSLT stylesheet contained in the XmlReader</summary>
		public void Load(XmlReader reader, XmlResolver resolver){ this.xslTransform.Load(reader, resolver); }

		/// <summary> Loads the XSLT stylesheet contained in the XPathNavigator</summary>
		public void Load(XPathNavigator navigator, XmlResolver resolver) {this.xslTransform.Load(navigator, resolver); }

		#endregion 

		#region Transform() method Overloads

		/// <summary> Transforms the XML data in the IXPathNavigable using the specified args and outputs the result to an XmlReader</summary>
		public XmlReader Transform(IXPathNavigable ixn, XsltArgumentList arglist)
		{  
			return this.xslTransform.Transform(ixn, this.AddExsltExtensionObjects(arglist));
		}

		/// <summary> Transforms the XML data in the input file and outputs the result to an output file</summary>
		public void Transform(string infile, string outfile)
		{ 
			// Use using so that the file is not held open after the call

			using (StreamWriter outStream = new StreamWriter(outfile)) 
			{
				if (_multiOutput)
					this.xslTransform.Transform(new XPathDocument(infile), this.AddExsltExtensionObjects(null),
						new MultiXmlTextWriter(outStream)); 
				else
					this.xslTransform.Transform(new XPathDocument(infile), this.AddExsltExtensionObjects(null), outStream);	    
			}
		}

		/// <summary> Transforms the XML data in the XPathNavigator using the specified args and outputs the result to an XmlReader</summary>
		public XmlReader Transform(XPathNavigator navigator, XsltArgumentList arglist)
		{
		    return this.xslTransform.Transform(navigator, this.AddExsltExtensionObjects(arglist)); 
		}

		/// <summary> Transforms the XML data in the IXPathNavigable using the specified args and outputs the result to a Stream</summary>
		public void Transform(IXPathNavigable ixn, XsltArgumentList arglist, Stream stream)
		{ 
		    if (_multiOutput)
		        this.xslTransform.Transform(ixn, this.AddExsltExtensionObjects(arglist), 
		            new MultiXmlTextWriter(stream, Encoding.UTF8)); 
		    else
		        this.xslTransform.Transform(ixn, this.AddExsltExtensionObjects(arglist), stream);
		}

		/// <summary> Transforms the XML data in the IXPathNavigable using the specified args and outputs the result to a TextWriter</summary>
		public void Transform(IXPathNavigable ixn, XsltArgumentList arglist, TextWriter writer)
		{ 
		    if (_multiOutput)
		        this.xslTransform.Transform(ixn, this.AddExsltExtensionObjects(arglist), 
		            new MultiXmlTextWriter(writer)); 
		    else
		        this.xslTransform.Transform(ixn, this.AddExsltExtensionObjects(arglist), writer);
		}

		/// <summary> Transforms the XML data in the IXPathNavigable using the specified args and outputs the result to an XmlWriter</summary>
		public void Transform(IXPathNavigable ixn, XsltArgumentList arglist, XmlWriter writer)
		{ 
		    this.xslTransform.Transform(ixn, this.AddExsltExtensionObjects(arglist), writer); 
		}
		
        /// <summary> Transforms the XML data in the IXPathNavigable using the specified args and outputs the result to an MultiXmlTextWriter</summary>
        public void Transform(IXPathNavigable ixn, XsltArgumentList arglist, MultiXmlTextWriter writer)
        { 
            this.xslTransform.Transform(ixn, this.AddExsltExtensionObjects(arglist), writer); 
        }

		/// <summary> Transforms the XML data in the XPathNavigator using the specified args and outputs the result to a Stream</summary>
		public void Transform(XPathNavigator navigator, XsltArgumentList arglist, Stream stream)
		{ 
		    if (_multiOutput)		         		    
		        this.xslTransform.Transform(navigator, this.AddExsltExtensionObjects(arglist), 
		            new MultiXmlTextWriter(stream, Encoding.UTF8));
		    else
		        this.xslTransform.Transform(navigator, this.AddExsltExtensionObjects(arglist), stream);
		}

		/// <summary> Transforms the XML data in the XPathNavigator using the specified args and outputs the result to a TextWriter</summary>
		public void Transform(XPathNavigator navigator, XsltArgumentList arglist, TextWriter writer)
		{ 
		    if (_multiOutput)
		        this.xslTransform.Transform(navigator, this.AddExsltExtensionObjects(arglist), 
		            new MultiXmlTextWriter(writer));
            else
                this.xslTransform.Transform(navigator, this.AddExsltExtensionObjects(arglist), writer);                		        
		}

		/// <summary> Transforms the XML data in the XPathNavigator using the specified args and outputs the result to an XmlWriter</summary>
		public void Transform(XPathNavigator navigator, XsltArgumentList arglist, XmlWriter writer)
		{ 
		    this.xslTransform.Transform(navigator, this.AddExsltExtensionObjects(arglist), writer); 
		}
		
        /// <summary> Transforms the XML data in the XPathNavigator using the specified args and outputs the result to an MultiXmlTextWriter</summary>
        public void Transform(XPathNavigator navigator, XsltArgumentList arglist, MultiXmlTextWriter writer)
        { 
            this.xslTransform.Transform(navigator, this.AddExsltExtensionObjects(arglist), writer); 
        }

		#endregion 

		#region Public Methods 

		#endregion 

		#region Private Methods 

		/// <summary>
		/// Adds the objects that implement the EXSLT extensions to the provided argument 
		/// list. The extension objects added depend on the value of the SupportedFunctions
		/// property.
		/// </summary>
		/// <param name="list">The argument list</param>
		/// <returns>An XsltArgumentList containing the contents of the list passed in 
		/// and objects that implement the EXSLT. </returns>
		/// <remarks>If null is passed in then a new XsltArgumentList is constructed. </remarks>
		private XsltArgumentList AddExsltExtensionObjects(XsltArgumentList list){
			if(list == null){
				list = new XsltArgumentList();
			}      
		
      lock (sync) 
      {
        //remove all our extension objects in case the XSLT argument list is being reused
        list.RemoveExtensionObject(ExsltNamespaces.Common); 
        list.RemoveExtensionObject(ExsltNamespaces.Math); 
        list.RemoveExtensionObject(ExsltNamespaces.Random); 
        list.RemoveExtensionObject(ExsltNamespaces.DatesAndTimes); 
        list.RemoveExtensionObject(ExsltNamespaces.RegularExpressions); 
        list.RemoveExtensionObject(ExsltNamespaces.Strings);
        list.RemoveExtensionObject(ExsltNamespaces.Sets); 
        list.RemoveExtensionObject(ExsltNamespaces.GDNDatesAndTimes);
        list.RemoveExtensionObject(ExsltNamespaces.GDNMath);
        list.RemoveExtensionObject(ExsltNamespaces.GDNRegularExpressions);
        list.RemoveExtensionObject(ExsltNamespaces.GDNSets);
        list.RemoveExtensionObject(ExsltNamespaces.GDNStrings);
        list.RemoveExtensionObject(ExsltNamespaces.GDNDynamic);
			            
        //add extension objects as specified by SupportedFunctions
        if((this.SupportedFunctions & ExsltFunctionNamespace.Common) > 0)
        { 
          list.AddExtensionObject(ExsltNamespaces.Common, this.exsltCommon); 
        }

        if((this.SupportedFunctions & ExsltFunctionNamespace.Math) > 0)
        { 
          list.AddExtensionObject(ExsltNamespaces.Math, this.exsltMath); 
        }

        if((this.SupportedFunctions & ExsltFunctionNamespace.Random) > 0)
        { 
          list.AddExtensionObject(ExsltNamespaces.Random, this.exsltRandom); 
        }

        if((this.SupportedFunctions & ExsltFunctionNamespace.DatesAndTimes) > 0)
        { 
          list.AddExtensionObject(ExsltNamespaces.DatesAndTimes, this.exsltDatesAndTimes); 
        }

        if((this.SupportedFunctions & ExsltFunctionNamespace.RegularExpressions) > 0)
        { 
          list.AddExtensionObject(ExsltNamespaces.RegularExpressions, this.exsltRegularExpressions); 
        }

        if((this.SupportedFunctions & ExsltFunctionNamespace.Strings) > 0)
        { 
          list.AddExtensionObject(ExsltNamespaces.Strings, this.exsltStrings); 
        }

        if((this.SupportedFunctions & ExsltFunctionNamespace.Sets) > 0)
        { 
          list.AddExtensionObject(ExsltNamespaces.Sets, this.exsltSets); 
        }
			
        if((this.SupportedFunctions & ExsltFunctionNamespace.GDNDatesAndTimes) > 0)
        { 
          list.AddExtensionObject(ExsltNamespaces.GDNDatesAndTimes, this.gdnDatesAndTimes); 
        }
            
        if((this.SupportedFunctions & ExsltFunctionNamespace.GDNMath) > 0)
        { 
          list.AddExtensionObject(ExsltNamespaces.GDNMath, this.gdnMath); 
        }
            
        if((this.SupportedFunctions & ExsltFunctionNamespace.GDNRegularExpressions) > 0)
        { 
          list.AddExtensionObject(ExsltNamespaces.GDNRegularExpressions, this.gdnRegularExpressions); 
        }
            
        if((this.SupportedFunctions & ExsltFunctionNamespace.GDNSets) > 0)
        { 
          list.AddExtensionObject(ExsltNamespaces.GDNSets, this.gdnSets); 
        }
            
        if((this.SupportedFunctions & ExsltFunctionNamespace.GDNStrings) > 0)
        { 
          list.AddExtensionObject(ExsltNamespaces.GDNStrings, this.gdnStrings); 
        }
        
        if((this.SupportedFunctions & ExsltFunctionNamespace.GDNDynamic) > 0)
        { 
          list.AddExtensionObject(ExsltNamespaces.GDNDynamic, this.gdnDynamic); 
        }
      }
			
			return list; 
		}

		#endregion

	}
}
