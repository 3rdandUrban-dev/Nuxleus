using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Xameleon.Configuration {

  public class XsltParam : ConfigurationElement {

    [ConfigurationProperty("name", IsRequired = true)]
    public string Name {
      get {
        return this["name"] as string;
      }
    }

    [ConfigurationProperty("select", IsRequired = true)]
    public string Select {
      get {
        return this["select"] as string;
      }
    }
  }
}
