using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Nuxleus.Configuration {

  public class AwsKey : ConfigurationElement {

    [ConfigurationProperty("name", IsRequired = true)]
    public string Name {
      get {
        return this["name"] as string;
      }
    }

    [ConfigurationProperty("public-key", IsRequired = true)]
    public string PublicKey {
      get {
        return this["public"] as string;
      }
    }

    [ConfigurationProperty("private-key", IsRequired = true)]
    public string PrivateKey {
      get {
        return this["private"] as string;
      }
    }
  }
}
