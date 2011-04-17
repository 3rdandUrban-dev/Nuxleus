using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Nuxleus.Configuration {

  public class AwsKeyCollection : ConfigurationElementCollection {

    public AwsKey this[int index] {
      get {
        return base.BaseGet(index) as AwsKey;
      }
      set {
        if (base.BaseGet(index) != null) {
          base.BaseRemoveAt(index);
        }
        this.BaseAdd(index, value);
      }
    }

    protected override ConfigurationElement CreateNewElement() {
      return new AwsKey();
    }

    protected override object GetElementKey(ConfigurationElement element) {
      return ((AwsKey)element).Name;
    }

    [ConfigurationProperty("externalFile", IsRequired = false)]
    public string ExternalFile {
      get {
        return this["externalFile"] as string;
      }
    }
  }
}
