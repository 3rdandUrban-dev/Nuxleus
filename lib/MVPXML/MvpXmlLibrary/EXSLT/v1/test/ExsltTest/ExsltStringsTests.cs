using System;
using NUnit.Framework;

namespace ExsltTest
{
	/// <summary>
	/// Collection of unit tests for EXSLT Strings module functions.
	/// </summary>
	[TestFixture]
	public class ExsltStringsTests : ExsltUnitTests
	{        
        protected override string TestDir 
        {
            get { return "tests/EXSLT/Strings/"; }
        }
        protected override string ResultsDir 
        {
            get { return "results/EXSLT/Strings/"; }
        }                

        /// <summary>
        /// Tests the following function:
        ///     str:tokenize()
        /// </summary>
        [Test]
        public void TokenizeTest() 
        {
            RunAndCompare("source.xml", "tokenize.xslt", "tokenize.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     str:replace()
        /// </summary>
        [Test]
        public void ReplaceTest() 
        {
            RunAndCompare("source.xml", "replace.xslt", "replace.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     str:padding()
        /// </summary>
        [Test]
        public void PaddingTest() 
        {
            RunAndCompare("source.xml", "padding.xslt", "padding.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     str:align()
        /// </summary>
        [Test]
        public void AlignTest() 
        {
            RunAndCompare("source.xml", "align.xslt", "align.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     str:encode-uri()
        /// </summary>
        [Test]
        public void EncodeUriTest() 
        {
            RunAndCompare("source.xml", "encode-uri.xslt", "encode-uri.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     str:decode-uri()
        /// </summary>
        [Test]
        public void DecodeUriTest() 
        {
            RunAndCompare("source.xml", "decode-uri.xslt", "decode-uri.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     str:concat()
        /// </summary>
        [Test]
        public void ConcatTest() 
        {
            RunAndCompare("source.xml", "concat.xslt", "concat.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     str:split()
        /// </summary>
        [Test]
        public void SplitTest() 
        {
            RunAndCompare("source.xml", "split.xslt", "split.xml");
        }
	}
}
