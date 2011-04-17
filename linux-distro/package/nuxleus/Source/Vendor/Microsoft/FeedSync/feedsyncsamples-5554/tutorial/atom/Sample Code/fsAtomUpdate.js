//  *********************************************************************************
//  File:	fsAtomUpdate.js
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
var g_GUID = g_Arguments(1);
var g_Title = g_Arguments(2);
var g_Description = g_Arguments(3);

//  Get optional parameters if specified, otherwise use defaults

//  *********************************************************************************
//  BIG HONKING NOTE:  The "by" value should be a unique value per user/endpoint -
//                     this sample uses a random number to generate uniqueness.
//                     Other applications should considering using a more robust
//                     and persistant value.

	var g_By = "fsAtomUpdate.js" + Math.random();
	if (g_Arguments.length > 4)
		g_By = g_Arguments(4);

//  *********************************************************************************

var g_ResolveConflicts = false;
if (g_Arguments.length > 5)
    g_ResolveConflicts = (g_Arguments(5) == "true") ? true : false;
    
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
	WScript.Echo("Exception while loading '" + g_SourcePath +"': " + e.message);
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

//  Get "title" element from "entry" element
var g_pITitleXmlDOMElement = g_pIEntryXmlDOMElement.selectSingleNode("title");

//  Validate that "title" element exists
if (g_pITitleXmlDOMElement == null)
	{
	//  Create "title" element
	g_pITitleXmlDOMElement = g_pISourceAtomXmlDOMDocument.createElement("title");

	//  Append "title" element to "entry" element
	g_pIEntryXmlDOMElement.appendChild(g_pITitleXmlDOMElement);
	}

//  Set title for "title" element
g_pITitleXmlDOMElement.text = g_Title;

//  Get "content" element from "entry" element
var g_pIContentXmlDOMElement = g_pIEntryXmlDOMElement.selectSingleNode("content");

//  Validate that "content" element exists
if (g_pIContentXmlDOMElement == null)
	{
	//  Create & populate "content" element
	g_pIContentXmlDOMElement = g_pISourceAtomXmlDOMDocument.createElement("content");

	//  Append "content" element to "entry" element
	g_pIEntryXmlDOMElement.appendChild(g_pIContentXmlDOMElement);
	}

//  Set description for "content" element
g_pIContentXmlDOMElement.text = g_Description;

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
var g_Sequence = g_Updates;
if (g_By != "")
    {
    //  Get "sx:history" sub-elements of "sx:sync" element
    var pIHistoryXmlDOMElements = g_pISyncXmlDOMElement.selectNodes("sx:history");

    //  Iterate "sx:history" sub-elements of main item's "sx:sync" element
    for (var HistoryIndex = 0; HistoryIndex < pIHistoryXmlDOMElements.length; ++HistoryIndex)
        {
        //  Get reference to next "sx:history" sub-element of main item
        var pIHistoryXmlDOMElement = pIHistoryXmlDOMElements(HistoryIndex);

        //  Get "by" attribute from main item's "sx:history" 
        //  sub-element
        var By = pIHistoryXmlDOMElement.getAttribute("by");

        //  Get "sequence" attribute from main item's 
        //  "sx:history" sub-element
        var Sequence = pIHistoryXmlDOMElement.getAttribute("sequence");

        //  If "by" attributes don't match, continue loop
        if ((By == g_By) && (Sequence >= g_Sequence))
            g_Sequence = Sequence + 1;
        }
    }
    
g_pIHistoryXmlDOMElement.setAttribute("sequence", g_Sequence);

//  Set "when" attribute for "sx:history" element
g_pIHistoryXmlDOMElement.setAttribute("when", g_RFC3339DateTime);

//  Set "by" attribute for "sx:history" element
g_pIHistoryXmlDOMElement.setAttribute("by", g_By);

//  Insert "sx:history" element as topmost sub-element of "sx:sync" element
g_pISyncXmlDOMElement.insertBefore(g_pIHistoryXmlDOMElement, g_pISyncXmlDOMElement.childNodes[0]);
	
//  Set "updates" attribute for "sx:sync" element
g_pISyncXmlDOMElement.setAttribute("updates", g_Updates);

//  Get "noconflicts" attribute for "sx:sync" element
var g_NoConflicts = (g_pISyncXmlDOMElement.getAttribute("noconflicts") == "true") ? true : false;

//  See if conflict resolution should be performed
if (!g_NoConflicts && g_ResolveConflicts)
    {
    //  *********************************************************************************
    //  BIG HONKING NOTE:  This sample resolves all conflicts and does not accomodate for
    //                     selective conflict resolution
    //  *********************************************************************************

    //  Get the "sx:conflicts" sub-element
    var pIConflictsXmlDOMElement = g_pISyncXmlDOMElement.selectSingleNode("sx:conflicts");

    //  Validate that "sx:conflicts" sub-element exists
    if (pIConflictsXmlDOMElement != null)
        {
        //  Construct hashtable for item history
        var ItemHistoryHashtable = new Object();

        //  Get "sx:history" sub-elements of "sx:sync" element
        var pIHistoryXmlDOMElements = g_pISyncXmlDOMElement.selectNodes("sx:history");
        
        //  Get "entry" sub-elements of "sx:conflicts" element
        var pIConflictItemXmlDOMElements = pIConflictsXmlDOMElement.selectNodes("entry");
        
        //  Iterate "entry" sub-elements of "sx:conflicts" element
        for (var ConflictItemIndex = 0; ConflictItemIndex < pIConflictItemXmlDOMElements.length; ++ConflictItemIndex)
            {
            //  Get next conflict "entry" element
    		var pIConflictItemXmlDOMElement = pIConflictItemXmlDOMElements(ConflictItemIndex);

            //  Get the conflict item's "sx:sync" sub-element
            var pIConflictSyncXmlDOMElement = pIConflictItemXmlDOMElement.selectSingleNode("sx:sync");

            //  Get "sx:history" sub-elements of conflict item's "sx:sync" element
            var pIConflictHistoryXmlDOMElements = pIConflictSyncXmlDOMElement.selectNodes("sx:history");

            //  Iterate "sx:history" sub-elements of conflict item's "sx:sync" element
            for (var ConflictHistoryIndex = 0; ConflictHistoryIndex < pIConflictHistoryXmlDOMElements.length; ++ConflictHistoryIndex)
                {
                //  Get next conflict "sx:history" sub-element
                var pIConflictHistoryXmlDOMElement = pIConflictHistoryXmlDOMElements(ConflictHistoryIndex);

                //  Get "sequence" attribute from conflict item's topmost 
                //  "sx:history" sub-element
                var ConflictSequence = pIConflictHistoryXmlDOMElement.getAttribute("sequence");
                
                //  Get "by" attribute from conflict item's topmost "sx:history" 
                //  sub-element
                var ConflictBy = pIConflictHistoryXmlDOMElement.getAttribute("by");
                
                //  Get "when" attribute from conflict item's topmost "sx:history" 
                //  sub-element
                var ConflictWhen = pIConflictHistoryXmlDOMElement.getAttribute("when");
                
                var ConflictHistoryRepresented = false;
                
                //  Iterate "sx:history" sub-elements of main item's "sx:sync" element
                for (var HistoryIndex = 0; HistoryIndex < pIHistoryXmlDOMElements.length; ++HistoryIndex)
                    {
                    //  Get reference to next "sx:history" sub-element of main item
                    var pIHistoryXmlDOMElement = pIHistoryXmlDOMElements(HistoryIndex);

                    //  Get "sequence" attribute from main item's 
                    //  "sx:history" sub-element
                    var Sequence = pIHistoryXmlDOMElement.getAttribute("sequence");
                    
                    //  Get "by" attribute from main item's "sx:history" 
                    //  sub-element
                    var By = pIHistoryXmlDOMElement.getAttribute("by");
                    
                    //  Get "when" attribute from main item's "sx:history" 
                    //  sub-element
                    var When = pIHistoryXmlDOMElement.getAttribute("when");
                    
                    //  See if "by" attribute exists for main item's "sx:history" 
                    //  element and if it does, see if it's value matches "by" 
                    //  attribute value for conflict's "sx:history" element
                    if ((By != null) && (By == ConflictBy))
                        {
                        //  See if "sequence" attribute for the main item's 
                        //  "sx:history" element is greater than or equal to the 
                        //  "sequence" attribute for the conflict's "sx:history"
                        //  element
                        if (Sequence >= ConflictSequence)
                            {
                            //  Indicate conflict history represented
                            ConflictHistoryRepresented = true;
                            }

                        //  Stop iterating main item's "sx:history" elements
                        break;
                        }
                            
                    //  See if "by" attribute does not exist for both main item's
                    //  "sx:history" element and conflict's "sx:history" element
                    else if ((By == null) && (ConflictBy == null))
                        {
                        //  See if "when" attribute exists for both main item's
                        //  "sx:history" element and conflict's "sx:history" 
                        //  element
                        if ((When != null) && (ConflictWhen != null))
                            {
                            //  Compare date values - since we use RFC3339 values, we can use 
                            //  string comparison when comparing datetimes
                            if (When == ConflictWhen)
                                {
                                //  Indicate conflict history represented
                                ConflictHistoryRepresented = true;
            				    
                                //  Stop iterating "sx:history" elements
                                break;
                                }
                            }
                        }
                    }
                    
                if (ConflictHistoryRepresented)
                    {
                    //  Continue iterating conflict's "sx:history" elements
                    continue;
                    }
                
                //  Create clone of conflict item's "sx:history" sub-element
                var pIClonedConflictHistoryXmlDOMElement = pIConflictHistoryXmlDOMElement.cloneNode(true);

                //  Insert cloned conflict item's "sx:history" sub-element after 
                //  main item's topmost "sx:history" sub-element
                g_pISyncXmlDOMElement.insertBefore(pIClonedConflictHistoryXmlDOMElement, g_pIHistoryXmlDOMElement.nextSibling);
                }
            }
                   
	    //  Since we have resolved all conflicts, we remove the "sx:conflicts" 
	    //  sub-element from current parent element
	    pIConflictsXmlDOMElement.parentNode.removeChild(pIConflictsXmlDOMElement);
	    }
    }

//  Save modified contents to standand output stream
WScript.StdOut.Write(g_pISourceAtomXmlDOMDocument.xml);

//  -------------------------- MAIN (END) --------------------------


function DisplayUsage(i_Text)
	{
	var Text = "Usage:\r\nfsAtomUpdate.js [SourcePath] [GUID] [Title] [Description] [By] [ResolveConflicts]\r\n\r\nParameters:\r\n  SourcePath=fully qualified filename of original Atom document\r\n  GUID=unique value of existing item\r\n  Title=value of the title attribute of the new item element (use quotes to enter multiple words)\r\n  Description=value of the title attribute of the new item element (use quotes to enter multiple words)\r\n  By=unique entity making the change (optional - default is \"\")\r\n  ResolveConflicts=true or false (optional - default is false)\r\n";

	//  If text was provided, prepend it to default usage text
	if ((i_Text != null) && (i_Text != ""))
		Text = i_Text + "\r\n\r\n" + Text;
	
	WScript.Echo(Text);
	}	