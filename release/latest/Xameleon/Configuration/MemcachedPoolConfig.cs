using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Xameleon.Configuration {

  public class MemcachedPoolConfig : ConfigurationElement {

    [ConfigurationProperty("name", IsRequired = false)]
    public string Name {
      get {
        return this["name"] as string;
      }
    }

    [ConfigurationProperty("initConnections", DefaultValue = (int)3, IsRequired = false)]
    public int InitConnections {
      get {
        return (int)this["initConnections"];
      }
    }

    [ConfigurationProperty("minConnections", DefaultValue = (int)3, IsRequired = false)]
    public int MinConnections {
      get {
        return (int)this["minConnections"];
      }
    }

    [ConfigurationProperty("maxConnections", DefaultValue = (int)5, IsRequired = false)]
    public int MaxConnections {
      get {
        return (int)this["maxConnections"];
      }
    }

    [ConfigurationProperty("socketConnectTimeout", DefaultValue = (int)1000, IsRequired = false)]
    public int SocketConnectTimeout {
      get {
        return (int)this["socketConnectTimeout"];
      }
    }

    [ConfigurationProperty("socketConnect", DefaultValue = (int)3000, IsRequired = false)]
    public int SocketConnect {
      get {
        return (int)this["socketConnect"];
      }
    }

    [ConfigurationProperty("maintenanceSleep", DefaultValue = (int)30, IsRequired = false)]
    public int MaintenanceSleep {
      get {
        return (int)this["maintenanceSleep"];
      }
    }

    [ConfigurationProperty("failover", DefaultValue = (bool)false, IsRequired = false)]
    public bool Failover {
      get {
        return (bool)this["failover"];
      }
    }

    [ConfigurationProperty("nagle", DefaultValue = (bool)true, IsRequired = false)]
    public bool Nagle {
      get {
        return (bool)this["nagle"];
      }
    }
  }
}
