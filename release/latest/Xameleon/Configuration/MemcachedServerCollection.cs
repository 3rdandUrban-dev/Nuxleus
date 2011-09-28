using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Xameleon.Configuration {

  public class MemcachedServerCollection : ConfigurationElementCollection {

    public MemcachedServer this[int index] {
      get {
        return base.BaseGet(index) as MemcachedServer;
      }
      set {
        if (base.BaseGet(index) != null) {
          base.BaseRemoveAt(index);
        }
        this.BaseAdd(index, value);
      }
    }

    protected override ConfigurationElement CreateNewElement() {
      return new MemcachedServer();
    }

    protected override object GetElementKey(ConfigurationElement element) {
      return ((MemcachedServer)element).Name;
    }
  }
}
