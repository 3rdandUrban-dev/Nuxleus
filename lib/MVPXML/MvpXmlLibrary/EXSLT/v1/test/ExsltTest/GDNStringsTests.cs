using System;
using NUnit.Framework;

namespace ExsltTest
{
    /// <summary>
    /// Collection of unit tests for GotDotNet Strings module functions.
    /// </summary>
    [TestFixture]
    public class GDNStringsTests : ExsltUnitTests
    {        
        protected override string TestDir 
        {
            get { return "tests/GotDotNet/Strings/"; }
        }
        protected override string ResultsDir 
        {
            get { return "results/GotDotNet/Strings/"; }
        }                

        /// <summary>
        /// Tests the following function:
        ///     str2:lowercase()
        /// </summary>
        [Test]
        public void LowercaseTest() 
        {
            RunAndCompare("source.xml", "lowercase.xslt", "lowercase.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     str2:uppercase()
        /// </summary>
        [Test]
        public void UppercaseTest() 
        {
            RunAndCompare("source.xml", "uppercase.xslt", "uppercase.xml");
        }
    }
}
