using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Nuxleus.Configuration {

  public class BungeeApp : ConfigurationElement {

    [ConfigurationProperty("name", IsRequired = true)]
    public string Name {
      get {
        return this["name"] as string;
      }
    }

    [ConfigurationProperty("version", IsRequired = true)]
    public string Version {
      get {
        return this["version"] as string;
      }
    }

    [ConfigurationProperty("deployID", IsRequired = true)]
    public string DeployID {
      get {
        return this["deployID"] as string;
      }
    }

    [ConfigurationProperty("deployURL", IsRequired = true)]
    public string DeployURL {
      get {
        return this["deployURL"] as string;
      }
    }

    [ConfigurationProperty("z", IsRequired = true)]
    public string Z {
      get {
        return this["z"] as string;
      }
    }

    [ConfigurationProperty("elementID", IsRequired = true)]
    public string ElementID {
      get {
        return this["elementID"] as string;
      }
    }
  }
}
