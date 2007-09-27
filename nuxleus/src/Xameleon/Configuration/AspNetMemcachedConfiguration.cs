using System;
using System.Configuration;

namespace Xameleon.Configuration {

  public class AspNetMemcachedConfiguration : ConfigurationSection {

    public AspNetMemcachedConfiguration() { }

    public static AspNetMemcachedConfiguration GetConfig() {
      return (AspNetMemcachedConfiguration)ConfigurationManager.GetSection("Xameleon.WebApp/memcached");
    }

    [ConfigurationProperty("useCompression", DefaultValue = "no", IsRequired = false)]
    public string UseCompression {
      get {
        return this["useCompression"] as string;
      }
    }

    [ConfigurationProperty("server", IsRequired = true)]
    public MemcachedServerCollection MemcachedServerCollection {
      get {
        return this["server"] as MemcachedServerCollection;
      }
    }

    [ConfigurationProperty("poolConfig", IsRequired = true)]
    public MemcachedPoolConfig PoolConfig {
      get {
        return this["poolConfig"] as MemcachedPoolConfig;
      }
    }
  }
}
