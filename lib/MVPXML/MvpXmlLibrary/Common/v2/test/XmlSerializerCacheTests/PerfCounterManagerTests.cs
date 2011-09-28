#region Using directives

using System;
using System.Collections;
using System.Text;
using NUnit.Framework;
using Mvp.Xml.Common.Serialization;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace Mvp.Xml.Serialization.Tests
{
	[TestFixture]
	public class PerfCounterManagerTests
	{
		public PerfCounterManagerTests()
		{

		}

		internal const string CATEGORY = "Mvp.Xml.XmlSerializerCache";
		internal const string CATEGORY_DESCRIPTION = "Performance counters to instrument the classes from the Mvp.Xml project.";
		internal const string SERIALIZER_HITS_NAME = "Cache Hits";
		internal const string SERIALIZER_HITS_DESCRIPTION = "Number of times instances were retrieved from the serializer cache.";
		internal const string CACHED_INSTANCES_NAME = "Cached Instances";
		internal const string CACHED_INSTANCES_DESCRIPTION = "Number of XmlSerializer instances in the cache.";

		internal static string GetCounterInstanceName(int index)
		{
			string fileName = Environment.CommandLine.Split(' ')[0];
			foreach (char c in System.IO.Path.GetInvalidPathChars())
			{
				fileName = fileName.Replace(c, '%');
			}
			fileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
			System.Diagnostics.Debug.WriteLine(String.Format("File name is: {0}", fileName));
			string name = String.Format("{0}-{1}#{2}"
					, fileName
					, AppDomain.CurrentDomain.FriendlyName
					, index);
			return name;	
		}

		[Test]
		public void CacheHit()
		{
			string instanceName = GetCounterInstanceName(0);
			using (PerfCounterManager manager = new PerfCounterManager())
			{
				using (PerformanceCounter counter = new PerformanceCounter(CATEGORY
					, SERIALIZER_HITS_NAME
					, instanceName
					, true))
				{
					Assert.IsNotNull(counter);
					Assert.AreEqual(0.0, (decimal)counter.NextValue());

					manager.IncrementHitCount();

					Assert.AreEqual(1.0, (decimal)counter.NextValue());
				}
			}

			using (PerformanceCounter counter = new PerformanceCounter(CATEGORY
				, SERIALIZER_HITS_NAME
				, instanceName
				, true))
			{
				try
				{
					long l = counter.RawValue;
				}
				catch (InvalidOperationException)
				{
					// the counter instance went away when we 
					// disposed the manager object.
					// This exception is what we wanted.
				}	
			}
		}

		[Test]
		public void InstanceCount()
		{
			string instanceName = GetCounterInstanceName(0);
			using (PerfCounterManager manager = new PerfCounterManager())
			{
				using (PerformanceCounter counter = new PerformanceCounter(CATEGORY
					, CACHED_INSTANCES_NAME
					, instanceName
					, true))
				{
					Assert.IsNotNull(counter);
					Assert.AreEqual(0.0, (decimal)counter.NextValue());

					manager.IncrementInstanceCount();

					Assert.AreEqual(1.0, (decimal)counter.NextValue());
				}
			}

			using (PerformanceCounter counter = new PerformanceCounter(CATEGORY
				, CACHED_INSTANCES_NAME
				, instanceName
				, true))
			{
				try
				{
					long l = counter.RawValue;
				}
				catch (InvalidOperationException)
				{
					// the counter instance went away when we 
					// disposed the manager object.
					// This exception is what we wanted.
				}
			}
		}

	}
}
