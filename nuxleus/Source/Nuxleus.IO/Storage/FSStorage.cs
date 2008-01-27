//
// fsstorage.cs: Storage base interfaces
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 

using System;
using System.IO;

namespace Nuxleus.Storage {
    public class FileSystemStorageResourceInfo {
        private FileInfo target = null;

        public FileSystemStorageResourceInfo () { }

        /// <summary>
        /// Get or sets FileInfo to be used when the data has to be stored on the filesystem
        /// </summary>
        /// <value>FileInfo instance.</value>
        public FileInfo Target { get { return target; } set { target = value; } }

    }
}