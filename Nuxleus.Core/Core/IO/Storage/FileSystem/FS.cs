//
// istorage.cs: Storage base interfaces
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, 3rd&Urban, LLC
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Nuxleus.PubSub {
    public class FileSystemStorage : IStorage {
        private string storagePath = null;

        public FileSystemStorage ( string storagePath ) {
            this.storagePath = storagePath;
        }

        public IStorageInfo Info ( string collection ) {
            string path = Path.Combine(this.storagePath, collection);
            return new StorageInfo(null, path, collection);
        }

        public IStorageInfo Info ( string collection, string resource ) {
            string path = Path.Combine(Path.Combine(this.storagePath, collection), resource);
            return new StorageInfo(resource, path, collection);
        }

        public StreamReader LoadEntry ( IStorageInfo info ) {
            FileInfo fio = new FileInfo(info.Key);
            if (fio.Exists) {
                return fio.OpenText();
            }

            return null;
        }

        public void SaveEntry ( IStorageInfo info, string entry ) {
            StreamWriter sw = File.CreateText(info.Key);
            sw.Write(entry);
            sw.Close();
        }

        public void DeleteEntry ( IStorageInfo info ) {
            FileInfo fio = new FileInfo(info.Key);
            if (fio.Exists) {
                fio.Delete();
            }
        }

        public FileStream LoadContent ( IStorageInfo info ) {
            FileInfo fio = new FileInfo(info.Key);
            if (fio.Exists) {
                return fio.OpenRead();
            }

            return null;
        }

        public void SaveContent ( IStorageInfo info, Byte[] content ) {
            FileStream fs = File.Create(info.Key);
            fs.Write(content, 0, content.Length);
            fs.Close();
        }

        public void DeleteContent ( IStorageInfo info ) {
            FileInfo fio = new FileInfo(info.Key);
            if (fio.Exists) {
                fio.Delete();
            }
        }

        public void Persist ( IList<IStorageInfo> infos ) { }

        public bool Exists ( IStorageInfo info ) {
            FileInfo fio = new FileInfo(info.Key);
            return fio.Exists;
        }

        public IDictionary<string, IStorageInfo> ListResources ( string collection ) {
            string path = Path.Combine(this.storagePath, collection);

            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            IDictionary<string, IStorageInfo> members = new Dictionary<string, IStorageInfo>();
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo fi in files) {
                string basename = fi.Name;
                members.Add(basename, new StorageInfo(basename, dir.FullName, collection));
            }

            return members;
        }

        public IDictionary<string, IStorageInfo> ListResources ( string collection, string ext ) {
            string path = Path.Combine(this.storagePath, collection);

            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            IDictionary<string, IStorageInfo> members = new Dictionary<string, IStorageInfo>();
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles(String.Format("*.{0}", ext), SearchOption.TopDirectoryOnly);
            foreach (FileInfo fi in files) {
                string basename = fi.Name;
                members.Add(basename, new StorageInfo(basename, dir.FullName, collection));
            }

            return members;
        }
    }
}