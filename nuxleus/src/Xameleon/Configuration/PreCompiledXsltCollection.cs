using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Xameleon.Configuration {

  public class PreCompiledXsltCollection : ConfigurationElementCollection {

    public PreCompiledXslt this[int index] {
      get {
        return base.BaseGet(index) as PreCompiledXslt;
      }
      set {
        if (base.BaseGet(index) != null) {
          base.BaseRemoveAt(index);
        }
        this.BaseAdd(index, value);
      }
    }

    protected override ConfigurationElement CreateNewElement() {
      return new PreCompiledXslt();
    }

    protected override object GetElementKey(ConfigurationElement element) {
      return ((PreCompiledXslt)element).Name;
    }

    [ConfigurationProperty("base-uri", IsRequired = false)]
    public string BaseUri {
      get {
        return this["base-uri"] as string;
      }
    }
  }
}
