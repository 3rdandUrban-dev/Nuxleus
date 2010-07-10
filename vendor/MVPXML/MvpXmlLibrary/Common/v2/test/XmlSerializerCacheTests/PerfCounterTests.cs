#region Using directives

using System;
using System.Collections;
using System.Text;
using NUnit.Framework;
using Mvp.Xml.Common.Serialization;
using System.Xml.Serialization;
using System.Diagnostics;

#endregion

namespace Mvp.Xml.Serialization.Tests
{
	[TestFixture]
	public class PerfCounterTests
	{
		public PerfCounterTests()
		{

		}

		[Test]
		public void BothCounters()
		{
			using (XmlSerializerCache cache = new XmlSerializerCache())
			{
				string instanceName = PerfCounterManagerTests.GetCounterInstanceName(0);
				using (PerformanceCounter instanceCounter = new PerformanceCounter(PerfCounterManagerTests.CATEGORY
					, PerfCounterManagerTests.CACHED_INSTANCES_NAME
					, instanceName
					, true))
				{
					Assert.AreEqual(0, instanceCounter.RawValue);
					using (PerformanceCounter hitCounter = new PerformanceCounter(PerfCounterManagerTests.CATEGORY
						, PerfCounterManagerTests.SERIALIZER_HITS_NAME
						, instanceName
						, true))
					{
						Assert.AreEqual(0, hitCounter.RawValue);
						XmlRootAttribute root = new XmlRootAttribute( "theRoot" );
						XmlSerializer ser = cache.GetSerializer(typeof(SerializeMe), root);

						Assert.AreEqual(1, instanceCounter.RawValue);
						Assert.AreEqual(0, hitCounter.RawValue);
						ser = cache.GetSerializer(typeof(SerializeMe), root);

						Assert.AreEqual(1, instanceCounter.RawValue);
						Assert.AreEqual(1, hitCounter.RawValue);

					}
				}
			}
		}


	}
}
