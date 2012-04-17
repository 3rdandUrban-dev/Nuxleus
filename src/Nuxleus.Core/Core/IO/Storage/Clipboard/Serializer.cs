using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Nuxleus.Utility.S3;

namespace Nuxleus.IO
{

    [Serializable]
    public class GlobalClipSerializer
    {

        public GlobalClipSerializer() { }
        private XmlTextWriter _OutputWriter;

        public void SetOutputWriter(XmlTextWriter writer)
        {
            this._OutputWriter = writer;
        }

        private XmlTextWriter GetOutputWriter()
        {
            return this._OutputWriter;
        }

        public void ToXml(ListBucketResponse list, String storage)
        {
            XmlTextWriter XWriter = this.GetOutputWriter();
            XWriter.WriteStartDocument();
            XWriter.Formatting = Formatting.Indented;
            XWriter.Indentation = 2;
            XWriter.WriteStartElement("bucket");
            XWriter.WriteAttributeString("name", storage);
            foreach (ListEntry entry in list.Entries)
            {
                Owner o = entry.Owner;
                if (o == null)
                {
                    o = new Owner("", "");
                }
                XWriter.WriteStartElement("File");
                XWriter.WriteAttributeString("key", entry.Key);
                XWriter.WriteElementString("ETag", entry.ETag);
                XWriter.WriteElementString("LastModified", entry.LastModified.ToString());
                XWriter.WriteElementString("ID", o.Id);
                XWriter.WriteElementString("DisplayName", o.DisplayName);
                XWriter.WriteElementString("Size", entry.Size.ToString());
                XWriter.WriteElementString("StorageClass", entry.StorageClass);
                XWriter.WriteEndElement();
            }
            XWriter.WriteEndElement();
            XWriter.Flush();
            XWriter.Close();
        }

        public void ToAtom(ListBucketResponse list, String storage)
        {
            XmlTextWriter XWriter = this.GetOutputWriter();
            XWriter.WriteStartDocument();
            XWriter.Formatting = Formatting.Indented;
            XWriter.Indentation = 2;
            XWriter.WriteStartElement("feed", "http://www.w3.org/2005/Atom");
            //XWriter.WriteAttributeString("name", storage);
            foreach (ListEntry entry in list.Entries)
            {
                Owner o = entry.Owner;
                if (o == null)
                {
                    o = new Owner("", "");
                }
                XWriter.WriteStartElement("entry");

                // 06/12/06 01:15AM MDT : This was (hopefully) obviously a cut/paste job 
                // from above. Not sure how I plan to implement this in the final assembly,
                // but I know it won't be a part of the this particular class.
                //
                // For now I am just commenting out the obvious stuff that is worthless.
                //

                //XWriter.WriteAttributeString("key", entry.Key);
                //XWriter.WriteElementString("ETag", entry.ETag);
                //XWriter.WriteElementString("LastModified", entry.LastModified.ToString());
                //XWriter.WriteElementString("ID", o.Id);
                //XWriter.WriteElementString("DisplayName", o.DisplayName);
                //XWriter.WriteElementString("Size", entry.Size.ToString());
                //XWriter.WriteElementString("StorageClass", entry.StorageClass);

                XWriter.WriteEndElement();
            }
            XWriter.WriteEndElement();
            XWriter.Flush();
            XWriter.Close();
        }
    }
}


