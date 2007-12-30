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
    public class ListDomainsResult
    {
    
        private List<String> domainNameField;

        private String nextTokenField;


        /// <summary>
        /// Gets and sets the DomainName property.
        /// </summary>
        [XmlElementAttribute(ElementName = "DomainName")]
        public List<String> DomainName
        {
            get
            {
                if (this.domainNameField == null)
                {
                    this.domainNameField = new List<String>();
                }
                return this.domainNameField;
            }
            set { this.domainNameField =  value; }
        }



        /// <summary>
        /// Sets the DomainName property
        /// </summary>
        /// <param name="list">DomainName property</param>
        /// <returns>this instance</returns>
        public ListDomainsResult WithDomainName(params String[] list)
        {
            foreach (String item in list)
            {
                DomainName.Add(item);
            }
            return this;
        }          
 


        /// <summary>
        /// Checks of DomainName property is set
        /// </summary>
        /// <returns>true if DomainName property is set</returns>
        public Boolean IsSetDomainName()
        {
            return (DomainName.Count > 0);
        }




        /// <summary>
        /// Gets and sets the NextToken property.
        /// </summary>
        [XmlElementAttribute(ElementName = "NextToken")]
        public String NextToken
        {
            get { return this.nextTokenField ; }
            set { this.nextTokenField= value; }
        }



        /// <summary>
        /// Sets the NextToken property
        /// </summary>
        /// <param name="nextToken">NextToken property</param>
        /// <returns>this instance</returns>
        public ListDomainsResult WithNextToken(String nextToken)
        {
            this.nextTokenField = nextToken;
            return this;
        }



        /// <summary>
        /// Checks if NextToken property is set
        /// </summary>
        /// <returns>true if NextToken property is set</returns>
        public Boolean IsSetNextToken()
        {
            return  this.nextTokenField != null;

        }




        /// <summary>
        /// XML fragment representation of this object
        /// </summary>
        /// <returns>XML fragment for this object.</returns>
        /// <remarks>
        /// Name for outer tag expected to be set by calling method. 
        /// This fragment returns inner properties representation only
        /// </remarks>


        protected internal String ToXMLFragment() {
            StringBuilder xml = new StringBuilder();
            List<String> domainNameList  =  this.DomainName;
            foreach (String domainName in domainNameList) { 
                xml.Append("<DomainName>");
                xml.Append(EscapeXML(domainName));
                xml.Append("</DomainName>");
            }	
            if (IsSetNextToken()) {
                xml.Append("<NextToken>");
                xml.Append(EscapeXML(this.NextToken));
                xml.Append("</NextToken>");
            }
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