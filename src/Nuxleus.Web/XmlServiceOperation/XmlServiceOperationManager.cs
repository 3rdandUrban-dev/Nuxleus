// Copyright (c) 2007 by M. David Peterson
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Xml;
using Nuxleus.Core;
using Nuxleus.Cryptography;

namespace Nuxleus.Web
{

    public class XmlServiceOperationManager
    {

        Dictionary<int, XmlReader> m_xmlReaderDictionary;
        Dictionary<int, int> m_xmlSourceETagDictionary;
        static String m_hashkey = (String)HttpContext.Current.Application["hashkey"];
        static HashAlgorithm m_hashAlgorithm = HashAlgorithm.MD5;

        public XmlServiceOperationManager(Dictionary<int, XmlReader> xmlReaderDictionary)
            : this(xmlReaderDictionary, new Dictionary<int, int>())
        {
        }

        public XmlServiceOperationManager(Dictionary<int, XmlReader> xmlReaderDictionary, Dictionary<int, int> xmlSourceETagDictionary)
        {
            m_xmlReaderDictionary = xmlReaderDictionary;
            m_xmlSourceETagDictionary = xmlSourceETagDictionary;
        }

        public bool HasXmlSourceChanged(int eTag, Uri uri)
        {

            int uriHashcode = uri.GetHashCode();

            if (m_xmlSourceETagDictionary.ContainsKey(uriHashcode))
            {
                if (m_xmlSourceETagDictionary[uriHashcode] == eTag)
                {
                    this.LogInfo("Source has not changed. {0}.  Count: {1}", eTag, m_xmlSourceETagDictionary.Count);
                    return false;
                }
                else
                {
                    this.LogInfo("Source has changed. {0}.  Count: {1}", eTag, m_xmlSourceETagDictionary.Count);
                    return true;
                }
            }
            else
            {
                this.LogInfo("Source has changed. {0}.  Count: {1}", eTag, m_xmlSourceETagDictionary.Count);
                return true;
            }
        }

        public void AddXmlReader(Uri uri)
        {
            addXmlReader(GenerateETagKey(uri), uri);
        }

        private void addXmlReader(int key, Uri uri)
        {
            XmlReader reader = createNewXmlReader(uri.OriginalString);
            int uriHashcode = uri.GetHashCode();
            m_xmlReaderDictionary[uriHashcode] = reader;
            m_xmlSourceETagDictionary[uriHashcode] = key;
        }

        public XmlReader GetXmlReader(Uri uri)
        {
            return getXmlReader(GenerateETagKey(uri), uri);
        }

        public XmlReader GetXmlReader(int eTagKey, Uri uri)
        {
            return getXmlReader(eTagKey, uri);
        }

        private XmlReader getXmlReader(int key, Uri xmlUri)
        {
            int uriHashcode = xmlUri.GetHashCode();
            if (m_xmlSourceETagDictionary.ContainsKey(uriHashcode))
            {
                this.LogInfo("Dictionary contains key: {0}", uriHashcode);
                if (m_xmlSourceETagDictionary[uriHashcode] == key)
                {
                    this.LogInfo("{0} matches {1}", m_xmlSourceETagDictionary[uriHashcode], key);
                    return getXmlReader(uriHashcode, xmlUri, false);
                }
                else
                {
                    this.LogInfo("{0} does not match {1}", m_xmlSourceETagDictionary[uriHashcode], key);
                    m_xmlSourceETagDictionary[uriHashcode] = key;
                    return getXmlReader(uriHashcode, xmlUri, true);
                }
            }
            else
            {
                this.LogInfo("Dictionary does not contain key: {0}", uriHashcode);
                m_xmlSourceETagDictionary[uriHashcode] = key;
                return getXmlReader(uriHashcode, xmlUri, true);
            }
        }

        private XmlReader getXmlReader(int key, Uri xmlUri, bool replaceExistingXmlReader)
        {

            if (m_xmlReaderDictionary.ContainsKey(key) && !replaceExistingXmlReader)
            {
                this.LogInfo("Dictionary contains key: {0} and is not being replaced.", key);
                return m_xmlReaderDictionary[key];
            }
            else
            {
                if (m_xmlReaderDictionary.ContainsKey(key))
                {
                    this.LogInfo("Dictionary contains key: {0} but is being replaced because the source file has changed.", key);
                }
                else
                {
                    this.LogInfo("Dictionary does not contain key: {0}, so a new XmlReader is being created.", key);
                }
                XmlReader reader = createNewXmlReader(xmlUri.OriginalString);
                m_xmlReaderDictionary[key] = reader;
                this.LogInfo("XmlReaderDictionary currently contains: {0} entries.", m_xmlReaderDictionary.Count);
                return reader;
            }
        }

        private static XmlReader createNewXmlReader(string xmlSourceUri)
        {
            return XmlReader.Create(xmlSourceUri);
        }

        public static int GenerateETagKey(Uri sourceUri, params object[] objectParams)
        {
            FileInfo fileInfo = new FileInfo(sourceUri.LocalPath);
            return HashcodeGenerator.GetHMACBase64Hashcode(m_hashkey, m_hashAlgorithm, fileInfo.LastWriteTimeUtc, fileInfo.Length, sourceUri, objectParams);
        }

        public Dictionary<int, XmlReader> XmlReaderDictionary
        {
            get { return m_xmlReaderDictionary; }
            set { m_xmlReaderDictionary = value; }
        }
        public Dictionary<int, int> XmlSourceETagDictionary
        {
            get { return m_xmlSourceETagDictionary; }
            set { m_xmlSourceETagDictionary = value; }
        }
    }
}
