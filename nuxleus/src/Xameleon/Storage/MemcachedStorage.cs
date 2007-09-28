//
// fsstorage.cs: Storage base interfaces
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, Sylvain Hellegouarch
// 

using System;
using System.IO;
using Xameleon.Memcached;

namespace Xameleon.Storage {
  public class MemcachedStorageResourceInfo {
    Xameleon.Memcached.Client client = null;
    string key = null;

    public MemcachedStorageResourceInfo() {}

    // <summary>
    /// Get or sets the memcached client instance if the data has to be stored in memcached
    /// </summary>
    /// <value>Xameleon.Memcached.Client instance.</value>
    public Xameleon.Memcached.Client Client { get { return client; } set { client = value; } }

    /// <summary>
    /// Gets or sets the key to be used when storing into memcached
    /// </summary>
    /// <value>An identifier for the memcached servers.</value>
    public string Key { get { return key; } set { key = value; } }

  }
}