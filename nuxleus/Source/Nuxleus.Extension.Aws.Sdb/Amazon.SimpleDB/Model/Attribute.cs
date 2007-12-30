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
    public class Attribute
    {
    
        private String nameField;

        private String valueField;


        /// <summary>
        /// Gets and sets the Name property.
        /// </summary>
        [XmlElementAttribute(ElementName = "Name")]
        public String Name
        {
            get { return this.nameField ; }
            set { this.nameField= value; }
        }



        /// <summary>
        /// Sets the Name property
        /// </summary>
        /// <param name="name">Name property</param>
        /// <returns>this instance</returns>
        public Attribute WithName(String name)
        {
            this.nameField = name;
            return this;
        }



        /// <summary>
        /// Checks if Name property is set
        /// </summary>
        /// <returns>true if Name property is set</returns>
        public Boolean IsSetName()
        {
            return  this.nameField != null;

        }


        /// <summary>
        /// Gets and sets the Value property.
        /// </summary>
        [XmlElementAttribute(ElementName = "Value")]
        public String Value
        {
            get { return this.valueField ; }
            set { this.valueField= value; }
        }



        /// <summary>
        /// Sets the Value property
        /// </summary>
        /// <param name="value">Value property</param>
        /// <returns>this instance</returns>
        public Attribute WithValue(String value)
        {
            this.valueField = value;
            return this;
        }



        /// <summary>
        /// Checks if Value property is set
        /// </summary>
        /// <returns>true if Value property is set</returns>
        public Boolean IsSetValue()
        {
            return  this.valueField != null;

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
            if (IsSetName()) {
                xml.Append("<Name>");
                xml.Append(EscapeXML(this.Name));
                xml.Append("</Name>");
            }
            if (IsSetValue()) {
                xml.Append("<Value>");
                xml.Append(EscapeXML(this.Value));
                xml.Append("</Value>");
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