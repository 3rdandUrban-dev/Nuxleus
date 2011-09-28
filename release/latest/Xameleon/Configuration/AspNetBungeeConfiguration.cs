using System;
using System.Configuration;

namespace Xameleon.Configuration {

  public class AspNetBungeeAppConfiguration : ConfigurationSection {


    public static AspNetBungeeAppConfiguration GetConfig() {
      return ConfigurationManager.GetSection("Xameleon.WebApp/bungee") as AspNetBungeeAppConfiguration;
    }

    [ConfigurationProperty("application", IsRequired = true)]
    public BungeeAppCollection BungeeAppCollection {
      get {
        return this["application"] as BungeeAppCollection;
      }
    }
  }
}
