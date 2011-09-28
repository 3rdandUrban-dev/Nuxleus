    /* Copyright 2006 Microsoft Corporation.  Microsoft's copyrights in this work are licensed under the Creative Commons */
/* Attribution-ShareAlike 2.5 License.  To view a copy of this license visit http://creativecommons.org/licenses/by-sa/2.5 */

function HCard(firstName, lastName, email, phone, defaultStreet, defaultCity, defaultState, defaultZip, defaultCountry, defaultLatitude, defaultLongitude, homeStreet, homeCity, homeState, homeZip, homeCountry, homeLatitude, homeLongitude, businessStreet, businessCity, businessState, businessZip, businessCountry, businessLatitude, businessLongitude,currentStreet, currentCity, currentState, currentZip, currentCountry, currentLatitude, currentLongitude, uid, nickName, url, org)
{
    this.formatType = "vcard";
    this.formatRootClassName = "vcard";
    this.FirstName = firstName;
    this.LastName = lastName;
    this.Email = email;
    this.Phone = phone;
    
    this.DefaultStreet = defaultStreet;
    this.DefaultCity = defaultCity;
    this.DefaultState = defaultState;
    this.DefaultZip = defaultZip;
    this.DefaultCountry = defaultCountry;
    this.DefaultLatitude = defaultLatitude;
    this.DefaultLongitude = defaultLongitude;
    
    this.HomeStreet = homeStreet;
    this.HomeCity = homeCity;
    this.HomeState = homeState;
    this.HomeZip = homeZip;
    this.HomeCountry = homeCountry;
    this.HomeLatitude = homeLatitude;
    this.HomeLongitude = homeLongitude;
    
    this.BusinessStreet = businessStreet;
    this.BusinessCity = businessCity;
    this.BusinessState = businessState;
    this.BusinessZip = businessZip;
    this.BusinessCountry = businessCountry;
    this.BusinessLatitude = businessLatitude;
    this.BusinessLongitude = businessLongitude;
    
    this.CurrentStreet = currentStreet;
    this.CurrentCity = currentCity;
    this.CurrentState = currentState;
    this.CurrentZip = currentZip;
    this.CurrentCountry = currentCountry;
    this.CurrentLatitude = currentLatitude;
    this.CurrentLongitude = currentLongitude;          
    
    this.UID = uid;
    this.NickName = nickName;
    this.Org = org;
    this.URL = url;
    
    
    this.updateCallback;
    this.HTML;
    
    var self = this;
    
    this.clearProps = function()
    {
        self.FirstName = null;
        self.LastName = null;
        self.Email = null;
        self.Phone = null;
        
        self.DefaultStreet = null;
        self.DefaultCity = null;
        self.DefaultState = null;
        self.DefaultZip = null;
        self.DefaultCountry = null;
        self.DefaultLatitude = null;
        self.DefaultLongitude = null;
        
        self.HomeStreet = null;
        self.HomeCity = null;
        self.HomeState = null;
        self.HomeZip = null;
        self.HomeCountry = null;
        self.HomeLatitude = null;
        self.HomeLongitude = null;
        
        self.BusinessStreet = null;
        self.BusinessCity = null;
        self.BusinessState = null;
        self.BusinessZip = null;
        self.BusinessCountry = null;
        self.BusinessLatitude = null;
        self.BusinessLongitude = null;
        
        self.CurrentStreet = null;
        self.CurrentCity = null;
        self.CurrentState = null;
        self.CurrentZip = null;
        self.CurrentCountry = null;
        self.CurrentLatitude = null;
        self.CurrentLongitude = null;          
        
        self.UID = null;
        self.NickName = null;
        self.Org = null;
        self.URL = null;
        
        self.buildHtml();
    }
    
    this.buildHtml = function()
    {
        var UrlIsUID = (self.UID == self.Url);
        
        var hCardString = (UrlIsUID || !self.UID) ? "<div class=\"vcard\">" : "<div class=\"vcard uid\" title=\"" + self.UID + "\">";  
        
        if (self.FirstName || self.LastName)
        {    
            hCardString += "<div class=\"fn n\">";
            if (self.FirstName)
                hCardString += "<span class=\"given-name\">" + self.FirstName + "</span> ";
            if (self.LastName)
                hCardString += "<span class=\"family-name\">" + self.LastName + "</span>";
            hCardString += "</div>";
        }
        
        if (self.NickName)
            hCardString += "<span class=\"nickname\">" + self.NickName + "</span>";    
            
        if (self.Org)
            hCardString += "<span class=\"org\">" + self.Org + "</span>";      
        
        if (self.Email)
            hCardString += "<a class=\"email\" href=\"mailto:" + self.Email + "\">" + self.Email + "</a>";
        
        if (self.Phone)
            hCardString += "<div class=\"tel\"><span class=\"value\">" + self.Phone + "</span></div>";
        
        if (self.DefaultStreet || self.DefaultCity || self.DefaultState || self.DefaultZip || self.DefaultCountry || self.DefaultLatitude || self.DefaultLongitude)
        {
            hCardString += "<div class=\"adr\">";
            
            if (self.DefaultStreet)
                hCardString += "<div class=\"street-address\">" + self.DefaultStreet + "</div>";
            
            if (self.DefaultCity)
                hCardString += "<span class=\"locality\">" + self.DefaultCity + "</span> ";
                
            if (self.DefaultState)
                hCardString += "<span class=\"region\">" + self.DefaultState + "</span> ";
                
            if (self.DefaultZip)
                hCardString += "<span class=\"postal-code\">" + self.DefaultZip + "</span> ";
                
            if (self.DefaultCountry)
                hCardString += "<span class=\"country-name\">" + self.DefaultCountry + "</span>";
                
            if (self.DefaultLatitude && self.DefaultLongitude)
            {
                hCardString += "<div class=\"geo\"><abbr class=\"latitude\" title=\"" + self.DefaultLatitude + "\">" + self.DefaultLatitude + "</abbr><abbr class=\"longitude\" title=\"" + self.DefaultLongitude + "\">" + self.DefaultLongitude + "</abbr></div>";
            }
                     
            hCardString += "</div>";                  
        }
        
        if (self.HomeStreet || self.HomeCity || self.HomeState || self.HomeZip || self.HomeCountry || self.HomeLatitude || self.HomeLongitude)
        {
            hCardString += "<div class=\"adr\"><span class=\"type\">Home</span>:";
            
            if (self.HomeStreet)
                hCardString += "<div class=\"street-address\">" + self.HomeStreet + "</div>";
            
            if (self.HomeCity)
                hCardString += "<span class=\"locality\">" + self.HomeCity + "</span> ";
                
            if (self.HomeState)
                hCardString += "<span class=\"region\">" + self.HomeState + "</span> ";
                
            if (self.HomeZip)
                hCardString += "<span class=\"postal-code\">" + self.HomeZip + "</span> ";
                
            if (self.HomeCountry)
                hCardString += "<span class=\"country-name\">" + self.HomeCountry + "</span>";  
                
            if (self.HomeLatitude && self.HomeLongitude)
            {
                hCardString += "<div class=\"geo\"><abbr class=\"latitude\" title=\"" + self.HomeLatitude + "\">" + self.HomeLatitude + "</abbr><abbr class=\"longitude\" title=\"" + self.HomeLongitude + "\">" + self.HomeLongitude + "</abbr></div>";
            }
                            
            hCardString += "</div>";                  
        }
        
        if (self.BusinessStreet || self.BusinessCity || self.BusinessState || self.BusinessZip || self.BusinessCountry || self.BusinessLatitude || self.BusinessLongitude)
        {
            hCardString += "<div class=\"adr\"><span class=\"type\">Work</span>:";
            
            if (self.BusinessStreet)
                hCardString += "<div class=\"street-address\">" + self.BusinessStreet + "</div>";
            
            if (self.BusinessCity)
                hCardString += "<span class=\"locality\">" + self.BusinessCity + "</span> ";
                
            if (self.BusinessState)
                hCardString += "<span class=\"region\">" + self.BusinessState + "</span> ";
                
            if (self.BusinessZip)
                hCardString += "<span class=\"postal-code\">" + self.BusinessZip + "</span> ";
                
            if (self.BusinessCountry)
                hCardString += "<span class=\"country-name\">" + self.BusinessCountry + "</span>";
                
            if (self.BusinessLatitude && self.BusinessLongitude)
            {
                hCardString += "<div class=\"geo\"><abbr class=\"latitude\" title=\"" + self.BusinessLatitude + "\">" + self.BusinessLatitude + "</abbr><abbr class=\"longitude\" title=\"" + self.BusinessLongitude + "\">" + self.BusinessLongitude + "</abbr></div>";
            }
                     
            hCardString += "</div>";                  
        }
        
        if (self.CurrentStreet || self.CurrentCity || self.CurrentState || self.CurrentZip || self.CurrentCountry || self.CurrentLatitude || self.CurrentLongitude)
        {
            hCardString += "<div class=\"adr\"><span class=\"type\">Current</span>:";
            
            if (self.CurrentStreet)
                hCardString += "<div class=\"street-address\">" + self.CurrentStreet + "</div>";
            
            if (self.CurrentCity)
                hCardString += "<span class=\"locality\">" + self.CurrentCity + "</span> ";
                
            if (self.CurrentState)
                hCardString += "<span class=\"region\">" + self.CurrentState + "</span> ";
                
            if (self.CurrentZip)
                hCardString += "<span class=\"postal-code\">" + self.CurrentZip + "</span> ";
                
            if (self.CurrentCountry)
                hCardString += "<span class=\"country-name\">" + self.CurrentCountry + "</span>";  
                
            if (self.CurrentLatitude && self.CurrentLongitude)
            {
                hCardString += "<div class=\"geo\"><abbr class=\"latitude\" title=\"" + self.CurrentLatitude + "\">" + self.CurrentLatitude + "</abbr><abbr class=\"longitude\" title=\"" + self.CurrentLongitude + "\">" + self.CurrentLongitude + "</abbr></div>";
            }
                     
            hCardString += "</div>";                  
        }        
        
        if (self.URL)
            hCardString += UrlIsUID ? "<a class=\"url uid\" href=\"" + self.URL + "\">" + self.URL + "</a>" : "<a class=\"url\" href=\"" + self.URL + "\">" + self.URL + "</a>";         
        
        hCardString += "</div>";
        self.HTML = hCardString;
    }

    this.initFromXml = function(hCardXmlNode)
    {
        this.clearProps();
        self.xmlData = hCardXmlNode;
        self.parseXml(hCardXmlNode);
        
        
        if (hCardXmlNode.xml)
        {
            self.HTML =  hCardXmlNode.xml;
        }
        else
        {
            var serializer = new XMLSerializer();
            self.HTML = serializer.serializeToString(hCardXmlNode);
        }
    }
    
    // Initialize all contact properties from the hCard XML segment and rebuild hCard HTML.
    this.initFromXmlString = function(hCardXmlString)
    {
        var hCardXmlNode;
        
        // IE 5+
        if (window.ActiveXObject)
        {
            hCardXmlNode = new ActiveXObject("Microsoft.XMLDOM");
            hCardXmlNode.async=false;
            hCardXmlNode.loadXML(hCardXmlString);
            hCardXmlNode.setProperty("SelectionLanguage", "XPath");
        }
        // Mozilla etc.
        else if (typeof DOMParser != "undefined")
        {
            var domParser = new DOMParser();
            hCardXmlNode = domParser.parseFromString(hCardXmlString, 'application/xml');
        }
        
        this.clearProps();
        self.HTML = hCardXmlString;
        self.xmlData = hCardXmlNode;
        self.parseXml(hCardXmlNode);
    }    
        
    this.parseXml = function(hCardXmlNode)
    {        
        // IE 5+
        if (window.ActiveXObject)
        {
            var node;
            
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'given-name')]");
            if (node)
                self.FirstName = node.nodeTypedValue;
                               
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//a[contains(@class, 'uid')]/@href");
            if (node)
                self.UID = node.nodeTypedValue;  
            
            if (!self.UID)
            {        
                node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]/@title");
                if (node)
                    self.UID = node.nodeTypedValue;           
            }                
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'url')]/@href");
            if (node)
                self.URL = node.nodeTypedValue;
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'nickname')]");
            if (node)
                self.NickName = node.nodeTypedValue;                
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'org')]");
            if (node)
                self.Org = node.nodeTypedValue;                                 
            
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'family-name')]");
            if (node)
                self.LastName = node.nodeTypedValue;
            
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//a[contains(@class, 'email')]");
            if (node)
                self.Email = node.nodeTypedValue;
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'tel')]//*[contains(@class, 'value')]");
            if (node)
                self.Phone = node.nodeTypedValue;           
                                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'street-address')]");
            if (node)
                self.DefaultStreet = node.nodeTypedValue;
            
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'locality')]");
            if (node)
                self.DefaultCity = node.nodeTypedValue;
            
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'region')]");
            if (node)
                self.DefaultState = node.nodeTypedValue;
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'postal-code')]");
            if (node)
                self.DefaultZip = node.nodeTypedValue;           
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'country-name')]");
            if (node)
                self.DefaultCountry = node.nodeTypedValue;   
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'geo')]/abbr[contains(@class, 'latitude')]/@title");
            if (node)
                self.DefaultLatitude = node.nodeTypedValue;
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'geo')]/abbr[contains(@class, 'longitude')]/@title");
            if (node)
                self.DefaultLongitude = node.nodeTypedValue;       
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Home' or *[contains(@class, 'type')] = 'home')]//*[contains(@class, 'street-address')]");
            if (node)
                self.HomeStreet = node.nodeTypedValue;
            
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Home' or *[contains(@class, 'type')] = 'home')]//*[contains(@class, 'locality')]");
            if (node)
                self.HomeCity = node.nodeTypedValue;
            
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Home' or *[contains(@class, 'type')] = 'home')]//*[contains(@class, 'region')]");
            if (node)
                self.HomeState = node.nodeTypedValue;
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Home' or *[contains(@class, 'type')] = 'home')]//*[contains(@class, 'postal-code')]");
            if (node)
                self.HomeZip = node.nodeTypedValue;           
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Home' or *[contains(@class, 'type')] = 'home')]//*[contains(@class, 'country-name')]");
            if (node)
                self.HomeCountry = node.nodeTypedValue;
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Home' or *[contains(@class, 'type')] = 'home')]/div[contains(@class, 'geo')]/abbr[contains(@class, 'latitude')]/@title");
            if (node)
                self.HomeLatitude = node.nodeTypedValue;
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Home' or *[contains(@class, 'type')] = 'home')]/div[contains(@class, 'geo')]/abbr[contains(@class, 'longitude')]/@title");
            if (node)
                self.HomeLongitude = node.nodeTypedValue;                                
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Work' or *[contains(@class, 'type')] = 'work')]//*[contains(@class, 'street-address')]");
            if (node)
                self.BusinessStreet = node.nodeTypedValue;
            
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Work' or *[contains(@class, 'type')] = 'work')]//*[contains(@class, 'locality')]");
            if (node)
                self.BusinessCity = node.nodeTypedValue;
            
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Work' or *[contains(@class, 'type')] = 'work')]//*[contains(@class, 'region')]");
            if (node)
                self.BusinessState = node.nodeTypedValue;
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Work' or *[contains(@class, 'type')] = 'work')]//*[contains(@class, 'postal-code')]");
            if (node)
                self.BusinessZip = node.nodeTypedValue;           
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Work' or *[contains(@class, 'type')] = 'work')]//*[contains(@class, 'country-name')]");
            if (node)
                self.BusinessCountry = node.nodeTypedValue;   
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Work' or *[contains(@class, 'type')] = 'work')]/div[contains(@class, 'geo')]/abbr[contains(@class, 'latitude')]/@title");
            if (node)
                self.BusinessLatitude = node.nodeTypedValue;
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Work' or *[contains(@class, 'type')] = 'work')]/div[contains(@class, 'geo')]/abbr[contains(@class, 'longitude')]/@title");
            if (node)
                self.BusinessLongitude = node.nodeTypedValue;       
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Current' or *[contains(@class, 'type')] = 'current')]//*[contains(@class, 'street-address')]");
            if (node)
                self.CurrentStreet = node.nodeTypedValue;
            
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Current' or *[contains(@class, 'type')] = 'current')]//*[contains(@class, 'locality')]");
            if (node)
                self.CurrentCity = node.nodeTypedValue;
            
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Current' or *[contains(@class, 'type')] = 'current')]//*[contains(@class, 'region')]");
            if (node)
                self.CurrentState = node.nodeTypedValue;
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Current' or *[contains(@class, 'type')] = 'current')]//*[contains(@class, 'postal-code')]");
            if (node)
                self.CurrentZip = node.nodeTypedValue;           
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Current' or *[contains(@class, 'type')] = 'current')]//*[contains(@class, 'country-name')]");
            if (node)
                self.CurrentCountry = node.nodeTypedValue;   
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Current' or *[contains(@class, 'type')] = 'current')]/div[contains(@class, 'geo')]/abbr[contains(@class, 'latitude')]/@title");
            if (node)
                self.CurrentLatitude = node.nodeTypedValue;
                
            node = hCardXmlNode.selectSingleNode("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Current' or *[contains(@class, 'type')] = 'current')]/div[contains(@class, 'geo')]/abbr[contains(@class, 'longitude')]/@title");
            if (node)
                self.CurrentLongitude = node.nodeTypedValue;                                     
        }
        // Mozilla etc.
        else if (typeof DOMParser != "undefined")
        {
            if (document.evaluate)
            {               
                var node;
                
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'given-name')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.FirstName = node.textContent;
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//a[contains(@class, 'uid')]/@href", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.UID = node.textContent;
                    
                if (!self.UID)
                {        
                    node = document.evaluate("//*[contains(@class, 'vcard')]/@title", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                    if (node)
                        self.UID = node.textContent;           
                }                      
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'url')]/@href", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.URL = node.textContent;
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'nickname')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.NickName = node.textContent;    
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'org')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.Org = node.textContent;                                   
                
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'family-name')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.LastName = node.textContent;
                
                node = document.evaluate("//*[contains(@class, 'vcard')]//a[contains(@class, 'email')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.Email = node.textContent;
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'tel')]//*[contains(@class, 'value')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.Phone = node.textContent;           
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'street-address')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.DefaultStreet = node.textContent;
                
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'locality')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.DefaultCity = node.textContent;
                
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'region')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.DefaultState = node.textContent;
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'postal-code')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.DefaultZip = node.textContent;           
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'country-name')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.DefaultCountry = node.textContent;
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'geo')]/abbr[contains(@class, 'latitude')]/@title", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.DefaultLatitude = node.textContent;
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'geo')]/abbr[contains(@class, 'longitude')]/@title", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.DefaultLongitude = node.textContent;        
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Home' or *[contains(@class, 'type')] = 'home')]//*[contains(@class, 'street-address')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.HomeStreet = node.textContent;
                
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Home' or *[contains(@class, 'type')] = 'home')]//*[contains(@class, 'locality')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.HomeCity = node.textContent;
                
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Home' or *[contains(@class, 'type')] = 'home')]//*[contains(@class, 'region')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.HomeState = node.textContent;
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Home' or *[contains(@class, 'type')] = 'home')]//*[contains(@class, 'postal-code')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.HomeZip = node.textContent;           
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Home' or *[contains(@class, 'type')] = 'home')]//*[contains(@class, 'country-name')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.HomeCountry = node.textContent;
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Home' or *[contains(@class, 'type')] = 'home')]/div[contains(@class, 'geo')]/abbr[contains(@class, 'latitude')]/@title", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.HomeLatitude = node.textContent;
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Home' or *[contains(@class, 'type')] = 'home')]/div[contains(@class, 'geo')]/abbr[contains(@class, 'longitude')]/@title", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.HomeLongitude = node.textContent;                   
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Work' or *[contains(@class, 'type')] = 'work')]//*[contains(@class, 'street-address')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.BusinessStreet = node.textContent;
                
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Work' or *[contains(@class, 'type')] = 'work')]//*[contains(@class, 'locality')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.BusinessCity = node.textContent;
                
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Work' or *[contains(@class, 'type')] = 'work')]//*[contains(@class, 'region')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.BusinessState = node.textContent;
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Work' or *[contains(@class, 'type')] = 'work')]//*[contains(@class, 'postal-code')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.BusinessZip = node.textContent;           
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Work' or *[contains(@class, 'type')] = 'work')]//*[contains(@class, 'country-name')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.BusinessCountry = node.textContent;
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Work' or *[contains(@class, 'type')] = 'work')]/div[contains(@class, 'geo')]/abbr[contains(@class, 'latitude')]/@title", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.BusinessLatitude = node.textContent;
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Work' or *[contains(@class, 'type')] = 'work')]/div[contains(@class, 'geo')]/abbr[contains(@class, 'longitude')]/@title", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.BusinessLongitude = node.textContent;        
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Current' or *[contains(@class, 'type')] = 'current')]//*[contains(@class, 'street-address')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.CurrentStreet = node.textContent;
                
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Current' or *[contains(@class, 'type')] = 'current')]//*[contains(@class, 'locality')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.CurrentCity = node.textContent;
                
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Current' or *[contains(@class, 'type')] = 'current')]//*[contains(@class, 'region')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.CurrentState = node.textContent;
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Current' or *[contains(@class, 'type')] = 'current')]//*[contains(@class, 'postal-code')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.CurrentZip = node.textContent;           
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Current' or *[contains(@class, 'type')] = 'current')]//*[contains(@class, 'country-name')]", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.CurrentCountry = node.textContent;
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Current' or *[contains(@class, 'type')] = 'current')]/div[contains(@class, 'geo')]/abbr[contains(@class, 'latitude')]/@title", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.CurrentLatitude = node.textContent;
                    
                node = document.evaluate("//*[contains(@class, 'vcard')]//*[contains(@class, 'adr')  and (*[contains(@class, 'type')] = 'Current' or *[contains(@class, 'type')] = 'current')]/div[contains(@class, 'geo')]/abbr[contains(@class, 'longitude')]/@title", hCardXmlNode, null, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
                if (node)
                    self.CurrentLongitude = node.textContent;
            }
        }
    }
    
    self.buildHtml();
    self.initFromXmlString(self.HTML);
}