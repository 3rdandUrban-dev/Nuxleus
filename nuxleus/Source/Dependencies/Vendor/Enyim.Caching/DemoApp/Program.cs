using System;
using System.Collections.Generic;
using System.Text;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using System.Net;

namespace DemoApp
{
	class Program
	{
		static void Main(string[] args)
		{
			// create a MemcachedClient
			// in your application you can cache the client in a static variable or just recreate it every time
			MemcachedClient mc = new MemcachedClient();

			ServerStats ms = mc.Stats();

			// store a string in the cache
			mc.Store(StoreMode.Set, "MyKey", "Hello World");

			// retrieve the item from the cache
			Console.WriteLine(mc.Get("MyKey"));

			// store some other items
			mc.Store(StoreMode.Set, "D1", 1234L);
			mc.Store(StoreMode.Set, "D2", DateTime.Now);
			mc.Store(StoreMode.Set, "D3", true);
			mc.Store(StoreMode.Set, "D4", new Product());

			Console.WriteLine("D1: {0}", mc.Get("D1"));
			Console.WriteLine("D2: {0}", mc.Get("D2"));
			Console.WriteLine("D3: {0}", mc.Get("D3"));
			Console.WriteLine("D4: {0}", mc.Get("D4"));

			// delete them from the cache
			mc.Remove("D1");
			mc.Remove("D2");
			mc.Remove("D3");
			mc.Remove("D4");

			// add an item which is valid for 10 mins
			mc.Store(StoreMode.Set, "D4", new Product(), new TimeSpan(0, 10, 0));

			Console.ReadLine();
		}

		// objects must be serializable to be able to store them in the cache
		[Serializable]
		class Product
		{
			public double Price = 1.24;
			public string Name = "Mineral Water";

			public override string ToString()
			{
				return String.Format("Product {{{0}: {1}}}", this.Name, this.Price);
			}
		}
	}
}
