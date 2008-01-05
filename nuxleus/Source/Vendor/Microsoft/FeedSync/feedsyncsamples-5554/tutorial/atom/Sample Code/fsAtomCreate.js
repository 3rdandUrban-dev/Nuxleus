//  *********************************************************************************
//  File:	fsAtomCreate.js
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

	var g_By = "fsAtomCreate.js" + Math.random();
	if (g_Arguments.length > 4)
		g_By = g_Arguments(4);

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

//  Create "entry" element
var g_pIEntryXmlDOMElement = g_pISourceAtomXmlDOMDocument.createNode(1, "entry", "http://www.w3.org/2005/Atom");

//  Create "sx:sync" element
var g_pISyncXmlDOMElement = g_pISourceAtomXmlDOMDocument.createElement("sx:sync");

//  Set "id" attribute for "sx:sync" element
g_pISyncXmlDOMElement.setAttribute("id", g_NewGUID);

//  Set "updates" attribute for "sx:sync" element
g_pISyncXmlDOMElement.setAttribute("updates", "1");

//  Set "deleted" attribute for "sx:sync" element
g_pISyncXmlDOMElement.setAttribute("deleted", "false");

//  Set "noconflicts" attribute for "sx:sync" element
g_pISyncXmlDOMElement.setAttribute("noconflicts", "false");

//  Create "history" element
var g_pIHistoryXmlDOMElement = g_pISourceAtomXmlDOMDocument.createElement("sx:history");

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

//  Append "sx:sync" element to "entry" element
g_pIEntryXmlDOMElement.appendChild(g_pISyncXmlDOMElement);

//  Create & populate "title" element
var g_pITitleXmlDOMElement = g_pISourceAtomXmlDOMDocument.createNode(1, "title", "http://www.w3.org/2005/Atom");
g_pITitleXmlDOMElement.text = g_Title;

//  Append "title" element to "entry" element
g_pIEntryXmlDOMElement.appendChild(g_pITitleXmlDOMElement);

//  Create & populate "content" element
var g_pIContentXmlDOMElement = g_pISourceAtomXmlDOMDocument.createNode(1, "content", "http://www.w3.org/2005/Atom");
g_pIContentXmlDOMElement.text = g_Description;

//  Append "content" element to "entry" element
g_pIEntryXmlDOMElement.appendChild(g_pIContentXmlDOMElement);

//  Create & populate "id" element (see http://diveintomark.org/archives/2004/05/28/howto-atom-id)
var g_pIIDXmlDOMElement = g_pISourceAtomXmlDOMDocument.createNode(1, "id", "http://www.w3.org/2005/Atom");
g_pIIDXmlDOMElement.text = "tag:example.com," + g_Year + "-" + g_Month + "-" + g_Day + ":" + Math.random();

//  Append "id" element to "entry" element
g_pIEntryXmlDOMElement.appendChild(g_pIIDXmlDOMElement);

//  Create & populate "author" element
var g_pIAuthorXmlDOMElement = g_pISourceAtomXmlDOMDocument.createNode(1, "author", "http://www.w3.org/2005/Atom");

//  Create & populate "name" element
var g_pINameXmlDOMElement = g_pISourceAtomXmlDOMDocument.createNode(1, "name", "http://www.w3.org/2005/Atom");
g_pINameXmlDOMElement.text = g_By;

//  Append "name" element to "author" element
g_pIAuthorXmlDOMElement.appendChild(g_pINameXmlDOMElement);

//  Append "author" element to "entry" element
g_pIEntryXmlDOMElement.appendChild(g_pIAuthorXmlDOMElement);

//  Create & populate "updated" element
var g_pIUpdatedXmlDOMElement = g_pISourceAtomXmlDOMDocument.createNode(1, "updated", "http://www.w3.org/2005/Atom");
g_pIUpdatedXmlDOMElement.text = g_RFC3339DateTime;

//  Append "updated" element to "entry" element
g_pIEntryXmlDOMElement.appendChild(g_pIUpdatedXmlDOMElement);

//  Append "entry" element to "feed" element
g_pIFeedXmlDOMElement.appendChild(g_pIEntryXmlDOMElement);

//  Save modified contents to standand output stream
WScript.StdOut.Write(g_pISourceAtomXmlDOMDocument.xml);

//  -------------------------- MAIN (END) --------------------------


function DisplayUsage(i_Text)
	{
	var Text = "Usage:\r\nfsAtomCreate.js [SourcePath] [NewGUID] [Title] [Description] [By]\r\n\r\nParameters:\r\n  SourcePath=fully qualified filename of original Atom document\r\n  NewGUID=unique value for new item\r\n  Title=value of the title attribute of the new item element (use quotes to enter multiple words)\r\n  Description=value of the title attribute of the new item element (use quotes to enter multiple words)\r\n  By=unique entity making the change (optional - default is \"\")\r\n";

	//  If text was provided, prepend it to default usage text
	if ((i_Text != null) && (i_Text != ""))
		Text = i_Text + "\r\n\r\n" + Text;
	
	WScript.Echo(Text);
	}	