using System;
using NUnit.Framework;

namespace ExsltTest
{
    /// <summary>
    /// Collection of unit tests for EXSLT RegularExpressions module functions.
    /// </summary>
    [TestFixture]
    public class ExsltRegularExpressionsTests : ExsltUnitTests
    {        

        protected override string TestDir 
        {
            get { return "tests/EXSLT/RegularExpressions/"; }
        }
        protected override string ResultsDir 
        {
            get { return "results/EXSLT/RegularExpressions/"; }
        }   
                       
        /// <summary>
        /// Tests the following function:
        ///     regexp:test()
        /// </summary>
        [Test]
        public void TestTest() 
        {
            RunAndCompare("source.xml", "test.xslt", "test.xml");
        }   
         
        /// <summary>
        /// Tests the following function:
        ///     regexp:match()
        /// </summary>
        [Test]
        public void MatchTest() 
        {
            RunAndCompare("source.xml", "match.xslt", "match.xml");
        }   

        /// <summary>
        /// Tests the following function:
        ///     regexp:replace()
        /// </summary>
        [Test]
        public void ReplaceTest() 
        {
            RunAndCompare("source.xml", "replace.xslt", "replace.xml");
        }   
    }
}
