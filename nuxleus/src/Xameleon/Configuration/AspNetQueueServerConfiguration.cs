using System;
using System.Configuration;

namespace Xameleon.Configuration {

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
        return Convert.ToInt32(this["port"] as string);
      }
    }
  }
}
