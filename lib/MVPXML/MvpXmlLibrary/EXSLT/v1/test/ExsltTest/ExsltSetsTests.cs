using System;
using NUnit.Framework;

namespace ExsltTest
{
    /// <summary>
    /// Collection of unit tests for EXSLT Sets module functions.
    /// </summary>
    [TestFixture]
    public class ExsltSetsTests : ExsltUnitTests
    {        

        protected override string TestDir 
        {
            get { return "tests/EXSLT/Sets/"; }
        }
        protected override string ResultsDir 
        {
            get { return "results/EXSLT/Sets/"; }
        }   
                       
        /// <summary>
        /// Tests the following function:
        ///     set:difference()
        /// </summary>
        [Test]
        public void DifferenceTest() 
        {
            RunAndCompare("source.xml", "difference.xslt", "difference.xml");
        }
        
        /// <summary>
        /// Tests the following function:
        ///     set:intersection()
        /// </summary>
        [Test]
        public void IntersectionTest() 
        {
            RunAndCompare("source.xml", "intersection.xslt", "intersection.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     set:distinct()
        /// </summary>
        [Test]
        public void DistinctTest() 
        {
            RunAndCompare("source.xml", "distinct.xslt", "distinct.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     set:has-same-node()
        /// </summary>
        [Test]
        public void HasSameNodeTest() 
        {
            RunAndCompare("source.xml", "has-same-node.xslt", "has-same-node.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     set:leading()
        /// </summary>
        [Test]
        public void LeadingTest() 
        {
            RunAndCompare("source.xml", "leading.xslt", "leading.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     set:trailing()
        /// </summary>
        [Test]
        public void TrailingTest() 
        {
            RunAndCompare("source.xml", "trailing.xslt", "trailing.xml");
        }        
    }
}
