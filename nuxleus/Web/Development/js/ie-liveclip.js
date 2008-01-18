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
  this.parseWebClipboardXml = function(xmlString) 
  {
    // Undone: catch exceptions and return empty clipData?
    var xmlDocument;
    var clipData = new LiveClipboardContent();
    {
      xmlDocument = new ActiveXObject("Microsoft.XMLDOM");
      xmlDocument.async=false;
      xmlDocument.loadXML(xmlString);
      xmlDocument.setProperty("SelectionLanguage", "XPath");
      xmlDocument.setProperty("SelectionNamespaces", "xmlns:lc='http://www.microsoft.com/schemas/liveclipboard'");
      clipData.version = xmlDocument.selectSingleNode("/liveclipboard/@version").nodeTypedValue;
      var node;
      clipData.source = (node = xmlDocument.selectSingleNode("/liveclipboard/@source")) ? node.nodeTypedValue : null;
      clipData.description = (node = xmlDocument.selectSingleNode("/liveclipboard/@description")) ? node.nodeTypedValue : null;
      var dataFormatNodes = xmlDocument.selectNodes("/liveclipboard/lc:data/lc:format");
      
      for (var i = 0; i < dataFormatNodes.length; i++)
      {
        var format = new DataFormat();
        var typeNode = dataFormatNodes[i].selectSingleNode("@type");
        
        if (typeNode)
          format.type = typeNode.nodeTypedValue;
        
        format.contentType = dataFormatNodes[i].selectSingleNode("@contenttype").nodeTypedValue;
        
        var dataItems = dataFormatNodes[i].selectNodes("lc:item")
        
        for (var j = 0; j < dataItems.length; j++)
        {
          var dataItemNode = dataItems[j];
          var item = new DataItem();
          
          if (dataItemNode.childNodes)
          {
            for (var i = 0; i < dataItemNode.childNodes.length; i++)
            {
              // Find first element node.
              if (dataItemNode.childNodes[i].nodeType == 1)
              {
                item.xmlData = dataItemNode.childNodes[i];
                break;
              }
            }
          }
          
          if (!item.xmlData)
            item.data = dataItemNode.nodeTypedValue;
          
          var linkNode = dataItems[j].selectSingleNode("@ref");
          
          if (linkNode)
            item.link = linkNode.nodeTypedValue;
          
          format.items[j] = item;
        }
        
        clipData.data.formats[i] = format;
      }
      
      var feedNodes = xmlDocument.selectNodes("/liveclipboard/lc:feeds/lc:feed");
      
      for (var i = 0; i < feedNodes.length; i++)
      {
        var feed = new Feed();
        feed.type = feedNodes[i].selectSingleNode("@type").nodeTypedValue;
        feed.description = feedNodes[i].selectSingleNode("@description").nodeTypedValue;
        feed.authType = feedNodes[i].selectSingleNode("@authtype").nodeTypedValue;
        feed.link = feedNodes[i].selectSingleNode("@ref").nodeTypedValue;
        
        var itemMapNode = feedNodes[i].selectSingleNode("lc:feeditems");
        
        if (itemMapNode)
        {
          feed.itemMap = new FeedItemMap();
          feed.itemMap.itemDataType = itemMapNode.selectSingleNode("@type").nodeTypedValue;
          feed.itemMap.itemContentType = itemMapNode.selectSingleNode("@contenttype").nodeTypedValue;
          feed.itemMap.path = itemMapNode.selectSingleNode("@xpath").nodeTypedValue;
          var itemMapNodes = itemMapNode.selectNodes("lc:feedItem");
          
          if (itemMapNodes)
          {
            feed.itemMap.itemIds = new Array(itemMapNodes.length);
            
            for (var j = 0; j < itemMapNodes.length; j++)
            {
              feed.itemMap.itemIds[j] = itemMapNodes[j].selectSingleNode("@id").nodeTypedValue;
            }
          }
        }
        
        clipData.feeds.feeds[i] = feed;
      }
      
      var presentationFormatNodes = xmlDocument.selectNodes("/liveclipboard/lc:presentations/lc:format");
      
      for (var i = 0; i < presentationFormatNodes.length; i++)
      {
        var format = new PresentationFormat();
        format.contentType = presentationFormatNodes[i].selectSingleNode("@contenttype").nodeTypedValue;
        format.data = presentationFormatNodes[i].nodeTypedValue;
        
        clipData.presentations.formats[i] = format;
      }
      
      return clipData;
    }
  }
  
  return clipData;
}
