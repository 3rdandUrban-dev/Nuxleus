using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using System.Threading;
namespace MemcachedTest
{
	/// <summary>
	///This is a test class for Enyim.Caching.MemcachedClient and is intended
	///to contain all Enyim.Caching.MemcachedClient Unit Tests
	///</summary>
	[TestClass]
	public class MemcachedClientTest
	{
		public const string TestObjectKey = "Hello_World";

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get { return testContextInstance; }
			set { testContextInstance = value; }
		}

		[global::System.Serializable]
		public class TestData
		{
			public TestData() { }

			public string FieldA;
			public string FieldB;
			public int FieldC;
			public bool FieldD;
		}

		/// <summary>
		///A test for Store (StoreMode, string, byte[], int, int)
		///</summary>
		[TestMethod]
		public void StoreObjectTest()
		{
			TestData td = new TestData();
			td.FieldA = "Hello";
			td.FieldB = "World";
			td.FieldC = 19810619;
			td.FieldD = true;

			MemcachedClient client = new MemcachedClient();

			client.Store(StoreMode.Set, TestObjectKey, td);
		}

		[TestMethod]
		public void GetObjectTest()
		{
			TestData td = new MemcachedClient().Get<TestData>(TestObjectKey);

			Assert.IsNotNull(td, "Get returned null.");
			Assert.AreEqual(td.FieldA, "Hello", "Object was corrupted.");
			Assert.AreEqual(td.FieldB, "World", "Object was corrupted.");
			Assert.AreEqual(td.FieldC, 19810619, "Object was corrupted.");
			Assert.AreEqual(td.FieldD, true, "Object was corrupted.");
		}

		[TestMethod]
		public void DeleteObjectTest()
		{
			MemcachedClient mc = new MemcachedClient();
			mc.Remove(TestObjectKey);

			Assert.IsNull(mc.Get(TestObjectKey), "Remove failed.");
		}

		[TestMethod]
		public void StoreStringTest()
		{
			MemcachedClient mc = new MemcachedClient();
			mc.Store(StoreMode.Set, "TestString", "Hello world!");

			Assert.AreEqual("Hello world!", mc.Get<string>("TestString"));
		}

		[TestMethod]
		public void StoreLongTest()
		{
			MemcachedClient mc = new MemcachedClient();
			mc.Store(StoreMode.Set, "TestLong", 65432123456L);

			Assert.AreEqual(65432123456L, mc.Get<long>("TestLong"));
		}

		[TestMethod]
		public void StoreArrayTest()
		{
			byte[] bigBuffer = new byte[200 * 1024];

			for (int i = 0; i < bigBuffer.Length / 256; i++)
			{
				for (int j = 0; j < 256; j++)
				{
					bigBuffer[i * 256 + j] = (byte)j;
				}
			}

			MemcachedClient mc = new MemcachedClient();

			mc.Store(StoreMode.Set, "BigBuffer", bigBuffer);

			byte[] bigBuffer2 = mc.Get<byte[]>("BigBuffer");

			for (int i = 0; i < bigBuffer.Length / 256; i++)
			{
				for (int j = 0; j < 256; j++)
				{
					if (bigBuffer2[i * 256 + j] != (byte)j)
					{
						Assert.AreEqual(j, bigBuffer[i * 256 + j], "Data should be {0} but its {1}");
						break;
					}
				}
			}
		}

		[TestMethod]
		public void ExpirationTest()
		{
			MemcachedClient mc = new MemcachedClient();

			mc.Store(StoreMode.Set, "ExpirationTest:TimeSpan", "ExpirationTest:TimeSpan", new TimeSpan(0, 0, 5));

			Assert.AreEqual("ExpirationTest:TimeSpan", mc.Get("ExpirationTest:TimeSpan"), "Expires:Timespan store failed");

			Thread.Sleep(8000);
			Assert.IsNull(mc.Get("ExpirationTest:TimeSpan"), "ExpirationTest:TimeSpan item did not expire");

			DateTime expiresAt = DateTime.Now.AddSeconds(5);

			mc.Store(StoreMode.Set, "Expires:DateTime", "Expires:DateTime", expiresAt);

			Assert.AreEqual("Expires:DateTime", mc.Get("Expires:DateTime"), "Expires:DateTime store failed");

			expiresAt = expiresAt.AddSeconds(4); // wait more than the expiration

			while (DateTime.Now < expiresAt)
			{
				Thread.Sleep(100);
			}

			object o = mc.Get("Expires:DateTime");

			Assert.IsNull(o, "Expires:DateTime item did not expire");
		}

		[TestMethod]
		public void AddSetReplaceTest()
		{
			MemcachedClient mc = new MemcachedClient();
			mc.Store(StoreMode.Set, "VALUE", "1");

			Assert.AreEqual("1", mc.Get("VALUE"), "Store failed");

			mc.Store(StoreMode.Add, "VALUE", "2");
			Assert.AreEqual("1", mc.Get("VALUE"), "Item should not have been Added");

			mc.Store(StoreMode.Replace, "VALUE", "4");
			Assert.AreEqual("4", mc.Get("VALUE"), "Replace failed");

			mc.Remove("VALUE");

			mc.Store(StoreMode.Replace, "VALUE", "8");
			Assert.IsNull(mc.Get("VALUE"), "Item should not have been Replaced");

			mc.Remove("VALUE");

			mc.Store(StoreMode.Add, "VALUE", "16");
			Assert.AreEqual("16", mc.Get("VALUE"), "Add failed");
		}

		//[TestMethod]
		//public void AbsolutExpirationTest()
		//{
		//    Assert.Fail("Implement the test.");
		//}

		//[TestMethod]
		//public void SlidingExpirationTest()
		//{
		//    Assert.Fail("Implement the test.");
		//}

		[TestMethod]
		public void IncrementTest()
		{
			MemcachedClient mc = new MemcachedClient();
			mc.Store(StoreMode.Set, "VALUE", "100");

			Assert.AreEqual(102L, mc.Increment("VALUE", 2));
			Assert.AreEqual(112L, mc.Increment("VALUE", 10));
		}

		[TestMethod]
		public void DecrementTest()
		{
			MemcachedClient mc = new MemcachedClient();
			mc.Store(StoreMode.Set, "VALUE", "100");

			Assert.AreEqual(98L, mc.Decrement("VALUE", 2));
			Assert.AreEqual(88L, mc.Decrement("VALUE", 10));
		}

		[TestMethod]
		public void FlushTest()
		{
			MemcachedClient mc = new MemcachedClient();
			mc.Store(StoreMode.Set, "qwer", "1");
			mc.Store(StoreMode.Set, "tyui", "1");
			mc.Store(StoreMode.Set, "polk", "1");
			mc.Store(StoreMode.Set, "mnbv", "1");
			mc.Store(StoreMode.Set, "zxcv", "1");
			mc.Store(StoreMode.Set, "gfsd", "1");

			Assert.AreEqual("1", mc.Get("mnbv"), "Setup for FlushAll() failed");

			mc.FlushAll();

			Assert.IsNull(mc.Get("qwer"), "FlushAll() failed.");
			Assert.IsNull(mc.Get("tyui"), "FlushAll() failed.");
			Assert.IsNull(mc.Get("polk"), "FlushAll() failed.");
			Assert.IsNull(mc.Get("mnbv"), "FlushAll() failed.");
			Assert.IsNull(mc.Get("zxcv"), "FlushAll() failed.");
			Assert.IsNull(mc.Get("gfsd"), "FlushAll() failed.");
		}
	}
}
