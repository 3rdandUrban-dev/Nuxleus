//  *********************************************************************************
//  File:	fsAtomMerge.js
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
var g_LocalPath = g_Arguments(0);
var g_IncomingPath = g_Arguments(1);

var g_pILocalAtomXmlDOMDocument = null;

try
	{
	//  Create instance of XML DOM
	g_pILocalAtomXmlDOMDocument = new ActiveXObject("Microsoft.XMLDOM");
	g_pILocalAtomXmlDOMDocument.async = false;

	//  Load local document	
	var Success = g_pILocalAtomXmlDOMDocument.load(g_LocalPath);
	if (!Success)
		throw new Error(0, "IXmlDocument::load failed");
	}
catch (e)
	{
	WScript.Echo("Exception while loading '" + g_LocalPath +"': " + e.message);
	WScript.Quit();
	}

//  Get local "feed" element
var g_pILocalFeedXmlDOMElement = g_pILocalAtomXmlDOMDocument.documentElement;

//  Check if FeedSync namespace exists, if not then display error
if (g_pILocalFeedXmlDOMElement.getAttribute("xmlns:sx") == null)
	{
	WScript.Echo("Can't process local Atom file - it does not contain a 'sx' namespace!");
	WScript.Quit();
	}

var g_pIIncomingAtomXmlDOMDocument = null;

try
	{
	//  Create instance of XML DOM
	g_pIIncomingAtomXmlDOMDocument = new ActiveXObject("Microsoft.XMLDOM");
	g_pIIncomingAtomXmlDOMDocument.async = false;

	//  Load incoming document	
	var Success = g_pIIncomingAtomXmlDOMDocument.load(g_IncomingPath);
	if (!Success)
		throw new Error(0, "IXmlDocument::load failed");
	}
catch (e)
	{
	WScript.Echo("Exception while loading '" + g_IncomingPath +"': " + e.message);
	WScript.Quit();
	}

//  Get incoming "feed" element
var g_pIIncomingFeedXmlDOMElement = g_pIIncomingAtomXmlDOMDocument.documentElement;

//  Check if FeedSync namespace exists, if not then display error
if (g_pIIncomingFeedXmlDOMElement.getAttribute("xmlns:sx") == null)
	{
	WScript.Echo("Can't process incoming Atom file - it does not contain a 'sx' namespace!");
	WScript.Quit();
	}

//  *********************************************************************************
//  BIG HONKING NOTE:  We only deal with "entry" elements when merging, so any other
//                     changes made in the incoming document are ignored.  Remember that
//                     the goal of FeedSync isn't to replicate Atom files, it is to 
//                     replicate items via Atom.
//  *********************************************************************************

//  Create hashtable for local FSNodes
var g_LocalFSNodeHashtable = new Object();

//  Populate local FSNode hashtable
PopulateFSNodesFromXmlDOMElement(g_LocalFSNodeHashtable, g_pILocalFeedXmlDOMElement);

//  Create hashtable for incoming FSNodes
var g_IncomingFSNodeHashtable = new Object();

//  Populate incoming FSNode hashtable
PopulateFSNodesFromXmlDOMElement(g_IncomingFSNodeHashtable, g_pIIncomingFeedXmlDOMElement);

var g_pIOutputAtomXmlDOMDocument = 	null;

try
	{
	//  Create instance of XML DOM
	g_pIOutputAtomXmlDOMDocument = new ActiveXObject("Microsoft.XMLDOM");
	g_pIOutputAtomXmlDOMDocument.async = false;

	//  Create output feed document based on local feed document
	g_pILocalAtomXmlDOMDocument.save(g_pIOutputAtomXmlDOMDocument);
	if (!Success)
		throw new Error(0, "IXmlDocument::load failed");
	}
catch (e)
	{
	WScript.Echo("Exception while saving local document: " + e.message);
	WScript.Quit();
	}

//  Create output array
var g_OutputFSNodeArray = new Array();

//  Get output "feed" element
var g_pIOutputFeedXmlDOMElement = g_pIOutputAtomXmlDOMDocument.documentElement;

//  Get "entry" elements of "feed" element
var g_pIEntryXmlDOMElements = g_pIOutputFeedXmlDOMElement.selectNodes("entry");

//  *********************************************************************************
//  BIG HONKING NOTE:  Iterate all "entry" elements of the "feed" element (start
//                     with last and progress to first) in order to remove them.  We 
//                     remove them because there is further processing below that 
//                     will take the appropriate "entry" elements from the local and 
//                     incoming documents and add them.  Note that we don't just remove 
//                     the "feed" element and add a new one in it's place because 
//                     a) there could be attributes on the "feed" element and b)
//                     there could be non-"entry" elements of the "feed" element.

	for (var Index = g_pIEntryXmlDOMElements.length - 1; Index >= 0; --Index)
		{
		//  Get next "entry" element
		var pIEntryXmlDOMElement = g_pIEntryXmlDOMElements(Index);
	
		//  Remove "entry" element from output "feed" element
		g_pIOutputFeedXmlDOMElement.removeChild(pIEntryXmlDOMElement);
		}
		
//  *********************************************************************************

var g_HashtableKey = null;

//  Iterate items in local FSNode hashtable
for (g_HashtableKey in g_LocalFSNodeHashtable)
	{
	//  Get FSNode from local hashtable
	var LocalFSNode = g_LocalFSNodeHashtable[g_HashtableKey];

	//  Get reference to local FSSyncNode
	var LocalFSSyncNode = LocalFSNode.m_FSSyncNode;

	//  Get FSNode from incoming hashtable
	var IncomingFSNode = g_IncomingFSNodeHashtable[g_HashtableKey];
	
	//  Validate incoming FSNode exists, if not then node was added to local
	//  document
	if (IncomingFSNode == null)
		{
		//  Create clone of LocalFSNode
		var ClonedFSNode = CloneFSNode(LocalFSNode);

		//  Add cloned FSNode to output array
		g_OutputFSNodeArray[g_OutputFSNodeArray.length] = ClonedFSNode;
		
		//  Continue loop
		continue;
		}

    //  Get merged FSNode
    var MergedFSNode = MergeFSNodes(LocalFSNode, IncomingFSNode);

	//  Add merged FSNode to output array
	g_OutputFSNodeArray[g_OutputFSNodeArray.length] = MergedFSNode;
	
	//  Set incoming hashtable entry to null so we don't process it during
	//  second pass below
	g_IncomingFSNodeHashtable[g_HashtableKey] = null;
	}

//  Iterate items in incoming FSNode hashtable - all remaining items are
//  guaranteed to be additions to incoming document
for (g_HashtableKey in g_IncomingFSNodeHashtable)
	{
	//  Get FSNode from incoming hashtable
	var IncomingFSNode = g_IncomingFSNodeHashtable[g_HashtableKey];

	//  Validate FSNode exists, if not then just continue loop because
	//  entry was set to null when processing local hashtable
	if (IncomingFSNode == null)
		continue;

	//  Create clone of IncomingFSNode
	var ClonedFSNode = CloneFSNode(IncomingFSNode);
	
	//  Add cloned FSNode to output array
	g_OutputFSNodeArray[g_OutputFSNodeArray.length] = ClonedFSNode;
	}
	
//  Iterate output FSNodes
for (var Index = 0; Index < g_OutputFSNodeArray.length; ++Index)
	{
	//  Get next output FSNode
	var OutputFSNode = g_OutputFSNodeArray[Index];
	
	//  Append output FSNode's element to "feed" element
	g_pIOutputFeedXmlDOMElement.appendChild(OutputFSNode.m_pIXmlDOMElement);
	}

//  Save modified contents to standand output stream
WScript.StdOut.Write(g_pIOutputAtomXmlDOMDocument.xml);

//  -------------------------- MAIN (END) --------------------------


//  -------------------------- FSNodeClass (BEGIN) --------------------------

function FSNodeClass(i_pIXmlDOMElement)
	{
	//  Assign m_pIOXmlDOMElement member variable
	this.m_pIXmlDOMElement = i_pIXmlDOMElement;

	//  Assign m_FSSyncNode member variable by creating a new instance of 
	//  FSSyncNode and passing a reference to the current FSNode
	this.m_FSSyncNode = new FSSyncNodeClass(this);
	}

//  -------------------------- FSNodeClass (END) --------------------------



//  -------------------------- FSSyncNodeClass (BEGIN) --------------------------
	
function FSSyncNodeClass(i_FSNode)
	{
	//  Assign m_FSNode member variable
	this.m_FSNode = i_FSNode;
	
	//  Get reference to FSNode's XmlDOMElement
	var pIXmlDOMElement = this.m_FSNode.m_pIXmlDOMElement;
	
	//  Assign m_pIXmlDOMElement member variable
	this.m_pIXmlDOMElement = pIXmlDOMElement.selectSingleNode("sx:sync");
	
	//  Validate m_pISyncXmlDOMElement member variable
	if (this.m_pIXmlDOMElement == null)
		{
		WScript.Echo("Unable to find 'sx:sync' element where parent id='" + this.m_FSNode.m_ParentID + "'");
		WScript.Quit(0);
		}

	//  Assign m_ID member variable
	this.m_ID = this.m_pIXmlDOMElement.getAttribute("id");

	//  Validate m_ID member variable
	if (this.m_ID == null)
		{
		WScript.Echo("Unable to find 'id' attribute for 'sx:sync' element where parent id='" + this.m_FSNode.m_ParentID + "'");
		WScript.Quit(0);
		}

	//  Assign m_Updates member variable
	this.m_Updates = this.m_pIXmlDOMElement.getAttribute("updates");

	//  Validate m_Updates member variable
	if (this.m_Updates == null)
		{
		WScript.Echo("Unable to find 'updates' attribute for 'sx:sync' element where id='" + this.m_ID + "'");
		WScript.Quit(0);
		}

	//  Assign m_NoConflicts member variable
	var NoConflicts = this.m_pIXmlDOMElement.getAttribute("noconflicts");
	
	//  Validate m_Conflict member variable
	if (NoConflicts == "true")
		this.m_NoConflicts = true;
	else
		this.m_NoConflicts = false;

    //  Assign m_FSConflictNodes member variable by creating a new array
    this.m_FSConflictNodes = new Array();

    if (!this.m_NoConflicts)
		{
		//  Get reference to "sx:conflicts" element
		this.m_pIConflictsXmlDOMElement = this.m_pIXmlDOMElement.selectSingleNode("sx:conflicts");
    	
		//  Validate that "sx:conflicts" element exists
		if (this.m_pIConflictsXmlDOMElement != null)
		    {
		    //  Get conflict "entry" elements
		    var pIConflictItemXmlDOMElements = this.m_pIConflictsXmlDOMElement.selectNodes("entry");
    		
		    //  Iterate conflict "entry" elements
		    for (var Index = 0; Index < pIConflictItemXmlDOMElements.length; ++Index)
			    {
			    //  Get reference to next conflict "entry" element
			    var pIConflictItemXmlDOMElement = pIConflictItemXmlDOMElements(Index);
    			
			    //  Assign array entry by creating a new instance of FSNode and passing
			    //  a reference to the current conflict "entry" element
			    this.m_FSConflictNodes[Index] = new FSNodeClass(pIConflictItemXmlDOMElement);
			    }
		    }
		}

    //  Assign m_FSHistoryNodes member variable by creating a new array
    this.m_FSHistoryNodes = new Array();

    //  Get "sx:history" elements
    var pIHistoryXmlDOMElements = this.m_pIXmlDOMElement.selectNodes("sx:history");
		    
    //  Iterate "sx:history" elements
    for (var Index = 0; Index < pIHistoryXmlDOMElements.length; ++Index)
	    {
	    //  Get reference to next "sx:history" element
	    var pIHistoryXmlDOMElement = pIHistoryXmlDOMElements(Index);
		
	    //  Assign array entry by creating a new instance of FSHistoryNode and passing
	    //  a reference to the current "sx:history" element
	    this.m_FSHistoryNodes[Index] = new FSHistoryNodeClass(this, pIHistoryXmlDOMElement);
	    }
	}
	
//  -------------------------- FSSyncNodeClass (END) --------------------------


//  -------------------------- FSHistoryNodeClass (BEGIN) --------------------------

function FSHistoryNodeClass(i_FSSyncNode, i_pIHistoryXmlDOMElement)
	{
	//  Assign m_FSSyncNode member variable
	this.m_FSSyncNode = i_FSSyncNode;
	
	//  Assign m_pIXmlDOMElement member variable		
	this.m_pIXmlDOMElement = i_pIHistoryXmlDOMElement;
	
	//  Validate m_pIXmlDOMElement member variable
	if (this.m_pIXmlDOMElement == null)
		{
		WScript.Echo("Unable to find 'sx:history' element for 'sx:sync' element where id='" + this.m_FSSyncNode.m_ID + "'");
		return;
		}

	//  Assign m_Sequence member variable
	this.m_Sequence = this.m_pIXmlDOMElement.getAttribute("sequence");
        
	//  Assign m_When member variable
	this.m_When = this.m_pIXmlDOMElement.getAttribute("when");
	
	//  Assign m_By member variable
	this.m_By = this.m_pIXmlDOMElement.getAttribute("by");
	
	//  Validate that either m_When or m_By member variable have been assigned
	if ((this.m_When == null) && (this.m_By == null))
		{
		WScript.Echo("Unable to find 'when' or 'by' attribute in 'sx:history' element for 'sx:sync' element where id='" + this.m_FSSyncNode.m_ID + "'");
		WScript.Quit(0);
		}
	}

//  -------------------------- FSHistoryNodeClass (BEGIN) --------------------------


function PopulateFSNodesFromXmlDOMElement(i_Hashtable, i_pIXmlDOMElement)
	{
	//  Get "entry" elements
	var pIItemXmlDOMElements = i_pIXmlDOMElement.selectNodes("entry");

	//  Iterate "entry" elements
	for (var Index = 0; Index < pIItemXmlDOMElements.length; ++Index)
		{
		//  Get reference to next "entry" element
		var pIItemXmlDOMElement = pIItemXmlDOMElements(Index);
		
		//  Create new instance of FSNodeClass
		var FSNode = new FSNodeClass(pIItemXmlDOMElement);
				
		//  Get reference to FSSyncNode
		var FSSyncNode = FSNode.m_FSSyncNode;

		//  Add FSNode to hashtable using FSSyncNode's id as key
		i_Hashtable[FSSyncNode.m_ID] = FSNode;
		}
	}
	
function MergeFSNodes(i_LocalFSNode, i_IncomingFSNode)
	{
	//  Create collection for local item and local item's conflicts
	var LocalItemCollection = new Array();
	
	//  Created clone of local FSNode
	var ClonedLocalFSNode = CloneFSNode(i_LocalFSNode);
	
	//  Get reference to local FSSyncNode
	var ClonedLocalFSSyncNode = ClonedLocalFSNode.m_FSSyncNode;
	
	//  Populate collection with clone of local item's conflicts
	for (var Index = 0; Index < ClonedLocalFSSyncNode.m_FSConflictNodes.length; ++Index)
	    LocalItemCollection[LocalItemCollection.length] = CloneFSNode(ClonedLocalFSSyncNode.m_FSConflictNodes[Index]);

	//  See if "sx:conflicts" element exists, if so remove it
	if (ClonedLocalFSSyncNode.m_pIConflictsXmlDOMElement != null)
	    ClonedLocalFSSyncNode.m_pIConflictsXmlDOMElement.parentNode.removeChild(ClonedLocalFSSyncNode.m_pIConflictsXmlDOMElement);
	    
	//  Populate collection with clone of local item
	LocalItemCollection[LocalItemCollection.length] = ClonedLocalFSNode;

	//  Create collection for incoming item and incoming item's conflicts
	var IncomingItemCollection = new Array();
	
	//  Created clone of incoming FSNode
	var ClonedIncomingFSNode = CloneFSNode(i_IncomingFSNode);

	//  Get reference to incoming FSSyncNode
	var ClonedIncomingFSSyncNode = ClonedIncomingFSNode.m_FSSyncNode;
	
	//  Populate collection with clone of incoming item's conflicts
	for (var Index = 0; Index < ClonedIncomingFSSyncNode.m_FSConflictNodes.length; ++Index)
	    IncomingItemCollection[IncomingItemCollection.length] = CloneFSNode(ClonedIncomingFSSyncNode.m_FSConflictNodes[Index]);

	//  See if "sx:conflicts" element exists, if so remove it
	if (ClonedIncomingFSSyncNode.m_pIConflictsXmlDOMElement != null)
	    ClonedIncomingFSSyncNode.m_pIConflictsXmlDOMElement.parentNode.removeChild(ClonedIncomingFSSyncNode.m_pIConflictsXmlDOMElement);
	    
	//  Populate collection with clone of incoming item
	IncomingItemCollection[IncomingItemCollection.length] = ClonedIncomingFSNode;

	//  Create collection for merge result 
	var MergeResultItemCollection = new Array();

	var WinnerFSNode = null;

	//  Process collections using local item collection as outer collection
	//  and incoming item collection as inner collection	
	WinnerFSNode = ProcessCollections(LocalItemCollection, IncomingItemCollection, MergeResultItemCollection, WinnerFSNode);
	
	//  Process collections using incoming item collection as outer collection
	//  and local item collection as inner collection	
	WinnerFSNode = ProcessCollections(IncomingItemCollection, LocalItemCollection, MergeResultItemCollection, WinnerFSNode);

	//  Get reference to winner's FSSyncNode
	var WinnerFSSyncNode = WinnerFSNode.m_FSSyncNode;

	//  If the "noconflicts" attribute is true, or if there is only one
	//  item in the merge result collection (i.e. the winner), then we are 
	//  done processing
	if (WinnerFSSyncNode.m_NoConflicts || (MergeResultItemCollection.length == 1))
	    return WinnerFSNode;
    
	//  Create "sx:conflicts" element for winner
	var pIWinnerConflictsXmlDOMElement = g_pIOutputAtomXmlDOMDocument.createElement("sx:conflicts");
    
	//  Append "sx:conflicts" element to winner's "sx:sync" element
	WinnerFSSyncNode.m_pIXmlDOMElement.appendChild(pIWinnerConflictsXmlDOMElement);
    
	//  Get reference to winner's conflict nodes
	var WinnerFSConflictNodes = WinnerFSSyncNode.m_FSConflictNodes;
    
	//  Create empty array to hold winner's conflict nodes
	WinnerFSConflictNodes = new Array();
    
	//  Iterate items in merge result collection    
	for (var Index = 0; Index < MergeResultItemCollection.length; ++Index)
	    {
	    //  Get next item in merge result collection
	    var MergeResultItem = MergeResultItemCollection[Index];
        
	    //  If the merge result item matches the winner item, just
	    //  continue the loop
	    if (0 == CompareFSNodes(WinnerFSNode, MergeResultItem))
	        continue;

	    //  Get reference to merge result item's element
	    var pIMergeResultItemXmlDOMElement = MergeResultItemCollection[Index].m_pIXmlDOMElement;
                    
	    //  Append merge result's element to winner's "sx:conflicts" element
	    pIWinnerConflictsXmlDOMElement.appendChild(pIMergeResultItemXmlDOMElement);
        
	    //  Add new item to winner's conflict nodes
	    WinnerFSConflictNodes[WinnerFSConflictNodes.length] = new FSNodeClass(pIMergeResultItemXmlDOMElement);
	    }
        
	return WinnerFSNode;
	}
 
function ProcessCollections(i_OuterFSNodeCollection, i_InnerFSNodeCollection, io_MergeFSNodeCollection, i_WinnerFSNode)
    {
    //  Iterate outer FSNode collection
    for (var OuterFSNodeCollectionIndex = 0; OuterFSNodeCollectionIndex < i_OuterFSNodeCollection.length; ++OuterFSNodeCollectionIndex)
        {
        //  Get next FSNode in outer collection
        var OuterFSNode = i_OuterFSNodeCollection[OuterFSNodeCollectionIndex];
        
        //  Get reference to outer FSSyncNode
        var OuterFSSyncNode = OuterFSNode.m_FSSyncNode;

        var OuterFSNodeSubsumed = false;
        
        //  Iterate inner FSNode collection
        for (var InnerFSNodeCollectionIndex = 0; InnerFSNodeCollectionIndex < i_InnerFSNodeCollection.length; ++InnerFSNodeCollectionIndex)
            {
            //  Get next FSNode in inner collection
            var InnerFSNode = i_InnerFSNodeCollection[InnerFSNodeCollectionIndex];

            //  Check value of inner FSNode exists - if not then
            //  just continue loop
            if (InnerFSNode == null)
                continue;
                            
            //  Get reference to inner FSSyncNode
            var InnerFSSyncNode = InnerFSNode.m_FSSyncNode;

            //  Get the topmost "sx:history" element for the outer FSSyncNode
            var OuterFSHistoryNode = OuterFSNode.m_FSSyncNode.m_FSHistoryNodes[0];        
            
            //  Iterate FSHistoryNodes for inner FSSyncNode
            for (var HistoryIndex = 0; HistoryIndex < InnerFSSyncNode.m_FSHistoryNodes.length; ++HistoryIndex)
                {
                //  Get next FSHistoryNode
                var InnerFSHistoryNode = InnerFSSyncNode.m_FSHistoryNodes[HistoryIndex];
                
                //  See if "by" attribute exists for outer FSHistoryNode and if
                //  it does, see if it's value matches "by" attribute value for 
                //  inner FSHistoryNode
                if ((OuterFSHistoryNode.m_By != null) && (OuterFSHistoryNode.m_By == InnerFSHistoryNode.m_By))
                    {
                    //  See if "sequence" attribute for the inner FSHistoryNode 
                    //  is greater than or equal to the "sequence" attribute for
                    //  the outer FSHistoryNode
                    if (InnerFSHistoryNode.m_Sequence >= OuterFSHistoryNode.m_Sequence)
                        {
                        //  Indicate subsumption
                        OuterFSNodeSubsumed = true;
                        }

                    //  Stop iterating FSHistoryNodes
                    break;
                    }
                    
                //  See if "by" attribute does not exist for both outer FSHistoryNode
                //  and inner FSHistoryNode
                else if ((OuterFSHistoryNode.m_By == null) && (InnerFSHistoryNode.m_By == null))
                    {
                    //  See if "when" attribute exists for both outer FSHistoryNode
                    //  and inner FSHistoryNode
                    if ((InnerFSHistoryNode.m_When != null) && (OuterFSHistoryNode.m_When != null))
                        {
                        //  See if normalized dates match - if so then the outer FSNode
                        //  is subsumed
                        if (InnerFSHistoryNode.m_When == OuterFSHistoryNode.m_When)
                            {
                            //  Indicate subsumption
                            OuterFSNodeSubsumed = true;
        				    
                            //  Stop iterating FSHistoryNodes
                            break;
                            }
                        }
                    }
                }

            //  Check for subsumption
            if (OuterFSNodeSubsumed)
                {
                //  Stop iterating inner FSNodes        
                break;
                }
            }

        //  Check for subsumption
        if (OuterFSNodeSubsumed)
            {
            //  Remove outer FSNode from outer FSNode collection
            i_OuterFSNodeCollection[OuterFSNodeCollectionIndex] = null;
            
            //  Continue iterating outer FSNodes
            continue;
            }

        //  See if outer FSSyncNode has any FSConflictNodes
        if (OuterFSSyncNode.m_FSConflictNodes.length > 0)
            {
            //  Remove the "sx:conflicts" sub-element for outer
            //  FSSyncNode
            OuterFSSyncNode.m_pIConflictsXmlDOMElement.parentNode.removeChild(OuterFSSyncNode.m_pIConflictsXmlDOMElement);
            }

        //  Add the outer FSNode to the merge result collection
        io_MergeFSNodeCollection[io_MergeFSNodeCollection.length] = OuterFSNode;

        //  See if winner FSNode has not been assigned yet or 
        //  if the outer FSNode represents a more recent update
        //  than that of the current winner FSNode
        if ((i_WinnerFSNode == null) || (-1 == CompareFSNodes(i_WinnerFSNode, OuterFSNode)))
            {
            //  Assign the outer FSNode as the winner FSNode
            i_WinnerFSNode = OuterFSNode;
            }
        }

    return i_WinnerFSNode;
    }
    
function CompareFSNodes(i_FSNode1, i_FSNode2)
    {
	//  This function compares the two FSNodes and returns:
	//     1 if i_FSNode1 is newer than i_FSNode2	
	//    -1 if i_FSNode2 is newer than i_FSNode1
	//     0 if FSNodes are equal
	//     null if FSNodes are equal but conflict data is different
	//

	//  Get reference to FSSyncNode for i_FSNode1
	var FSSyncNode1 = i_FSNode1.m_FSSyncNode;
	
	//  Get reference to FSSyncNode for i_FSNode2
	var FSSyncNode2 = i_FSNode2.m_FSSyncNode;

	//  Compare "updates" attributes - if they are equal then do subsequent checks
	if (FSSyncNode1.m_Updates == FSSyncNode2.m_Updates)
		{
		//  Get reference to topmost FSHistoryNode for FSSyncNode1
		var FSHistoryNode1 = FSSyncNode1.m_FSHistoryNodes[0];

		//  Get reference to topmost FSHistoryNode for FSSyncNode2
		var FSHistoryNode2 = FSSyncNode2.m_FSHistoryNodes[0];
						
		//  See if "when" attribute exist for either FSHistoryNode
		if ((FSHistoryNode1.m_When != null) || (FSHistoryNode2.m_When != null))
			{
			//  See if "when" attribute exist for both FSHistoryNodes
			if ((FSHistoryNode1.m_When != null) && (FSHistoryNode2.m_When != null))
				{
				//  Compare date values - since we use RFC3339 values, we can use 
				//  string comparison when comparing datetimes
				if (FSHistoryNode1.m_When > FSHistoryNode2.m_When)
					{
					//  FSHistoryNode1 node has a later "when" attribute, so i_FSNode1
					//  is newer
					return 1;
					}
				else if (FSHistoryNode2.m_When > FSHistoryNode1.m_When)
					{
					//  FSHistoryNode2 node has a later "when" attribute, so i_FSNode2
					//  is newer
					return -1;
					}
				else
					{
					//  Same "when" attribute value for both FSHistoryNodes - try further 
					//  checking below
					}
				}
			else if (FSHistoryNode1.m_When != null)
				{
				//  FSHistoryNode1 has a "when" attribute but FSHistoryNode2 does not, so
				//  i_FSNode1 is newer
				return 1;
				}
			else
				{
				//  FSHistoryNode2 has a "when" attribute but FSHistoryNode1 does not, so
				//  i_FSNode2 is newer
				return -1;
				}
			}
		else
			{
			//  Neither FSHistoryNode has "when" attribute - try further checking below
			}
		
		//  See if "by" attribute exist for either FSHistoryNode
		if ((FSHistoryNode1.m_By != null) || (FSHistoryNode2.m_By != null))
			{
			//  See if "by" attribute exist for both FSHistoryNodes
			if ((FSHistoryNode1.m_By != null) && (FSHistoryNode2.m_By != null))
				{
				//  Compare "by" values
				if (FSHistoryNode1.m_By > FSHistoryNode2.m_By)
					{
					//  FSHistoryNode1 node has a later "by" attribute, so i_FSNode1
					//  is newer
					return 1;
					}
				else if (FSHistoryNode1.m_By < FSHistoryNode2.m_By)
					{
					//  FSHistoryNode2 node has a later "by" attribute, so i_FSNode2
					//  is newer
					return -1;
					}
				else
					{
					//  Same "by" attribute value for both FSHistoryNodes - so we must
					//  compare conflict items

					//  If number of conflict item nodes is different, items are equal
					//  but conflict item data is different
					if (FSSyncNode1.m_FSConflictNodes.length != FSSyncNode2.m_FSConflictNodes.length)
					    return null;

					//  Check if conflict item nodes exist
					if (FSSyncNode1.m_FSConflictNodes.length > 0)
					    {
					    //  Iterate conflict nodes for item 1    
					    for (var Index1 = 0; Index1 < FSSyncNode1.m_FSConflictNodes.length; ++Index)
					        {
					        var MatchingConflictItem = false;
    					    
					        //  Get reference to next conflict node for item 1
					        var ConflictNode1 = FSSyncNode1.m_FSConflictNodes[Index1];
    					    
					        //  Iterate conflict nodes for item 2
					        for (var Index2 = 0; Index2 < FSSyncNode2.m_FSConflictNodes.length; ++Index)
					            {
					            //  Get reference to next conflict node for item 2
					            var ConflictNode2 = FSSyncNode2.m_FSConflictNodes[Index2];
    					        
					            //  Compare conflict nodes
					            if (0 == CompareFSNodes(ConflictNode1, ConflictNode2))
					                {
					                MatchingConflictItem = true;
					                break;
					                }
					            }
					        }
    					    
				        if (!MatchingConflictItem)
				            {
				            //  No matching conflict item - so items are equal but conflict
				            //  item data is different
				            return null;
				            }
					    }
					    
					    //  Items are equal
					    return 0;
					}
				}
			}
		else if (FSHistoryNode1.m_By != null)
			{
			//  FSHistoryNode1 has a "by" attribute but FSHistoryNode2 does not, so
			//  i_FSNode1 is newer
			return 1;
			}
		else if (FSHistoryNode2.m_By != null)
			{
			//  FSHistoryNode2 has a "by" attribute but FSHistoryNode1 does not, so
			//  i_FSNode2 is newer
			return -1;
			}
		else
			{
			//  Neither FSHistoryNode has "by" attribute - so we can't tell which
			//  FSNode is newer
			return 0;
			}
		}	
	else if (FSSyncNode1.m_Updates> FSSyncNode2.m_Updates)
		{
		//  FSSyncNode1 has a later "updates" attribute, so i_FSNode1 is newer
		return 1;
		}
	else
		{
		//  FSSyncNode2 has a later "updates" attribute, so i_FSNode2 is newer
		return -1;
		}
	}

function CloneFSNode(i_FSNode)
	{
	//  Get reference to original FSNode's XmlDOMElement
	var pIXmlDOMElement = i_FSNode.m_pIXmlDOMElement;
	
	//  Create (deep copy) clone of XmlDOMElement
	var pIClonedXmlDOMElement = pIXmlDOMElement.cloneNode(true);
	
	//  Create new instance of FSNode
	var ClonedFSNode = new FSNodeClass(pIClonedXmlDOMElement);
	
	//  Return new instance of FSNode
	return ClonedFSNode;
	}

function DisplayUsage(i_Text)
	{
	var Text = "Usage:\r\nfsAtomMerge.js [LocalPath] [IncomingPath]\r\n\r\nParameters:\r\n  LocalPath=fully qualified filename of local Atom document (required)\r\n  IncomingPath=fully qualified filename for incoming Atom document (required)\r\n";

	//  If text was provided, prepend it to default usage text
	if ((i_Text != null) && (i_Text != ""))
		Text = i_Text + "\r\n\r\n" + Text;
	
	WScript.Echo(Text);
	}	
