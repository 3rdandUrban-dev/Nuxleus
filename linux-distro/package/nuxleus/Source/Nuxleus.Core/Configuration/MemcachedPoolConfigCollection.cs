using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Nuxleus.Configuration {

  public class MemcachedPoolConfigCollection : ConfigurationElementCollection {

    public MemcachedPoolConfig this[int index] {
      get {
        return base.BaseGet(index) as MemcachedPoolConfig;
      }
      set {
        if (base.BaseGet(index) != null) {
          base.BaseRemoveAt(index);
        }
        this.BaseAdd(index, value);
      }
    }

    protected override ConfigurationElement CreateNewElement() {
      return new MemcachedPoolConfig();
    }

    protected override object GetElementKey(ConfigurationElement element) {
      return ((MemcachedPoolConfig)element).Name;
    }
  }
}
