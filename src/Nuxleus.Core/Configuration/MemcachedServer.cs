using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Nuxleus.Configuration {

  public class MemcachedServer : ConfigurationElement {

    [ConfigurationProperty("name", IsRequired = true)]
    public string Name {
      get {
        return this["name"] as string;
      }
    }

    [ConfigurationProperty("ip", IsRequired = true)]
    public string IP {
      get {
        return this["ip"] as string;
      }
    }

    [ConfigurationProperty("port", IsRequired = true)]
    public string Port {
      get {
        return this["port"] as string;
      }
    }

  }
}
