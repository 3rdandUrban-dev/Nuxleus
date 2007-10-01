var foc;
  function changeTabVis(tab){
    document.getElementById(tab).className = 'menuItem on';
    document.getElementById(foc).className = 'menuItem off';
    foc = tab;
  };
  function setTab(tab){
    foc = tab;
  };

var curDiv = 'none';
function checkshowHide(div){
if (div != curDiv && curDiv != 'none')document.getElementById(curDiv).style.display = 'none';
document.getElementById(div).style.display = 'inline';
curDiv = div;
};
function Hide(div){
document.getElementById(div).style.display = 'none';
curDiv = div;
};
function showHide(div){
var current = document.getElementById(div).style.display;
if (div == curDiv && current == 'inline') document.getElementById(div).style.display = 'none';
else {
document.getElementById(div).style.display = 'inline';
if (curDiv != 'none' && curDiv != div) document.getElementById(curDiv).style.display = 'none';
}
curDiv = div;
};

var urlXML;
var urlXSL;
var docXML;
var docXSL;
var target;
var feedID;
var cache;
var processor;
var pos = 1;
var atom;
var style;
var mainContentDOM;
var curFeed = '';
var xmlFeeds = new Array();
var parameters = new Array();
var lt = new RegExp("&lt;", "g");
var gt = new RegExp("&gt;", "g");
var amp = new RegExp("&amp;", "g");
var xmlURLList = new Array();
xmlURLList[0] = new Array("summary-old", "http://www.xsltblog.com/atom-30.xml");
xmlURLList[1] = new Array("atom30", "http://www.xsltblog.com/atom-30.xml");
xmlURLList[2] = new Array("atom60", "http://www.xsltblog.com/atom-60.xml");
xmlURLList[3] = new Array("summary", "http://www.xsltblog.com/atom-90.xml");
xmlURLList[4] = new Array("atomcomplete", "http://www.xsltblog.com/atom-90.xml");
xmlURLList[5] = new Array("codeoftheday", "http://www.xsltblog.com/codeoftheday/atom-90.xml");
xmlURLList[6] = new Array("inthenews", "http://www.xsltblog.com/inthenews/atom-90.xml");
xmlURLList[7] = new Array("general", "http://www.xsltblog.com/general/atom-90.xml");
xmlURLList[8] = new Array("announcements", "http://www.xsltblog.com/announcements/atom-90.xml");
xmlURLList[9] = new Array("quoteoftheday", "http://www.xsltblog.com/quoteoftheday/atom-90.xml");
xmlURLList[10] = new Array("frontpagesummary", "http://www.xsltblog.com/atom-30.xml");
var transformCache = new Array();
var transformCacheLength;

 
function initialise(section) {
var datafeed = section;
var datafeedQF = section.concat('QF');
SetParam('sect', section);
SetParam('pos', '30');
initialiseTransform('leadContent', loadXMLFeed(datafeed), '/leadContent.xsl', datafeed);
SetParam('pos','1');
SetParam('feed',datafeed);
initialiseTransform('dataFeed', loadXMLFeed(datafeed), '/quickview.xsl', datafeedQF);
};

function setFeedID(feed){
  feedID = feed;
};

function getFeedID(){
  return feedID;
};

function loadAtomFeed(atomFeed){
if (_SARISSA_IS_IE){
  var xmlhttp = new XMLHttpRequest();
  xmlhttp.open("GET", atomFeed, false);
  xmlhttp.send(null); 
return xmlhttp.responseXML;

  }
if (_SARISSA_IS_MOZ){
  var xmlDoc = Sarissa.getDomDocument();
  xmlDoc.async = false;
  xmlDoc.load(atomFeed);
  return xmlDoc;
  }
};

function loadStylesheet(style){
  var xslDoc = Sarissa.getDomDocument();
  xslDoc.async = false;
  xslDoc.load(style);
  return xslDoc;
};

function loadXMLFeed(feedID){
var inMemory = false;
var xmlFeedLength = xmlFeeds.length;
var i;
for (i = 0; i< xmlFeedLength; i++){
   if (xmlFeeds[i][0] == feedID){
      inMemory = true;
      return xmlFeeds[i][1];
   }
}
if (!inMemory){
for (i = 0; i< xmlURLList.length; i++){
   if (xmlURLList[i][0] == feedID){
      var tempDoc = loadAtomFeed(xmlURLList[i][1]);
      xmlFeeds[xmlFeedLength] = new Array(feedID, tempDoc);
      return tempDoc;
   }
  }
 }
 return false;
};

function initialiseContentView() {
initialiseTransform('leadContent', loadXMLFeed('summary'), '/leadContent.xsl', 'summary');
};

function initialiseTransform(targetID, dom, style, xmlfeed){
  setTarget(targetID);
  if (curFeed != ''){ 
  var curDisplay = document.getElementById(curFeed);
  //curDisplay.style.display = "none";
  //curDisplay.style.background = "#e5eef9";
  }
  var newDisplay = document.getElementById(targetID);
  //newDisplay.style.display = "inline";
  //newDisplay.style.background = "#f9fbf8";
  curFeed = targetID;
  checkTransformCache(target, dom, style, xmlfeed);
};

function checkTransformCache(target, dom, style, xmlfeed){

var inCache = false;
var i;

  if (transformCache.length){
    for (i = 0; i< transformCache.length; i++){
       if (transformCache[i][0] == xmlfeed){
          target.innerHTML = transformCache[i][1];
          inCache = true;
       }
    }
  }

  if (!inCache){
    setDOMSource(dom);
    setStylesheet(style);
    setFeedID(xmlfeed);
    Transform();
  }
 
};

function setDOMSource(obj){
  urlXML = obj;
};

function setgetPos(position){
  pos = pos + position;
  return pos;
};

function setTarget(id){
  target = document.getElementById(id);
};

function setStylesheet(style){
  var tempXSL = loadStylesheet(style);
  urlXSL = tempXSL;
};

function SetParam(name, value){
  var paramID;
  if (parameters.length) paramID = parameters.length;
  else paramID = 0;
  parameters[paramID] = new Array(name, value);
};

function Transform(){
  var processor = new XSLTProcessor();
  processor.importStylesheet(urlXSL);
  doTransform(processor);
  };
  
function doTransform(processor){
var i;
  if (parameters.length){
    for (i = 0; i< parameters.length; i++){
       processor.setParameter(null, parameters[i][0], parameters[i][1]);
    }
  };
var transformResult = processor.transformToDocument(urlXML);
//Sarissa.updateContentFromNode(urlXML, target, processor);
doeHack(Sarissa.serialize(transformResult));
};
function doeHack(outputString){
  var escapedOutput = outputString.replace(lt, "<").replace(gt, ">").replace(amp, "&");
  target.innerHTML = escapedOutput;
  tcLength = transformCache.length;
  var tcLength;
  if (transformCache.length > 0 ) tcLength = transformCache.length;
  else tcLength = 0;
  transformCache[tcLength] = new Array(getFeedID(), escapedOutput);
};
function cancel(){
    return true;
};
