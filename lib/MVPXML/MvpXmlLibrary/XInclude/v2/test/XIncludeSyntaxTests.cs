using System;
using System.Diagnostics;

using Mvp.Xml.XInclude;
using NUnit.Framework;

namespace Mvp.Xml.XInclude.Test
{
    /// <summary>
    /// XInclude syntax tests.
    /// </summary>
    [TestFixture]
    public class XIncludeSyntaxTests
    {        

        public XIncludeSyntaxTests() 
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Error));
        }

        /// <summary>
        /// No href and no xpointer attribute.
        /// </summary>
        [Test]
        [ExpectedException(typeof(XIncludeSyntaxError))]
        public void NoHrefAndNoXPointerAttributes() 
        {
            XIncludingReader xir = new XIncludingReader("../../tests/nohref.xml");
            while (xir.Read());
            xir.Close();
        }        

        /// <summary>
        /// xi:include child of xi:include.
        /// </summary>
        [Test]
        [ExpectedException(typeof(XIncludeSyntaxError))]
        public void IncludeChildOfInclude() 
        {
            XIncludingReader xir = new XIncludingReader("../../tests/includechildofinclude.xml");
            while (xir.Read());
            xir.Close();
        }
                
        /// <summary>
        /// xi:fallback not child of xi:include.
        /// </summary>
        [Test]
        [ExpectedException(typeof(XIncludeSyntaxError))]
        public void FallbackNotChildOfInclude() 
        {
            XIncludingReader xir = new XIncludingReader("../../tests/fallbacknotchildinclude.xml");
            while (xir.Read());
            xir.Close();
        }     
       
        /// <summary>
        /// Unknown value of parse attribute.
        /// </summary>
        [Test]
        [ExpectedException(typeof(XIncludeSyntaxError))]
        public void UnknownParseAttribute() 
        {
            XIncludingReader xir = new XIncludingReader("../../tests/unknownparseattr.xml");
            while (xir.Read());
            xir.Close();
        }     

        /// <summary>
        /// Two xi:fallback.
        /// </summary>
        [Test]
        [ExpectedException(typeof(XIncludeSyntaxError))]
        public void TwoFallbacks() 
        {
            XIncludingReader xir = new XIncludingReader("../../tests/twofallbacks.xml");
            while (xir.Read());
            xir.Close();
        }             
    }
}
