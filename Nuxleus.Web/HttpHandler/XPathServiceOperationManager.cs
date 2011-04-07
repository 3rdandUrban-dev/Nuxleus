// Copyright (c) 2007 by M. David Peterson
// The code contained in this file is licensed under The MIT License
// Please see http://www.opensource.org/licenses/mit-license.php for specific detail.

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Xml.XPath;
using Nuxleus.Core;
using Nuxleus.Cryptography;

namespace Nuxleus.Web {

    public class XPathServiceOperationManager
    {

        Dictionary<int, XPathNavigator> m_XPathNavigatorDictionary;
        Dictionary<int, int> m_xmlSourceETagDictionary;
        static String m_hashkey = (String)HttpContext.Current.Application["hashkey"];
        static HashAlgorithm m_hashAlgorithm = HashAlgorithm.MD5;

        public XPathServiceOperationManager ( Dictionary<int, XPathNavigator> xpathNavigatorDictionary )
            : this(xpathNavigatorDictionary, new Dictionary<int, int>()) {
        }

        public XPathServiceOperationManager ( Dictionary<int, XPathNavigator> xpathNavigatorDictionary, Dictionary<int, int> xmlSourceETagDictionary ) {
            m_XPathNavigatorDictionary = xpathNavigatorDictionary;
            m_xmlSourceETagDictionary = xmlSourceETagDictionary;
        }

        public bool HasXmlSourceChanged ( int eTag, Uri uri ) {

            int uriHashcode = uri.GetHashCode();

            if (m_xmlSourceETagDictionary.ContainsKey(uriHashcode)) {
                if (m_xmlSourceETagDictionary[uriHashcode] == eTag) {
                    this.LogInfo("Source has not changed. {0}.  Count: {1}", eTag, m_xmlSourceETagDictionary.Count);
                    return false;
                } else {
                    this.LogInfo("Source has changed. {0}.  Count: {1}", eTag, m_xmlSourceETagDictionary.Count);
                    return true;
                }
            } else {
                this.LogInfo("Source has changed. {0}.  Count: {1}", eTag, m_xmlSourceETagDictionary.Count);
                return true;
            }
        }

        public void AddXPathNavigator ( Uri uri ) {
            addXPathNavigator(GenerateETagKey(uri), uri);
        }

        private void addXPathNavigator ( int key, Uri uri ) {
            XPathNavigator reader = createNewXPathNavigator(uri.OriginalString);
            int uriHashcode = uri.GetHashCode();
            m_XPathNavigatorDictionary[uriHashcode] = reader;
            m_xmlSourceETagDictionary[uriHashcode] = key;
        }

        public XPathNavigator GetXPathNavigator ( Uri uri ) {
            return getXPathNavigator(GenerateETagKey(uri), uri);
        }

        public XPathNavigator GetXPathNavigator ( int eTagKey, Uri uri ) {
            return getXPathNavigator(eTagKey, uri);
        }

        private XPathNavigator getXPathNavigator ( int key, Uri xmlUri ) {
            int uriHashcode = xmlUri.GetHashCode();
            if (m_xmlSourceETagDictionary.ContainsKey(uriHashcode)) {
                this.LogInfo("Dictionary contains key: {0}", uriHashcode);
                if (m_xmlSourceETagDictionary[uriHashcode] == key) {
                    this.LogInfo("{0} matches {1}", m_xmlSourceETagDictionary[uriHashcode], key);
                    return getXPathNavigator(uriHashcode, xmlUri, false);
                } else {
                    this.LogInfo("{0} does not match {1}", m_xmlSourceETagDictionary[uriHashcode], key);
                    m_xmlSourceETagDictionary[uriHashcode] = key;
                    return getXPathNavigator(uriHashcode, xmlUri, true);
                }
            } else {
                this.LogInfo("Dictionary does not contain key: {0}", uriHashcode);
                m_xmlSourceETagDictionary[uriHashcode] = key;
                return getXPathNavigator(uriHashcode, xmlUri, true);
            }
        }

        private XPathNavigator getXPathNavigator ( int key, Uri xmlUri, bool replaceExistingXmlReader ) {

            if (m_XPathNavigatorDictionary.ContainsKey(key) && !replaceExistingXmlReader) {
                this.LogInfo("Dictionary contains key: {0} and is not being replaced.", key);
                return m_XPathNavigatorDictionary[key];
            } else {
                if (m_XPathNavigatorDictionary.ContainsKey(key)) {
                    this.LogInfo("Dictionary contains key: {0} but is being replaced because the source file has changed.", key);
                } else {
                    this.LogInfo("Dictionary does not contain key: {0}, so a new XmlReader is being created.", key);
                }
                XPathNavigator reader = createNewXPathNavigator(xmlUri.OriginalString);
                m_XPathNavigatorDictionary[key] = reader;
                this.LogInfo("XmlReaderDictionary currently contains: {0} entries.", m_XPathNavigatorDictionary.Count);
                return reader;
            }
        }

        private static XPathNavigator createNewXPathNavigator ( string xmlSourceUri ) {
            return new XPathDocument(xmlSourceUri).CreateNavigator();
        }

        public static int GenerateETagKey ( Uri sourceUri, params object[] objectParams ) {
            FileInfo fileInfo = new FileInfo(sourceUri.LocalPath);
            return HashcodeGenerator.GetHMACBase64Hashcode(m_hashkey, m_hashAlgorithm, fileInfo.LastWriteTimeUtc, fileInfo.Length, sourceUri, objectParams);
        }

        public Dictionary<int, XPathNavigator> XmlReaderDictionary {
            get { return m_XPathNavigatorDictionary; }
            set { m_XPathNavigatorDictionary = value; }
        }
        public Dictionary<int, int> XmlSourceETagDictionary {
            get { return m_xmlSourceETagDictionary; }
            set { m_xmlSourceETagDictionary = value; }
        }
    }
}
