using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Xameleon.Configuration {

  public class BaseSetting : ConfigurationElement {

    [ConfigurationProperty("key", IsRequired = true)]
    public string Key {
      get {
        return this["key"] as string;
      }
    }

    [ConfigurationProperty("value", IsRequired = true)]
    public string Value {
      get {
        return this["value"] as string;
      }
    }
  }
}
