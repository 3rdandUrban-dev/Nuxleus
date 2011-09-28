//  *********************************************************************************
//  File:	fsRSSCreate.js
//  Notes:	You must run this file with cscript.exe (i.e. not wscript.exe)
//
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.
//  *********************************************************************************


//  -------------------------- MAIN (BEGIN) --------------------------

//  Validate arguments
var g_Arguments = WScript.Arguments;
if (g_Arguments.length < 4)
	{
	DisplayUsage();
	WScript.Quit();
	}

//  Get required parameters
var g_SourcePath = g_Arguments(0);
var g_NewGUID = g_Arguments(1);
var g_Title = g_Arguments(2);
var g_Description = g_Arguments(3);

//  Get optional parameters if specified, otherwise use defaults

//  *********************************************************************************
//  BIG HONKING NOTE:  The "by" value should be a unique value per user/endpoint -
//                     this sample uses a random number to generate uniqueness.
//                     Other applications should considering using a more robust
//                     and persistant value.

	var g_By = "fsRSSCreate.js" + Math.random();
	if (g_Arguments.length > 4)
		g_By = g_Arguments(4);

//  *********************************************************************************
	
var g_pISourceRSSXmlDOMDocument = null;

try
	{
	//  Create instance of XML DOM
	g_pISourceRSSXmlDOMDocument = new ActiveXObject("Microsoft.XMLDOM");
	g_pISourceRSSXmlDOMDocument.async = false;

	//  Load source document	
	var Success = g_pISourceRSSXmlDOMDocument.load(g_SourcePath);
	if (!Success)
		throw new Error(0, "IXmlDocument::load failed");
	}
catch (e)
	{
	WScript.Echo("Exception while loading '" + g_SourcePath + "': " + e.message);
	WScript.Quit();
	}

//  Get "rss" element
var g_pIRSSXmlDOMElement = g_pISourceRSSXmlDOMDocument.documentElement;

//  Check if FeedSync namespace exists
if (g_pIRSSXmlDOMElement.getAttribute("xmlns:sx") == null)
	{
	WScript.Echo("Not a valid FeedSync file!");
	WScript.Quit();
	}

//  Get "channel" element
var g_pIChannelXmlDOMElement = g_pIRSSXmlDOMElement.selectSingleNode("channel");

//  Validate that "channel" element exists
if (g_pIChannelXmlDOMElement == null)
	{
	WScript.Echo("Unable to get 'channel' element from 'rss' element");
	WScript.Quit(0);
	}

//  Create "item" element
var g_pIItemXmlDOMElement = g_pISourceRSSXmlDOMDocument.createElement("item");

//  Create "sx:sync" element
var g_pISyncXmlDOMElement = g_pISourceRSSXmlDOMDocument.createElement("sx:sync");

//  Set "id" attribute for "sx:sync" element
g_pISyncXmlDOMElement.setAttribute("id", g_NewGUID);

//  Set "updates" attribute for "sx:sync" element
g_pISyncXmlDOMElement.setAttribute("updates", "1");

//  Set "deleted" attribute for "sx:sync" element
g_pISyncXmlDOMElement.setAttribute("deleted", "false");

//  Set "noconflicts" attribute for "sx:sync" element
g_pISyncXmlDOMElement.setAttribute("noconflicts", "false");

//  Create "history" element
var g_pIHistoryXmlDOMElement = g_pISourceRSSXmlDOMDocument.createElement("sx:history");

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

//  Set "sequence" attribute for "sx:history" element
g_pIHistoryXmlDOMElement.setAttribute("sequence", "1");

//  Set "when" attribute for "sx:history" element
g_pIHistoryXmlDOMElement.setAttribute("when", g_RFC3339DateTime);

//  Set "by" attribute for "sx:history" element
g_pIHistoryXmlDOMElement.setAttribute("by", g_By);

//  Append "sx:history" element to "sx:sync" element
g_pISyncXmlDOMElement.appendChild(g_pIHistoryXmlDOMElement);

//  Append "sx:sync" element to "item" element
g_pIItemXmlDOMElement.appendChild(g_pISyncXmlDOMElement);

//  Create & populate "title" element
var g_pITitleXmlDOMElement = g_pISourceRSSXmlDOMDocument.createElement("title");
g_pITitleXmlDOMElement.text = g_Title;

//  Append "title" element to "item" element
g_pIItemXmlDOMElement.appendChild(g_pITitleXmlDOMElement);

//  Create & populate "description" element
var g_pIDescriptionXmlDOMElement = g_pISourceRSSXmlDOMDocument.createElement("description");
g_pIDescriptionXmlDOMElement.text = g_Description;

//  Append "description" element to "item" element
g_pIItemXmlDOMElement.appendChild(g_pIDescriptionXmlDOMElement);

//  Append "item" element to "channel" element
g_pIChannelXmlDOMElement.appendChild(g_pIItemXmlDOMElement);

//  Save modified contents to standand output stream
WScript.StdOut.Write(g_pISourceRSSXmlDOMDocument.xml);

//  -------------------------- MAIN (END) --------------------------


function DisplayUsage(i_Text)
	{
	var Text = "Usage:\r\nfsRSSCreate.js [SourcePath] [NewGUID] [Title] [Description] [By]\r\n\r\nParameters:\r\n  SourcePath=fully qualified filename of original RSS document\r\n  NewGUID=unique value for new item\r\n  Title=value of the title attribute of the new item element (use quotes to enter multiple words)\r\n  Description=value of the title attribute of the new item element (use quotes to enter multiple words)\r\n  By=unique entity making the change (optional - default is \"\")\r\n";

	//  If text was provided, prepend it to default usage text
	if ((i_Text != null) && (i_Text != ""))
		Text = i_Text + "\r\n\r\n" + Text;
	
	WScript.Echo(Text);
	}	