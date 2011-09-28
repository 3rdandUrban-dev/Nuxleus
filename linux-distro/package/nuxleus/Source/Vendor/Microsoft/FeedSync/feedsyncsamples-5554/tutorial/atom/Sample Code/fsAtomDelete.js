//  *********************************************************************************
//  File:	fsAtomDelete.js
//  Notes:	You must run this file with cscript.exe (i.e. not wscript.exe)
//
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.
//  *********************************************************************************


//  -------------------------- MAIN (BEGIN) --------------------------

//  Validate arguments
var g_Arguments = WScript.Arguments;
if (g_Arguments.length < 2)
	{
	DisplayUsage();
	WScript.Quit();
	}

//  Get required parameters
var g_SourcePath = g_Arguments(0);
var g_GUID = g_Arguments(1);

//  Get optional parameters if specified, otherwise use defaults

//  *********************************************************************************
//  BIG HONKING NOTE:  The "by" value should be a unique value per user/endpoint -
//                     this sample uses a random number to generate uniqueness.
//                     Other applications should considering using a more robust
//                     and persistant value.

	var g_By = "fsAtomDelete.js" + Math.random();
	if (g_Arguments.length > 2)
		g_By = g_Arguments(2);

//  *********************************************************************************
	
var g_pISourceAtomXmlDOMDocument = null;

try
	{
	//  Create instance of XML DOM
	g_pISourceAtomXmlDOMDocument = new ActiveXObject("Microsoft.XMLDOM");
	g_pISourceAtomXmlDOMDocument.async = false;

	//  Load source document	
	var Success = g_pISourceAtomXmlDOMDocument.load(g_SourcePath);
	if (!Success)
		throw new Error(0, "IXmlDocument::load failed");
	}
catch (e)
	{
	WScript.Echo("Exception while loading '" + g_SourcePath + "': " + e.message);
	WScript.Quit();
	}

//  Get "feed" element
var g_pIFeedXmlDOMElement = g_pISourceAtomXmlDOMDocument.documentElement;

//  Check if FeedSync namespace exists
if (g_pIFeedXmlDOMElement.getAttribute("xmlns:sx") == null)
	{
	WScript.Echo("Not a valid FeedSync file!");
	WScript.Quit();
	}

//  Get "sx:sync" element with matching id
var g_pISyncXmlDOMElement = g_pIFeedXmlDOMElement.selectSingleNode("//entry/sx:sync[@id='" + g_GUID + "']");

//  Validate that "sx:sync" element exists
if (g_pISyncXmlDOMElement == null)
	{
	WScript.Echo("Unable to find 'sx:sync' element with id='" + g_GUID + "'");
	WScript.Quit(0);
	}

//  Get "updates" attribute from "sx:sync" element
var g_Updates = g_pISyncXmlDOMElement.getAttribute("updates");

//  Validate "updates" attribute
if (g_Updates == null)
	{
	WScript.Echo("Unable to get 'updates' attribute from 'sx:sync' element with id='" + g_GUID + "'");
	WScript.Quit(0);
	}

//  Get corresponding "entry" element
var g_pIEntryXmlDOMElement = g_pISyncXmlDOMElement.parentNode;
	
//  Validate that "entry" element exists
if (g_pIEntryXmlDOMElement == null)
	{
	WScript.Echo("Unable to get 'entry' element for 'sx:sync' element with id='" + g_GUID + "'");
	WScript.Quit(0);
	}

//  Get "author" element from "entry" element
var g_pIAuthorXmlDOMElement = g_pIEntryXmlDOMElement.selectSingleNode("author");

//  Validate that "author" element exists
if (g_pIAuthorXmlDOMElement == null)
	{
	//  Create & populate "author" element
	g_pIAuthorXmlDOMElement = g_pISourceAtomXmlDOMDocument.createElement("author");

	//  Append "author" element to "entry" element
	g_pIEntryXmlDOMElement.appendChild(g_pIAuthorXmlDOMElement);
	}

//  Get "name" element from "author" element
var g_pINameXmlDOMElement = g_pIAuthorXmlDOMElement.selectSingleNode("name");

//  Validate that "name" element exists
if (g_pINameXmlDOMElement == null)
	{
	//  Create & populate "name" element
	g_pINameXmlDOMElement = g_pISourceAtomXmlDOMDocument.createElement("name");

	//  Append "name" element to "author" element
	g_pIAuthorXmlDOMElement.appendChild(g_pINameXmlDOMElement);
	}

//  Set by for "name" element
g_pINameXmlDOMElement.text = g_By;
	
//  Get the current timedate and format it as RFC 3339
var g_CurrentDateTime = new Date();
var g_Year = g_CurrentDateTime.getYear();
if (g_Year < 70)
    g_Year += 2000;
else if (g_Year < 1900)
    g_Year += 1900;

var g_Month = g_CurrentDateTime.getMonth() + 1;
if (g_Month <= 9)
    g_Month = "0" + g_Month;

var g_Day = g_CurrentDateTime.getDate();
if (g_Day <= 9)
    g_Day = "0" + g_Day;

var g_HourUTC = g_CurrentDateTime.getUTCHours();
if (g_HourUTC <= 9)
    g_HourUTC = "0" + g_HourUTC;

var g_MinuteUTC = g_CurrentDateTime.getUTCMinutes();
if (g_MinuteUTC <= 9)
    g_MinuteUTC = "0" + g_MinuteUTC;
    
var g_Second = g_CurrentDateTime.getSeconds();
if (g_Second <= 9)
    g_Second = "0" + g_Second;

var g_RFC3339DateTime = g_Year + "-" + g_Month + "-" + g_Day + "T" + g_HourUTC + ":" + g_MinuteUTC + ":" + g_Second + "Z";

//  Get "updated" element from "updated" element
var g_pIUpdatedXmlDOMElement = g_pIEntryXmlDOMElement.selectSingleNode("updated");

//  Validate that "updated" element exists
if (g_pIUpdatedXmlDOMElement == null)
	{
	//  Create "updated" element
	g_pIUpdatedXmlDOMElement = g_pISourceAtomXmlDOMDocument.createElement("updated");

	//  Append "updated" element to "entry" element
	g_pIEntryXmlDOMElement.appendChild(g_pIUpdatedXmlDOMElement);
	}

//  Set updated for "updated" element
g_pIUpdatedXmlDOMElement.text = g_RFC3339DateTime;

//  Increment "updates" attribute
++g_Updates;

//  Create "sx:history" element
var g_pIHistoryXmlDOMElement = g_pISourceAtomXmlDOMDocument.createElement("sx:history");

//  Set "sequence" attribute for "sx:history" element
g_pIHistoryXmlDOMElement.setAttribute("sequence", g_Updates);

//  Set "when" attribute for "sx:history" element
g_pIHistoryXmlDOMElement.setAttribute("when", g_RFC3339DateTime);

//  Set "by" attribute for "sx:history" element
g_pIHistoryXmlDOMElement.setAttribute("by", g_By);

//  Insert "sx:history" element as topmost sub-element of "sx:sync" element
g_pISyncXmlDOMElement.insertBefore(g_pIHistoryXmlDOMElement, g_pISyncXmlDOMElement.childNodes[0]);
	
//  Set "updates" attribute for "sx:sync" element
g_pISyncXmlDOMElement.setAttribute("updates", g_Updates);

//  Set "deleted" attribute for "sx:sync" element
g_pISyncXmlDOMElement.setAttribute("deleted", "true");

//  Save modified contents to standand output stream
WScript.StdOut.Write(g_pISourceAtomXmlDOMDocument.xml);

//  -------------------------- MAIN (END) --------------------------


function DisplayUsage(i_Text)
	{
	var Text = "Usage:\r\nfsAtomDelete.js [SourcePath] [GUID]\r\n\r\nParameters:\r\n  SourcePath=fully qualified filename of original Atom document\r\n  GUID=unique value of existing item\r\n";

	//  If text was provided, prepend it to default usage text
	if ((i_Text != null) && (i_Text != ""))
		Text = i_Text + "\r\n\r\n" + Text;
	
	WScript.Echo(Text);
	}	