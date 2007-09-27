//
// storemanager.cs: Manage the underlying storages
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
using System.Text;
using Xameleon.Atom;

namespace Xameleon.Amplee
{
    public struct StoreManager
    {
        IStorage _memberStorage;
        IStorage _mediaStorage;

        public StoreManager(params string[] stringArray)
        {
            _mediaStorage = null;
            _memberStorage = null;
        }

        public IStorage MemberStorage { get { return _memberStorage; } set { _memberStorage = value; } }
        public IStorage MediaStorage { get { return _mediaStorage; } set { _mediaStorage = value; } }
    }
}