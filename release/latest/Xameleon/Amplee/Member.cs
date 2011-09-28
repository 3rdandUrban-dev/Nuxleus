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
using System.Xml;
using System.Text;
using Xameleon.Atom;
using Xameleon.Cryptography;

namespace Xameleon.Amplee
{
    public interface IMember
    {
        AtomEntry Entry { get; }
        FileStream Content { get; }
        string MediaType { get; set; }
        string MemberId { get; set; }
        string MediaId { get; set; }
        bool Draft { get; set; }
        AtomPubCollection Collection { get; set; }
    }

    public class Member : IMember
    {
        private AtomEntry entry = null;
        private string mediatype = null;
        private string mediaId = null;
        private string memberId = null;
        private bool draft = false;
        private AtomPubCollection collection = null;

        public Member(AtomPubCollection collection)
        {
            this.collection = collection;
        }

        public AtomEntry Entry
        {
            get
            {
                return this.entry;
            }
            set
            {
                this.entry = value;
            }
        }

        public FileStream Content
        {
            get
            {
                return this.Collection.GetContent(this.Collection.GetContentInfo(this.MediaId));
            }
        }

        public string MediaType
        {
            get
            {
                return this.mediatype;
            }
            set
            {
                this.mediatype = value;
            }
        }

        public string MemberId
        {
            get
            {
                return this.memberId;
            }
            set
            {
                this.memberId = value;
            }
        }

        public string MediaId
        {
            get
            {
                return this.mediaId;
            }
            set
            {
                this.mediaId = value;
            }
        }

        public bool Draft
        {
            get
            {
                return this.draft;
            }
            set
            {
                this.draft = value;
            }
        }

        public AtomPubCollection Collection
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

        public void LoadEntry()
        {
            IStorageInfo info = this.Collection.GetMetadataInfo(this.MemberId);
            string xml = this.Collection.GetMetadata(info).ReadToEnd();

            if (this.Entry == null)
                this.Entry = new AtomEntry();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            this.Entry.Document = doc;
        }

        public static IMember FromEntry(AtomPubCollection collection, XmlDocument doc)
        {
            Member mb = new Member(collection);
            mb.Entry = new AtomEntry();
            mb.Entry.Document = doc;
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");

            mb.MemberId = mb.GenerateResourceId(mb.Entry);
            mb.MediaType = "application/atom+xml;type=entry";
            mb.MediaId = (string)mb.Collection.ConvertId(mb.MemberId)[1];
            XmlNode link = doc.SelectSingleNode("./atom:link[@rel='edit-media']", nsmgr);
            if (link != null)
            {
                string type = ((XmlElement)link).GetAttribute("type");
                if (type != String.Empty)
                {
                    mb.MediaType = type;
                }
            }

            return mb;
        }

        public virtual string GenerateResourceId(AtomEntry entry)
        {
            return String.Format("{0}.{1}", Hash.MD5(entry.Id), this.Collection.MemberExtension);
        }

        public virtual string GenerateResourceId(AtomEntry entry, string slug)
        {
            return this.GenerateResourceId(entry);
        }

        public virtual string GenerateResourceId(string slug)
        {
            return String.Format("{0}.{1}", slug, this.Collection.MemberExtension);
        }
    }

}