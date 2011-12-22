//
// istorage.cs: Storage base interfaces
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 

using System;
using System.IO;
using System.Text;

namespace Nuxleus.Storage {
    public delegate void StoreResource ( StorageResourceInfo info );

    public class Store {
        private Store () { }

        public static void Process ( StorageResourceInfo info ) {
            if (info.FileResourceInfo != null) {
                FileStream fs = info.FileResourceInfo.Target.OpenWrite();
                fs.Write(info.Data, 0, info.Data.Length);
            }

            if (info.MemcachedResourceInfo != null) {
                info.MemcachedResourceInfo.Client.Store(Enyim.Caching.Memcached.StoreMode.Set, info.MemcachedResourceInfo.Key, info.Data);
            }
        }
    }

    public class StorageResourceInfo {
        private FileSystemStorageResourceInfo fileInfo = null;
        private MemcachedStorageResourceInfo memInfo = null;
        private byte[] data = null;

        public StorageResourceInfo () { }

        public FileSystemStorageResourceInfo FileResourceInfo {
            get { return fileInfo; }
            set { fileInfo = value; }
        }

        public MemcachedStorageResourceInfo MemcachedResourceInfo {
            get { return memInfo; }
            set { memInfo = value; }
        }

        /// <summary>
        /// Gets or sets the actual data to be stored
        /// </summary>
        /// <value>bytes representing the content.</value>
        public byte[] Data { get { return data; } set { data = value; } }

        public void DataFromString ( string data ) {
            this.Data = Encoding.UTF8.GetBytes(data);
        }

    }
}