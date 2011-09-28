namespace Mvp.Xml.Tests.XPathNavigatorReaderTests
{
    
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="mvp-xml")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="mvp-xml", IsNullable=false)]
    public class titles
    {
        
        /// <remarks/>
        public string title_id;
        
        /// <remarks/>
        public string title;
        
        /// <remarks/>
        public string type;
        
        /// <remarks/>
        public System.UInt16 pub_id;
        
        /// <remarks/>
        public System.Decimal price;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool priceSpecified;
        
        /// <remarks/>
        public System.UInt16 advance;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool advanceSpecified;
        
        /// <remarks/>
        public System.Byte royalty;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool royaltySpecified;
        
        /// <remarks/>
        public System.UInt16 ytd_sales;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ytd_salesSpecified;
        
        /// <remarks/>
        public string notes;
        
        /// <remarks/>
        public System.DateTime pubdate;
    }
}
