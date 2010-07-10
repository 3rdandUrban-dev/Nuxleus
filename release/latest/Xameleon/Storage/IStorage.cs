//
// istorage.cs: Storage base interfaces
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, Sylvain Hellegouarch
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Xameleon.Amplee
{
    public interface IStorageInfo
    {
        string Name { get; set; }
        string Key { get; set; }
        string Collection { get; set; }
    }

    public class StorageInfo : IStorageInfo
    {
        private string name = null;
        private string key = null;
        private string collection = null;

        public StorageInfo() { }

        public StorageInfo(string name, string key, string collection)
        {
            this.name = name;
            this.key = key;
            this.collection = collection;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public string Key
        {
            get
            {
                return this.key;
            }
            set
            {
                this.key = value;
            }
        }

        public string Collection
        {
            get
            {
                return this.collection;
            }
            set
            {
                this.collection = value;
            }
        }
    }

    public interface IStorage
    {
        IStorageInfo Info(string collection);
        IStorageInfo Info(string collection, string resource);

        StreamReader LoadEntry(IStorageInfo info);
        void SaveEntry(IStorageInfo info, string entry);
        void DeleteEntry(IStorageInfo info);

        FileStream LoadContent(IStorageInfo info);
        void SaveContent(IStorageInfo info, Byte[] content);
        void DeleteContent(IStorageInfo info);

        void Persist(IList<IStorageInfo> infos);

        bool Exists(IStorageInfo info);

        IDictionary<string, IStorageInfo> ListResources(string collection);
        IDictionary<string, IStorageInfo> ListResources(string collection, string ext);
    }
}