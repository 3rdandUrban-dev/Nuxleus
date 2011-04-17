using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Nuxleus.Core;

namespace Nuxleus.Web {

    public enum ResponseType { REDIRECT, RETURN_LOCATION, QUERY_RESPONSE, ERROR }
    public enum ErrorType { NO_ERROR, INVALID_REQUEST, NO_PERMISSIONS }

    public class Message
    {

        string m_status;
        ResponseType m_responseType;
        ErrorType m_errorType;

        public Message ( string status ) {
            m_status = status;
            m_responseType = ResponseType.REDIRECT;
            m_errorType = ErrorType.NO_ERROR;
        }

        public Message ( string status, ResponseType responseType ) {
            m_status = status;
            m_responseType = responseType;
            m_errorType = ErrorType.NO_ERROR;
        }

        public Message ( string status, ErrorType errorType, ResponseType responseType ) {
            m_status = status;
            m_errorType = errorType;
            m_responseType = responseType;
        }

        public void WriteResponseMessage ( XmlWriter writer, string innerXml, NuxleusAsyncResult asyncResult ) {

            using (writer) {
                writer.WriteStartDocument();
                if (m_responseType == ResponseType.REDIRECT) {
                    writer.WriteProcessingInstruction("xml-stylesheet", "type='text/xsl' href='/service/transform/openid-redirect.xsl'");
                }
                writer.WriteStartElement("auth");
                writer.WriteAttributeString("xml:base", "http://dev.amp.fm/");
                writer.WriteAttributeString("status", m_status);
                if (m_responseType == ResponseType.REDIRECT) {
                    writer.WriteElementString("url", "http://dev.amp.fm/");
                }
                if (m_responseType == ResponseType.QUERY_RESPONSE && innerXml != null) {
                    writer.WriteStartElement("response");
                    writer.WriteRaw(innerXml);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            asyncResult.CompleteCall();

        }
    }
}
