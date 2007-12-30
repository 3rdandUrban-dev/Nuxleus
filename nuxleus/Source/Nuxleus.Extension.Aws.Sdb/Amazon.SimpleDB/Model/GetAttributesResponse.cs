/******************************************************************************* 
 *  Copyright 2007 Amazon Technologies, Inc.  
 *  Licensed under the Apache License, Version 2.0 (the "License"); 
 *  
 *  You may not use this file except in compliance with the License. 
 *  You may obtain a copy of the License at: http://aws.amazon.com/apache2.0
 *  This file is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
 *  CONDITIONS OF ANY KIND, either express or implied. See the License for the 
 *  specific language governing permissions and limitations under the License.
 * ***************************************************************************** 
 *    __  _    _  ___ 
 *   (  )( \/\/ )/ __)
 *   /__\ \    / \__ \
 *  (_)(_) \/\/  (___/
 * 
 *  Amazon Simple DB CSharp Library
 *  API Version: 2007-11-07
 *  Generated: Thu Dec 27 02:53:43 PST 2007 
 * 
 */

using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;


namespace Amazon.SimpleDB.Model
{
    [XmlTypeAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/")]
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2007-11-07/", IsNullable = false)]
    public class GetAttributesResponse
    {
    
        private  GetAttributesResult getAttributesResultField;
        private  ResponseMetadata responseMetadataField;

        /// <summary>
        /// Gets and sets the GetAttributesResult property.
        /// </summary>
        [XmlElementAttribute(ElementName = "GetAttributesResult")]
        public GetAttributesResult GetAttributesResult
        {
            get { return this.getAttributesResultField ; }
            set { this.getAttributesResultField = value; }
        }



        /// <summary>
        /// Sets the GetAttributesResult property
        /// </summary>
        /// <param name="getAttributesResult">GetAttributesResult property</param>
        /// <returns>this instance</returns>
        public GetAttributesResponse WithGetAttributesResult(GetAttributesResult getAttributesResult)
        {
            this.getAttributesResultField = getAttributesResult;
            return this;
        }



        /// <summary>
        /// Checks if GetAttributesResult property is set
        /// </summary>
        /// <returns>true if GetAttributesResult property is set</returns>
        public Boolean IsSetGetAttributesResult()
        {
            return this.getAttributesResultField != null;
        }




        /// <summary>
        /// Gets and sets the ResponseMetadata property.
        /// </summary>
        [XmlElementAttribute(ElementName = "ResponseMetadata")]
        public ResponseMetadata ResponseMetadata
        {
            get { return this.responseMetadataField ; }
            set { this.responseMetadataField = value; }
        }



        /// <summary>
        /// Sets the ResponseMetadata property
        /// </summary>
        /// <param name="responseMetadata">ResponseMetadata property</param>
        /// <returns>this instance</returns>
        public GetAttributesResponse WithResponseMetadata(ResponseMetadata responseMetadata)
        {
            this.responseMetadataField = responseMetadata;
            return this;
        }



        /// <summary>
        /// Checks if ResponseMetadata property is set
        /// </summary>
        /// <returns>true if ResponseMetadata property is set</returns>
        public Boolean IsSetResponseMetadata()
        {
            return this.responseMetadataField != null;
        }






        /// <summary>
        /// XML Representation for this object
        /// </summary>
        /// <returns>XML String</returns>

        public String ToXML() {
            StringBuilder xml = new StringBuilder();
            xml.Append("<GetAttributesResponse xmlns=\"http://sdb.amazonaws.com/doc/2007-11-07/\">");
            if (IsSetGetAttributesResult()) {
                GetAttributesResult  getAttributesResult = this.GetAttributesResult;
                xml.Append("<GetAttributesResult>");
                xml.Append(getAttributesResult.ToXMLFragment());
                xml.Append("</GetAttributesResult>");
            } 
            if (IsSetResponseMetadata()) {
                ResponseMetadata  responseMetadata = this.ResponseMetadata;
                xml.Append("<ResponseMetadata>");
                xml.Append(responseMetadata.ToXMLFragment());
                xml.Append("</ResponseMetadata>");
            } 
            xml.Append("</GetAttributesResponse>");
            return xml.ToString();
        }

        /**
         * 
         * Escape XML special characters
         */
        private String EscapeXML(String str) {
            StringBuilder sb = new StringBuilder();
            foreach (Char c in str)
            {
                switch (c) {
                case '&':
                    sb.Append("&amp;");
                    break;
                case '<':
                    sb.Append("&lt;");
                    break;
                case '>':
                    sb.Append("&gt;");
                    break;
                case '\'':
                    sb.Append("&#039;");
                    break;
                case '"':
                    sb.Append("&quot;");
                    break;
                default:
                    sb.Append(c);
                    break;
                }
            }
            return sb.ToString();
        }




    }

}