using System;
using NUnit.Framework;

namespace ExsltTest
{
    /// <summary>
    /// Collection of unit tests for GotDotNet Sets module functions.
    /// </summary>
    [TestFixture]
    public class GDNSetsTests : ExsltUnitTests
    {        

        protected override string TestDir 
        {
            get { return "tests/GotDotNet/Sets/"; }
        }
        protected override string ResultsDir 
        {
            get { return "results/GotDotNet/Sets/"; }
        }   
                       
        /// <summary>
        /// Tests the following function:
        ///     set2:subset()
        /// </summary>
        [Test]
        public void SubsetTest() 
        {
            RunAndCompare("source.xml", "subset.xslt", "subset.xml");
        }                
    }
}
