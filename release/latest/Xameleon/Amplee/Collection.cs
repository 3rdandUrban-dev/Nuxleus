//
// collection.cs: AtomPub collection implementation
//
// Author:
//   Sylvain Hellegouarch (sh@defuze.org)
//
// Copyright (C) 2007, Sylvain Hellegouarch
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using Xameleon.Atom;

namespace Xameleon.Amplee
{
    public class AtomPubCollection
    {
        private AtomPubWorkspace workspace = null;
        private IList<string> accepts = new List<string>();
        private AtomPubCategories categories = new AtomPubCategories();
        private string xmlLang = null;
        private string xmlBase = null;
        private string name = null;
        private string uri = null;
        private string title = null;
        private string memberExt = "atom";
        private string baseUri = null;
        private string baseEditUri = null;
        private string baseMediaEditUri = null;

        public AtomPubCollection()
        {
        }

        public AtomPubCollection(AtomPubWorkspace workspace, string name)
        {
            this.name = name;
            this.workspace = workspace;
            this.workspace.Collections.Add(this);
        }

        public AtomPubWorkspace Workspace
        {
            get
            {
                return this.workspace;
            }
            set
            {
                this.workspace = value;
            }
        }

        public StoreManager Store
        {
            get
            {
                return this.Workspace.Service.Store;
            }
        }

        public string Lang
        {
            get
            {
                return this.xmlLang;
            }
            set
            {
                this.xmlLang = value;
            }
        }

        public string Base
        {
            get
            {
                if (this.xmlBase != null)
                    return this.xmlBase;
                return this.Workspace.Base;
            }
            set
            {
                this.xmlBase = value;
            }
        }

        public string PublicBaseUri
        {
            get
            {
                return this.Base + this.baseUri;
            }
            set
            {
                this.baseUri = value;
            }
        }

        public string BaseEditUri
        {
            get
            {
                return this.Base + this.baseEditUri;
            }
            set
            {
                this.baseEditUri = value;
            }
        }

        public string BaseMediaEditUri
        {
            get
            {
                return this.Base + this.baseMediaEditUri;
            }
            set
            {
                this.baseMediaEditUri = value;
            }
        }

        public string MemberExtension
        {
            get
            {
                return this.memberExt;
            }
            set
            {
                this.memberExt = value;
            }
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

        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
            }
        }

        public string Uri
        {
            get
            {
                return this.uri;
            }
            set
            {
                this.uri = value;
            }
        }

        public IList<string> Accepts
        {
            get
            {
                return this.accepts;
            }
        }

        public AtomPubCategories Categories
        {
            get
            {
                return this.categories;
            }
        }

        public ArrayList ConvertId(string someId)
        {
            ArrayList ids = new ArrayList(2);
            if (this.MemberExtension != null)
            {
                string ext = String.Format(".{0}", this.MemberExtension);
                if (someId.EndsWith(ext))
                {
                    ids.Add(someId);
                    ids.Add(someId.Substring(0, someId.LastIndexOf(ext)));
                }
                else
                {
                    ids.Add(String.Format("{0}{1}", someId, ext));
                    ids.Add(someId);
                }
            }
            else
            {
                ids.Add(someId);
                ids.Add(someId);
            }

            return ids;
        }

        public IStorageInfo GetMetadataInfo(string memberId)
        {
            return this.Store.MemberStorage.Info(this.Name, memberId);
        }

        public StreamReader GetMetadata(IStorageInfo info)
        {
            return this.Store.MemberStorage.LoadEntry(info);
        }

        public IStorageInfo GetContentInfo(string mediaId)
        {
            return this.Store.MediaStorage.Info(this.Name, mediaId);
        }

        public FileStream GetContent(IStorageInfo info)
        {
            return this.Store.MediaStorage.LoadContent(info);
        }

        public XmlDocument CollectionFeed
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);

                XmlElement root = doc.CreateElement("atom", "feed", "http://www.w3.org/2005/Atom");
                doc.InsertBefore(decl, doc.DocumentElement);
                doc.AppendChild(root);

                if (this.Base != null)
                {
                    root.SetAttribute("base", "http://www.w3.org/XML/1998/namespace", this.Base);
                }

                XmlElement e = doc.CreateElement("atom", "id", "http://www.w3.org/2005/Atom");
                e.InnerText = String.Format("urn:uuid:{0}", Guid.NewGuid().ToString());
                root.AppendChild(e);

                e = doc.CreateElement("atom", "title", "http://www.w3.org/2005/Atom");
                e.InnerText = this.Title;
                e.SetAttribute("type", "text");
                root.AppendChild(e);

                e = doc.CreateElement("atom", "updated", "http://www.w3.org/2005/Atom");
                e.InnerText = DateTime.UtcNow.ToString("o");
                root.AppendChild(e);

                IDictionary<string, IStorageInfo> members = this.Store.MemberStorage.ListResources(this.Name, this.MemberExtension);

                foreach (KeyValuePair<string, IStorageInfo> kvp in members)
                {
                    IMember mb = this.GetMember(kvp.Key);
                    XmlNode n = doc.ImportNode(mb.Entry.Node, true);
                    root.AppendChild(n);
                }

                return doc;
            }
        }

        public IMember GetMember(string memberId)
        {
            return this.LoadMember(memberId);
        }

        public IMember LoadMember(string memberId)
        {
            IStorageInfo info = this.GetMetadataInfo(memberId);
            IMember mb = this.LoadMember(memberId, info);
            return mb;
        }

        public IMember LoadMember(string memberId, IStorageInfo info)
        {
            string entry = this.GetMetadata(info).ReadToEnd();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(entry);
            return Member.FromEntry(this, doc);
        }

        public IList<IMember> ReloadMembers()
        {
            IDictionary<string, IStorageInfo> members = this.Store.MemberStorage.ListResources(this.Name, this.MemberExtension);

            IList<IMember> mbs = new List<IMember>();

            foreach (KeyValuePair<string, IStorageInfo> kvp in members)
            {
                IMember mb = this.LoadMember(kvp.Key, kvp.Value);
                mbs.Add(mb);
            }

            return mbs;
        }

        public void Attach(IMember member, string entry)
        {
            IStorageInfo info = this.GetMetadataInfo(member.MemberId);
            this.Store.MemberStorage.SaveEntry(info, entry);
        }

        public void Attach(IMember member, string entry, byte[] content)
        {
            IStorageInfo info = this.GetMetadataInfo(member.MemberId);
            this.Store.MemberStorage.SaveEntry(info, entry);

            info = this.GetContentInfo(member.MediaId);
            this.Store.MediaStorage.SaveContent(info, content);
        }

        public void Prune(string memberId, string mediaId)
        {
            IStorageInfo info = this.GetMetadataInfo(memberId);
            if (this.Store.MemberStorage.Exists(info))
                this.Store.MemberStorage.DeleteEntry(info);

            info = this.GetContentInfo(memberId);
            if (this.Store.MediaStorage.Exists(info))
                this.Store.MediaStorage.DeleteContent(info);
        }

        public void Prune(string memberId)
        {
            IStorageInfo info = this.GetMetadataInfo(memberId);
            if (this.Store.MemberStorage.Exists(info))
                this.Store.MemberStorage.DeleteEntry(info);
        }

        public XmlDocument Document
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);

                XmlElement root = doc.CreateElement("app", "collection", "http://www.w3.org/2007/app");
                doc.InsertBefore(decl, doc.DocumentElement);
                doc.AppendChild(root);

                if (this.Base != null)
                {
                    root.SetAttribute("base", "http://www.w3.org/XML/1998/namespace", this.Base);
                }

                if (this.Lang != null)
                {
                    root.SetAttribute("lang", "http://www.w3.org/XML/1998/namespace", this.Lang);
                }

                if (this.Uri != null)
                {
                    root.SetAttribute("href", this.Uri);
                }

                if (this.Title != null)
                {
                    XmlElement e = doc.CreateElement("atom", "title", "http://www.w3.org/2005/Atom");
                    e.InnerText = this.Title;
                    root.AppendChild(e);
                }

                foreach (string mimetype in this.accepts)
                {
                    XmlElement e = doc.CreateElement("app", "accept", "http://www.w3.org/2007/app");
                    e.InnerText = mimetype;
                    root.AppendChild(e);
                }

                XmlNode cat = doc.ImportNode(this.Categories.Node, true);
                root.AppendChild(cat);

                return doc;
            }
        }

        public XmlNode Node
        {
            get
            {
                XmlDocument doc = this.Document;
                return doc.DocumentElement;
            }
        }
    }
}
