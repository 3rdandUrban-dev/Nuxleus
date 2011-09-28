using System;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Text;

using Mvp.Xml.XInclude;
using NUnit.Framework;

namespace Mvp.Xml.XInclude.Test
{
	/// <summary>
	/// Edinburgh University test cases from the XInclude Test suite.
	/// </summary>
	[TestFixture]
	public class LTG_Edinburgh_UnivTests
	{
		public LTG_Edinburgh_UnivTests()
		{
			Debug.Listeners.Add(new TextWriterTraceListener(Console.Error));
		}

        /// <summary>
        /// Utility method for running tests.
        /// </summary>        
        public static void RunAndCompare(string source, string result) 
        {
            XIncludeReaderTests.RunAndCompare(
                "../../XInclude-Test-Suite/EdUni/test/" + source, 
                "../../XInclude-Test-Suite/EdUni/test/" + result);
        }
        

		
        /// <summary>
        /// Simple whole-file inclusion.        
        /// </summary>
        [Test]
        public void eduni_1() 
        {
            RunAndCompare("book.xml", "../result/book.xml");            
        }  
  

		
        /// <summary>
        /// Verify that xi:include elements in the target have been processed in the acquired infoset, ie before the xpointer is applied.        
        /// </summary>
        [Test]
        public void eduni_2() 
        {
            RunAndCompare("extract.xml", "../result/extract.xml");            
        }  
  

		
        /// <summary>
        /// Check xml:lang fixup        
        /// </summary>
        [Test]
        public void eduni_3() 
        {
            RunAndCompare("lang.xml", "../result/lang.xml");            
        }  
  

	
	}
}
