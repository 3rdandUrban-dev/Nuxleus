/* Copyright 2006 Microsoft Corporation.  Microsoft's copyrights in this work are licensed under the Creative Commons */
/* Attribution-ShareAlike 2.5 License.  To view a copy of this license visit http://creativecommons.org/licenses/by-sa/2.5 */


var ControlsOnPage = new Array();

function setSelectionRange(input, begin, end) 
{
    if (input.setSelectionRange) {
        input.focus();
        input.setSelectionRange(begin, end);
    }
    else if (input.createTextRange) 
    {
        var range = input.createTextRange();
        range.collapse(true);
        range.moveEnd('character', selectionEnd);
        range.moveStart('character', selectionStart);
        range.select();
    }
}
function selectAllText(input)
{
    setSelectionRange(input, 0, input.value.length);
}

function resolveNamespace(prefix) 
{
    if(prefix == "lc") 
    {
      return "http://www.microsoft.com/schemas/liveclipboard";
    }

    return null;
}

LiveClipboardContent = function()
{
    this.version = "0.92";
    this.data = new ClipboardData();
    this.feeds = new ClipboardFeeds();
    this.presentations = new ClipboardPresentations();
}

ClipboardData = function()
{
    // Array of DataFormat
    this.formats = new Array();
}

DataFormat = function() 
{
    // Type of the data, e.g. "vcard".
    this.type;
    
    // ContentType, e.g. "application/xhtml+xml"
    this.contentType;
    
    // Array of DataItem
    this.items = new Array();
}

DataItem = function()
{
    // CDATA encoded data.
    this.data;
    
    // XML node -- only present if the item data is XML.
    this.xmlData;
    
    // Url to the source content.
    this.link;
}

ClipboardFeeds = function()
{
    // Array of Feed
    this.feeds = new Array();
}

Feed = function()
{
    this.type;
    this.description;
    this.link;
    this.authType;
    this.itemMap = new FeedItemMap();
}

FeedItemMap = function()
{
    // Type of the associated data, e.g. "vcard".
    this.itemDataType;
    
    // ContentType, e.g. "application/xhtml+xml"
    this.itemContentType;
    
    // XPath location of the associated data, e.g. "/rss/channel/item/description"
    this.path;
    this.itemIds = new Array();
}

ClipboardPresentations = function()
{
    // Array of PresentationFormat
    this.formats = new Array();
}

PresentationFormat = function()
{
    // ContentType, e.g. "text/html"
    this.contentType;
    
    // CDATA encoded data.
    this.data;
}

