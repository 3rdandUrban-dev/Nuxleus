using System;
using NUnit.Framework;

namespace ExsltTest
{
    /// <summary>
    /// Collection of unit tests for GotDotNet Math module functions.
    /// </summary>
    [TestFixture]
    public class GDNMathTests : ExsltUnitTests
    {        
        protected override string TestDir 
        {
            get { return "tests/GotDotNet/Math/"; }
        }
        protected override string ResultsDir 
        {
            get { return "results/GotDotNet/Math/"; }
        }   
        
        /// <summary>
        /// Tests the following function:
        ///     math2:avg()
        /// </summary>
        [Test]
        public void AvgTest() 
        {
            RunAndCompare("source.xml", "avg.xslt", "avg.xml");
        }                 
    }
}
