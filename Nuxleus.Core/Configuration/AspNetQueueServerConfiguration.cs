using System;
using System.Configuration;

namespace Nuxleus.Configuration {

  public class AspNetQueueServerConfiguration : ConfigurationSection {


    public static AspNetQueueServerConfiguration GetConfig() {
      return (AspNetQueueServerConfiguration)ConfigurationManager.GetSection("Xameleon.WebApp/queueserver");
    }

    [ConfigurationProperty("ip", IsRequired = true)]
    public string IP {
      get {
        return this["ip"] as string;
      }
    }

    [ConfigurationProperty("port", IsRequired = true)]
    public int Port {
      get {
        return (int)this["port"];
      }
    }

    [ConfigurationProperty("poolSize", IsRequired = false,
			   DefaultValue=5)]
    public int PoolSize {
      get {
        return (int)this["poolSize"];
      }
    }

    [ConfigurationProperty("threshold", IsRequired = false,
			   DefaultValue=10)]
    public int Threshold {
      get {
        return (int)this["threshold"];
      }
    }
  }
}
