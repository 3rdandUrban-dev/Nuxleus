#region using

using System;
using System.Xml.Xsl;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;

#endregion

namespace Mvp.Xml.Exslt
{
    /// <summary>
    /// Custom <see cref="XsltContext"/> implementation providing support for EXSLT 
    /// functions in XPath-only environment.
    /// </summary>
    public class ExsltContext : XsltContext
    {

        #region Private Fields and Properties

        private XmlNameTable _nt;

        /// <summary>
        /// Bitwise enumeration used to specify which EXSLT functions should be accessible to 
        /// in the ExsltContext object. The default value is ExsltFunctionNamespace.All 
        /// </summary>
        private ExsltFunctionNamespace _supportedFunctions = ExsltFunctionNamespace.All;

        /// <summary>
        /// Extension object which implements the functions in the http://exslt.org/math namespace
        /// </summary>
        private ExsltMath exsltMath = new ExsltMath();


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
        /// Extension object which implements the functions in the http://exslt.org/random namespace
        /// </summary>
        private ExsltRandom exsltRandom = new ExsltRandom();

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

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new ExsltContext instance.
        /// </summary>        
        public ExsltContext(XmlNameTable nt)
            : base((NameTable)nt)
        {
            _nt = nt;
            AddExtensionNamespaces();
        }

        /// <summary>
        /// Creates new ExsltContext instance.
        /// </summary>        
        public ExsltContext(NameTable nt, ExsltFunctionNamespace supportedFunctions)
            : this(nt)
        {
            SupportedFunctions = supportedFunctions;
        }

        #endregion

        #region Private methods
        private void AddExtensionNamespaces()
        {
            //remove all our extension objects in case the ExsltContext is being reused            
            RemoveNamespace("math", ExsltNamespaces.Math);
            RemoveNamespace("date", ExsltNamespaces.DatesAndTimes);
            RemoveNamespace("regexp", ExsltNamespaces.RegularExpressions);
            RemoveNamespace("str", ExsltNamespaces.Strings);
            RemoveNamespace("set", ExsltNamespaces.Sets);
            RemoveNamespace("random", ExsltNamespaces.Random);
            RemoveNamespace("date2", ExsltNamespaces.GDNDatesAndTimes);
            RemoveNamespace("math2", ExsltNamespaces.GDNMath);
            RemoveNamespace("regexp2", ExsltNamespaces.GDNRegularExpressions);
            RemoveNamespace("set2", ExsltNamespaces.GDNSets);
            RemoveNamespace("str2", ExsltNamespaces.GDNStrings);
            RemoveNamespace("dyn2", ExsltNamespaces.GDNDynamic);

            //add extension objects as specified by SupportedFunctions            
            if ((this.SupportedFunctions & ExsltFunctionNamespace.Math) > 0)
                AddNamespace("math", ExsltNamespaces.Math);

            if ((this.SupportedFunctions & ExsltFunctionNamespace.DatesAndTimes) > 0)
                AddNamespace("date", ExsltNamespaces.DatesAndTimes);

            if ((this.SupportedFunctions & ExsltFunctionNamespace.RegularExpressions) > 0)
                AddNamespace("regexp", ExsltNamespaces.RegularExpressions);

            if ((this.SupportedFunctions & ExsltFunctionNamespace.Strings) > 0)
                AddNamespace("str", ExsltNamespaces.Strings);

            if ((this.SupportedFunctions & ExsltFunctionNamespace.Sets) > 0)
                AddNamespace("set", ExsltNamespaces.Sets);

            if ((this.SupportedFunctions & ExsltFunctionNamespace.Random) > 0)
                AddNamespace("random", ExsltNamespaces.Random);

            if ((this.SupportedFunctions & ExsltFunctionNamespace.GDNDatesAndTimes) > 0)
                AddNamespace("date2", ExsltNamespaces.GDNDatesAndTimes);

            if ((this.SupportedFunctions & ExsltFunctionNamespace.GDNMath) > 0)
                AddNamespace("math2", ExsltNamespaces.GDNMath);

            if ((this.SupportedFunctions & ExsltFunctionNamespace.GDNRegularExpressions) > 0)
                AddNamespace("regexp2", ExsltNamespaces.GDNRegularExpressions);

            if ((this.SupportedFunctions & ExsltFunctionNamespace.GDNSets) > 0)
                AddNamespace("set2", ExsltNamespaces.GDNSets);

            if ((this.SupportedFunctions & ExsltFunctionNamespace.GDNStrings) > 0)
                AddNamespace("str2", ExsltNamespaces.GDNStrings);

            if ((this.SupportedFunctions & ExsltFunctionNamespace.GDNDynamic) > 0)
                AddNamespace("dyn2", ExsltNamespaces.GDNDynamic);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Bitwise enumeration used to specify which EXSLT functions should be accessible to 
        /// in the ExsltContext. The default value is ExsltFunctionNamespace.All 
        /// </summary>
        public ExsltFunctionNamespace SupportedFunctions
        {
            set
            {
                if (Enum.IsDefined(typeof(ExsltFunctionNamespace), value))
                    _supportedFunctions = value;
            }
            get { return _supportedFunctions; }
        }
        #endregion

        #region XsltContext Overrides

        /// <summary>
        /// See <see cref="XsltContext.CompareDocument"/>
        /// </summary>        
        public override int CompareDocument(string baseUri, string nextbaseUri)
        {
            return 0;
        }

        /// <summary>
        /// See <see cref="XsltContext.PreserveWhitespace"/>
        /// </summary>
        public override bool PreserveWhitespace(XPathNavigator node)
        {
            return true;
        }

        /// <summary>
        /// See <see cref="XsltContext.Whitespace"/>
        /// </summary>
        public override bool Whitespace
        {
            get { return true; }
        }

        /// <summary>
        /// Resolves variables.
        /// </summary>
        /// <param name="prefix">The variable's prefix</param>
        /// <param name="name">The variable's name</param>
        /// <returns></returns>
        public override IXsltContextVariable ResolveVariable(string prefix, string name)
        {
            return null;
        }

        /// <summary>
        /// Resolves custom function in XPath expression.
        /// </summary>
        /// <param name="prefix">The prefix of the function as it appears in the XPath expression.</param>
        /// <param name="name">The name of the function.</param>
        /// <param name="argTypes">An array of argument types for the function being resolved. 
        /// This allows you to select between methods with the same name (for example, overloaded 
        /// methods). </param>
        /// <returns>An IXsltContextFunction representing the function.</returns>
        public override IXsltContextFunction ResolveFunction(string prefix, string name,
            XPathResultType[] argTypes)
        {
            switch (LookupNamespace(_nt.Get(prefix)))
            {
                case ExsltNamespaces.DatesAndTimes:
                    return GetExtensionFunctionImplementation(exsltDatesAndTimes, name, argTypes);
                case ExsltNamespaces.Math:
                    return GetExtensionFunctionImplementation(exsltMath, name, argTypes);
                case ExsltNamespaces.RegularExpressions:
                    return GetExtensionFunctionImplementation(exsltRegularExpressions, name, argTypes);
                case ExsltNamespaces.Sets:
                    return GetExtensionFunctionImplementation(exsltSets, name, argTypes);
                case ExsltNamespaces.Strings:
                    return GetExtensionFunctionImplementation(exsltStrings, name, argTypes);
                case ExsltNamespaces.Random:
                    return GetExtensionFunctionImplementation(exsltRandom, name, argTypes);
                case ExsltNamespaces.GDNDatesAndTimes:
                    return GetExtensionFunctionImplementation(gdnDatesAndTimes, name, argTypes);
                case ExsltNamespaces.GDNMath:
                    return GetExtensionFunctionImplementation(gdnMath, name, argTypes);
                case ExsltNamespaces.GDNRegularExpressions:
                    return GetExtensionFunctionImplementation(gdnRegularExpressions, name, argTypes);
                case ExsltNamespaces.GDNSets:
                    return GetExtensionFunctionImplementation(gdnSets, name, argTypes);
                case ExsltNamespaces.GDNStrings:
                    return GetExtensionFunctionImplementation(gdnStrings, name, argTypes);
                case ExsltNamespaces.GDNDynamic:
                    return GetExtensionFunctionImplementation(gdnDynamic, name, argTypes);
                default:
                    throw new XPathException(string.Format("Unrecognized extension function namespace: prefix='{0}', namespace URI='{1}'",
                        prefix, LookupNamespace(_nt.Get(prefix))), null);
            }
        }
        #endregion

        /// <summary>
        /// Finds appropriate implementation for an extension function - public 
        /// method with the same number of arguments and compatible argument types.
        /// </summary>
        /// <param name="obj">Extension object</param>
        /// <param name="name">Function name</param>
        /// <param name="argTypes">Types of arguments</param>
        /// <returns></returns>
        private ExsltContextFunction GetExtensionFunctionImplementation(object obj, string name, XPathResultType[] argTypes)
        {
            //For each method in object's type
            foreach (MethodInfo mi in obj.GetType().GetMethods())
            {
                //We are interested in methods with given name
                if (mi.Name == name)
                {
                    ParameterInfo[] parameters = mi.GetParameters();
                    ////We are interested in methods with given number of arguments
                    if (parameters.Length == argTypes.Length)
                    {
                        bool mismatch = false;
                        //Now let's check out if parameter types are compatible with actual ones
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            ParameterInfo pi = parameters[i];
                            XPathResultType paramType = ConvertToXPathType(pi.ParameterType);
                            if (paramType == XPathResultType.Any || paramType == argTypes[i])
                                continue;
                            else
                            {
                                mismatch = true;
                                break;
                            }
                        }
                        if (!mismatch)
                            //Create lightweight wrapper around method info
                            return new ExsltContextFunction(mi, argTypes, obj);
                    }
                }
            }
            throw new XPathException("Extension function not found: " + name, null);
        }

        /// <summary>
        /// Converts CLI type to XPathResultType type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static XPathResultType ConvertToXPathType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return XPathResultType.Boolean;
                case TypeCode.String:
                    return XPathResultType.String;
                case TypeCode.Object:
                    if (typeof(IXPathNavigable).IsAssignableFrom(type) ||
                        typeof(XPathNavigator).IsAssignableFrom(type))
                        return XPathResultType.Navigator;
                    else if (typeof(XPathNodeIterator).IsAssignableFrom(type))
                        return XPathResultType.NodeSet;
                    else
                        return XPathResultType.Any;
                case TypeCode.DateTime:
                case TypeCode.DBNull:
                case TypeCode.Empty:
                    return XPathResultType.Error;
                default:
                    return XPathResultType.Number;
            }
        }

        //TODO: test it

        ///// <summary>
        ///// This is a workaround for some problem, see
        ///// http://www.tkachenko.com/blog/archives/000042.html for more 
        ///// details.
        ///// </summary>
        ///// <param name="prefix">Prefix to be resolved</param>
        ///// <returns>Resolved namespace</returns>
        //public override string LookupNamespace(string prefix)
        //{
        //  if (prefix == String.Empty)
        //    return prefix;
        //  string uri = base.LookupNamespace(NameTable.Get(prefix));
        //  if (uri == null)
        //    throw new XsltException("Undeclared namespace prefix - " + prefix, null);

        //  return uri;
        //}
    }
} // namespace GotDotNet.Exslt