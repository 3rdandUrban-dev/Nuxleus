using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Xameleon.Configuration {

  public class XsltParamCollection : ConfigurationElementCollection {

    public XsltParam this[int index] {
      get {
        return base.BaseGet(index) as XsltParam;
      }
      set {
        if (base.BaseGet(index) != null) {
          base.BaseRemoveAt(index);
        }
        this.BaseAdd(index, value);
      }
    }

    protected override ConfigurationElement CreateNewElement() {
      return new XsltParam();
    }

    protected override object GetElementKey(ConfigurationElement element) {
      return ((XsltParam)element).Name;
    }

  }
}
