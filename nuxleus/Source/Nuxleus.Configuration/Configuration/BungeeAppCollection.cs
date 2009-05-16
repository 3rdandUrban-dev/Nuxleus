using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Nuxleus.Configuration {

  public class BungeeAppCollection : ConfigurationElementCollection {

    public BungeeApp this[int index] {
      get {
        return base.BaseGet(index) as BungeeApp;
      }
      set {
        if (base.BaseGet(index) != null) {
          base.BaseRemoveAt(index);
        }
        this.BaseAdd(index, value);
      }
    }

    protected override ConfigurationElement CreateNewElement() {
      return new BungeeApp();
    }

    protected override object GetElementKey(ConfigurationElement element) {
      return ((BungeeApp)element).Name;
    }

    [ConfigurationProperty("defaultAppVersion", IsRequired = true)]
    public string DefaultAppVersion {
      get {
        return this["defaultAppVersion"] as string;
      }
    }
  }
}
