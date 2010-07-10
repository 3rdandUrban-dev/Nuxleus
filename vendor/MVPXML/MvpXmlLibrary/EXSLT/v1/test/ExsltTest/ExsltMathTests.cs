using System;
using NUnit.Framework;

namespace ExsltTest
{
    /// <summary>
    /// Collection of unit tests for EXSLT Math module functions.
    /// </summary>
    [TestFixture]
    public class ExsltMathTests : ExsltUnitTests
    {        

        protected override string TestDir 
        {
            get { return "tests/EXSLT/Math/"; }
        }
        protected override string ResultsDir 
        {
            get { return "results/EXSLT/Math/"; }
        }   
                       
        /// <summary>
        /// Tests the following function:
        ///     math:min()
        /// </summary>
        [Test]
        public void MinTest() 
        {
            RunAndCompare("source.xml", "min.xslt", "min.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:max()
        /// </summary>
        [Test]
        public void MaxTest() 
        {
            RunAndCompare("source.xml", "max.xslt", "max.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:highest()
        /// </summary>
        [Test]
        public void HighestTest() 
        {
            RunAndCompare("source.xml", "highest.xslt", "highest.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:lowest()
        /// </summary>
        [Test]
        public void LowestTest() 
        {
            RunAndCompare("source.xml", "lowest.xslt", "lowest.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:abs()
        /// </summary>
        [Test]
        public void AbsTest() 
        {
            RunAndCompare("source.xml", "abs.xslt", "abs.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:sqrt()
        /// </summary>
        [Test]
        public void SqrtTest() 
        {
            RunAndCompare("source.xml", "sqrt.xslt", "sqrt.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:power()
        /// </summary>
        [Test]
        public void PowerTest() 
        {
            RunAndCompare("source.xml", "power.xslt", "power.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:constant()
        /// </summary>
        [Test]
        public void ConstantTest() 
        {
            RunAndCompare("source.xml", "constant.xslt", "constant.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:log()
        /// </summary>
        [Test]
        public void LogTest() 
        {
            RunAndCompare("source.xml", "log.xslt", "log.xml");
        }            
    
        /// <summary>
        /// Tests the following function:
        ///     math:random()
        /// </summary>
        [Test]
        public void RandomTest() 
        {
            RunAndCompare("source.xml", "random.xslt", "random.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:sin()
        /// </summary>
        [Test]
        public void SinTest() 
        {
            RunAndCompare("source.xml", "sin.xslt", "sin.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:cos()
        /// </summary>
        [Test]
        public void CosTest() 
        {
            RunAndCompare("source.xml", "cos.xslt", "cos.xml");
        }            
        
        /// <summary>
        /// Tests the following function:
        ///     math:tan()
        /// </summary>
        [Test]
        public void TanTest() 
        {
            RunAndCompare("source.xml", "tan.xslt", "tan.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:asin()
        /// </summary>
        [Test]
        public void AsinTest() 
        {
            RunAndCompare("source.xml", "asin.xslt", "asin.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:acos()
        /// </summary>
        [Test]
        public void AcosTest() 
        {
            RunAndCompare("source.xml", "acos.xslt", "acos.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:atan()
        /// </summary>
        [Test]
        public void AtanTest() 
        {
            RunAndCompare("source.xml", "atan.xslt", "atan.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:atan2()
        /// </summary>
        [Test]
        public void Atan2Test() 
        {
            RunAndCompare("source.xml", "atan2.xslt", "atan2.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:exp()
        /// </summary>
        [Test]
        public void ExpTest() 
        {
            RunAndCompare("source.xml", "exp.xslt", "exp.xml");
        }            
    }
}
