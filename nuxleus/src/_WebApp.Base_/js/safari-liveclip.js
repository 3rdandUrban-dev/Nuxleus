/* Copyright 2006 Microsoft Corporation.  Microsoft's copyrights in this work are licensed under the Creative Commons */
/* Attribution-ShareAlike 2.5 License.  To view a copy of this license visit http://creativecommons.org/licenses/by-sa/2.5 */

WebClip = function(clipBoardControlContainer, copyCallback, pasteCallback, onControlSelected, onControlDeSelected, disableContentParsing) 
{
  // Populate the input container (usually a div) with control container w/ input.
  
  var self = this;
  var clipBoardControlInput = document.createElement("textarea");
  clipBoardControlInput.rows = 1;;
  clipBoardControlInput.className = "CopyPasteInput";
  clipBoardControlInput.setAttribute("autocomplete", "off");  
  clipBoardControlInput.value = "intialValueToHideCursor";
  var lastKnownClipBoardValue = clipBoardControlInput.value;
  
  this.controlSelectedCallback = onControlSelected;
  this.controlDeSelectedCallback = onControlDeSelected;
  this.clickSelected = false;    
  
  this.controlDiv = document.createElement("div");
  this.controlDiv.className = "webClipControlDiv";
  this.controlDiv.appendChild(clipBoardControlInput);
  clipBoardControlContainer.appendChild(this.controlDiv);
  
  ControlsOnPage[ControlsOnPage.length] = self;
  
  var pauseInputCheck = false;
  
  this.GetCurrentValue = function()
  {
    return clipBoardControlInput.value;
  }
  
  this.PrepareForCopyPaste = function()
  {
    pauseInputCheck = true;
    clipBoardControlInput.value = (disableContentParsing ? copyCallback() : self.serializeWebClipboard(copyCallback()));
    lastKnownClipBoardValue = clipBoardControlInput.value;
    pauseInputCheck = false;
    clipBoardControlInput.select();
  }
  
  this._onClick = function(e)
  {
    // Have to register onclick separately in Mozilla, because the text selection is unpredictable with left click (puts a cursor
    // in the input instead of select all every other time).
    
    self.PrepareForCopyPaste();
    
    for (var i = 0; i < ControlsOnPage.length; i++) 
    {
      ControlsOnPage[i].clickSelected = false;
      ControlsOnPage[i].controlDiv.className = "webClipControlDiv";
      ControlsOnPage[i].controlDeSelectedCallback()
    }
    
    self.clickSelected = true;
    self.controlDiv.className = "webClipControlSelectedDiv";
    self.controlSelectedCallback();
  }
  
  this._onMouseDown = function(e)
  {
    if (!e) 
    {
      e = window.event;
    }
    
    if (e.button == 2)
    {        
      self.PrepareForCopyPaste();
      
      for (var i = 0; i < ControlsOnPage.length; i++) 
      {
        if (ControlsOnPage[i].clickSelected)
        {
          ControlsOnPage[i].clickSelected = false;
          ControlsOnPage[i].controlDiv.className = "webClipControlDiv";
          ControlsOnPage[i].controlDeSelectedCallback()
        }
      }
      
      self.clickSelected = true;
      self.controlDiv.className = "webClipControlSelectedDiv";
      self.controlSelectedCallback();
    }
  }
  
  this._onMouseUp = function(e)
  {
    if (!e) 
    {
      e = window.event;
    }
    
    // Don't leave selected for right-click.
    // The input will still be active.  If it is unselected here, copy/paste from the context menu won't work, 
    //         because this event fires before the menu is drawn.
    if (e.button == 2)
    {
      self.clickSelected = false;
      self.controlDiv.className = "webClipControlDiv";
      self.controlDeSelectedCallback();
    }        
  }
  
  this._onFocus = function(e)
  {
    self.clickSelected = true;
    self.controlDiv.className = "webClipControlSelectedDiv";
    self.controlSelectedCallback();
  }
  
  this._onBlur = function(e)
  {
    self.clickSelected = false;
    self.controlDiv.className = "webClipControlDiv";
    self.controlDeSelectedCallback();
  }
  
  this.checkInputValue = function()
  {
    if (!pauseInputCheck && (clipBoardControlInput.value != lastKnownClipBoardValue))
    {
      lastKnownClipBoardValue = clipBoardControlInput.value;
      clipBoardControlInput.blur();
      self.handlePastedData(lastKnownClipBoardValue);
    }
    
    window.setTimeout(self.checkInputValue, 50);
  }
  
  this.handlePastedData = function(dataString)
  {
    var clipData = (disableContentParsing ? null : self.parseWebClipboardXml(dataString));
    pasteCallback(clipData, dataString);
  }
  
  this.serializeWebClipboard = function(clipData)
  {
    var xmlString = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><liveclipboard version=\"0.92\"" + (clipData.source ?  ("  source=\"" + clipData.source + "\"") : "") + (clipData.description ? (" description=\"" + clipData.description + "\"") : "") + " xmlns:lc=\"http://www.microsoft.com/schemas/liveclipboard\">";
    
    if (clipData.data && clipData.data.formats && (clipData.data.formats.length > 0))
    {
      xmlString += "<lc:data>";
      for (var i = 0; i < clipData.data.formats.length; i++)
      {
        xmlString += "<lc:format type=\"" + clipData.data.formats[i].type + "\" contenttype=\"" + clipData.data.formats[i].contentType + "\">";
        
        for (var j = 0; j < clipData.data.formats[i].items.length; j++)
        {
          xmlString += "<lc:item" + ((clipData.data.formats[i].items[j].link == null) ? "" : (" ref=\"" + clipData.data.formats[i].items[j].link + "\"")) + ">";
          
          if (clipData.data.formats[i].items[j].xmlData)
          {
            if (clipData.data.formats[i].items[j].xmlData.xml)
            {
              xmlString += (clipData.data.formats[i].items[j].xmlData.xml);
            }
            else
            {
              var serializer = new XMLSerializer();
              xmlString += serializer.serializeToString(clipData.data.formats[i].items[j].xmlData);
            }
          }
          else if (clipData.data.formats[i].items[j].data)
          {
            xmlString += ("<![CDATA[" + clipData.data.formats[i].items[j].data + "]]>");
          }
          
          xmlString += "</lc:item>";
        }
        
        xmlString += "</lc:format>";
      }
      
      xmlString += "</lc:data>"
    }
    
    if (clipData.feeds && clipData.feeds.feeds && (clipData.feeds.feeds.length > 0))
    {
      xmlString += "<lc:feeds>";
      
      for (var i = 0; i < clipData.feeds.feeds.length; i++)
      {
        xmlString += "<lc:feed type=\"" + clipData.feeds.feeds[i].type + "\" " + ((clipData.feeds.feeds[i].link == null) ? "" : ("ref=\"" + clipData.feeds.feeds[i].link + "\"")) + " description=\"" + clipData.feeds.feeds[i].description + "\" authtype=\"" + clipData.feeds.feeds[i].authType + "\">";
        
        if (clipData.feeds.feeds[i].itemMap)
        {
          xmlString += "<lc:feeditems type=\"" + clipData.feeds.feeds[i].itemMap.itemDataType + "\" contenttype=\"" + clipData.feeds.feeds[i].itemMap.itemContentType + "\"" + ((clipData.feeds.feeds[i].itemMap.path == null) ? "" : (" xpath=\"" + clipData.feeds.feeds[i].itemMap.path + "\"")) + ">";                   
          
          if (clipData.feeds.feeds[i].itemMap.itemIds)
          {
            for (var j = 0; j < clipData.feeds.feeds[i].itemMap.itemIds.length; j++)
            {
              xmlString += "<lc:feedItem id=\"" + clipData.feeds.feeds[i].itemMap.itemIds[j] + "\"/>";
            }
          }
          
          xmlString += "</lc:feeditems>";
        }
        
        xmlString += "</lc:feed>";
      }
      
      xmlString += "</lc:feeds>";
    }
    
    if (clipData.presentations && clipData.presentations.formats && (clipData.presentations.formats.length > 0))
    {
      xmlString += "<lc:presentations>";
      
      for (var i = 0; i < clipData.presentations.formats.length; i++)
      {
        xmlString += "<lc:format type=\"" + clipData.presentations.formats[i].contentType + "\">";
        xmlString += ("<![CDATA[" + clipData.presentations.formats[i].data + "]]>");
        xmlString += "</lc:format>";
      }
      
      xmlString += "</lc:presentations>"
    }
    
    xmlString += "</liveclipboard>";
    
    return xmlString;
  }
  
  this.parseWebClipboardXml = function(xmlString) 
  {
    // Undone: catch exceptions and return empty clipData?
    var xmlDocument;
    var clipData = new LiveClipboardContent();
    
    if ((xmlString != null) && (xmlString != "")) 
    {
      
      var domParser = new DOMParser();
      var xmlDocument = domParser.parseFromString(xmlString, 'application/xml');
      
      if (document.evaluate)
      {
        clipData.version = document.evaluate("/*/@version", xmlDocument, resolveNamespace, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext().nodeValue;
        var node;
        clipData.source = (node = document.evaluate("/*/@source", xmlDocument, resolveNamespace, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext()) ? node.nodeValue : null;
        clipData.description = (node = document.evaluate("/*/@description", xmlDocument, resolveNamespace, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext()) ? node.nodeValue : null;
        var formatNodeResult = document.evaluate("/*/lc:data/lc:format", xmlDocument, resolveNamespace, 0 /*XPathResult.ANY_TYPE*/, null);
        var formatNode = formatNodeResult.iterateNext();
        
        while (formatNode)
        {
          var format = new DataFormat();
          var typeNode = formatNode.getAttributeNode("type");
          
          if (typeNode)
            format.type = typeNode.nodeValue;
          
          format.contentType = formatNode.getAttributeNode("contenttype").nodeValue;
          
          var dataItemResult = document.evaluate("lc:item", formatNode, resolveNamespace, 0 /*XPathResult.ANY_TYPE*/, null);
          var dataItemNode = dataItemResult.iterateNext();
          
          while (dataItemNode)
          {
            var item = new DataItem();
            
            if (dataItemNode.childNodes)
            {
              for (var i = 0; i < dataItemNode.childNodes.length; i++)
              {
                if (dataItemNode.childNodes[i].nodeType == Node.ELEMENT_NODE)
                {
                  item.xmlData = dataItemNode.childNodes[i];
                  break;
                }
              }
            }
            
            if (!item.xmlData)
              item.data = dataItemNode.textContent;
            
            var linkNode = dataItemNode.getAttributeNode("ref");
            
            if (linkNode)
              item.link = linkNode.nodeValue;
            
            format.items[format.items.length] = item;
            dataItemNode = dataItemResult.iterateNext();
          }
          
          clipData.data.formats[clipData.data.formats.length] = format;
          formatNode = formatNodeResult.iterateNext();
        }
        
        var feedNodeResult = document.evaluate("/*/lc:feeds/lc:feed", xmlDocument, resolveNamespace, 0 /*XPathResult.ANY_TYPE*/, null);
        var feedNode = feedNodeResult.iterateNext();
        
        while (feedNode)
        {
          var feed = new Feed();
          feed.type = feedNode.getAttributeNode("type").nodeValue;
          feed.description = feedNode.getAttributeNode("description").nodeValue;
          //feed.itemType = feedNode.getAttributeNode("itemtype").nodeValue;
          feed.authType = feedNode.getAttributeNode("authtype").nodeValue;
          feed.link = feedNode.getAttributeNode("ref").nodeValue;
          
          var itemMapNode = document.evaluate("lc:feeditems", feedNode, resolveNamespace, 0 /*XPathResult.ANY_TYPE*/, null).iterateNext();
          
          if (itemMapNode)
          {
            feed.itemMap = new FeedItemMap();
            
            if (itemMapNode.getAttributeNode("type"))
              feed.itemMap.itemDataType = itemMapNode.getAttributeNode("type").nodeValue;
            
            if (itemMapNode.getAttributeNode("contenttype"))
              feed.itemMap.itemContentType = itemMapNode.getAttributeNode("contenttype").nodeValue;
            
            if (itemMapNode.getAttributeNode("xpath"))
              feed.itemMap.path = itemMapNode.getAttributeNode("xpath").nodeValue;
            
            var itemMappingNodesResult = document.evaluate("lc:feedItem", itemMapNode, resolveNamespace, 0 /*XPathResult.ANY_TYPE*/, null);
            var itemMappingNode = itemMappingNodesResult.iterateNext();
            
            while (itemMappingNode)
            {
              if (itemMappingNode.getAttributeNode("id"))
                feed.itemMap.itemIds[feed.itemMap.itemIds.length] = itemMappingNode.getAttributeNode("id").nodeValue;
              
              itemMappingNode = itemMappingNodesResult.iterateNext();
            }
          }
          clipData.feeds.feeds[clipData.feeds.feeds.length] = feed;
          feedNode = feedNodeResult.iterateNext();
        }
        
        var presentationNodeResult = document.evaluate("/*/lc:presentations/lc:format", xmlDocument, resolveNamespace, 0 /*XPathResult.ANY_TYPE*/, null);
        var presentationNode = presentationNodeResult.iterateNext();
        
        while (presentationNode)
        {
          var format = new PresentationFormat();
          format.contentType = presentationNode.getAttributeNode("contenttype").nodeValue;
          format.data = presentationNode.textContent;
          clipData.presentations.formats[i] = format;
          presentationNode = presentationNodeResult.iterateNext();
        }                    
      }
      else
      {
        for (var i = 0; i < xmlDocument.childNodes.length; i++)
        {
          if (xmlDocument.childNodes[i].nodeName == "liveclipboard")
          {
            for (var j = 0; j < xmlDocument.childNodes[i].attributes.length; j++)
            {
              if (xmlDocument.childNodes[i].attributes[j].nodeName == "version")
                clipData.version = xmlDocument.childNodes[i].attributes[j].nodeValue;
              
              if (xmlDocument.childNodes[i].attributes[j].nodeName == "source")
                clipData.source = xmlDocument.childNodes[i].attributes[j].nodeValue;
              
              if (xmlDocument.childNodes[i].attributes[j].nodeName == "description")
                clipData.description = xmlDocument.childNodes[i].attributes[j].nodeValue;                                                                        
            }
            
            for (var j = 0; j < xmlDocument.childNodes[i].childNodes.length; j++)
            {
              if (xmlDocument.childNodes[i].childNodes[j].nodeName == "lc:data")
              {
                for (var k = 0; k < xmlDocument.childNodes[i].childNodes[j].childNodes.length; k++)
                {
                  if (xmlDocument.childNodes[i].childNodes[j].childNodes[k].nodeName == "lc:format")
                  {
                    var format = new DataFormat();
                    
                    for (var l = 0; l < xmlDocument.childNodes[i].childNodes[j].childNodes[k].attributes.length; l++)
                    {                                            
                      if (xmlDocument.childNodes[i].childNodes[j].childNodes[k].attributes[l].nodeName == "type")
                        format.type = xmlDocument.childNodes[i].childNodes[j].childNodes[k].attributes[l].nodeValue;
                      
                      else if (xmlDocument.childNodes[i].childNodes[j].childNodes[k].attributes[l].nodeName == "contenttype")
                        format.contentType = xmlDocument.childNodes[i].childNodes[j].childNodes[k].attributes[l].nodeValue;
                    }
                    
                    for (var l = 0; l < xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes.length; l++)
                    {
                      if (xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].nodeName == "lc:item")
                      {
                        var dataItem = new DataItem();
                        
                        if (xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].childNodes)
                        {
                          for (var m = 0; m    < xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].childNodes.length; m++)
                          {
                            if (xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].childNodes[m].nodeType == Node.ELEMENT_NODE)
                            {
                              dataItem.xmlData = xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].childNodes[m];
                              break;
                            }
                          }
                        }
                        
                        if (!dataItem.xmlData)
                          dataItem.data = xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].textContent;
                        
                        format.items[format.items.length] = dataItem;
                        
                        for (var m = 0; m < xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].attributes.length; m++)
                        {
                          if (xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].attributes[m].nodeName == "ref")
                            format.items[l].link = xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].attributes[m].nodeValue;
                        }
                      }
                    }
                    
                    clipData.data.formats[clipData.data.formats.length] = format;
                  }
                }
              }
              else if (xmlDocument.childNodes[i].childNodes[j].nodeName == "lc:feeds")
              {
                for (var k = 0; k < xmlDocument.childNodes[i].childNodes[j].childNodes.length; k++)
                {
                  if (xmlDocument.childNodes[i].childNodes[j].childNodes[k].nodeName == "lc:feed")
                  {
                    var feed = new Feed();
                    
                    for (var l = 0; l < xmlDocument.childNodes[i].childNodes[j].childNodes[k].attributes.length; l++)
                    {
                      if (xmlDocument.childNodes[i].childNodes[j].childNodes[k].attributes[l].nodeName == "type")
                        feed.type = xmlDocument.childNodes[i].childNodes[j].childNodes[k].attributes[l].nodeValue;
                      
                      else if (xmlDocument.childNodes[i].childNodes[j].childNodes[k].attributes[l].nodeName == "ref")
                        feed.link = xmlDocument.childNodes[i].childNodes[j].childNodes[k].attributes[l].nodeValue;
                      
                      else if (xmlDocument.childNodes[i].childNodes[j].childNodes[k].attributes[l].nodeName == "description")
                        feed.description = xmlDocument.childNodes[i].childNodes[j].childNodes[k].attributes[l].nodeValue;
                      
                      else if (xmlDocument.childNodes[i].childNodes[j].childNodes[k].attributes[l].nodeName == "authtype")
                        feed.authType = xmlDocument.childNodes[i].childNodes[j].childNodes[k].attributes[l].nodeValue;
                    }
                    
                    for (var l = 0; l < xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes.length; l++)
                    {
                      if (xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].nodeName == "lc:feeditems")
                      {
                        feed.itemMap = new FeedItemMap();
                        
                        for (var m = 0; m < xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].attributes.length; m++)
                        {
                          if (xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].attributes[m].nodeName == "type")
                            feed.itemMap.itemDataType = xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].attributes[m].nodeValue;
                          
                          else if (xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].attributes[m].nodeName == "contenttype")
                            feed.itemMap.itemContentType = xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].attributes[m].nodeValue;                                                            
                          
                          else if (xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].attributes[m].nodeName == "xpath")
                            feed.itemMap.path = xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].attributes[m].nodeValue;
                        }
                        
                        for (var m = 0; m < xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].childNodes.length; m++)
                        {
                          if (xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].childNodes[m].nodeName == "lc:feedItem")
                          {
                            for (var n = 0; n < xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].childNodes[m].attributes.length; n++)
                            {
                              if (xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].childNodes[m].attributes[n].nodeName == "id")
                                feed.itemMap.itemIds[feed.itemMap.itemIds.length] = xmlDocument.childNodes[i].childNodes[j].childNodes[k].childNodes[l].childNodes[m].attributes[n].nodeValue;
                            }
                          }
                        }
                      }
                    }
                    
                    clipData.feeds.feeds[clipData.feeds.feeds.length] = feed;
                  }
                  
                }                                
              }
              else if (xmlDocument.childNodes[i].childNodes[j].nodeName == "lc:presentations")
              {
                for (var k = 0; k < xmlDocument.childNodes[i].childNodes[j].childNodes.length; k++)
                {                                
                  if (xmlDocument.childNodes[i].childNodes[j].childNodes[k].nodeName == "lc:format")
                  {
                    var format = new PresentationFormat();
                    
                    for (var l = 0; l < xmlDocument.childNodes[i].childNodes[j].childNodes[k].attributes.length; l++)
                    {
                      if (xmlDocument.childNodes[i].childNodes[j].childNodes[k].attributes[l].nodeName == "contenttype")
                        format.contentType = xmlDocument.childNodes[i].childNodes[j].childNodes[k].attributes[l].nodeValue;
                    }
                    
                    format.data = xmlDocument.childNodes[i].childNodes[j].childNodes[k];                                            
                    clipData.presentations.formats[clipData.presentations.formats.length] = format;
                  }
                }
              }
            }
          }
        }
      }
    }
    
    return clipData;
  }
  
  clipBoardControlInput.onmousedown = self._onMouseDown;
  clipBoardControlInput.onmouseup = self._onMouseUp;
  clipBoardControlInput.onclick = self._onClick;
  clipBoardControlInput.onfocus = self._onFocus;
  clipBoardControlInput.onblur = self._onBlur; 
  
  clipBoardControlInput.blur();
  window.setTimeout(self.checkInputValue, 50);
}
