//  *********************************************************************************
//  File:	fsAtomConvert.js
//  Notes:	You must run this file with cscript.exe (i.e. not wscript.exe)
//
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.
//  *********************************************************************************


//  -------------------------- MAIN (BEGIN) --------------------------

//  Validate arguments
var g_Arguments = WScript.Arguments;
if (g_Arguments.length < 1)
	{
	DisplayUsage();
	WScript.Quit();
	}

//  Get required parameters
var g_SourcePath = g_Arguments(0);

var g_pIOutputXmlDOMDocument = null;

//  Load source Atom document	
var g_pISourceAtomXmlDOMDocument = LoadXmlDocument(g_SourcePath);
if (g_pISourceAtomXmlDOMDocument == null)
	{
	WScript.Echo("Exception while loading '" + g_SourcePath + "': " + e.message);
	WScript.Quit(0);
	}

//  We will use incrementing counter for ids
var g_Counter = 100;
	
//  Get source "feed" element
var g_pISourceAtomXmlDOMElement = g_pISourceAtomXmlDOMDocument.documentElement;

//  Set FeedSync namespace
g_pISourceAtomXmlDOMElement.setAttribute("xmlns:sx", "http://feedsync.org/2007/feedsync");
	
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
	
//  Process all child "item" elements
CreateItemElements(g_pISourceAtomXmlDOMElement, g_RFC3339DateTime);

//  Save modified contents to standand output stream
WScript.StdOut.Write(g_pISourceAtomXmlDOMDocument.xml);

//  -------------------------- MAIN (END) --------------------------


function CreateItemElements(i_pIParentXmlDOMElement, i_RFC3339DateTime)
	{
	//  Get "entry" elements
	var pIEntryXmlDOMElements = i_pIParentXmlDOMElement.selectNodes("entry");

	//  Iterate "entry" elements
	for (var Index = 0; Index < pIEntryXmlDOMElements.length; ++Index)
		{
		//  Get next "entry" element
		var pIItemXmlDOMElement = pIEntryXmlDOMElements(Index);

		//  Create "sx:sync" element
		var pISyncXmlDOMElement = g_pISourceAtomXmlDOMDocument.createElement("sx:sync");

		//  Set "id" attribute for "sx:sync" element
		
		//  *********************************************************************************
		//  BIG HONKING NOTE:  The "id" attribute should be a globally unique value - this
		//                     sample does not attempt to do this.
			
			pISyncXmlDOMElement.setAttribute("id", ++g_Counter);

		//  *********************************************************************************
		
		//  Set "updates" attribute for "sx:sync" element
		pISyncXmlDOMElement.setAttribute("updates", "1");

		//  Set "deleted" attribute for "sx:sync" element
		pISyncXmlDOMElement.setAttribute("deleted", "false");

		//  Set "noconflicts" attribute for "sx:sync" element
		pISyncXmlDOMElement.setAttribute("noconflicts", "false");

		//  Create "sx:history: element
		var pIHistoryXmlDOMElement = g_pISourceAtomXmlDOMDocument.createElement("sx:history");

		//  Set "sequence" attribute for "sx:history" element
		pIHistoryXmlDOMElement.setAttribute("sequence", "1");

		//  Set "when" attribute for "sx:history" element
		pIHistoryXmlDOMElement.setAttribute("when", i_RFC3339DateTime);

		//  *********************************************************************************
		//  BIG HONKING NOTE:  The "by" value should be a unique value per user/endpoint -
		//                     this sample does not attempt to do this.

			//  Set "by" attribute for "sx:history" element (use name of utility as value)
			pIHistoryXmlDOMElement.setAttribute("by", "fsAtomConvert.js");
				
		//  *********************************************************************************
				
		//  Append "sx:history" element to "sx:sync" element
		pISyncXmlDOMElement.appendChild(pIHistoryXmlDOMElement);
		
		//  Insert/append "sx:sync" element to "item" element
		if (pIItemXmlDOMElement.childNodes.length > 0)
			pIItemXmlDOMElement.insertBefore(pISyncXmlDOMElement, pIItemXmlDOMElement.childNodes[0]);
		else
			pIItemXmlDOMElement.appendChild(pISyncXmlDOMElement);
		}
	}

function LoadXmlDocument(i_Path)
	{
	var pIXmlDOMDocument = null;
	
	try
		{
		//  Create instance of XML DOM
		pIXmlDOMDocument = new ActiveXObject("Microsoft.XMLDOM");
		pIXmlDOMDocument.async = false;

		//  Load source document	
		var Success = pIXmlDOMDocument.load(i_Path);
		if (Success)
			return pIXmlDOMDocument;
		}
	catch (e)
		{
		//  Swallow all exceptions
		}
		
	return null;
	}
	
function DisplayUsage(i_Text)
	{
	var Text = "Usage:\r\nfsAtomConvert.js [SourcePath]\r\n\r\nParameters:\r\n  SourcePath=fully qualified filename of original Atom document\r\n";

	//  If text was provided, prepend it to default usage text
	if ((i_Text != null) && (i_Text != ""))
		Text = i_Text + "\r\n\r\n" + Text;
	
	WScript.Echo(Text);
	}	