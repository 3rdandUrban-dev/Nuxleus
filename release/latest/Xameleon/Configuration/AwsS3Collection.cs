using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Xameleon.Configuration {

  public class AwsS3Collection : ConfigurationElementCollection {

    public AwsS3 this[int index] {
      get {
        return base.BaseGet(index) as AwsS3;
      }
      set {
        if (base.BaseGet(index) != null) {
          base.BaseRemoveAt(index);
        }
        this.BaseAdd(index, value);
      }
    }

    protected override ConfigurationElement CreateNewElement() {
      return new AwsS3();
    }

    protected override object GetElementKey(ConfigurationElement element) {
      return ((AwsS3)element).Name;
    }

    [ConfigurationProperty("defaultBucket", IsRequired = true)]
    public string UseBucket {
      get {
        return this["useBucket"] as string;
      }
    }

    [ConfigurationProperty("defaultKeyPrefix", IsRequired = false)]
    public string DefaultKeyPrefix {
      get {
        return this["defaultKeyPrefix"] as string;
      }
    }

  }
}
