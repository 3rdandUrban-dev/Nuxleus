using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Nuxleus.Async;

namespace Nuxleus.Web
{
    public enum ResponseType { REDIRECT, RETURN_LOCATION }

    public struct Message
    {
        string m_status;
        ResponseType m_responseType;

        public Message(string status)
        {
            m_status = status;
            m_responseType = ResponseType.REDIRECT;
        }

        public Message(string status, ResponseType responseType)
        {
            m_status = status;
            m_responseType = responseType;
        }

        public void WriteResponseMessage (XmlWriter writer, NuxleusAsyncResult asyncResult)
        {
            using (writer)
            {
                writer.WriteStartDocument();
                writer.WriteProcessingInstruction("xml-stylesheet", "type='text/xsl' href='/service/transform/openid-redirect.xsl'");
                writer.WriteStartElement("auth");
                    writer.WriteStartAttribute("xml:base");
                        writer.WriteString("http://dev.amp.fm/");
                    writer.WriteEndAttribute();
                    writer.WriteStartAttribute("status");
                        writer.WriteString(m_status);
                    writer.WriteEndAttribute();
                    writer.WriteStartElement("url");
                        writer.WriteString("http://dev.amp.fm/");
                    writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            asyncResult.CompleteCall();
            
        }
    }
}
