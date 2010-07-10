using System;
using NUnit.Framework;

namespace ExsltTest
{
    /// <summary>
    /// Collection of unit tests for GotDotNet Dates and Times module functions.
    /// </summary>
    [TestFixture]
    public class GDNDatesAndTimesTests : ExsltUnitTests
    {        
        protected override string TestDir 
        {
            get { return "tests/GotDotNet/DatesAndTimes/"; }
        }
        protected override string ResultsDir 
        {
            get { return "results/GotDotNet/DatesAndTimes/"; }
        }   
        
        /// <summary>
        /// Tests the following function:
        ///     date2:avg()
        /// </summary>
        [Test]
        public void AvgTest() 
        {
            RunAndCompare("source.xml", "avg.xslt", "avg.xml");
        } 
        
        /// <summary>
        /// Tests the following function:
        ///     date2:min()
        /// </summary>
        [Test]
        public void MinTest() 
        {
            RunAndCompare("source.xml", "min.xslt", "min.xml");
        } 

        /// <summary>
        /// Tests the following function:
        ///     date2:max()
        /// </summary>
        [Test]
        public void MaxTest() 
        {
            RunAndCompare("source.xml", "max.xslt", "max.xml");
        } 

        /// <summary>
        /// Tests the following function:
        ///     date2:day-name()
        /// </summary>
        [Test]
        public void DayNameTest() 
        {
            RunAndCompare("source.xml", "day-name.xslt", "day-name.xml");
        } 

        /// <summary>
        /// Tests the following function:
        ///     date2:day-abbreviation()
        /// </summary>
        [Test]
        public void DayAbbreviationTest() 
        {
            RunAndCompare("source.xml", "day-abbreviation.xslt", "day-abbreviation.xml");
        } 

        /// <summary>
        /// Tests the following function:
        ///     date2:month-name()
        /// </summary>
        [Test]
        public void MonthNameTest() 
        {
            RunAndCompare("source.xml", "month-name.xslt", "month-name.xml");
        } 

        /// <summary>
        /// Tests the following function:
        ///     date2:month-abbreviation()
        /// </summary>
        [Test]
        public void MonthAbbreviationTest() 
        {
            RunAndCompare("source.xml", "month-abbreviation.xslt", "month-abbreviation.xml");
        } 
    }
}
